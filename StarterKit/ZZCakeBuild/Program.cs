using Cake.Frosting;

namespace CakeBuild
{
    /// <summary>
    /// Проект-сборщик.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Точка входа сборщика.
        /// </summary>
        /// <param name="args">Аргументы.</param>
        /// <returns>Код завершения.</returns>
        public static int Main(string[] args)
        {
            CakeHost host = new CakeHost();
            host.UseContext<BuildContext>();
            return host.Run(args);
        }
    }
}