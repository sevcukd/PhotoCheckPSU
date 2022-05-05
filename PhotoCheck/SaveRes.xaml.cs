
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

namespace PhotoCheck
{
    /// <summary>
    /// Interaction logic for SaveRes.xaml
    /// </summary>
    public partial class SaveRes : System.Windows.Window
    {
        public ObservableCollection<Wares> ListWares { get; set; }
        public string pathToPhoto { get; set; } = @"d:\Pictures\Products\";
        public string pathToExel { get; set; } = @"";
        public List<SQLWares> listWares { get; set; }

        public string query1 = @"SELECT w.code_wares,w.name_wares,w.Code_Direction, w.articl FROM dbo.Wares w "; //000148259
        public string varConectionString = @"Server=10.1.0.22;Database=DW;Uid=dwreader;Pwd=DW_Reader;Connect Timeout=180;";
        public SqlConnection connection = null;
        public eTypeCommit TypeCommit { get; set; }
        public List<PhotoInfo> photoInfos { get; set; }
        public SaveRes(List<PhotoInfo> photo)
        {
            InitializeComponent();
            GC.Collect();
            //MessageBox.Show(ListWares[0].Articl);
            PathToPhotoTextBox.Text = pathToPhoto;
            PathToExelTextBox.Text = pathToExel;

            TypeCommit = eTypeCommit.Auto;
            connection = new SqlConnection(varConectionString);
            connection.Open();
            listWares = connection.Query<SQLWares>(query1).ToList();
            //System.Windows.MessageBox.Show(listWares[0].articl);


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
            pathToPhoto = dialog.SelectedPath;
            PathToPhotoTextBox.Text = pathToPhoto;
        }

        private void OpenToFilePathExel(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel file (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog.ShowDialog();
            pathToExel = Path.GetFullPath(openFileDialog.FileName);
            //System.Windows.MessageBox.Show(pathToExel, "Message", MessageBoxButton.OK, MessageBoxImage.Information);
            PathToExelTextBox.Text = pathToExel;

        }

        private void FindPhoto(object sender, RoutedEventArgs e)
        {
            ListWares = new ObservableCollection<Wares>();



            //System.Windows.MessageBox.Show(photoInfos.Count().ToString());


            string pathToFile = pathToExel.ToString();

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
            ObjExcel.Quit();
            //System.Windows.MessageBox.Show(strArray[5]);
            try
            {
                for (int i = 0; i < strArray.Length; i++)
                {
                    strArray[i] = Convert.ToInt32(strArray[i]).ToString();
                }
            }
            catch (Exception)
            {


            }

            int tempInt = 0;

            try
            {
                for (; tempInt < listWares.Count; tempInt++)
                {
                    if (listWares[tempInt].articl != null)
                    {
                        listWares[tempInt].articl = Convert.ToInt32(listWares[tempInt].articl).ToString();
                    }
                    else
                    {
                        listWares[tempInt].articl = "Пусто";
                    }


                }

            }
            catch (Exception)
            {
            }


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
                                    photoPath = photo.photoPath,
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


            //System.Windows.MessageBox.Show(ListWares.Count().ToString(), "Message", MessageBoxButton.OK, MessageBoxImage.Information);
            WaresList.ItemsSource = ListWares;
        }

        private void CopyPhoto(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button btn = sender as System.Windows.Controls.Button;
            if (btn.DataContext is Wares)
            {
                Wares temp = btn.DataContext as Wares;
                System.Windows.Clipboard.SetImage(new BitmapImage(new Uri(temp.photoPath)));
            }
        }

        private void FindPhotoBuCode(object sender, RoutedEventArgs e)
        {
            ListWares = new ObservableCollection<Wares>();
            ListWares.Clear();
            foreach (var item in listWares)
            {
                if (item.code_wares == CodeWaresTextBox.Text)
                {
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
            
            foreach (var item in listWares)
            {
                if (item.articl == ArtclWaresTextBox.Text)
                {
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
                    }
                    CodeWaresTextBox.Text = item.code_wares;
                    NameFindWaresTextBloc.Text = item.name_wares;
                }
            }
            WaresList.ItemsSource = ListWares;
        }
    }
}

