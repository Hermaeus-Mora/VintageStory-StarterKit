using Vintagestory.API.Server;

namespace StarterKit.Commands
{
    /// <summary>
    /// Команда.
    /// </summary>
    public abstract class Command
    {
        /// <summary>
        /// API сервера.
        /// </summary>
        protected readonly ICoreServerAPI API;
        /// <summary>
        /// Конфигурация мода.
        /// </summary>
        protected readonly StarterKitConfig Config;

        /// <summary>
        /// Конструктор команды.
        /// </summary>
        /// <param name="api">API сервера.</param>
        /// <param name="config">Конфигурация мода.</param>
        internal Command(ICoreServerAPI api, StarterKitConfig config)
        {
            API = api;
            Config = config;
        }
    }
}