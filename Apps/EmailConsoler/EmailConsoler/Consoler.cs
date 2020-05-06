using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailConsoler
{
    public static class Consoler
    {
        private static object colorLock = new object();

        public static void WriteDoneLine(string value)
        {
            lock (colorLock)
            {
                value = DateTime.Now.ToString("yyMMdd-hh:mm:ss") + "> " + value;
                //Console.BackgroundColor = ConsoleColor.Green;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(value.PadRight(Console.WindowWidth - 1)); // <-- see note
                Console.ResetColor();                
            }
        }

        public static void WriteInfoLine(string value)
        {
            lock (colorLock)
            {
                value = DateTime.Now.ToString("yyMMdd-hh:mm:ss") + "> " + value;
                //Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(value.PadRight(Console.WindowWidth - 1)); // <-- see note
                Console.ResetColor();
            }
        }

        public static void WriteAppLine(string value)
        {
            lock (colorLock)
            {
                value = DateTime.Now.ToString("yyMMdd-hh:mm:ss") + "> " + value;
                //Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(value.PadRight(Console.WindowWidth - 1)); // <-- see note
                Console.ResetColor();
            }
        }

        public static void WriteErrorLine(string value)
        {
            lock (colorLock)
            {
                value = DateTime.Now.ToString("yyMMdd-hh:mm:ss") + "> " + value;
                //Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(value.PadRight(Console.WindowWidth - 1)); // <-- see note
                Console.ResetColor();
            }
        }
    }
}
