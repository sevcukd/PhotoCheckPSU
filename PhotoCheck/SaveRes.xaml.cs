using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace PhotoCheck
{
    /// <summary>
    /// Interaction logic for SaveRes.xaml
    /// </summary>
    public partial class SaveRes : Window
    {
        public ObservableCollection<Wares> ListWares { get; set; }
        public string pathToPhoto { get; set; } = @"d:\Pictures\Good\";
        public SaveRes(ObservableCollection<Wares> wares)
        {
            InitializeComponent();
            ListWares = wares;
            GC.Collect();
            MessageBox.Show(ListWares.Count().ToString());
            PathToPhotoTextBox.Text = pathToPhoto;
        }

        private void MovePhotoButton(object sender, RoutedEventArgs e)
        {
            foreach (var ware in ListWares)
            {

                try
                {
                    switch (ware.savePhotoStatus)
                    {

                        case 0: // перемітити - добре фото
                            FileInfo file = new FileInfo(ware.photoPath);
                            //MessageBox.Show(pathToPhoto + ware.photoFullName);
                            file.MoveTo(pathToPhoto + ware.photoFullName);
                            //File.Move(ware.photoPath, pathToPhoto+ware.photoFullName);
                            break;
                        case 1: // невірне фото
                            FileInfo file2 = new FileInfo(ware.photoPath);
                            file2.MoveTo(pathToPhoto + "невірне фото" + file2.Name);
                            break;
                        case 2: // невірний код
                            FileInfo file3 = new FileInfo(ware.photoPath);
                            file3.MoveTo(pathToPhoto + "невірний код" + file3.Name);
                            break;

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }


            }
            MessageBox.Show("Переміщення завершено!", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OpenToFilePath(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            pathToPhoto = dialog.SelectedPath+@"\";
            PathToPhotoTextBox.Text = pathToPhoto;
        }
    }
}
