
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using System.Data.SqlClient;
using Dapper;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace PhotoCheck
{
    /// <summary>
    /// Interaction logic for SaveRes.xaml
    /// </summary>
    public partial class SaveRes : System.Windows.Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<Wares> ListWares { get; set; }
        public string pathToPhoto { get; set; } = @"\\truenas\Public\PHOTOBANK\Medium\"; // \\truenas\Public\PHOTOBANK\Medium\   d:\Pictures\Products\
        public string pathToExel { get; set; } = @"";
        public List<SQLWares> listWares { get; set; }
        public bool isColumWrite { get; set; }
        public bool isExcelPath { get; set; }
        public bool isExcelOk
        {
            get
            {
                if (isColumWrite && isExcelPath)
                {
                    return true;
                }
                else return false;
            }
        }

        public string query1 = @"SELECT w.code_wares,w.name_wares,w.Code_Direction, w.articl FROM dbo.Wares w "; //000148259
        public string varConectionString = @"Server=10.1.0.22;Database=DW;Uid=dwreader;Pwd=DW_Reader;Connect Timeout=180;";
        public SqlConnection connection = null;
        public eTypeCommit TypeCommit { get; set; }
        public List<PhotoInfo> photoInfos { get; set; }
        public List<PhotoInfo> photoArtcl { get; set; }
        public SaveRes(List<PhotoInfo> photo)
        {
            InitializeComponent();

            //MessageBox.Show(ListWares[0].Articl);
            PathToPhotoTextBox.Text = pathToPhoto;
            PathToExelTextBox.Text = pathToExel;

            TypeCommit = eTypeCommit.Auto;
            connection = new SqlConnection(varConectionString);
            connection.Open();
            listWares = connection.Query<SQLWares>(query1).ToList();
            //System.Windows.MessageBox.Show(listWares[0].articl);
            FindPhotoToPath();

        }


        private void FindPhotoToPath()
        {
            string[] files = null;
            files = System.IO.Directory.GetFiles(pathToPhoto);
            photoInfos = new List<PhotoInfo>();



            for (int i = 0; i < files.Length; i++)
            {
                try
                {

                    photoInfos.Add(new PhotoInfo() { photoName = Path.GetFileNameWithoutExtension(files[i]), photoPath = Path.GetFullPath(files[i]), photoFullName = Path.GetFileName(files[i]) });

                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }

            }
        }
        private void OpenToFilePath(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            pathToPhoto = dialog.SelectedPath + @"\";
            PathToPhotoTextBox.Text = pathToPhoto;
        }

        private void OpenToFilePathExel(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel file (*.xlsx)|*.xlsx;*.XLSM;*.XLTX;*.XLS;*.XLT;*.xlsb|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog.ShowDialog();
            try
            {
                pathToExel = Path.GetFullPath(openFileDialog.FileName);
            }
            catch (Exception)
            {
            }

            //System.Windows.MessageBox.Show(pathToExel, "Message", MessageBoxButton.OK, MessageBoxImage.Information);
            PathToExelTextBox.Text = pathToExel;

        }

        private void FindPhoto(object sender, RoutedEventArgs e)
        {
            ListWares = new ObservableCollection<Wares>();

            FindPhotoToPath();

            //System.Windows.MessageBox.Show(photoInfos.Count().ToString());


            string pathToFile = pathToExel.ToString();
            try
            {


                //Создаём приложение.
                Microsoft.Office.Interop.Excel.Application ObjExcel = new Microsoft.Office.Interop.Excel.Application();
                //Открываем книгу.                                                                                                                                                        
                Microsoft.Office.Interop.Excel.Workbook ObjWorkBook = ObjExcel.Workbooks.Open(pathToFile, 0, false, 5, "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
                //Выбираем таблицу(лист).
                Microsoft.Office.Interop.Excel.Worksheet ObjWorkSheet;
                ObjWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)ObjWorkBook.Sheets[1];

                // Указываем номер столбца (таблицы Excel) из которого будут считываться данные.
                int numCol = Convert.ToInt32(ExcelColum.Text);

                //int numCol2 = 4;

                Range usedColumn = ObjWorkSheet.UsedRange.Columns[numCol];
                System.Array myvalues = (System.Array)usedColumn.Cells.Value2;
                string[] strArray = myvalues.OfType<object>().Select(o => o.ToString()).ToArray();

                //Range usedColumn2 = ObjWorkSheet.UsedRange.Columns[numCol2];
                //System.Array myvalues2 = (System.Array)usedColumn2.Cells.Value2;
                //string[] strArray2 = myvalues2.OfType<object>().Select(o => o.ToString()).ToArray();

                // Выходим из программы Excel.
                ObjExcel.DisplayAlerts = false;
                ObjExcel.Quit();
                try
                {
                    ObjExcel.DisplayAlerts = true;
                }
                catch (Exception)
                {
                }
                GC.Collect();

                for (int i = 0; i < strArray.Length; i++)
                {
                    int temp = strArray[i].Length;
                    switch (temp)
                    {
                        case 8:
                            break;
                        case 7:
                            strArray[i] = "0" + strArray[i];
                            break;
                        case 6:
                            strArray[i] = "00" + strArray[i];
                            break;
                        case 5:
                            strArray[i] = "000" + strArray[i];
                            break;
                        case 4:
                            strArray[i] = "0000" + strArray[i];
                            break;
                        case 3:
                            strArray[i] = "00000" + strArray[i];
                            break;
                        case 2:
                            strArray[i] = "000000" + strArray[i];
                            break;
                        case 1:
                            strArray[i] = "0000000" + strArray[i];
                            break;

                        default:
                            strArray[i] = strArray[i].Substring(temp - 8);
                            break;
                    }
                }
                //System.Windows.MessageBox.Show(strArray[0]);






                for (int i = 0; i < strArray.Length; i++)
                {
                    foreach (var item in listWares)
                    {
                        if (item.articl == strArray[i])
                        {
                            int temp = 0;
                            foreach (var photo in photoInfos)
                            {
                                if (item.code_wares == photo.photoName)
                                {

                                    Wares dataUser = new Wares()
                                    {
                                        photoPath = photo.photoPath,
                                        photoFullName = photo.photoFullName,
                                        kodeWares = item.code_wares,
                                        nameWares = item.name_wares,
                                        Articl = item.articl,

                                    };
                                    ListWares.Add(dataUser);
                                    //RadioButtonList.Items.Add(dataUser);
                                    break;
                                }

                                if (photoInfos.Count - 1 == temp)
                                {
                                    Wares dataUser = new Wares()
                                    {
                                        photoPath = "Spar.jpg",
                                        photoFullName = photo.photoFullName,
                                        kodeWares = item.code_wares,
                                        nameWares = item.name_wares,
                                        Articl = item.articl,

                                    };
                                    ListWares.Add(dataUser);
                                }
                                temp++;

                            }
                            break;
                        }

                    }
                }
            }

            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Увага!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            //System.Windows.MessageBox.Show(ListWares.Count().ToString(), "Message", MessageBoxButton.OK, MessageBoxImage.Information);
            WaresList.ItemsSource = ListWares;
        }

        private void CopyPhoto(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button btn = sender as System.Windows.Controls.Button;
            if (btn.DataContext is Wares)
            {
                Wares temp = btn.DataContext as Wares;
                try
                {
                    System.Windows.Clipboard.SetImage(new BitmapImage(new Uri(temp.photoPath)));
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Дане фото не можна скопіювати! {ex.Message}", "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }

        private void FindPhotoBuCode(object sender, RoutedEventArgs e)
        {
            ListWares = new ObservableCollection<Wares>();
            ListWares.Clear();



            int temp = CodeWaresTextBox.Text.Length;
            switch (temp)
            {
                case 9:
                    break;
                case 8:
                    CodeWaresTextBox.Text = "0" + CodeWaresTextBox.Text;
                    break;
                case 7:
                    CodeWaresTextBox.Text = "00" + CodeWaresTextBox.Text;
                    break;
                case 6:
                    CodeWaresTextBox.Text = "000" + CodeWaresTextBox.Text;
                    break;
                case 5:
                    CodeWaresTextBox.Text = "0000" + CodeWaresTextBox.Text;
                    break;
                case 4:
                    CodeWaresTextBox.Text = "00000" + CodeWaresTextBox.Text;
                    break;
                case 3:
                    CodeWaresTextBox.Text = "000000" + CodeWaresTextBox.Text;
                    break;
                case 2:
                    CodeWaresTextBox.Text = "0000000" + CodeWaresTextBox.Text;
                    break;
                case 1:
                    CodeWaresTextBox.Text = "00000000" + CodeWaresTextBox.Text;
                    break;
                default:
                    CodeWaresTextBox.Text = CodeWaresTextBox.Text.Substring(temp - 9);
                    break;
            }




            foreach (var item in listWares)
            {
                if (item.code_wares == CodeWaresTextBox.Text)
                {
                    int aa = 0;
                    foreach (var photo in photoInfos)
                    {
                        if (item.code_wares == photo.photoName)
                        {

                            Wares dataUser = new Wares()
                            {
                                photoPath = photo.photoPath,
                                photoFullName = photo.photoFullName,
                                kodeWares = item.code_wares,
                                nameWares = item.name_wares,
                                Articl = item.articl,

                            };
                            ListWares.Add(dataUser);
                            break;
                        }
                        aa++;
                        if (aa == photoInfos.Count)
                        {
                            Wares dataUser = new Wares()
                            {
                                photoPath = "Spar.jpg",
                                photoFullName = photo.photoFullName,
                                kodeWares = item.code_wares,
                                nameWares = item.name_wares,
                                Articl = item.articl,

                            };
                            ListWares.Add(dataUser);
                            break;
                        }

                    }
                    ArtclWaresTextBox.Text = item.articl;
                    NameFindWaresTextBloc.Text = item.name_wares;

                }
            }
            WaresList.ItemsSource = ListWares;
        }

        private void FindPhotoByActcl(object sender, RoutedEventArgs e)
        {
            ListWares = new ObservableCollection<Wares>();
            ListWares.Clear();



            int temp = ArtclWaresTextBox.Text.Length;
            switch (temp)
            {
                case 8:
                    break;
                case 7:
                    ArtclWaresTextBox.Text = "0" + ArtclWaresTextBox.Text;
                    break;
                case 6:
                    ArtclWaresTextBox.Text = "00" + ArtclWaresTextBox.Text;
                    break;
                case 5:
                    ArtclWaresTextBox.Text = "000" + ArtclWaresTextBox.Text;
                    break;
                case 4:
                    ArtclWaresTextBox.Text = "0000" + ArtclWaresTextBox.Text;
                    break;
                case 3:
                    ArtclWaresTextBox.Text = "00000" + ArtclWaresTextBox.Text;
                    break;
                case 2:
                    CodeWaresTextBox.Text = "000000" + ArtclWaresTextBox.Text;
                    break;
                case 1:
                    ArtclWaresTextBox.Text = "0000000" + ArtclWaresTextBox.Text;
                    break;

                default:
                    ArtclWaresTextBox.Text = ArtclWaresTextBox.Text.Substring(temp - 8);
                    break;
            }





            foreach (var item in listWares)
            {
                if (item.articl == ArtclWaresTextBox.Text)
                {
                    int aa = 0;
                    foreach (var photo in photoInfos)
                    {
                        if (item.code_wares == photo.photoName)
                        {

                            Wares dataUser = new Wares()
                            {
                                photoPath = photo.photoPath,
                                photoFullName = photo.photoFullName,
                                kodeWares = item.code_wares,
                                nameWares = item.name_wares,
                                Articl = item.articl,

                            };
                            ListWares.Add(dataUser);
                            break;
                        }
                        aa++;
                        if (aa == photoInfos.Count)
                        {
                            Wares dataUser = new Wares()
                            {
                                photoPath = "Spar.jpg",
                                photoFullName = photo.photoFullName,
                                kodeWares = item.code_wares,
                                nameWares = item.name_wares,
                                Articl = item.articl,

                            };
                            ListWares.Add(dataUser);
                            break;
                        }
                    }
                    CodeWaresTextBox.Text = item.code_wares;
                    NameFindWaresTextBloc.Text = item.name_wares;
                }
            }
            WaresList.ItemsSource = ListWares;
        }

        private void CopyPhotoToRepository(object sender, RoutedEventArgs e)
        {

            System.Windows.Controls.Button btn = sender as System.Windows.Controls.Button;
            if (btn.DataContext is Wares)
            {
                Wares temp = btn.DataContext as Wares;
                //System.Windows.MessageBox.Show(temp.photoFullName);
                try
                {
                    System.IO.File.Copy(temp.photoPath, CopyPhotoPath.Text + temp.photoFullName, true);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Дане фото не можна скопіювати! {ex.Message}", "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }

        private void OpenToFilePathSavePhoto(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            CopyPhotoPath.Text = dialog.SelectedPath + @"\";
        }

        private void ChangeArticl(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void NumColumChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            System.Windows.Controls.TextBox textBox = (System.Windows.Controls.TextBox)sender;
            Regex regex = new Regex(@"^[0-9]", RegexOptions.Compiled);
            isColumWrite = regex.IsMatch(textBox.Text);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("isExcelOk"));
            //System.Windows.MessageBox.Show(isColumWrite.ToString());
        }

        private void ChangeExcelPath(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            pathToExel = PathToExelTextBox.Text;
            if (PathToExelTextBox.Text != "")
                isExcelPath = true;

            else
                isExcelPath = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("isExcelOk"));
        }

        private void PathToPhotoCanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            pathToPhoto = PathToPhotoTextBox.Text;
        }

        private void ClickPhotoPathToCode(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            TextBoxCodePath.Text = dialog.SelectedPath + @"\";
        }

        private void ClickPhotoPathArtcl(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            TextBoxActclPath.Text = dialog.SelectedPath + @"\";
        }

        private void TextChangedActclPath(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void TextChangedCodePath(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void CopyAndRenamePhoto(object sender, RoutedEventArgs e)
        {
            string[] filesArtcl = null;
            filesArtcl = System.IO.Directory.GetFiles(TextBoxActclPath.Text);
            photoArtcl = new List<PhotoInfo>();



            for (int i = 0; i < filesArtcl.Length; i++)
            {
                try
                {

                    photoArtcl.Add(new PhotoInfo() { photoName = Path.GetFileNameWithoutExtension(filesArtcl[i]), photoPath = Path.GetFullPath(filesArtcl[i]), photoFullName = Path.GetFileName(filesArtcl[i]) });

                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }

            }




            for (int i = 0; i < photoArtcl.Count; i++)
            {
                int temp = photoArtcl[i].photoName.Length;
                switch (temp)
                {
                    case 8:
                        break;
                    case 7:
                        photoArtcl[i].photoName = "0" + photoArtcl[i].photoName;
                        break;
                    case 6:
                        photoArtcl[i].photoName = "00" + photoArtcl[i].photoName;
                        break;
                    case 5:
                        photoArtcl[i].photoName = "000" + photoArtcl[i].photoName;
                        break;
                    case 4:
                        photoArtcl[i].photoName = "0000" + photoArtcl[i].photoName;
                        break;
                    case 3:
                        photoArtcl[i].photoName = "00000" + photoArtcl[i].photoName;
                        break;
                    case 2:
                        photoArtcl[i].photoName = "000000" + photoArtcl[i].photoName;
                        break;
                    case 1:
                        photoArtcl[i].photoName = "0000000" + photoArtcl[i].photoName;
                        break;

                    default:
                        photoArtcl[i].photoName = photoArtcl[i].photoName.Substring(temp - 8);
                        break;
                }


            }
           //System.Windows.MessageBox.Show(photoArtcl.Count.ToString());

            foreach (var photo in photoArtcl)
            {
                foreach (var item in listWares)
                {
                    if (photo.photoName == item.articl)
                    {
                        try
                        {
                            System.IO.File.Copy(photo.photoPath, TextBoxCodePath.Text + item.code_wares + ".png", true);
                        }
                        catch (Exception ex)
                        {
                            System.Windows.MessageBox.Show($"Дане фото не можна скопіювати! {ex.Message}", "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        break;
                    }
                }
            }
        }
    }
}

