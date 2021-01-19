using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using UE4SourceGenerator.Properties;

namespace UE4SourceGenerator.Model
{
    public class MainModel
    {
        public static readonly string ValueTypeKey = "USTRUCT/UENUM";

        public string ProjectApiName { get; private set; } = "";
        public string LastSelectedDirectory { get; private set; } = "C:";

        Dictionary<string, string> headerTemplates = new();
        public IReadOnlyDictionary<string, string> HeaderTemplates => headerTemplates;

        Dictionary<string, string> sourceTemplates = new();
        public IReadOnlyDictionary<string, string> SourceTemplates => sourceTemplates;

        public MainModel()
        {
            LastSelectedDirectory = Settings.Default.LastSelectedDirectory;
            OnSelectedFolder(LastSelectedDirectory);
        }

        public void OnSelectedFolder(string lastSelectedDirectory)
        {
            LastSelectedDirectory = Settings.Default.LastSelectedDirectory = lastSelectedDirectory;
            Settings.Default.Save();

            ProjectApiName = "";
            headerTemplates.Clear();
            sourceTemplates.Clear();

            CollectTemplates(Directory.GetCurrentDirectory());

            for (var searchDirectory = lastSelectedDirectory; searchDirectory != Directory.GetDirectoryRoot(searchDirectory); searchDirectory = Directory.GetParent(searchDirectory).FullName)
            {
                var uprojectFIles = Directory.GetFiles(searchDirectory, "*.uproject");
                if (uprojectFIles.Length == 1)
                {
                    ProjectApiName = $"{Path.GetFileNameWithoutExtension(uprojectFIles[0]).ToUpper()}_API";
                    CollectTemplates(searchDirectory);
                    break;
                }
            }
        }

        void CollectTemplates(string parentDir)
        {
            var templatesPath = Path.Combine(parentDir, "SourceGeneratorTamplates");
            if (Directory.Exists(templatesPath))
            {
                var templateHeaderFiles = Directory.GetFiles(templatesPath, "*.h.txt");
                foreach (var templateFile in templateHeaderFiles)
                {
                    var templateFileName = Path.GetFileName(templateFile).Split('.')[0];
                    var templateContent = File.ReadAllText(templateFile, Encoding.UTF8);

                    headerTemplates.Add(templateFileName, templateContent);
                }

                var templateSourceFiles = Directory.GetFiles(templatesPath, "*.cpp.txt");
                foreach (var templateFile in templateSourceFiles)
                {
                    var templateFileName = Path.GetFileName(templateFile).Split('.')[0];
                    var templateContent = File.ReadAllText(templateFile, Encoding.UTF8);

                    sourceTemplates.Add(templateFileName, templateContent);
                }
            }
        }

        public void Generate(string typeName, string baseType, bool toFile)
        {
            if (string.IsNullOrWhiteSpace(typeName))
            {
                throw new SourceGenerateException("Type name is invalid.");
            }

            if (string.IsNullOrWhiteSpace(baseType))
            {
                throw new SourceGenerateException("Base type is invalid.");
            }

            var fileName = typeName.Substring(1);
            if (char.IsLower(fileName[0]))
            {
                throw new SourceGenerateException("A second char off type name must be upper case.");
            }

            var header = "";
            if (baseType == ValueTypeKey)
            {
                header = GetValueType(fileName, typeName, toFile);
            }
            else
            {
                if (typeName[0] != baseType[0])
                {
                    throw new SourceGenerateException("Type name prefix and base type prefix is not same.");
                }

                header = headerTemplates[baseType]
                    .Replace("{PROJECT_API}", ProjectApiName)
                    .Replace("{TypeName}", typeName)
                    .Replace("{FileName}", fileName);
            }
            if (toFile)
            {
                var headerFilePath = Path.Combine(LastSelectedDirectory, $"{fileName}.h");
                if (File.Exists(headerFilePath))
                {
                    throw new SourceGenerateException($"{headerFilePath} exists already.");
                }

                File.WriteAllText(headerFilePath, header, Encoding.UTF8);
            }
            else
            {
                Clipboard.SetText(header);
            }

            if (baseType != ValueTypeKey)
            {
                var source = GetDefaultSource(fileName);
                if (sourceTemplates.ContainsKey(baseType))
                {
                    source = sourceTemplates[baseType]
                        .Replace("{TypeName}", typeName)
                        .Replace("{FileName}", fileName);
                }

                var sourceFilePath = Path.Combine(LastSelectedDirectory, $"{fileName}.cpp");
                if (File.Exists(sourceFilePath))
                {
                    throw new SourceGenerateException($"{sourceFilePath} exists already.");
                }

                File.WriteAllText(sourceFilePath, source, Encoding.UTF8);
            }
        }

        string GetDefaultSource(string fileName)
        {
            return $@"#include ""{fileName}.h""";
        }

        string GetValueType(string fileName, string typeName, bool toFile)
        {
            if (typeName[0] == 'E')
            {
                if (toFile)
                {
                    return $@"#include ""CoreMinimal.h""

UENUM(BlueprintType)
enum class {typeName} : uint8
{{
    None = 0,
}};";
                }
                else
                {
                    return $@"UENUM(BlueprintType)
enum class {typeName} : uint8
{{
    None = 0,
}};";
                }
            }

            if (typeName[0] == 'F')
            {
                if (toFile)
                {
                    return $@"#include ""CoreMinimal.h""
#include ""{fileName}.generated.h""

USTRUCT(BlueprintType)
struct {ProjectApiName} {typeName}
{{
    GENERATED_BODY()
}};";
                }
                else
                {
                    return $@"USTRUCT(BlueprintType)
struct {ProjectApiName} {typeName}
{{
    GENERATED_BODY()
}};";
                }
            }

            throw new SourceGenerateException($"A prefix of {typeName} is not E ro F.");
        }
    }
}
