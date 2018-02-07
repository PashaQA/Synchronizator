using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            return true;
        }
    }
}
