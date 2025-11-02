using StarterKit.Commands;
using StarterKit.Localization;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
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
        /// Менеджер команд.
        /// </summary>
        public CommandManager? CommandManager { get; private set; }

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
            // Инициализация словарей
            Localizer.Initialize();
            Localizer.SetSource(Lang.CurrentLocale);

            // Инициализация и загрузка конфигурации
            Config = new StarterKitConfig(api);
            Config.Load(true);

            // Инициализация команд
            CommandManager = new CommandManager(api, Config);
        }
    }
}