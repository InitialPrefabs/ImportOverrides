#if GENERATE
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build;

namespace InitialPrefabs.ImportOverrides {

    /// <summary>
    /// A code generator for internal variables. Uses a string builder than any complex solution.
    /// </summary>
    internal class PrimitiveGenerator {

        private const string Tab = "    ";

        [MenuItem("Tools/Generate Variables")]
        private static async void GenerateVariables() {
            // Select a path
            var path = EditorUtility.SaveFilePanel("Variables", "Assets/com.initialprefabs.importoverrides/InitialPrefabs.ImportOverrides", "Variables", "cs");

            if (string.IsNullOrEmpty(path)) {
                return;
            }

            await Task.Run(async () => {
                var uniqueVariables = new HashSet<string>();
                var sb = new StringBuilder(512);
                _ = sb
                    .AppendLine("// Code generated, do not modify")
                    .AppendLine("namespace InitialPrefabs.ImportOverrides {")
                    .Append(Tab)
                    .AppendLine("public static class Variables {");

                foreach (var variable in typeof(TextureImporterSettings)
                    .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                    .Select(fieldInfo => fieldInfo.Name)
                    .OrderBy(name => name)) {

                    if (uniqueVariables.Add(variable)) {
                        // Okay just write this to a file for now in a string builder.
                        _ = sb.Append(Tab)
                            .Append(Tab)
                            .Append("public static readonly string ")
                            .Append(variable)
                            .Append(" = ")
                            .Append("nameof(")
                            .Append(variable)
                            .AppendLine(");");
                    }
                }

                foreach (var variable in typeof(TextureImporterPlatformSettings)
                    .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                    .Select(fieldInfo => fieldInfo.Name)
                    .OrderBy(name => name)) {

                    if (uniqueVariables.Add(variable)) {
                        _ = sb.Append(Tab)
                            .Append(Tab)
                            .Append("public static readonly string ")
                            .Append(variable)
                            .Append(" = ")
                            .Append("nameof(")
                            .Append(variable)
                            .AppendLine(");");
                    }
                }

                _ = sb.Append(Tab)
                    .AppendLine("}")
                    .AppendLine("}");
                await File.WriteAllTextAsync(path, sb.ToString());
            });
            AssetDatabase.ImportAsset(path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/Generate Build Targets Enum")]
        private static async void GenerateBuildTargetsEnum() {
            var path = EditorUtility.SaveFilePanel(
                "KnownBuildTargets", "Assets/com.initialprefabs.importoverrides", "KnownBuildTargets", "cs");
            UnityEngine.Debug.Log(path);
            if (string.IsNullOrEmpty(path)) {
                return;
            }

            await Task.Run(async () => {
                var sb = new StringBuilder(512);
                _ = sb.AppendLine("// Code generated, do not modify")
                    .AppendLine("namespace InitialPrefabs.ImportOverrides {")
                    .Append(Tab)
                    .AppendLine("public enum KnownBuildTargets {");

                foreach (var staticVariable in typeof(NamedBuildTarget)
                    .GetFields(BindingFlags.Public | BindingFlags.Static)) {

                    var isObsolete = staticVariable.CustomAttributes
                        .Any(customAttribute => customAttribute.AttributeType == typeof(ObsoleteAttribute));
                    if (isObsolete) {
                        continue;
                    }

                    _ = sb.Append(Tab).Append(Tab)
                        .Append(staticVariable.Name)
                        .AppendLine(",");
                }
                _ = sb.Append(Tab)
                    .AppendLine("}")
                    .AppendLine("}");

                await File.WriteAllTextAsync(path, sb.ToString());
            });
            AssetDatabase.ImportAsset(path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
#endif