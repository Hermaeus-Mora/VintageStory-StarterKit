using Cake.Common.IO;
using Cake.Frosting;

namespace CakeBuild
{
    /// <summary>
    /// Задача паковки.
    /// </summary>
    [TaskName("Package")]
    [IsDependentOn(typeof(BuildTask))]
    public sealed class PackageTask : FrostingTask<BuildContext>
    {
        /// <summary>
        /// Запускает задачу.
        /// </summary>
        /// <param name="context">Контекст.</param>
        public override void Run(BuildContext context)
        {
            context.EnsureDirectoryExists("../Releases");
            context.CleanDirectory("../Releases");
            context.EnsureDirectoryExists($"../Releases/{context.Name}");
            context.CopyFiles($"../{BuildContext.ProjectName}/bin/{context.BuildConfiguration}/Mods/mod/publish/*", $"../Releases/{context.Name}");

            if (context.DirectoryExists($"../{BuildContext.ProjectName}/assets"))
                context.CopyDirectory($"../{BuildContext.ProjectName}/assets", $"../Releases/{context.Name}/assets");

            context.CopyFile($"../{BuildContext.ProjectName}/modinfo.json", $"../Releases/{context.Name}/modinfo.json");
            if (context.FileExists($"../{BuildContext.ProjectName}/modicon.png"))
                context.CopyFile($"../{BuildContext.ProjectName}/modicon.png", $"../Releases/{context.Name}/modicon.png");

            context.Zip($"../Releases/{context.Name}", $"../Releases/{context.Name}_{context.Version}.zip");
        }
    }
}