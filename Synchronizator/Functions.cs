using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Synchronizator
{
    public abstract class Functions
    {
        /// <summary>
        /// Настраиваем пути синхронизации
        /// </summary>
        /// <returns>
        /// Возвращаем TRUE, если функция успешно выполнила работу
        /// </returns>
        public static Boolean SettingPaths()
        {
            Console.Clear();
            Assistant.RemindPaste();
            int number_file = 0;
            if (Directory.GetFiles(Assistant.paths) != new string[0])
            {
                string[] dirs = Directory.GetFiles(Assistant.paths);
                number_file = Assistant.CutNameWithoutExtension(dirs[dirs.Length - 1]) + 1;
            }
            Console.Write("Введите путь до папки приемника изменений: ");
            if (!Assistant.ValidatePaste(Console.ReadKey())) return false;
            string path_in = Clipboard.GetText();

            Console.WriteLine("");

            Console.Write("Введите путь до папки отправителя изменений: ");
            if (!Assistant.ValidatePaste(Console.ReadKey())) return false;
            string path_from = Clipboard.GetText();

            try
            {
                if ((Directory.GetFiles(path_in).Length == 0 && Directory.GetDirectories(path_in).Length == 0) ||
                    (Directory.GetFiles(path_from).Length == 0 && Directory.GetDirectories(path_from).Length == 0) ||
                    path_in == path_from)
                    return false;
            }
            catch (Exception)
            {
                return false;
            }

            using (StreamWriter sw = new StreamWriter(Assistant.paths + @"\" + number_file.ToString() + ".sys"))
            {
                sw.WriteLine("in+" + path_in);
                sw.WriteLine("fr+" + path_from);
            }

            Console.WriteLine("\n\nИзменения успешно сохранены.");
            Assistant.Sleep();
            return true;
        }

        /// <summary>
        /// Выполняем синхронизацию
        /// </summary>
        /// <returns>
        /// Возваращаем TRUE, если функция успешно выполнила работу
        /// </returns>
        public static Boolean Synch()
        {
            Console.Clear();
            string[] paths = Directory.GetFiles(Assistant.paths);
            string[] paths_choosed = new string[1] { "0" };
            List<int> exists_paths = new List<int>();
            if (paths.Length != 0 && paths.Length != 1)
            {
                Console.WriteLine("Сейчас у вас имеется ряд путей. ");
                Console.WriteLine("Укажите через запятую набор тех путей, которые хотите синхронизировать.");
                Console.WriteLine();
                Console.WriteLine("-------------------------");
                foreach (string path in paths)
                {
                    Console.WriteLine(Assistant.CutNameWithoutExtension(path).ToString());
                    Console.WriteLine("IN: " + Assistant.GetPath(Assistant.PathsType.IN, Assistant.paths + Assistant.CutNameWithoutExtension(path) + ".sys"));
                    Console.WriteLine("FROM: " + Assistant.GetPath(Assistant.PathsType.FROM, Assistant.paths + Assistant.CutNameWithoutExtension(path) + ".sys"));
                    Console.WriteLine();
                    exists_paths.Add(Assistant.CutNameWithoutExtension(path));
                }
                Console.WriteLine("-------------------------");
                Console.Write("Пути: ");

                paths_choosed = Console.ReadLine().Split(',');
            }

            List<string> delete_dirs = new List<string>();
            List<string> delete_files = new List<string>();
            List<string> dirs = new List<string>();
            List<string> files = new List<string>();

            foreach (string path_choosed in paths_choosed)
            {
                //Не большой блок проверки следующих параметров:
                //1. Выбраны только цифры
                //2. Выбранные цифры существуют.
                try
                {
                    Convert.ToInt32(path_choosed);
                }
                catch (Exception)
                {
                    return false;
                }
                bool number_exists = false;
                foreach (int exists_path in exists_paths)
                {
                    if (exists_path.ToString() == path_choosed)
                    {
                        number_exists = true;
                        break;
                    }
                }
                if (!number_exists && exists_paths.Count != 0) return false;

                string path_in = Assistant.GetPath(Assistant.PathsType.IN, Assistant.paths + path_choosed.ToString() + ".sys");
                string path_from = Assistant.GetPath(Assistant.PathsType.FROM, Assistant.paths + path_choosed.ToString() + ".sys");
                Console.WriteLine("Запущена проверка путей: ");
                Console.WriteLine("IN: " + path_in);
                Console.WriteLine("FROM: " + path_from);
                Console.WriteLine();

                try
                {
                    string[] dirs_in = Directory.GetDirectories(path_in);
                    string[] dirs_from = Directory.GetDirectories(path_from);
                    Console.WriteLine();
                    Console.WriteLine("Запущена проверка удаление/добавление новых альбомов..");
                    // Проверяем, не пришло ли нам меньше альбомов, чем у нас имеется.
                    foreach (string dir_in in dirs_in)
                    {
                        bool exists = false;
                        foreach (string dir_from in dirs_from)
                        {
                            if (Assistant.CutNameWithExtension(dir_in) == Assistant.CutNameWithExtension(dir_from))
                            {
                                exists = true;
                                break;
                            }
                        }
                        if (!exists) delete_dirs.Add(dir_in);
                    }

                    // Проверяем, не пришло ли нам больше альбомов, чем у нас имеется
                    foreach (string dir_from in dirs_from)
                    {
                        bool exists = false;
                        foreach (string dir_in in dirs_in)
                        {
                            if (Assistant.CutNameWithExtension(dir_from) == Assistant.CutNameWithExtension(dir_in))
                            {
                                exists = true;
                                break;
                            }
                        }
                        if (!exists) dirs.Add(dir_from);
                    }
                    Console.WriteLine("Выполнена проверка на удаление/добавление новых альбомов.");
                    Console.WriteLine();
                    int counter_dirs = 0;
                    Console.WriteLine();
                    Console.WriteLine("Запущена проверка на поиск новых файлов по каждому альбому..");
                    // Проверяем, не добавились ли новые файлы
                    foreach (string dir_from in dirs_from)
                    {
                        counter_dirs++;
                        Console.WriteLine("Проверяем альбом: " + Assistant.CutNameWithExtension(dir_from) + " " + counter_dirs.ToString() + "/" + dirs_from.Length.ToString());
                        foreach (string dir_in in dirs_in)
                        {
                            if (Assistant.CutNameWithExtension(dir_from) == Assistant.CutNameWithExtension(dir_in))
                            {
                                string[] files_in = Directory.GetFiles(dir_in);
                                string[] files_from = Directory.GetFiles(dir_from);
                                foreach (string file_from in files_from)
                                {
                                    bool exists = false;
                                    foreach (string file_in in files_in)
                                    {
                                        if (Assistant.CutNameWithExtension(file_in) == Assistant.CutNameWithExtension(file_from))
                                        {
                                            exists = true;
                                            break;
                                        }
                                    }
                                    if (!exists) files.Add(file_from);
                                }
                            }
                        }
                    }
                    counter_dirs = 0;
                    Console.WriteLine();
                    Console.WriteLine("Запущена проверка на поиск удаленных файлов по каждому альбому..");
                    // Проверяем, не стало ли файлов меньше
                    foreach (string dir_in in dirs_in)
                    {
                        counter_dirs++;
                        Console.WriteLine("Проверяем альбом: " + Assistant.CutNameWithExtension(dir_in) + " " + counter_dirs.ToString() + "/" + dirs_in.Length.ToString());
                        foreach (string dir_from in dirs_from)
                        {
                            if (Assistant.CutNameWithExtension(dir_in) == Assistant.CutNameWithExtension(dir_from))
                            {
                                string[] files_in = Directory.GetFiles(dir_in);
                                string[] files_from = Directory.GetFiles(dir_from);
                                foreach (string file_in in files_in)
                                {
                                    bool exists = false;
                                    foreach (string file_from in files_from)
                                    {
                                        if (Assistant.CutNameWithExtension(file_in) == Assistant.CutNameWithExtension(file_from))
                                        {
                                            exists = true;
                                            break;
                                        }
                                    }
                                    if (!exists) delete_files.Add(file_in);
                                }
                            }
                        }
                    }

                }
                catch (Exception)
                {
                    return false;
                }
                Console.WriteLine("\n--------------------");
                Console.WriteLine("По итогу мы имеем следующую картину:\n");
                if (dirs.Count > 0)
                {
                    Console.WriteLine("   Появились следующие альбомы:\n");
                    Assistant.PrintPaths(dirs);
                }
                if (files.Count > 0)
                {
                    Console.WriteLine("   Появились следующие файлы:\n");
                    Assistant.PrintPaths(files);
                }
                if (delete_dirs.Count > 0)
                {
                    Console.WriteLine("   Отсутствуют следующие альбомы:\n");
                    Assistant.PrintPaths(delete_dirs);
                }
                if(delete_files.Count > 0)
                {
                    Console.WriteLine("   Отсутствуют следующие файлы:\n");
                    Assistant.PrintPaths(delete_files);
                }
                Console.WriteLine("----------------------");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\nВведите те цифры, команды которых необходимо выполнить..\n");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nПримите решение..\n");
                Console.WriteLine("1. Удалить отсутствующие данные");
                Console.WriteLine("2. Скопировать файлы");
                Console.WriteLine("3. Скопировать альбомы.");
                Console.WriteLine();

                string[] lines = null;
                using (StreamReader sr = new StreamReader(Assistant.journal))
                {
                    lines = sr.ReadToEnd().Replace("\r\n", "-").Split('-');
                    for(int i=0;i<lines.Length;i++)
                    {
                        if (lines[i] != "")
                        {
                            int idx = lines[i].IndexOf(':');
                            int stats = Convert.ToInt32(lines[i].Remove(0, idx + 1));
                            if (lines[i].StartsWith("dirs"))
                            {
                                lines[i] = "dirs:" + (stats + dirs.Count).ToString();
                            }
                            else if (lines[i].StartsWith("files"))
                            {
                                lines[i] = "files:" + (stats + files.Count).ToString();
                            }
                            else if (lines[i].StartsWith("delete_dirs"))
                            {
                                lines[i] = "delete_dirs:" + (stats + delete_dirs.Count).ToString();
                            }
                            else if (lines[i].StartsWith("delete_files"))
                            {
                                lines[i] = "delete_files:" + (stats + delete_files.Count).ToString();
                            }
                        }
                    }
                }
                using (StreamWriter sw = new StreamWriter(Assistant.journal))
                {
                    foreach (string line in lines)
                        sw.WriteLine(line);
                }

                string answer = Console.ReadLine();
                Console.WriteLine();
                foreach (char choise in answer)
                {
                    // Выполняем удаление
                    if (choise == '1')
                    {
                        foreach (string path in delete_dirs)
                        {
                            Directory.Delete(path, true);
                        }
                        foreach(string path in delete_files)
                        {
                            File.Delete(path);
                        }
                        Console.WriteLine("Удаление выполнено успешно.");
                        continue;
                    }
                    // Выполняем копирование файлов
                    if (choise == '2')
                    {
                        foreach(string path in files)
                        {
                            string file = path.Replace(path_from, "");
                            File.Copy(path, path_in + file);
                        }
                        Console.WriteLine("Копирование файлов выполнено успешно.");
                        continue;
                    }
                    // Выполняем копирование папок
                    if (choise == '3')
                    {
                        foreach(string path in dirs)
                        {
                            string folder = path.Replace(path_from, "");
                            Directory.CreateDirectory(path_in + folder);
                            string[] files_in_folder = Directory.GetFiles(path);
                            foreach(string file in files_in_folder)
                                File.Copy(file, path_in + folder + "\\" + Assistant.CutNameWithExtension(file));
                        }
                        Console.WriteLine("Копирование альбомов выполнено успешно.");
                        continue;
                    }
                }
            }
            Console.WriteLine("\nСинхронизация успешно завершена.");
            Assistant.Sleep();
            return true;
        }

        /// <summary>
        /// Возвращает список скопированных данных
        /// </summary>
        /// <returns></returns>
        public static Boolean SeeJournal()
        {
            Console.Clear();
            Console.WriteLine("Общая статистика синхронизации.");
            Console.WriteLine();
            using (StreamReader sr = new StreamReader(Assistant.journal))
            {
                string[] lines = sr.ReadToEnd().Replace("\r\n", "-").Split('-');
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i] != "")
                    {
                        int idx = lines[i].IndexOf(':');
                        int stats = Convert.ToInt32(lines[i].Remove(0, idx + 1));

                        if (lines[i].StartsWith("dirs"))
                            Console.WriteLine("Скопировано альбомов: " + stats.ToString());
                        else if (lines[i].StartsWith("files"))
                            Console.WriteLine("Скопировано фотографий: " + stats.ToString());
                        else if (lines[i].StartsWith("delete_dirs"))
                            Console.WriteLine("Удалено альбомов: " + stats.ToString());
                        else if (lines[i].StartsWith("delete_files"))
                            Console.WriteLine("Удалено фотографий: " + stats.ToString());
                    }
                }
 
            }
            Console.WriteLine();
            Console.WriteLine("Для выхода из статистики нажмите любую клавишу.");
            Console.ReadKey();
            return true;
        }
    }
}
