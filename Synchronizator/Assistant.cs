using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Synchronizator
{
    public abstract class Assistant
    { 
        private static string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\";
        public static string home { get { return desktop + "Synch"; } }
        public static string paths { get { return home + "\\paths.sys"; } }
        public static string journal { get { return home + "\\journal.sys"; } }
        
        /// <summary>
        /// Создаем папку и файлы для полноценной работы приложения
        /// </summary>
        public static void  CreateWorkData()
        {
            if (!Directory.Exists(home))
                Directory.CreateDirectory(home);
            if (!File.Exists(paths))
                File.Create(paths);
            if (!File.Exists(journal))
                File.Create(journal);
        }

        /// <summary>
        /// Выводим выбор функций
        /// </summary>
        /// <returns></returns>
        public static ConsoleKeyInfo StartMessage()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Clear();
            Console.WriteLine("Выберите функцию");
            Console.WriteLine("------------------");
            Console.WriteLine("1 - Настроить пути");
            Console.WriteLine("2 - Синхронизировать");
            Console.WriteLine("------------------");
            Console.Write("Функция: ");
            return Console.ReadKey();
        }
    }
}
