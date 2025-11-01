using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace StarterKit
{
    /// <summary>
    /// Система мода.
    /// </summary>
    public sealed class StarterKitModSystem : ModSystem
    {
        /// <summary>
        /// Название мода.
        /// </summary>
        public const string ModName = "StarterKit";

        /// <summary>
        /// Конфигурация мода.
        /// </summary>
        public StarterKitConfig? Config { get; private set; }

        /// <summary>
        /// Команда стартового набора.
        /// </summary>
        private StarterKitCommand? StarterKitCommand { get; set; }

        /// <summary>
        /// Определяет, нужно ли запускать мод.
        /// </summary>
        /// <param name="forSide">Сторона.</param>
        /// <returns>Необходимость запуска.</returns>
        public override bool ShouldLoad(EnumAppSide forSide)
        {
            return forSide == EnumAppSide.Server;
        }
        /// <summary>
        /// Обработчик события запуска мода на сервере.
        /// </summary>
        /// <param name="api">API сервера.</param>
        public override void StartServerSide(ICoreServerAPI api)
        {
            Config = new StarterKitConfig(api);
            Config.Load(true);

            StarterKitCommand = new StarterKitCommand(api, Config);
        }
    }
}