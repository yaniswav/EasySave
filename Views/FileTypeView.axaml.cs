using Avalonia.Controls;
using System.Threading.Tasks;
using Avalonia.VisualTree;


namespace EasySave
{
    public partial class FileTypeView : Window
    {
        public string SelectedPath { get; private set; }
        public bool IsFileSelected { get; private set; }

        public FileTypeView()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
        

        
        private async void SelectFolderButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog();
            var result = await dialog.ShowAsync(new Window());
            if (!string.IsNullOrEmpty(result))
            {
                SelectedPath = result;
                IsFileSelected = false;
                this.Close();
            }
        }

        private async void SelectFileButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.AllowMultiple = false;
            var result = await dialog.ShowAsync(new Window());
            if (result != null && result.Length > 0)
            {
                SelectedPath = result[0];
                IsFileSelected = true;
                this.Close();
            }
        }
    }
}