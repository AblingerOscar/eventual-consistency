using System;
using System.Collections.Generic;
using System.Text;

namespace Cheetah.CLI
{
    internal class CLIApp
    {
        private IDictionary<string, Command> commands = new Dictionary<string, Command>();
        private bool running = false;

        public string inputPrompt = "> ";

        public void AddCommand(
            string name,
            Action<Arguments> action,
            string description,
            string[] aliases = null)
        {
            if (aliases == null)
                aliases = new string[0];

            var command = new Command(name, action, description, aliases);
            AddCommandToDictionary(command);
        }

        private void AddCommandToDictionary(Command command)
        {
            commands.Add(command.Name, command);
            foreach(var cliName in command.Aliases) {
                commands.Add(cliName, command);
            }
        }

        public void Start()
        {
            running = true;
            Console.WriteLine("CLI ready");
            Console.WriteLine("You can type help to see a list of all commands and their explanations");

            while(running)
            {
                Console.Write(inputPrompt);
                var input = Console.ReadLine();
                var cmdName = input.Split(' ', 2)[0];
                if (commands.ContainsKey(cmdName))
                    ExecuteCommand(cmdName, input);
                else
                    NoSuchCommandInfo(cmdName);
            }
        }

        private void ExecuteCommand(string cmdName, string input)
        {
            commands[cmdName].Action(new Arguments(input));
        }

        private void NoSuchCommandInfo(string cmdName)
        {
            Console.WriteLine($"Command '{cmdName}' not found. You can type 'help' to get a list of valid commands");
        }

        public void Stop()
        {
            running = false;
        }

        public void PrintHelpScreen(Arguments args)
        {
            if (args.ArgumentList.Length <= 1)
                PrintAllHelp();
            else
                PrintHelpFor(args.ArgumentList[1]);
        }

        private void PrintAllHelp()
        {
            foreach(var commandKVP in commands) {
                if (commandKVP.Key == commandKVP.Value.Name)
                    PrintHelpFor(commandKVP.Key);
            }
        }

        private void PrintHelpFor(string nameOrAlias)
        {
            Console.WriteLine(commands[nameOrAlias].ToString());
        }
    }
}
