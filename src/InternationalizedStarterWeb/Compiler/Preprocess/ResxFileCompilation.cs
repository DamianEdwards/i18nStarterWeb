using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Xml.Linq;
using Microsoft.Framework.FileSystemGlobbing;
using Microsoft.Framework.Runtime;
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
            JObject rawProjectFile = null;
            using (var fs = File.OpenRead(context.ProjectContext.ProjectFilePath))
            {
                rawProjectFile = JObject.Load(new JsonTextReader(new StreamReader(fs)));
            }

            var excludePatterns = PatternsCollectionHelper.GetPatternsCollection(
                rawProjectFile,
                context.ProjectContext.ProjectDirectory,
                context.ProjectContext.ProjectFilePath,
                "exclude",
                DefaultExcludePatterns);

            var matcher = new Matcher();
            matcher.AddInclude("**/*.resx").AddExcludePatterns(excludePatterns);

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

                var binDirPath = Path.Combine(Path.GetDirectoryName(resxFilePath), "bin");
                if (!Directory.Exists(binDirPath))
                {
                    Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(resxFilePath), "bin"));
                }

                // Put in "bin" sub-folder of resx file
                var targetPath = Path.Combine(binDirPath, Path.ChangeExtension(Path.GetFileName(resxFilePath), ".resources"));
                
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

        // Copied from https://github.com/aspnet/XRE/blob/4da679c078c1f2401577b7b4158cf5e5a07e54d8/src/Microsoft.Framework.Runtime.Hosting/PatternsCollectionHelper.cs
        private static class PatternsCollectionHelper
        {
            private static readonly char[] PatternSeparator = new[] { ';' };

            public static IEnumerable<string> GetPatternsCollection(JObject rawProject, string projectDirectory, string projectFilePath, string propertyName, IEnumerable<string> defaultPatterns = null)
            {
                var token = rawProject[propertyName];

                return GetPatternsCollection(token, projectDirectory, projectFilePath, defaultPatterns);
            }

            public static IEnumerable<string> GetPatternsCollection(JToken token, string projectDirectory, string projectFilePath, IEnumerable<string> defaultPatterns = null)
            {
                defaultPatterns = defaultPatterns ?? Enumerable.Empty<string>();

                if (token == null)
                {
                    return defaultPatterns;
                }

                try
                {
                    if (token.Type == JTokenType.Null)
                    {
                        return CreateCollection(projectDirectory);
                    }

                    if (token.Type == JTokenType.String)
                    {
                        return CreateCollection(projectDirectory, token.Value<string>());
                    }

                    if (token.Type == JTokenType.Array)
                    {
                        return CreateCollection(projectDirectory, token.ValueAsArray<string>());
                    }
                }
                catch (InvalidOperationException ex)
                {
                    throw FileFormatException.Create(ex, token, projectFilePath);
                }

                throw FileFormatException.Create("Value must be either string or array.", token, projectFilePath);
            }

            private static IEnumerable<string> CreateCollection(string projectDirectory, params string[] patternsStrings)
            {
                var patterns = patternsStrings.SelectMany(patternsString => GetSourcesSplit(patternsString));

                foreach (var pattern in patterns)
                {
                    if (Path.IsPathRooted(pattern))
                    {
                        throw new InvalidOperationException(string.Format("Patten {0} is a rooted path, which is not supported.", pattern));
                    }
                }

                return new List<string>(patterns.Select(pattern => FolderToPattern(pattern, projectDirectory)));
            }

            private static IEnumerable<string> GetSourcesSplit(string sourceDescription)
            {
                if (string.IsNullOrEmpty(sourceDescription))
                {
                    return Enumerable.Empty<string>();
                }

                return sourceDescription.Split(PatternSeparator, StringSplitOptions.RemoveEmptyEntries);
            }

            private static string FolderToPattern(string candidate, string projectDir)
            {
                // This conversion is needed to support current template

                // If it's already a pattern, no change is needed
                if (candidate.Contains('*'))
                {
                    return candidate;
                }

                // If the given string ends with a path separator, or it is an existing directory
                // we convert this folder name to a pattern matching all files in the folder
                if (candidate.EndsWith(@"\") ||
                    candidate.EndsWith("/") ||
                    Directory.Exists(Path.Combine(projectDir, candidate)))
                {
                    return Path.Combine(candidate, "**", "*");
                }

                // Otherwise, it represents a single file
                return candidate;
            }
        }
    }
}