using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace InitialPrefabs.ImportOverrides {

    class PrimitiveGenerator {

        private const string Tab = "    ";

        [MenuItem("Tools/Generate Variables")]
        private static async void GenerateVariables() {
            // Select a path
            string path = EditorUtility.SaveFilePanel("Variables", "Assets/com.initialprefabs.importoverrides", "Variables", "cs");

            if (string.IsNullOrEmpty(path)) {
                return;
            }

            await Task.Run(() => {
                var sb = new StringBuilder(512);
                sb
                    .AppendLine("// Code generated, do not modify")
                    .AppendLine("namespace InitialPrefabs.ImportOverrides {")
                    .Append(Tab)
                    .AppendLine("public static class Variables {");
                foreach (var variable in typeof(TextureImporterSettings)
                    .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                    .Select(fieldInfo => fieldInfo.Name)
                    .OrderBy(name => name)) {

                    // Okay just write this to a file for now in a string builder.
                    sb.Append(Tab)
                        .Append(Tab)
                        .Append("public static readonly string")
                        .Append(variable)
                        .Append(" = ")
                        .Append("nameof(")
                        .Append(variable)
                        .AppendLine(");");
                }
                sb.Append(Tab)
                    .AppendLine("}")
                    .AppendLine("}");
                File.WriteAllTextAsync(path, sb.ToString());
            });
            AssetDatabase.ImportAsset(path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}

