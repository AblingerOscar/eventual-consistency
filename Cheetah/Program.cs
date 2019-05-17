using System;

namespace Cheetah
{
    class Program
    {
        static void Main(string[] args)
        {
            PrintStartupMessage();
        }

        private static void PrintStartupMessage()
        {
            String cheetah = "" +
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


            Console.WriteLine(cheetah);
        }
    }
}
