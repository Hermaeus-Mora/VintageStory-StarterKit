using System;
using System.Collections.Generic;

namespace StarterKit.Tweaks.ItemStack.Attributes
{
    /// <summary>
    /// Конвертер значений атрибутов.
    /// </summary>
    public static class FromStringConverter
    {
        /// <summary>
        /// Конвертеры значений.
        /// </summary>
        private static readonly Dictionary<string, Func<string?, object?>> Converters;

        /// <summary>
        /// Статический конструктор.
        /// </summary>
        static FromStringConverter()
        {
            Converters = new Dictionary<string, Func<string?, object?>>(7);
            Converters.Add("bool", ConvertToBool);
            Converters.Add("byte[]", ConvertToBytes);
            Converters.Add("int", ConvertToInt32);
            Converters.Add("long", ConvertToInt64);
            Converters.Add("float", ConvertToFloat);
            Converters.Add("double", ConvertToDouble);
            Converters.Add("string", ConvertToString);
        }

        /// <summary>
        /// Конвертирует значение согласно типу.
        /// </summary>
        /// <param name="type">Тип.</param>
        /// <param name="value">Значение.</param>
        /// <returns>Конвертированное значение.</returns>
        public static object? Convert(string type, string value)
        {
            // Некорректный тип
            if (!Converters.TryGetValue(type, out Func<string?, object?>? convert))
                return null;

            // Конвертация
            return convert(value);
        }
        /// <summary>
        /// Конвертирует значение в <see cref="bool"/>.
        /// </summary>
        /// <param name="value">Текстовое значение.</param>
        /// <returns>Конвертированное значение.</returns>
        private static object? ConvertToBool(string? value)
        {
            return bool.TryParse(value, out bool result) ? result : null;
        }
        /// <summary>
        /// Конвертирует значение в <see cref="int"/>.
        /// </summary>
        /// <param name="value">Текстовое значение.</param>
        /// <returns>Конвертированное значение.</returns>
        private static object? ConvertToInt32(string? value)
        {
            return int.TryParse(value, out int result) ? result : null;
        }
        /// <summary>
        /// Конвертирует значение в <see cref="long"/>.
        /// </summary>
        /// <param name="value">Текстовое значение.</param>
        /// <returns>Конвертированное значение.</returns>
        private static object? ConvertToInt64(string? value)
        {
            return long.TryParse(value, out long result) ? result : null;
        }
        /// <summary>
        /// Конвертирует значение в <see cref="float"/>.
        /// </summary>
        /// <param name="value">Текстовое значение.</param>
        /// <returns>Конвертированное значение.</returns>
        private static object? ConvertToFloat(string? value)
        {
            return float.TryParse(value, out float result) ? result : null;
        }
        /// <summary>
        /// Конвертирует значение в <see cref="double"/>.
        /// </summary>
        /// <param name="value">Текстовое значение.</param>
        /// <returns>Конвертированное значение.</returns>
        private static object? ConvertToDouble(string? value)
        {
            return double.TryParse(value, out double result) ? result : null;
        }
        /// <summary>
        /// Конвертирует значение в <see cref="byte"/>[].
        /// </summary>
        /// <param name="value">Текстовое значение.</param>
        /// <returns>Конвертированное значение.</returns>
        private static object? ConvertToBytes(string? value)
        {
            if (value == null)
                return null;

            try { return System.Convert.FromHexString(value); }
            catch { return null; }
        }
        /// <summary>
        /// Конвертирует значение в <see cref="string"/>.
        /// </summary>
        /// <param name="value">Текстовое значение.</param>
        /// <returns>Конвертированное значение.</returns>
        private static object? ConvertToString(string? value)
        {
            return value;
        }
    }
}