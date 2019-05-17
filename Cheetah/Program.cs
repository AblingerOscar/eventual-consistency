using Cheetah.CLI;
using System;

namespace Cheetah
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome to a .NET eventual consistency application");
            Console.WriteLine("");

            InitializeCLI();
        }

        private static void InitializeCLI()
        {
            var cli = new CLIApp();

            AddCommands(cli);
            cli.Start();
        }

        private static void AddCommands(CLIApp cli)
        {
            cli.AddCommand(
                "help",
                cli.PrintHelpScreen,
                "Prints this help screen.\n" +
                "help <cmd> will only print the help screen for that command"
                );
            cli.AddCommand(
                "exit",
                (args) => cli.Stop(),
                "Stops all applications",
                new string[]{ "shutdown", "stop" });
        }
    }
}
