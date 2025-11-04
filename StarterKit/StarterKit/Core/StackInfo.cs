namespace StarterKit.Core
{
    /// <summary>
    /// Информация о стаке предметов.
    /// </summary>
    public sealed class StackInfo
    {
        /// <summary>
        /// Код предмета.
        /// </summary>
        public string? Code;
        /// <summary>
        /// Количество.
        /// </summary>
        public int Amount;
        /// <summary>
        /// Класс, которому предназначен стак.
        /// </summary>
        public string? ForClass;
        /// <summary>
        /// Атрибуты стака предметов.
        /// </summary>
        public StackAttribute[]? Attributes;
    }
}