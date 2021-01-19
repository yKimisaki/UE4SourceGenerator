using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UE4SourceGenerator.Model
{
    public partial class TemplateCollector
    {
        Dictionary<string, ITemplate> headerTemplates = new();
        public IReadOnlyDictionary<string, ITemplate> HeaderTemplates => headerTemplates;
        Dictionary<string, ITemplate> sourceTemplates = new();
        public IReadOnlyDictionary<string, ITemplate> SourceTemplates => sourceTemplates;

        public void CollectTemplates(string searchDirectory)
        {
            headerTemplates.Clear();
            sourceTemplates.Clear();

            CollectTemplatesCore(Directory.GetCurrentDirectory());
            for (var d = searchDirectory; d != Directory.GetDirectoryRoot(d); d = Directory.GetParent(d).FullName)
            {
                var uprojectFIles = Directory.GetFiles(d, "*.uproject");
                if (uprojectFIles.Length == 1)
                {
                    CollectTemplates(d);
                    break;
                }
            }
        }

        void CollectTemplatesCore(string parentDir)
        {
            var templatesPath = Path.Combine(parentDir, Constants.TemplateDirectoryName);
            if (Directory.Exists(templatesPath))
            {
                var templateHeaderFiles = Directory.GetFiles(templatesPath, "*.h.txt");
                foreach (var templateFile in templateHeaderFiles)
                {
                    var templateFileName = Path.GetFileName(templateFile).Split('.')[0];
                    var templateContent = File.ReadAllText(templateFile, Encoding.UTF8);

                    headerTemplates.Add(templateFileName, new HeaderTemplate(GetHeaderType(templateFileName), templateContent));
                }

                var templateSourceFiles = Directory.GetFiles(templatesPath, "*.cpp.txt");
                foreach (var templateFile in templateSourceFiles)
                {
                    var templateFileName = Path.GetFileName(templateFile).Split('.')[0];
                    var templateContent = File.ReadAllText(templateFile, Encoding.UTF8);

                    sourceTemplates.Add(templateFileName, new SourceTemplate(templateContent));
                }
            }
        }
    }
}
