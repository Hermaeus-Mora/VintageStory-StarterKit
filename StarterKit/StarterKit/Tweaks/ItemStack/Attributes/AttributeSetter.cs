using StarterKit.Localization;
using System;
using System.Collections.Generic;
using Vintagestory.API.Server;
using C = Vintagestory.API.Common;

namespace StarterKit.Tweaks.ItemStack.Attributes
{
    /// <summary>
    /// Установщик атрибутов.
    /// </summary>
    public static class AttributeSetter
    {
        /// <summary>
        /// Словарь сеттеров.
        /// </summary>
        private static readonly Dictionary<Type, Action<C.ItemStack, string, object>> Setters;

        /// <summary>
        /// Статический конструктор.
        /// </summary>
        static AttributeSetter()
        {
            Setters = new Dictionary<Type, Action<C.ItemStack, string, object>>(7);
            Setters.Add(typeof(bool), SetBool);
            Setters.Add(typeof(int), SetInt32);
            Setters.Add(typeof(long), SetInt64);
            Setters.Add(typeof(float), SetFloat);
            Setters.Add(typeof(double), SetDouble);
            Setters.Add(typeof(byte[]), SetBytes);
            Setters.Add(typeof(string), SetString);
        }

        /// <summary>
        /// Добавляет атрибут в соответствии с типом.
        /// </summary>
        /// <param name="stack">Стак предметов.</param>
        /// <param name="key">Ключ.</param>
        /// <param name="value">Значение.</param>
        /// <returns>Успех выполнения.</returns>
        public static bool Set(C.ItemStack stack, string? key, object? value, ICoreServerAPI api)
        {
            if (stack == null)
                return false;
            if (key == null)
                return false;
            if (value == null)
                return false;

            if (!Setters.TryGetValue(value.GetType(), out Action<C.ItemStack, string, object>? set))
                return false;

            try
            {
                set(stack, key, value);
            }
            catch (Exception ex)
            {
                api.Logger.Error(Localizer.Get("Attributes.SetException", key));
                api.Logger.Error(ex);
                return false;
            }
            return true;
        }
        /// <summary>
        /// Добавляет атрибут в соответствии с типом.
        /// </summary>
        /// <param name="stack">Стак предметов.</param>
        /// <param name="key">Ключ.</param>
        /// <param name="value">Значение.</param>
        private static void SetBool(C.ItemStack stack, string key, object value)
        {
            stack.Attributes.SetBool(key, (bool)value);
        }
        /// <summary>
        /// Добавляет атрибут в соответствии с типом.
        /// </summary>
        /// <param name="stack">Стак предметов.</param>
        /// <param name="key">Ключ.</param>
        /// <param name="value">Значение.</param>
        private static void SetInt32(C.ItemStack stack, string key, object value)
        {
            stack.Attributes.SetInt(key, (int)value);
        }
        /// <summary>
        /// Добавляет атрибут в соответствии с типом.
        /// </summary>
        /// <param name="stack">Стак предметов.</param>
        /// <param name="key">Ключ.</param>
        /// <param name="value">Значение.</param>
        private static void SetInt64(C.ItemStack stack, string key, object value)
        {
            stack.Attributes.SetLong(key, (long)value);
        }
        /// <summary>
        /// Добавляет атрибут в соответствии с типом.
        /// </summary>
        /// <param name="stack">Стак предметов.</param>
        /// <param name="key">Ключ.</param>
        /// <param name="value">Значение.</param>
        private static void SetFloat(C.ItemStack stack, string key, object value)
        {
            stack.Attributes.SetFloat(key, (float)value);
        }
        /// <summary>
        /// Добавляет атрибут в соответствии с типом.
        /// </summary>
        /// <param name="stack">Стак предметов.</param>
        /// <param name="key">Ключ.</param>
        /// <param name="value">Значение.</param>
        private static void SetDouble(C.ItemStack stack, string key, object value)
        {
            stack.Attributes.SetDouble(key, (double)value);
        }
        /// <summary>
        /// Добавляет атрибут в соответствии с типом.
        /// </summary>
        /// <param name="stack">Стак предметов.</param>
        /// <param name="key">Ключ.</param>
        /// <param name="value">Значение.</param>
        private static void SetBytes(C.ItemStack stack, string key, object value)
        {
            stack.Attributes.SetBytes(key, (byte[])value);
        }
        /// <summary>
        /// Добавляет атрибут в соответствии с типом.
        /// </summary>
        /// <param name="stack">Стак предметов.</param>
        /// <param name="key">Ключ.</param>
        /// <param name="value">Значение.</param>
        private static void SetString(C.ItemStack stack, string key, object value)
        {
            stack.Attributes.SetString(key, (string)value);
        }
    }
}