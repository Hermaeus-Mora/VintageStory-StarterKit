using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace StarterKit.Localization
{
    /// <summary>
    /// Локализатор.
    /// </summary>
    public static class Localizer
    {
        /// <summary>
        /// Расширение файлов.
        /// </summary>
        private const string Extension = ".json";
        /// <summary>
        /// Префикс расположения.
        /// </summary>
        private const string PathPrefix = "StarterKit.Localization.Dictionaries.";
        /// <summary>
        /// Язык по умолчанию.
        /// </summary>
        private const string DefaultLang = "ru";

        /// <summary>
        /// Набор словарей локализации.
        /// </summary>
        private static readonly SortedDictionary<string, LocalizationDictionary> Dictionaries;

        /// <summary>
        /// Исходный словарь.
        /// </summary>
        private static LocalizationDictionary? SourceDictionary;

        /// <summary>
        /// Статический конструктор.
        /// </summary>
        static Localizer()
        {
            Dictionaries = new SortedDictionary<string, LocalizationDictionary>(Comparer<string>.Default);
        }

        /// <summary>
        /// Инициализирует локализатор.
        /// </summary>
        internal static void Initialize()
        {
            // Получение текущей сборки
            Assembly assembly = Assembly.GetExecutingAssembly();

            // Получение путей к ресурсам и выборка словарей
            string[] pathes = assembly.GetManifestResourceNames();
            pathes = pathes.Where(x => x.StartsWith(PathPrefix) && x.EndsWith(Extension)).ToArray();

            // Обработка словарей
            foreach (string path in pathes)
            {
                // Получение потока словаря
                using Stream? stream = assembly.GetManifestResourceStream(path);
                if (stream == null)
                    continue;

                // Чтение словаря
                using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                string json = reader.ReadToEnd();
                if (string.IsNullOrWhiteSpace(json))
                    continue;

                // Определение языка и регистрация словаря
                string lang = path.Substring(PathPrefix.Length, path.Length - PathPrefix.Length - Extension.Length);
                Create(lang, json);
            }
        }
        /// <summary>
        /// Создаёт или обновляет словарь из JSON-строки.
        /// </summary>
        /// <param name="lang">Язык.</param>
        /// <param name="json">JSON-строка.</param>
        /// <returns>Успех выполнения.</returns>
        public static bool Create(string lang, string json)
        {
            // Проверка параметров
            if (lang == null)
                return false;
            if (json == null)
                return false;

            // Определение культуры
            CultureInfo? culture = null;
            try { culture = CultureInfo.GetCultureInfo(lang); }
            catch { return false; }
            if (culture == null)
                return false;

            // Поиск или создание словаря
            LocalizationDictionary? dictionary = null;
            if (!Dictionaries.TryGetValue(culture.Name, out dictionary))
            {
                // Создание и добавление словаря
                dictionary = new LocalizationDictionary(culture);
                Dictionaries.Add(culture.Name, dictionary);
            }

            // Чтение пар словаря из JSON
            Dictionary<string, string>? pairs = null;
            try { pairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(json); }
            catch { return false; }
            if (pairs == null)
                return false;

            // Добавление/обновление пар словаря
            foreach (KeyValuePair<string, string> pair in pairs)
                dictionary[pair.Key] = pair.Value;

            // Возврат
            return true;
        }

        /// <summary>
        /// Получает значение.
        /// </summary>
        /// <param name="key">Ключ.</param>
        /// <param name="args">Аргументы.</param>
        /// <returns>Значение.</returns>
        public static string Get(string key, params object[] args)
        {
            // Проверка параметров
            if (key == null)
                return string.Empty;

            // Проверка исходного словаря
            if (SourceDictionary == null)
                return key;

            // Получение значения из исходного словаря
            string? value = SourceDictionary[key];

            // Форматирование и возврат
            try { return value != null ? string.Format(value, args) : key; }
            catch { return key; }
        }
        /// <summary>
        /// Получает значение.
        /// </summary>
        /// <param name="lang">Язык.</param>
        /// <param name="key">Ключ.</param>
        /// <param name="args">Аргументы.</param>
        /// <returns>Значение.</returns>
        public static string Get(string lang, string key, params object[] args)
        {
            // Проверка параметров
            if (lang == null)
                return key;
            if (key == null)
                return string.Empty;

            // Определение культуры
            CultureInfo? culture = null;
            try { culture = CultureInfo.GetCultureInfo(lang); }
            catch { }
            if (culture == null)
            {
                if (SourceDictionary == null)
                    return key;

                // Установка исходной культуры, если указанная не найдена
                culture = SourceDictionary.Culture;
            }

            // Поиск словаря
            LocalizationDictionary? dictionary = null;
            if (!Dictionaries.TryGetValue(culture.Name, out dictionary))
            {
                if (SourceDictionary == null)
                    return key;

                // Установка исходного словаря, если указанный не найден
                dictionary = SourceDictionary;
            }

            // Получение значения из указанного или исходного словаря
            string? value = dictionary[key];
            if (value == null && !object.ReferenceEquals(dictionary, SourceDictionary) && SourceDictionary != null)
                value = SourceDictionary[key];

            // Форматирование и возврат
            try { return value != null ? string.Format(value, args) : key; }
            catch { return key; }
        }
        /// <summary>
        /// Устанавливает исходный словарь.
        /// </summary>
        /// <param name="lang">Язык.</param>
        /// <returns>Успех выполнения.</returns>
        public static bool SetSource(string lang)
        {
            // Проверка параметров
            if (lang == null)
                return false;

            // Определение культуры
            CultureInfo? culture = null;
            try { culture = CultureInfo.GetCultureInfo(lang); }
            catch { }
            culture ??= CultureInfo.GetCultureInfo(DefaultLang);

            // Поиск словаря
            if (!Dictionaries.TryGetValue(culture.Name, out LocalizationDictionary? source))
                return false;

            // Установка словаря и флага
            SourceDictionary = source;
            foreach (KeyValuePair<string, LocalizationDictionary> pair in Dictionaries)
                pair.Value.IsSource = pair.Key == culture.Name;

            // Возврат
            return true;
        }
    }
}