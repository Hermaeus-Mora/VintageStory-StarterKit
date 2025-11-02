using StarterKit.Tweaks.ItemStack.Attributes;

namespace StarterKit.Core
{
    /// <summary>
    /// Атрибут стака предметов.
    /// </summary>
    public sealed class StackAttribute
    {
        /// <summary>
        /// Указывает, была ли произведена конвертация значения.
        /// </summary>
        private bool IsConverted;
        /// <summary>
        /// Конвертированное значение.
        /// </summary>
        private object? ConvertedValue;

        /// <summary>
        /// Тип значения.
        /// </summary>
        public string? Type;
        /// <summary>
        /// Ключ.
        /// </summary>
        public string? Key;
        /// <summary>
        /// Значение.
        /// </summary>
        public string? Value;

        /// <summary>
        /// Конвертирует значение согласно типу.
        /// </summary>
        /// <returns>Конвертированное значение.</returns>
        public object? GetConverted()
        {
            // Кэшированное значение
            if (IsConverted)
                goto ReturnCopy;

            // Тип или значение не указаны
            if (Type == null || Value == null)
            {
                IsConverted = true;
                ConvertedValue = null;
                return ConvertedValue;
            }

            // Конвертация
            IsConverted = true;
            ConvertedValue = FromStringConverter.Convert(Type, Value);

        ReturnCopy:
            if (ConvertedValue is byte[] array)
                return array.Clone();
            return ConvertedValue;
        }
    }
}