using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Synchronizator
{
    class Program
    {
        static void Main(string[] args)
        {
            Assistant.CreateWorkData();

            while (true)
            {
                ConsoleKeyInfo key = Assistant.StartMessage();

                if (key.KeyChar == 49)
                {
                    if (Functions.SettingPaths()) break;
                }
                else if (key.KeyChar == 50)
                {
                    if (Functions.Synch()) break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nОшибочная команда..");
                    System.Threading.Thread.Sleep(2000);
                    Assistant.StartMessage();
                }
            }
        }
    }
}
