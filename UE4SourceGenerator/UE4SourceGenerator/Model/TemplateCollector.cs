using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UE4SourceGenerator.Model
{
    public partial class TemplateCollector
    {
        public string ProjectApi { get; private set; } = "";

        Dictionary<string, ITemplate> headerTemplates = new();
        public IReadOnlyDictionary<string, ITemplate> HeaderTemplates => headerTemplates;
        Dictionary<string, ITemplate> sourceTemplates = new();
        public IReadOnlyDictionary<string, ITemplate> SourceTemplates => sourceTemplates;

        public void CollectTemplates(string searchDirectory)
        {
            headerTemplates.Clear();
            sourceTemplates.Clear();

            headerTemplates.Add(Constants.StructTypeKey, HeaderTemplate.StructTemplate);
            headerTemplates.Add(Constants.EnumTypeKey, HeaderTemplate.EnumTemplate);

            CollectTemplatesCore(Directory.GetCurrentDirectory());
            for (var d = searchDirectory; d != Directory.GetDirectoryRoot(d); d = Directory.GetParent(d)?.FullName ?? "")
            {
                var moduleFiles = Directory.GetFiles(d, "*.uproject");
                if (!moduleFiles.Any())
                {
                    moduleFiles = Directory.GetFiles(d, "*.uplugin");
                }
                if (moduleFiles.Any())
                {
                    ProjectApi = $"{Path.GetFileNameWithoutExtension(moduleFiles[0]).ToUpper()}_API";
                    CollectTemplatesCore(d);
                    break;
                }
            }
        }

        void CollectTemplatesCore(string parentDir)
        {
            var templatesPath = Path.Combine(parentDir, Constants.TemplateDirectoryName);
            if (Directory.Exists(templatesPath))
            {
                var templateSourceFiles = Directory.GetFiles(templatesPath, "*.cpp.txt");
                foreach (var templateFile in templateSourceFiles)
                {
                    var templateFileName = Path.GetFileName(templateFile).Split('.')[0];
                    var templateContent = File.ReadAllText(templateFile, Encoding.UTF8);

                    sourceTemplates.Add(templateFileName, new SourceTemplate(templateContent));
                }

                var templateHeaderFiles = Directory.GetFiles(templatesPath, "*.h.txt");
                foreach (var templateFile in templateHeaderFiles)
                {
                    var templateFileName = Path.GetFileName(templateFile).Split('.')[0];
                    var templateContent = File.ReadAllText(templateFile, Encoding.UTF8);
                    headerTemplates.Add(templateFileName, new HeaderTemplate(GetHeaderType(templateFileName), templateContent));

                    if (templateFileName.HasObjectTypePrefix() || templateFileName.HasActorTypePrefix())
                    {
                        if (!sourceTemplates.ContainsKey(templateFileName))
                        {
                            sourceTemplates.Add(templateFileName, SourceTemplate.DefaultSourceTemplate);
                        }
                    }
                }
            }
        }
    }
}
