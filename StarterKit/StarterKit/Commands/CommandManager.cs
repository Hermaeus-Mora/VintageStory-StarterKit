using Vintagestory.API.Server;

namespace StarterKit.Commands
{
    /// <summary>
    /// Менеджер команд.
    /// </summary>
    public sealed class CommandManager
    {
        /// <summary>
        /// API сервера.
        /// </summary>
        private readonly ICoreServerAPI API;
        /// <summary>
        /// Конфигурация мода.
        /// </summary>
        private readonly StarterKitConfig Config;

        /// <summary>
        /// Команда стартового набора.
        /// </summary>
        public StarterKitCommand StarterKitCommand { get; }

        /// <summary>
        /// Конструктор команды.
        /// </summary>
        /// <param name="api">API сервера.</param>
        /// <param name="config">Конфигурация мода.</param>
        internal CommandManager(ICoreServerAPI api, StarterKitConfig config)
        {
            API = api;
            Config = config;

            StarterKitCommand = new StarterKitCommand(API, Config);
        }
    }
}