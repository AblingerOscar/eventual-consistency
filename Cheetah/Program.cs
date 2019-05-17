using Cheetah.CLI;
using System;

namespace Cheetah
{
    public class Program
    {
        private static readonly string CHEETAH = 
            "                                    __...__                       \n" +
            "                 .'`*-.        _.-*'       `*-._                  \n" +
            "            _.-*'      `+._.-*'                 `*-._             \n" +
            "       _.-*'           \\  `-                         `*-.        \n" +
            "    .-'  .--+           .                                `.       \n" +
            "  .'   _/,'`|           :                                  \\     \n" +
            "         ;  :                                            `  ;     \n" +
            " ;  s,     .'           ;                            /    ; |     \n" +
            "    \"                  |                          .'     : :     \n" +
            ":_        _.-._         :                                    .    \n" +
            " `T\"    .'     \\                              .-'.         : \\ \n" +
            "   `._.-'        \\        ;                .-*'    `.          ` \n" +
            "                  `.      :           _.-*';`*-.__.-*\\      ;.   \n" +
            "                   ;     ;`-.____.+*'     |      .'  .     |  `-. \n" +
            "                   |   -* `.      ,       :     :          :      \n" +
            "                   :    ;   `.    :             ;     ;    ;      \n" +
            "                   |          `.   ,       ;          :           \n" +
            "                   ;   '        \\  :           :       .  :      \n" +
            "                      ,          \\  \\       ;   `.     ;  '     \n" +
            "                  :  /            .  ,      :    /     :   \\     \n" +
            "              _._/  /          _._:  ;  _._/   .'       .  /      \n" +
            "            .'     /         .'     / .'     .'     _._/  /       \n" +
            "             *---*'           *---*'   *---*'     .'     /        \n" +
            "                                                   *---*'         \n";

        public static void Main(string[] args)
        {
            Console.WriteLine(CHEETAH);
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
