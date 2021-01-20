using System;
using System.Windows.Input;
using UE4SourceGenerator.Model;
using WindowsApi = Microsoft.WindowsAPICodePack;

namespace UE4SourceGenerator.Command
{
    public interface IOnSelectedDirectoryListener
    {
        string OutputDirectory { set; }
    }

    public class SelectDirectoryCommand : ICommand
    {
        IOnSelectedDirectoryListener listener;

        public SelectDirectoryCommand(IOnSelectedDirectoryListener listener)
        {
            this.listener = listener;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            var dialog = new WindowsApi::Dialogs.CommonOpenFileDialog();

            dialog.IsFolderPicker = true;
            dialog.Title = "Select directory";
            dialog.InitialDirectory = Properties.Settings.Default.LastSelectedDirectory;

            if (dialog.ShowDialog() == WindowsApi::Dialogs.CommonFileDialogResult.Ok)
            {
                listener.OutputDirectory = dialog.FileName;
            }
        }
    }
}
