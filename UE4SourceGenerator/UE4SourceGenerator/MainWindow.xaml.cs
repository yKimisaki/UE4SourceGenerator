using System.Windows;
using UE4SourceGenerator.ViewModel;

namespace UE4SourceGenerator
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new MainViewModel();
        }
    }
}