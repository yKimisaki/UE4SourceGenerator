using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using UE4SourceGenerator.Model;

namespace UE4SourceGenerator.Command
{
    public interface IOnGenerateListener
    {
        string OutputDirectory { get; }
        string SelectedBaseType { get; }
        TemplateReplacement TemplateReplacement { get; }
        TemplateCollector TemplateCollector { get; }
    }

    public class GenerateToFileCommand : ICommand
    {
        IOnGenerateListener listener;

        public GenerateToFileCommand(IOnGenerateListener listener)
        {
            this.listener = listener;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            try {
                if (listener.TemplateCollector.HeaderTemplates.TryGetValue(listener.SelectedBaseType, out var headerTemplate))
                {
                    var header = headerTemplate.Generate(listener.TemplateReplacement, GenerateTo.File);
                    var headerFilePath = Path.Combine(listener.OutputDirectory, listener.TemplateReplacement.FileName + ".h");
                    if (File.Exists(headerFilePath))
                    {
                        throw new SourceGenerateException($"{headerFilePath} exists already.");
                    }

                    if (listener.TemplateCollector.SourceTemplates.TryGetValue(listener.SelectedBaseType, out var sourceTemplate))
                    {
                        var source = sourceTemplate.Generate(listener.TemplateReplacement, GenerateTo.File);
                        var sourceFilePath = Path.Combine(listener.OutputDirectory, listener.TemplateReplacement.FileName + ".cpp");
                        if (File.Exists(sourceFilePath))
                        {
                            throw new SourceGenerateException($"{sourceFilePath} exists already.");
                        }

                        File.WriteAllText(sourceFilePath, source, Encoding.UTF8);
                    }

                    File.WriteAllText(headerFilePath, header, Encoding.UTF8);

                    MessageBox.Show($"Succeeded.");
                }
            }
            catch (SourceGenerateException e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
