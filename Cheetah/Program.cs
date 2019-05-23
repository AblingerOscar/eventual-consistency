using Cheetah.CLI;
using Cheetah.ServiceController;
using System;
using System.Collections.Generic;
using System.Linq;
using ViewService;

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

            new Program().InitializeCLI();
        }

        CLIApp cli;
        IServiceController serviceController;
        ISet<OutputLevel> disabledOutputLevels;
        ISet<LogReason> disabledReasons;

        private void InitializeCLI()
        {
            cli = new CLIApp();
            serviceController = new ServiceController.ServiceController();
            disabledOutputLevels = new HashSet<OutputLevel>();
            disabledReasons = new HashSet<LogReason>();

            RegisterLogging();

            AddCLICommands();
            AddServiceCommands();
            AddLoggingCommands();
            cli.Start();
        }

        private void RegisterLogging()
        {
            serviceController.OnServiceLog += (controller, args) =>
            {
                if (!disabledOutputLevels.Contains(args.OriginalArgs.OutputLevel) &&
                   !disabledReasons.Contains(args.OriginalArgs.Reason))
                    Console.WriteLine($"{args.ServiceInformation.ID} ({args.OriginalArgs.Reason}) > {args.OriginalArgs.Text}");
            };
        }

        private void AddCLICommands()
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
                "Stops all applications and exits the CLI",
                new string[] { "shutdown" });
        }

        private void AddServiceCommands()
        {
            cli.AddCommand(
                "create",
                AddCommand,
                new string[] {
                    "Creates a new service and a client for it",
                    "Option --start will also start them",
                    "Usage: create [--start]"
                }
                );
            cli.AddCommand(
                "start",
                StartCommand,
                new string[] {
                    "Start an already existing service and its client",
                    "Usage: start <serviceID>"
                }
                );
            cli.AddCommand(
                "stop",
                StopCommand,
                new string[] {
                    "Stops a service and his client",
                    "Usage: stop <serviceID>"
                });
            cli.AddCommand(
                "abort",
                AbortCommand,
                new string[] {
                    "Aborts a service without giving him a chance to persist etc.",
                    "Usage: abort <serviceID>"
                });
            cli.AddCommand(
                "send",
                SendCommand,
                new string[] {
                    "Sends views to the service",
                    "Usage: send <serviceID> [<viewAmount>]"
                }
                );
            cli.AddCommand(
                "periodic",
                PeriodicCommand,
                new string[]
                {
                    "Asks the client to start or stop sending periodic views to the service",
                    "Usage: periodic start <serviceID> <interval in milliseconds> [<viewAmount>]",
                    "       periodic stop <serviceID>"
                }
                );
            cli.AddCommand(
                "list",
                ListCommand,
                new string[]
                {
                    "Lists all Services",
                    "Option --running will only list running services",
                    "Usage: list [--running]"
                }
                );
        }

        private void AddLoggingCommands()
        {
            cli.AddCommand(
                "hide",
                HideCommand,
                new string[]
                {
                    "Hides Logs of the given output level or reason",
                    "Usage: hide reason <reason>",
                    "       hide level <level>"
                }
                );
            cli.AddCommand(
                "show",
                ShowCommand,
                new string[]
                {
                    "Shows Logs of the given output level or reason again",
                    "Usage: show reason <reason>",
                    "       show level <level>"
                }
                );
        }

        private ServiceInformation ReadServiceInformation(Arguments args, string helpCommand, int argsPosition = 1)
        {
            int? id = args.GetInt(argsPosition);
            if (!id.HasValue)
            {
                cli.DispatchCommand(helpCommand);
                return null;
            }
            var si = serviceController.AllServices.FirstOrDefault(inf => inf.ID == id.Value);
            if (si == null)
                Console.WriteLine($"No service with id {id.Value} found");

            return si;
        }

        #region Commands
        private void AddCommand(Arguments args)
        {
            var si = serviceController.CreateNewService();
            Console.WriteLine(si);
            if (args.ArgumentList.Length >= 2 && args.ArgumentList[1] == "--start")
                serviceController.StartService(si);
        }

        private void StartCommand(Arguments args)
        {
            var si = ReadServiceInformation(args, "help start");
            StartService(si);
        }

        private void StartService(ServiceInformation si)
        {
            if (si != null)
            {
                bool isAlreadyRunning = serviceController.StartService(si);
                if (isAlreadyRunning)
                    Console.WriteLine("Service is already running");
                else
                    Console.WriteLine("Service started");
            }
        }

        private void StopCommand(Arguments args)
        {
            var si = ReadServiceInformation(args, "help stop");
            StopService(si);
        }

        private void StopService(ServiceInformation si)
        {
            if (si != null)
            {
                serviceController.StopService(si);
                Console.WriteLine("Stopped the service");
            }
        }

        private void AbortCommand(Arguments args)
        {
            var si = ReadServiceInformation(args, "help abort");
            AbortService(si);
        }

        private void AbortService(ServiceInformation si)
        {
            if (si != null)
            {
                serviceController.AbortService(si);
                Console.WriteLine($"Service aborted");
            }
        }

        private void SendCommand(Arguments args)
        {
            var si = ReadServiceInformation(args, "help send");
            var amount = args.GetInt(2);
            SendViewsToService(si, amount);
        }

        private void SendViewsToService(ServiceInformation si, int? amount)
        {
            if (!amount.HasValue)
            {
                Console.WriteLine("Amount of views invalid");
                return;
            }
            SendViewsToService(si, amount.Value);
        }

        private void SendViewsToService(ServiceInformation si, int amount)
        {
            if (si != null)
            {
                serviceController.SendViews(si, amount);
                Console.WriteLine($"Sent Views");
            }
        }

        private void PeriodicCommand(Arguments args)
        {
            if (args.ArgumentList.Length < 2)
            {
                cli.DispatchCommand("help periodic");
                return;
            }
            if (args.ArgumentList[1] == "start")
                StartPeriodicCommand(args);
            else if (args.ArgumentList[1] == "stop")
                StopPeriodicCommand(args);
        }

        private void StartPeriodicCommand(Arguments args)
        {
            var si = ReadServiceInformation(args, "help periodic", 2);
            var interval = args.GetInt(3);
            var viewAmount = args.GetInt(4);
            StartPeriodicSending(si, interval, viewAmount);
        }

        private void StartPeriodicSending(ServiceInformation si, int? interval, int? viewAmount)
        {
            if (si == null)
                return;

            if (!interval.HasValue)
            {
                Console.WriteLine("Could not read interval");
                cli.DispatchCommand("help periodic");
            }
            int resolvedViewAmount = viewAmount.GetValueOrDefault(1);

            StartPeriodicSending(si, interval.Value, resolvedViewAmount);
        }

        private void StartPeriodicSending(ServiceInformation si, int interval, int viewAmount)
        {
            serviceController.StartSendingRepeatedViewsToService(si, interval, viewAmount);
            Console.WriteLine("Started periodic sendings");
        }

        private void StopPeriodicCommand(Arguments args)
        {
            var si = ReadServiceInformation(args, "help periodic", 2);
            StopPeriodicSending(si);
        }

        private void StopPeriodicSending(ServiceInformation si)
        {
            if (si != null)
            {
                serviceController.StopSendingRepeatedViewsToService(si);
                Console.WriteLine("Stopped periodic sendings");
            }
        }

        private void ListCommand(Arguments args)
        {
            IEnumerable<ServiceInformation> services;

            if (args.ArgumentList.Length >= 2 && args.ArgumentList[1] != "--running")
                services = serviceController.AllServices;
            else
                services = serviceController.RunningServices;

            foreach (var service in services)
                Console.WriteLine(service.ToString());
        }
        
        private void HideCommand(Arguments args)
        {
            if (args.ArgumentList.Length <= 2)
            {
                cli.DispatchCommand("help hide");
                return;
            }

            if(args.ArgumentList[1] == "reason")
            {
                if(Enum.TryParse(args.ArgumentList[2], out LogReason reason))
                {
                    disabledReasons.Add(reason);
                    Console.WriteLine("Hid reason " + reason);
                }
            } else if (args.ArgumentList[1] == "level")
            {
                if(Enum.TryParse(args.ArgumentList[2], out OutputLevel level))
                {
                    disabledOutputLevels.Add(level);
                    Console.WriteLine("Hid level " + level);
                }
            } else
                cli.DispatchCommand("help hide");
        }

        private void ShowCommand(Arguments args)
        {
            if (args.ArgumentList.Length <= 2)
            {
                cli.DispatchCommand("help show");
                return;
            }

            if(args.ArgumentList[1] == "reason" && Enum.TryParse(args.ArgumentList[2], out LogReason reason))
            {
                disabledReasons.Remove(reason);
                Console.WriteLine("Activated logs for a reason " + reason);
            } else if (args.ArgumentList[1] == "level" && Enum.TryParse(args.ArgumentList[2], out OutputLevel level))
            {
                disabledOutputLevels.Remove(level);
                Console.WriteLine("Activated logs for level " + level);
            } else
                cli.DispatchCommand("help hide");
        }
        #endregion
    }
}
