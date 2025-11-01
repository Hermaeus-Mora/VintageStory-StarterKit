using Cake.Frosting;

namespace CakeBuild
{
    /// <summary>
    /// Задача по умолчанию.
    /// </summary>
    [TaskName("Default")]
    [IsDependentOn(typeof(PackageTask))]
    public class DefaultTask : FrostingTask
    {
    }
}