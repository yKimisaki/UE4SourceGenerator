using System.Linq;
using System.Windows;
using UE4SourceGenerator.Model;
using MSAPI = Microsoft.WindowsAPICodePack;

namespace UE4SourceGenerator
{
    public partial class MainWindow : Window
    {
        MainModel model;

        public MainWindow()
        {
            InitializeComponent();

            model = new MainModel();
            UpdateView();
        }

        void SelectDirectory(object sender, RoutedEventArgs e)
        {
            var dlg = new MSAPI::Dialogs.CommonOpenFileDialog();

            dlg.IsFolderPicker = true;
            dlg.Title = "Select directory";
            dlg.InitialDirectory = Properties.Settings.Default.LastSelectedDirectory;

            if (dlg.ShowDialog() == MSAPI::Dialogs.CommonFileDialogResult.Ok)
            {
                model.OnSelectedFolder(dlg.FileName);
                UpdateView();
            }
        }

        void UpdateView()
        {
            var baseType = (string)BaseType.SelectedItem;
            var typeName = TypeName.Text;
            GenerateToClipboardButton.IsEnabled = (baseType == MainModel.ValueTypeKey);

            Output.Text = model.LastSelectedDirectory;
            if (model.HeaderTemplates.Keys.Any())
            {
                BaseType.ItemsSource = new[] { MainModel.ValueTypeKey }.Concat(model.HeaderTemplates.Keys);
            }
            else
            {
                BaseType.ItemsSource = null;
            }
            ApiName.Text = model.ProjectApiName;

            if (!string.IsNullOrWhiteSpace(baseType) && !string.IsNullOrWhiteSpace(typeName))
            {
                var prefix = baseType[0].ToString();

                if (baseType == MainModel.ValueTypeKey && typeName[0] != 'E')
                {
                    prefix = "F";
                }

                if (typeName.Length == 1)
                {
                    TypeName.Text = prefix;
                }
                else if (char.IsLower(typeName[1]))
                {
                    TypeName.Text = prefix + typeName;
                }
                else
                {
                    TypeName.Text = prefix + typeName.Substring(1);
                }
            }
        }

        void GenerateToFile(object sender, RoutedEventArgs e)
        {
            try
            {
                model.Generate(TypeName.Text, (string)BaseType.SelectedItem, true);
                MessageBox.Show($"Succeeded.");
            }
            catch (SourceGenerateException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch
            {
                throw;
            }
        }

        void GenerateToClipboard(object sender, RoutedEventArgs e)
        {
            try
            {
                model.Generate(TypeName.Text, (string)BaseType.SelectedItem, false);
                MessageBox.Show($"Succeeded.");
            }
            catch (SourceGenerateException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch
            {
                throw;
            }
        }

        void BaseType_Selected(object sender, RoutedEventArgs e)
        {
            UpdateView();
        }
    }
}