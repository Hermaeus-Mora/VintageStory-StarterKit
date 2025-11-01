using Cake.Common.IO;
using Cake.Core.IO;
using Cake.Frosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace CakeBuild
{
    /// <summary>
    /// Задача валидаци JSON.
    /// </summary>
    [TaskName("ValidateJson")]
    public sealed class ValidateJsonTask : FrostingTask<BuildContext>
    {
        /// <summary>
        /// Запускает задачу.
        /// </summary>
        /// <param name="context">Контекст</param>
        /// <exception cref="Exception"></exception>
        public override void Run(BuildContext context)
        {
            if (context.SkipJsonValidation)
                return;

            FilePathCollection jsonFiles = context.GetFiles($"../{BuildContext.ProjectName}/assets/**/*.json");
            foreach (FilePath file in jsonFiles)
            {
                try
                {
                    string json = File.ReadAllText(file.FullPath);
                    JToken.Parse(json);
                }
                catch (JsonException ex)
                {
                    throw new Exception($"Validation failed for JSON file: {file.FullPath}{Environment.NewLine}{ex.Message}", ex);
                }
            }
        }
    }
}