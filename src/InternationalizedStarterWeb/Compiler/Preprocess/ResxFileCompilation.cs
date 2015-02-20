using System;
using System.IO;
using System.Resources;
using System.Xml.Linq;
using Microsoft.Framework.FileSystemGlobbing;
using Microsoft.Framework.Runtime.FileGlobbing;
using Microsoft.Framework.Runtime.Roslyn;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InternationalizedStarterWeb.Compiler.Preprocess
{
    public class ResxFileCompilation : ICompileModule
    {
        public static readonly string[] DefaultExcludePatterns = new[] { @"obj\**\*.*", @"bin\**\*.*" };

        public void BeforeCompile(IBeforeCompileContext context)
        {
            JObject projectFile = null;
            using (var fs = File.OpenRead(context.ProjectContext.ProjectFilePath))
            {
                projectFile = JObject.Load(new JsonTextReader(new StreamReader(fs)));
            }
            
            var patterns = PatternsCollectionHelper.GetPatternsCollection(
                projectFile,
                context.ProjectContext.ProjectDirectory,
                "exclude",
                DefaultExcludePatterns);
            
            var matcher = new Matcher();
            matcher.AddInclude("**/*.resx").AddExcludePatterns(patterns);
            
            var resXFiles = matcher.GetResultsInFullPath(context.ProjectContext.ProjectDirectory);

            foreach (var resXFile in resXFiles)
            {
                WriteResourceFile(resXFile);
            }
        }

        public void AfterCompile(IAfterCompileContext context)
        {
            
        }

        private static void WriteResourceFile(string resxFilePath)
        {
            using (var fs = File.OpenRead(resxFilePath))
            {
                var document = XDocument.Load(fs);

                // Put in "bin" sub-folder of resx file
                var targetPath = Path.Combine(
                    Path.GetDirectoryName(resxFilePath),
                    "bin",
                    Path.ChangeExtension(
                        Path.GetFileName(resxFilePath), ".resources"));

                using (var targetStream = File.Create(targetPath))
                {
                    var rw = new ResourceWriter(targetStream);

                    foreach (var e in document.Root.Elements("data"))
                    {
                        var name = e.Attribute("name").Value;
                        var value = e.Element("value").Value;

                        rw.AddResource(name, value);
                    }

                    rw.Generate();
                }
            }
        }

    }
}