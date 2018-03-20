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
        public static string paths { get { return home + "\\paths\\"; } }
        public static string journal { get { return home + "\\journal.sys"; } }

        public enum PathsType
        {
            IN = 0,
            FROM = 1
        }

        /// <summary>
        /// Создаем папку и файлы для полноценной работы приложения
        /// </summary>
        public static void CreateWorkData()
        {
            if (!Directory.Exists(home))
                Directory.CreateDirectory(home);
            if (!Directory.Exists(paths))
                Directory.CreateDirectory(paths);
            if (!File.Exists(journal))
            {
                using (StreamWriter sw = new StreamWriter(journal))
                {
                    sw.WriteLine("dirs:0");
                    sw.WriteLine("files:0");
                    sw.WriteLine("delete_dirs:0");
                    sw.WriteLine("delete_files:0");
                }
            }
            
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
            Console.WriteLine("3 - Просмотреть список скопированных данных");
            Console.WriteLine("------------------");
            Console.Write("Функция: ");
            return Console.ReadKey();
        }

        /// <summary>
        /// Выводит на консоль напоминание о том, что тут реализовано копирование
        /// </summary>
        public static void RemindPaste()
        {
            Console.WriteLine("Система не принимает ничего другого, кроме Ctrl + V");
            Console.WriteLine("---------------");
            Console.WriteLine("");
        }

        /// <summary>
        /// Обворачиваем ожидалку для единого количества секунд ожидания
        /// </summary>
        /// <param name="seconds"></param>
        public static void Sleep(int seconds = 2000)
        {
            System.Threading.Thread.Sleep(seconds);
        }

        /// <summary>
        /// Проверяем, что пользователь действительно нажал Ctrl + V
        /// </summary>
        /// <param name="ki"></param>
        /// <returns></returns>
        public static Boolean ValidatePaste(ConsoleKeyInfo ki)
        {
            if (ki.Key != ConsoleKey.V || ki.Modifiers != ConsoleModifiers.Control)
                return false;
            return true;
        }

        /// <summary>
        /// Возвращаем путь до папки с базы.
        /// Реализована через функцию Remove из-за того, потому что в файле храним in+/to+ перед адресом.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static String GetPath(PathsType type, string path)
        {
            string path_ = string.Empty;
            using (StreamReader sr = new StreamReader(path))
            {
                string[] arr_paths = sr.ReadToEnd().Split('\n');
                if (type == PathsType.IN) path_ = arr_paths[0].Remove(0, 3);
                else path_ = arr_paths[1].Remove(0, 3);
                return path_.Replace("\n", "").Replace("\r", "");
            }

        }

        /// <summary>
        /// Возвращае имя файла с расширением либо папки
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static String CutNameWithExtension(string path)
        {
            int idx = path.LastIndexOf('\\');
            return path.Remove(0, idx + 1);
        }
        
        /// <summary>
        /// Возвращает имя файла без расширения
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Int32 CutNameWithoutExtension(string path)
        {
            string name = CutNameWithExtension(path);
            int idx = name.LastIndexOf('.');
            return Convert.ToInt32(name.Remove(idx));
        }

        /// <summary>
        /// Выводит на экран пути но новых файлов/папок
        /// </summary>
        /// <param name="paths"></param>
        public static void PrintPaths(List<string> paths)
        {
            foreach (string path in paths)
                Console.WriteLine("      " + path);
            Console.WriteLine("\n");
        }
    }
}
