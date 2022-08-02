using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Dapper;
using System.Data.SqlClient;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.ComponentModel;

namespace PhotoCheck
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window, IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<Wares> ListWares { get; set; }
        public ObservableCollection<Wares> ListWares2 { get; set; }
        public ObservableCollection<Wares> EmptuListWares { get; set; }
        public List<PhotoInfo> photoInfos { get; set; }
        public eTypeCommit TypeCommit { get; set; }
        public string pathPhoto { get; set; } = @"\\truenas\Public\PHOTOBANK\Check\"; //@"d:\Pictures\Products\";
        public string pathToPhoto { get; set; } = @"\\truenas\Public\PHOTOBANK\High\";
        //public string pathToPhoto { get; set; } = @"d:\Pictures\Good\";
        public string query1 = @"SELECT w.code_wares,w.name_wares,w.Code_Direction, w.articl FROM dbo.Wares w WHERE w.Code_Direction="; //000148259
        public string varConectionString = @"Server=10.1.0.22;Database=DW;Uid=dwreader;Pwd=DW_Reader;Connect Timeout=180;";
        public SqlConnection connection = null;
        public string SerchCode { get; set; }


        public MainWindow()
        {
            InitializeComponent();
            PathPhotoTextBox.Text = pathPhoto;
            PathToPhotoTextBox.Text = pathToPhoto;
            var query2 = @"SELECT gw.code_group_wares AS Code_Direction ,name FROM dbo.GROUP_WARES gw WHERE gw.code_parent_group_wares IS null";
            connection = new SqlConnection(varConectionString);
            connection.Open();
            TypeCommit = eTypeCommit.Auto;
            //var listWares = connection.Query<SQLWares>(query1).ToList();
            List<CodeGroup> FirstGroupWares = connection.Query<CodeGroup>(query2).ToList();
            var groupWares = FirstGroupWares.OrderBy(o => o.name).ToList();

            //  MessageBox.Show(listWares[0].name_wares);
            //  MessageBox.Show(groupWares[0].code_group_wares);

            //string pathToFile = @"d:\Work\Alkohol.xlsx";

            ////Создаём приложение.
            //Microsoft.Office.Interop.Excel.Application ObjExcel = new Microsoft.Office.Interop.Excel.Application();
            ////Открываем книгу.                                                                                                                                                        
            //Microsoft.Office.Interop.Excel.Workbook ObjWorkBook = ObjExcel.Workbooks.Open(pathToFile, 0, false, 5, "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
            ////Выбираем таблицу(лист).
            //Microsoft.Office.Interop.Excel.Worksheet ObjWorkSheet;
            //ObjWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)ObjWorkBook.Sheets[1];

            //// Указываем номер столбца (таблицы Excel) из которого будут считываться данные.
            //int numCol = 2;
            //int numCol2 = 4;

            //Range usedColumn = ObjWorkSheet.UsedRange.Columns[numCol];
            //System.Array myvalues = (System.Array)usedColumn.Cells.Value2;
            //string[] strArray = myvalues.OfType<object>().Select(o => o.ToString()).ToArray();

            //Range usedColumn2 = ObjWorkSheet.UsedRange.Columns[numCol2];
            //System.Array myvalues2 = (System.Array)usedColumn2.Cells.Value2;
            //string[] strArray2 = myvalues2.OfType<object>().Select(o => o.ToString()).ToArray();

            //// Выходим из программы Excel.
            //ObjExcel.Quit();







            DirectionList.ItemsSource = groupWares;


        }
        // To detect redundant calls
        private bool _disposedValue;

        // Instantiate a SafeHandle instance.
        private SafeHandle _safeHandle = new SafeFileHandle(IntPtr.Zero, true);
        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose() => Dispose(true);

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _safeHandle.Dispose();
                }

                _disposedValue = true;
            }
        }

        private void CheckRadiobutton(object sender, RoutedEventArgs e)
        {
            RadioButton rbtn = sender as RadioButton;
            if (rbtn.DataContext is Wares)
            {
                Wares temp = rbtn.DataContext as Wares;
                switch (rbtn.Content.ToString())
                {
                    case "Залишити фото":
                        temp.savePhotoStatus = 0;
                        break;
                    case "Невірне фото":
                        temp.savePhotoStatus = 1;
                        break;
                    case "Невірний код":
                        temp.savePhotoStatus = 2;
                        break;
                    default:
                        temp.savePhotoStatus = 3;
                        break;
                }
            }
        }

        private void CheckDirection(object sender, RoutedEventArgs e)
        {
            RadioButton ChBtn = sender as RadioButton;
            if (ChBtn.DataContext is CodeGroup)
            {
                CodeGroup temp = ChBtn.DataContext as CodeGroup;
                if (ChBtn.IsChecked == true)
                {
                    temp.Show = true;
                    SerchCode = temp.Code_Direction;
                }


                else temp.Show = false;
                // MessageBox.Show(temp.Show.ToString());
            }
        }

        private void OpenFilePath(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            //MessageBox.Show(dialog.SelectedPath);
            pathPhoto = dialog.SelectedPath + @"\";
            PathPhotoTextBox.Text = pathPhoto;
            //OpenFileDialog openFileDialog = new OpenFileDialog();
            //if (openFileDialog.ShowDialog() == true)
            //    MessageBox.Show(openFileDialog.FileName);
        }

        private void RunButton(object sender, RoutedEventArgs e)
        {
            string[] files = null;
            try
            {
                files = System.IO.Directory.GetFiles(pathPhoto);
                photoInfos = new List<PhotoInfo>();
                ListWares = new ObservableCollection<Wares>();
                for (int i = 0; i < files.Length; i++)
                {
                    try
                    {

                        photoInfos.Add(new PhotoInfo() { photoName = Path.GetFileNameWithoutExtension(files[i]), photoPath = Path.GetFullPath(files[i]), photoFullName = Path.GetFileName(files[i]) });

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                }

                try
                {
                    string aa = query1 + SerchCode;
                    connection = new SqlConnection(varConectionString);
                    connection.Open();
                    var listWares = connection.Query<SQLWares>(aa).ToList();
                    //BitmapImage image = new BitmapImage();
                    //image.BeginInit();
                    //Uri imageSource = new Uri("file://" + @"D:\Downloads\Spar.jpg");
                    //image.UriSource = imageSource;
                    //image.EndInit();
                    ////PhotoViev2.Source = image;
                    foreach (var item in listWares)
                    {
                        foreach (var photo in photoInfos)
                        {
                            if (item.code_wares == photo.photoName)
                            {

                                Wares dataUser = new Wares()
                                {
                                    photo = LoadImage(photo.photoPath),
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
                        }

                    }
                    WaresList.ItemsSource = ListWares;
                }
                catch (Exception)
                {
                    MessageBox.Show("Оберіть групу товарів!", "Увага!!!", MessageBoxButton.OK, MessageBoxImage.Warning);
                }


            }
            catch (Exception)
            {
                MessageBox.Show("Не правильно вказаний шлях!", "Увага!!!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }



        }
        public static ImageSource LoadImage(string fileName)
        {
            var image = new BitmapImage();

            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                image.BeginInit();
                image.DecodePixelHeight = 300;
                //image.DecodePixelWidth = 400;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();
            }

            return image;
        }
        private void OpenToFilePath(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            pathToPhoto = dialog.SelectedPath + @"\";
            PathToPhotoTextBox.Text = pathToPhoto;
        }
        void WaitCollect(int pMs = 1000)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Thread.Sleep(pMs);
        }
        private void MovePhotoButton(object sender, RoutedEventArgs e)
        {
            //ListWares2 = new ObservableCollection<Wares>();

            //foreach (var temp in ListWares)
            //{


            //    Wares dataUser = new Wares()
            //    {
            //        //photo  = "Spar.jpg",
            //        photoPath = temp.photoPath,
            //        photoFullName = temp.photoFullName,
            //        kodeWares = temp.kodeWares,
            //        nameWares = temp.nameWares,
            //        savePhotoStatus = temp.savePhotoStatus,
            //    };
            //    ListWares2.Add(dataUser);

            //}
            //WaresList.ItemsSource = ListWares2;
            //ListWares.Clear();
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
                            file2.MoveTo(pathPhoto + "невірне фото" + file2.Name);
                            break;
                        case 2: // невірний код
                            FileInfo file3 = new FileInfo(ware.photoPath);
                            file3.MoveTo(pathPhoto + "невірний код" + file3.Name);
                            break;
                        case 3: // нічого не робити)))
                            
                            break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }


            }
            MessageBox.Show("Переміщення завершено!", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Dispose(true);
            WaitCollect();
            //new SaveRes(ListWares).ShowDialog();
        }

        private void PhotoCatalogTextBox(object sender, TextChangedEventArgs e)
        {
            pathPhoto = PathPhotoTextBox.Text;
        }

        private void PhotoToCatalogTextBox(object sender, TextChangedEventArgs e)
        {
            pathToPhoto = PathToPhotoTextBox.Text;
        }

        private void CopyPhoto(object sender, RoutedEventArgs e)
        {
            SaveRes save = new SaveRes(photoInfos);
            save.Show();
        }
    }




    public enum eTypeCommit
    {
        Auto,
        Manual
    }


    public class SQLWares
    {
        public string code_wares { get; set; }
        public string name_wares { get; set; }
        public string Code_Direction { get; set; }
        public string articl { get; set; }
    }
    public class SQLKasaList
    {
        public string _code { get; set; }
        public string _Description { get; set; }
    }
    public class SQLExpressGoods
    {
        public string Order_Button { get; set; }
        public string Name_Button { get; set; }
        public string code_wares { get; set; }
        public string CodeWares { get; set; }
        public string name_wares { get; set; }
        public string articl { get; set; }
        
    }
    
    public class CodeGroup
    {
        public string Code_Direction { get; set; }
        public string name { get; set; }
        public bool Show { get; set; }
    }
    public class Wares
    {

        public ImageSource photo { get; set; }
        public string photoPath { get; set; }
        public string photoFullName { get; set; }
        public string kodeWares { get; set; }
        public string nameWares { get; set; }
        public string Articl { get; set; }
        public int savePhotoStatus { get; set; } = 4; // 0-лишити фото; 1-невірне фото; 2-невірний код

        ~Wares()
        {
            //MessageBox.Show("by!");
        }

    }
    public class PhotoInfo
    {
        public string photoName { get; set; }
        public string photoPath { get; set; }

        public string photoFullName { get; set; }
    }
}
