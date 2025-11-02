using System.Collections.Generic;
using System.Globalization;

namespace StarterKit.Localization
{
    /// <summary>
    /// Словарь строк локализации.
    /// </summary>
    public sealed class LocalizationDictionary
    {
        /// <summary>
        /// Элементы локализации.
        /// </summary>
        private readonly SortedDictionary<string, string> Items;

        /// <summary>
        /// Указывает, является ли словарь исходным.
        /// </summary>
        public bool IsSource { get; internal set; }
        /// <summary>
        /// Культура.
        /// </summary>
        public CultureInfo Culture { get; }

        /// <summary>
        /// Количество строк.
        /// </summary>
        public int Count
        {
            get => Items.Count;
        }

        /// <summary>
        /// Индексатор доступа к элементам.
        /// </summary>
        /// <param name="key">Ключ.</param>
        /// <returns>Значение.</returns>
        public string? this[string? key]
        {
            get
            {
                if (key == null)
                    return null;

                return Items.TryGetValue(key, out string? value) ? value : null;
            }
            set
            {
                if (key == null)
                    return;

                if (Items.ContainsKey(key))
                {
                    if (value != null)
                        Items[key] = value;
                    else
                        Items.Remove(key);
                }
                else
                {
                    if (value != null)
                        Items.Add(key, value);
                }
            }
        }

        /// <summary>
        /// Конструктор словаря строк локализации.
        /// </summary>
        /// <param name="culture">Культура.</param>
        public LocalizationDictionary(CultureInfo culture)
        {
            IsSource = false;
            Culture = culture;

            Items = new SortedDictionary<string, string>(Comparer<string>.Default);
        }
    }
}