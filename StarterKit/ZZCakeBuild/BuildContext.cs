using Cake.Common;
using Cake.Core;
using Cake.Frosting;
using Cake.Json;
using Vintagestory.API.Common;

namespace CakeBuild
{
    /// <summary>
    /// Контекст построителя сборки.
    /// </summary>
    public class BuildContext : FrostingContext
    {
        /// <summary>
        /// Имя проекта.
        /// </summary>
        public const string ProjectName = "StarterKit";

        /// <summary>
        /// Конфигурация сборки.
        /// </summary>
        public string BuildConfiguration { get; }
        /// <summary>
        /// Версия.
        /// </summary>
        public string Version { get; }
        /// <summary>
        /// Имя.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Пропускать JSON-валидацию.
        /// </summary>
        public bool SkipJsonValidation { get; }

        /// <summary>
        /// Контруктор построителя сборки.
        /// </summary>
        /// <param name="context">Контекст.</param>
        public BuildContext(ICakeContext context) : base(context)
        {
            BuildConfiguration = context.Argument("configuration", "Release");
            SkipJsonValidation = context.Argument("skipJsonValidation", false);
            ModInfo modInfo = context.DeserializeJsonFromFile<ModInfo>($"../{ProjectName}/modinfo.json");
            Version = modInfo.Version;
            Name = modInfo.ModID;
        }
    }
}