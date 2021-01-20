using System;
using System.Windows;
using System.Windows.Input;
using UE4SourceGenerator.Model;

namespace UE4SourceGenerator.Command
{
    public class GenerateToClipboardCommand : ICommand
    {
        IOnGenerateListener listener;

        public GenerateToClipboardCommand(IOnGenerateListener listener)
        {
            this.listener = listener;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => listener.SelectedBaseType.IsValueTypeKey();

        public void Execute(object parameter)
        {
            try
            {
                if (listener.TemplateCollector.HeaderTemplates.TryGetValue(listener.SelectedBaseType, out var headerTemplate))
                {
                    var header = headerTemplate.Generate(listener.TemplateReplacement, GenerateTo.File);

                    Clipboard.SetText(header);

                    MessageBox.Show($"Succeeded.");
                }
            }
            catch (SourceGenerateException e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
