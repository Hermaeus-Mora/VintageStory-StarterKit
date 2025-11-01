using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Clean;
using Cake.Common.Tools.DotNet.Publish;
using Cake.Frosting;

namespace CakeBuild
{
    /// <summary>
    /// Задача построения.
    /// </summary>
    [TaskName("Build")]
    [IsDependentOn(typeof(ValidateJsonTask))]
    public sealed class BuildTask : FrostingTask<BuildContext>
    {
        /// <summary>
        /// Запускает задачу.
        /// </summary>
        /// <param name="context">Контекст.</param>
        public override void Run(BuildContext context)
        {
            string project = $"../{BuildContext.ProjectName}/{BuildContext.ProjectName}.csproj";

            DotNetCleanSettings cleanSettings = new DotNetCleanSettings();
            cleanSettings.Configuration = context.BuildConfiguration;
            context.DotNetClean(project, cleanSettings);

            DotNetPublishSettings publishSettings = new DotNetPublishSettings();
            publishSettings.Configuration = context.BuildConfiguration;
            context.DotNetPublish(project, publishSettings);
        }
    }
}