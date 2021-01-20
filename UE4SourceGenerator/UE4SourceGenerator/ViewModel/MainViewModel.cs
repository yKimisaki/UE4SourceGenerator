using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using UE4SourceGenerator.Command;
using UE4SourceGenerator.Model;
using UE4SourceGenerator.Properties;

namespace UE4SourceGenerator.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged, IOnSelectedDirectoryListener, IOnGenerateListener
    {
        TemplateCollector templateCollector = new();
        TemplateCollector IOnGenerateListener.TemplateCollector => templateCollector;

        public SelectDirectoryCommand SelectDirectory { get; }
        public GenerateToFileCommand GenerateToFile { get; }
        public GenerateToClipboardCommand GenerateToClipboard { get; }

        string outputDirectory;
        public string OutputDirectory
        {
            get => outputDirectory;
            set
            {
                try
                {
                    outputDirectory = Settings.Default.LastSelectedDirectory = value;
                    Settings.Default.Save();

                    RaisePropertyChanged(nameof(OutputDirectory));

                    templateCollector.CollectTemplates(outputDirectory);
                    RaisePropertyChanged(nameof(ApiName));
                    RaisePropertyChanged(nameof(BaseTypes));
                }
                catch(TemplateCollectException e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        public string ApiName => templateCollector.ProjectApi;
        public IEnumerable<string> BaseTypes => templateCollector.HeaderTemplates.Keys;

        string selectedBaseType;
        public string SelectedBaseType
        {
            get => selectedBaseType;
            set
            {
                selectedBaseType = value;
                RaisePropertyChanged(nameof(SelectedBaseType));

                GenerateToClipboard.RaiseCanExecuteChanged();

                TypeName = typeName.CorrectTypeName(selectedBaseType);
            }
        }

        string typeName;
        public string TypeName
        {
            get => typeName;
            set
            {
                typeName = value;
                RaisePropertyChanged(nameof(TypeName));
            }
        }

        TemplateReplacement IOnGenerateListener.TemplateReplacement => new TemplateReplacement()
        {
            ProjectApi = templateCollector.ProjectApi,
            FileName = !string.IsNullOrWhiteSpace(typeName) && typeName.Length is > 1 ? typeName.Substring(1) : "",
            TypeName = typeName,
        };

        public event PropertyChangedEventHandler PropertyChanged;
        void RaisePropertyChanged(string propertyName)
          => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public MainViewModel()
        {
            SelectDirectory = new(this);
            GenerateToFile = new(this);
            GenerateToClipboard = new(this);

            OutputDirectory = Settings.Default.LastSelectedDirectory;
        }
    }
}