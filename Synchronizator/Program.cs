using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Synchronizator
{
    /// <summary>
    /// 6h
    /// 1. Попробовать запусть на папках, в которых нет подпапок, только файлы(музыка) - должно работать
    /// </summary>
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Assistant.CreateWorkData();
            Console.WindowHeight = Convert.ToInt32(Console.WindowHeight * 2.5);

            while (true)
            {
                ConsoleKeyInfo key = Assistant.StartMessage();
                bool error = false;

                if (key.KeyChar == 49)
                {
                    if (!Functions.SettingPaths()) error = true;
                }
                else if (key.KeyChar == 50)
                {
                    if (!Functions.Synch()) error = true; 
                }
                else if (key.KeyChar == 51)
                {
                    if (!Functions.SeeJournal()) error = true;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n\nОшибочная команда..");
                    Assistant.Sleep();
                    Assistant.StartMessage();
                }

                if (error)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n\nФункция оборвалась с ошибкой..");
                    Assistant.Sleep();
                }
            }
        }
    }
}
