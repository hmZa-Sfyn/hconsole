using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hconsole.io
{
    public class outp
    {
        public static void writep(string stuff)
        { 
            int Len = stuff.Length;
            string haf = "";
            for (int i = 0; i < Len+2; i++)
            {
                haf += "═";
            }
            Console.WriteLine("╔" + haf + "╗");
            Console.WriteLine($"║ {stuff} ║");
            Console.WriteLine("╚" + haf + "╝");
        }

        public static string readp(string q, int ei = 10)
        {
            int Len = q.Length + 2;
            string haf = "";
            for (int i = 0; i < Len - 2; i++)
            {
                haf += "═";
            }
            Console.WriteLine("╔═[?] " + q);
            Console.Write($"╚═> ");
            Console.ForegroundColor= ConsoleColor.Gray;
            string res = Console.ReadLine();
            Console.ForegroundColor= ConsoleColor.White;

            return res;
        }
    }
}
