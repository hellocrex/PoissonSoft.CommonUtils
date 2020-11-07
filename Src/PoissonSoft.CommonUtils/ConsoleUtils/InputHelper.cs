using System;
using System.Collections.Generic;
using System.Linq;

namespace PoissonSoft.CommonUtils.ConsoleUtils
{
    /// <summary>
    /// Утилиты для работы с пользовательским вводом в консоли
    /// </summary>
    public static class InputHelper
    {
        /// <summary>
        /// Ввод секретных данных (например, пароль пользователя)
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        public static string GetSecureData(string message)
        {
            Console.Write(message);
            string str = string.Empty;
            do
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter) break;
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (str.Length != 0)
                    {
                        str = str.Remove(str.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    str += key.KeyChar;
                    Console.Write("*");
                }
            }
            while (true);
            Console.WriteLine();
            return str;
        }

        /// <summary>
        /// Подтверждение действия пользователя
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool Confirm(string message)
        {
            var k = GetUserAction(message,
                new Dictionary<ConsoleKey, string> {{ConsoleKey.Y, "Yes"}, {ConsoleKey.N, "No"}});
            return k == ConsoleKey.Y;
        }

        /// <summary>
        /// Получение от пользователя строки
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string GetString(string message)
        {
            Console.Write(message);
            var value = Console.ReadLine();
            return value;
        }

        /// <summary>
        /// Получение от пользователя числа int
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static int GetInt(string message)
        {
            int result;
            do
            {
                Console.Write(message);
            }
            while (!int.TryParse(Console.ReadLine(), out result));
            return result;
        }

        /// <summary>
        /// Получение от пользователя числа long
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static long GetLong(string message)
        {
            long result;
            do
            {
                Console.WriteLine(message);
            }
            while (!long.TryParse(Console.ReadLine(), out result));
            return result;
        }

        /// <summary>
        /// Получение от пользователя числа decimal
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static decimal GetDecimal(string message)
        {
            decimal result;
            do
            {
                Console.WriteLine(message);
            }
            while (!decimal.TryParse(Console.ReadLine(), out result));
            return result;
        }

        /// <summary>
        /// Получение Enum от пользователя
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static TEnum GetEnum<TEnum>(string message) where TEnum : struct
        {
            var type = typeof(TEnum);
            if (!type.IsEnum) return default;

            var itemsString = string.Join("|", Enum.GetNames(type));
            var msg = $"{message} ({itemsString})";

            TEnum result;
            do
            {
                Console.WriteLine(msg);
            }
            while (!Enum.TryParse(Console.ReadLine(), out result));

            return result;
        }

        /// <summary>
        /// Выбор пользователем действия из предложенных вариантов
        /// </summary>
        /// <param name="description"></param>
        /// <param name="availableActions"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        public static ConsoleKey GetUserAction(string description, Dictionary<ConsoleKey, string> availableActions)
        {
            availableActions ??= new Dictionary<ConsoleKey, string> {{ConsoleKey.Y, "Yes"}};
            Console.WriteLine(description);
            Console.WriteLine(string.Join("\n", availableActions.Select(x => $"[{x.Key}] {x.Value}")));
            while (true)
            {
                var option = Console.ReadKey();
                var k = option.Key;
                if (availableActions.ContainsKey(k))
                {
                    Console.WriteLine();
                    return k;
                }
                Console.Write("\b \b");
            }
        }
    }

}

