using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;

namespace IDFileAdder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private string InputFolderName
        {
            get { return txtFolderName.Text; }
        }

        private List<FileInfo> AllFilesInFolder { get; set; }

        private void btnMoveUp_Click(object sender, RoutedEventArgs e)
        {
            if (lstFilesInFolder.SelectedIndex > 0)
            {
                FileInfo tempFI = AllFilesInFolder[lstFilesInFolder.SelectedIndex - 1];
                AllFilesInFolder[lstFilesInFolder.SelectedIndex - 1] = AllFilesInFolder[lstFilesInFolder.SelectedIndex];
                AllFilesInFolder[lstFilesInFolder.SelectedIndex] = tempFI;

                Refresh();
            }
        }

        private void btnMoveDown_Click(object sender, RoutedEventArgs e)
        {   
            if (lstFilesInFolder.SelectedIndex+1 < lstFilesInFolder.Items.Count)
            {
                FileInfo tempFI = AllFilesInFolder[lstFilesInFolder.SelectedIndex];
                AllFilesInFolder[lstFilesInFolder.SelectedIndex] = AllFilesInFolder[lstFilesInFolder.SelectedIndex + 1];
                AllFilesInFolder[lstFilesInFolder.SelectedIndex + 1] = tempFI;

                Refresh();
            }
        }

        private void Refresh()
        {
            CollectionViewSource.GetDefaultView(lstFilesInFolder.ItemsSource).Refresh();
        }

        private void btnAddIds_Click(object sender, RoutedEventArgs e)
        {
            Directory.SetCurrentDirectory(@InputFolderName);

            int i = int.Parse(ConfigurationManager.AppSettings.Get("StartIDIndex"));

            // Ascending
            if (cmbIDOrder.SelectedIndex == 0)
            {
                foreach (FileInfo currFile in AllFilesInFolder)
                {
                    currFile.MoveTo(i.ToString() + " - " + currFile.Name);
                    i++;
                }
            }
                //Descending
            else
            {
                foreach (FileInfo currFile in AllFilesInFolder)
                {
                    currFile.MoveTo(i.ToString() + " - " + currFile.Name);
                    i--;
                }
            }

            Refresh();
        }

        private void btnDisplayFiles_Click(object sender, RoutedEventArgs e)
        {
            ResetUserMessages();

            try
            {
                if (Directory.Exists(@InputFolderName))
                {
                    DirectoryInfo currDirInfo = new DirectoryInfo(@InputFolderName);
                    AllFilesInFolder = currDirInfo.GetFiles(ConfigurationManager.AppSettings.Get("FileSearchPattern")).ToList();
                    lstFilesInFolder.DataContext = AllFilesInFolder;
                }
                else
                {
                    LogAndWriteUserMessage("Folder Not Exists");
                }
            }
            catch (Exception ex)
            {
                LogAndWriteUserMessage(ex);
            }
        }

        #region LogAndWriteUserMessage + 2 overload
        //private void CreateLoggerDirectory()
        //{
        //    string path = ConfigurationManager.AppSettings.Get("LoggerPath");
        //    int index = path.LastIndexOf(@"\");
        //    path = path.Remove(index + 1);
        //    Directory.CreateDirectory(path);
        //}

        private void LogAndWriteUserMessage(Exception ex)
        {
            LoggerWriter.Logger.Write(DateTime.Now.ToString());
            LoggerWriter.Logger.Write(ex);
            txtUserMessages.Text = "General Error - See Logger";
            txtUserMessages.Foreground = Brushes.Red;
        }

        private void LogAndWriteUserMessage(string msg)
        {
            LoggerWriter.Logger.Write(DateTime.Now.ToString());
            LoggerWriter.Logger.Write(msg);
            txtUserMessages.Text = msg;
            txtUserMessages.Foreground = Brushes.Red;
        }

        private void ResetUserMessages()
        {
            txtUserMessages.Text = string.Empty;
            txtUserMessages.Foreground = Brushes.Black;
        }
        #endregion

        private void btnMoveSelectedItem_Click(object sender, RoutedEventArgs e)
        {
            if (lstFilesInFolder.SelectedItems.Count == 2)
            {
                int firstIndex = AllFilesInFolder.IndexOf((FileInfo) lstFilesInFolder.SelectedItems[0]);
                FileInfo firstFileInfo = AllFilesInFolder[firstIndex];

                int secondIndex = AllFilesInFolder.IndexOf((FileInfo)lstFilesInFolder.SelectedItems[1]);
                FileInfo secondFileInfo = AllFilesInFolder[secondIndex];

                // Swap
                AllFilesInFolder[firstIndex] = secondFileInfo;
                AllFilesInFolder[secondIndex] = firstFileInfo;

                lstFilesInFolder.UnselectAll();

                Refresh();
            }
        }

        private void lstFilesInFolder_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (lstFilesInFolder.SelectedItems.Count == 2)
            {
                e.Handled = true; 
            }

            // just for the lazy
            if (e.ClickCount == 2)
            {
                lstFilesInFolder.UnselectAll();
            }

        }

        private void btnClearSelection_Click(object sender, RoutedEventArgs e)
        {
            lstFilesInFolder.UnselectAll();
        }
    }
}
