using Vintagestory.API.Server;

namespace StarterKit.Extensions
{
    /// <summary>
    /// Набор методов-расширений для <see cref="IServerPlayer"/>.
    /// </summary>
    public static class IServerPlayerExtensions
    {
        /// <summary>
        /// Получает класс игрока.
        /// </summary>
        /// <param name="player">Игрок.</param>
        /// <returns>Класс игрока или null.</returns>
        public static string? GetClass(this IServerPlayer player)
        {
            if (player.Entity == null)
                return null;
            if (player.Entity.WatchedAttributes == null)
                return null;

            string? @class = null;
            try { @class = player.Entity.WatchedAttributes.GetString("characterClass"); }
            catch { }

            return @class;
        }
    }
}