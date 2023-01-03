using System.Windows.Controls;
using System;
using System.Threading.Tasks;
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
using System.Drawing;
using System.Drawing.Printing;
using RadioButton = System.Windows.Controls.RadioButton;
using Font = System.Drawing.Font;
using MessageBox = System.Windows.Forms.MessageBox;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;
using PhotoCheck.SQL;
using PhotoCheck.Models;
using Task = System.Threading.Tasks.Task;
using DocumentFormat.OpenXml.Drawing.Charts;

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
        public string PhotoSuppliers { get; set; } = @"";
        public string RenamedPhotos { get; set; } = @"\\truenas\Public\PHOTOBANK\Check\";
        public List<SQLWares> listWares { get; set; }
        public List<SQLKasaList> KasaList { get; set; }
        public List<SQLExpressGoods> ExpressGoods { get; set; }
        public List<SQLExpressGoods> SortedExpressGoods { get; set; }
        public List<SQLWeightGoods> WeightGoods { get; set; }
        public List<PhotoInfo> DuplicatePhotos { get; set; }
        public string SelectedExpressGoodsCode { get; set; }
        public string SelectedExpressGoodsName { get; set; }
        public string SelectedWeightGroup { get; set; }
        public List<SQLWeightGroups> WeightGroups { get; set; }
        int counter = 0;
        int lastWaresThisDoc = 0;
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
        public bool isPhotoSuppliersPath { get; set; }
        public bool isRenamedPhotosPath { get; set; }
        public bool isRenamedPathOk
        {
            get
            {
                if (isPhotoSuppliersPath && isRenamedPhotosPath)
                {
                    return true;
                }
                else return false;
            }
        }
        public bool isSelectedExpressGoods
        {
            get
            {
                if (SelectedExpressGoodsCode != null)
                    return true;
                else return false;
            }
        }
        public bool isSelectedWeightGroup
        {
            get
            {
                if (SelectedWeightGroup != null)
                    return true;
                else return false;
            }
        }
        // всі товари з 1с (якщо додати код групи то видать тільки по групі)
        //public string query1 = @"SELECT w.code_wares,w.name_wares,w.Code_Direction, w.articl, w.IsWeight FROM dbo.Wares w "; //000148259
        public string query1 = @"SELECT w.code_wares,w.name_wares,w.Code_Direction, w.articl, w.IsWeight 
, ( SELECT dbo.Concatenate(b.bar_code+',') FROM barcode b WHERE b.code_wares=w.code_wares  ) AS barcode
FROM dbo.Wares w";
        public string query2 = @"SELECT _code, _CASH_place._Description FROM DW.dbo.V1C_DIM_OPTION_WPC _CASH_place";
        public string query3 = @"SELECT  g.Order_Button , g.Name_Button, w1.code_wares AS CodeWares ,w1.name_wares,w1.articl,
  (SELECT max(bc.bar_code)  FROM barcode bc WHERE bc.code_wares=w1.code_wares) AS bar_code,w1.IsWeight
  FROM DW.dbo.V1C_DIM_OPTION_WPC O  
  JOIN DW.dbo.V1C_DIM_OPTION_WPC_FAST_GROUP G ON o._IDRRef=G._Reference18850_IDRRef
  JOIN DW.dbo.V1C_DIM_OPTION_WPC_FAST_WARES W ON o._IDRRef = W._Reference18850_IDRRef AND G.Order_Button_wares = W.Order_Button
  JOIN dw.dbo.Wares w1 ON w.Wares_RRef=w1._IDRRef
    WHERE o._Code=";
        public string query4 = @"SELECT  code, [desc] FROM dbo.V1C_dim_GroupWeighingScale";
        public string query5 = @"SELECT  
w.code_wares AS CodeWares ,w.name_wares,w.articl,r.PLU,w.IsWeight
FROM dbo.V1C_dim_GroupWeighingScale d
JOIN dbo.V1C_reg_Nomen_GroupWeighingScale r ON d.Group_Weighing_Scale_RRef = r.Group_Weighing_Scale_RRef
JOIN dw.dbo.Wares w ON nomen_RRef=w._IDRRef

WHERE d.code=";
        //асортиментна матриця
        public string query6 = @"SELECT   
  DISTINCT dn.code , dn.[desc] AS name ,dn.articul, is_weight,barcodes,barcode_last
  FROM sqlsrv2.for_cubes.dbo.fact_deficit_surplus  ds
  JOIN  sqlsrv2.for_cubes.dbo.dimen_nomen dn ON   dn.nomen_id= ds.nomen_id 
WHERE n_min_rest>0 AND  day_id = convert(char,getdate(),112)";

        public string varConectionString = @"Server=10.1.0.22;Database=DW;Uid=dwreader;Pwd=DW_Reader;Connect Timeout=180;";
        public SqlConnection connection = null;
        public eTypeCommit TypeCommit { get; set; }
        public List<PhotoInfo> photoInfos { get; set; }
        public List<PhotoInfo> photoArtcl { get; set; }
        public List<SQLAssortmentMatrix> AssortmentMatrix { get; set; }
        private readonly object _locker = new object();
        public SaveRes() //List<PhotoInfo> photo
        {
            InitializeComponent();
            listWares = new List<SQLWares>();

            PathToPhotoTextBox.Text = pathToPhoto;
            PathToExelTextBox.Text = pathToExel;
            PathToRenamedPhotos.Text = RenamedPhotos;
            //підключення до бази
            TypeCommit = eTypeCommit.Auto;
            connection = new SqlConnection(varConectionString);
            connection.Open();

            //список груп кас швидких товарів
            KasaList = connection.Query<SQLKasaList>(query2).ToList();
            KasaListShow.ItemsSource = KasaList;

            // пошук груп ваг
            WeightGroups = connection.Query<SQLWeightGroups>(query4).ToList();
            WeightGroups = WeightGroups.OrderBy(n => n.desc).ToList();
            WeightGroupsShow.ItemsSource = WeightGroups;
            //список всіх товарів з 1С
            FillingListWares();
            //Збір інформаціїї про фото
            FindPhotoToPath();


        }

        async void FillingListWares()
        {
            await Task.Run(() =>
            {
                lock (_locker)
                {
                    listWares = connection.Query<SQLWares>(query1).ToList();
                }
            });

        }
        private void FindPhotoToPath()
        {

            photoInfos = new List<PhotoInfo>();
            //щитуємо інформацію з каталогу
            string[] files = new string[] { };
            if (DirectoryInfo() != null)
            {
                files = DirectoryInfo();
            }

            //перевіряємо чи він не пустий
            if (files != null && files.Length <= 0)
            {
                MessageBox.Show("В обраному каталозі немає жодного файлу");

            }
            //перейменовуємо погані фото 
            RenameBadPhoto(files);
            //отримуємо всю інформацію по фото
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
            HelpList.Visibility = Visibility.Collapsed;
            SV_WaresList.Visibility = Visibility.Visible;

        }
        private string[] DirectoryInfo()
        {
            string[] files = null;
            if (Directory.Exists(pathToPhoto))
            {
                files = System.IO.Directory.GetFiles(pathToPhoto);
                return files;
            }
            else
            {
                System.Windows.MessageBox.Show($"Вказаний шлях відсутній або ви не маєте прав доступу до каталогу!", "Увага!", MessageBoxButton.OK, MessageBoxImage.Error);
                return files;
            }
        }
        private void RenameBadPhoto(string[] files)
        {
            string pathTo;
            string nameWares;
            if (files != null)
                for (int i = 0; i < files.Length; i++)
                {
                    nameWares = Path.GetFileNameWithoutExtension(files[i]);
                    if (nameWares.Length != 9)
                    {
                        if (int.TryParse(nameWares, out int res))
                        {
                            try
                            {
                                pathTo = pathToPhoto + res.ToString("D9") + Path.GetExtension(files[i]);
                                if (File.Exists(pathTo))
                                    File.Delete(pathTo);
                                System.IO.File.Move(Path.GetFullPath(files[i]), pathTo);
                                files[i] = pathTo;
                                continue;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }

                        }
                    }
                }
        }
        private void OpenToFilePath(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                pathToPhoto = dialog.SelectedPath + @"\";
                PathToPhotoTextBox.Text = pathToPhoto;
            }
        }

        private void OpenToFilePathExel(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel file (*.xlsx)|*.xlsx;*.XLSM;*.XLTX;*.XLS;*.XLT;*.xlsb|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

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

        }

        private void FindPhoto(object sender, RoutedEventArgs e)
        {
            //FindPhotoToPath();
            ListWares = new ObservableCollection<Wares>();



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
                    if (int.TryParse(strArray[i], out int res))
                    {
                        strArray[i] = res.ToString("D8");
                    }

                    //int temp = strArray[i].Length;
                    //switch (temp)
                    //{
                    //    case 8:
                    //        break;
                    //    case 7:
                    //        strArray[i] = "0" + strArray[i];
                    //        break;
                    //    case 6:
                    //        strArray[i] = "00" + strArray[i];
                    //        break;
                    //    case 5:
                    //        strArray[i] = "000" + strArray[i];
                    //        break;
                    //    case 4:
                    //        strArray[i] = "0000" + strArray[i];
                    //        break;
                    //    case 3:
                    //        strArray[i] = "00000" + strArray[i];
                    //        break;
                    //    case 2:
                    //        strArray[i] = "000000" + strArray[i];
                    //        break;
                    //    case 1:
                    //        strArray[i] = "0000000" + strArray[i];
                    //        break;

                    //    default:
                    //        strArray[i] = strArray[i].Substring(temp - 8);
                    //        break;
                    //}
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
                                        barcode = item.barcode,

                                    };
                                    ListWares.Add(dataUser);
                                    //RadioButtonList.Items.Add(dataUser);
                                    break;
                                }

                                if (photoInfos.Count - 1 == temp)
                                {
                                    Wares dataUser = new Wares()
                                    {
                                        photoPath = "Images\\Spar.jpg",
                                        photoFullName = photo.photoFullName,
                                        kodeWares = item.code_wares,
                                        nameWares = item.name_wares,
                                        Articl = item.articl,
                                        barcode = item.barcode,

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
            //FindPhotoToPath();
            ListWares = new ObservableCollection<Wares>();
            ListWares.Clear();



            int temp = CodeWaresTextBox.Text.Length;
            if (temp == 0)
            {
                System.Windows.MessageBox.Show("Введіть код!", "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (int.TryParse(CodeWaresTextBox.Text, out int res))
                CodeWaresTextBox.Text = res.ToString("D9");


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
                                IsWeight = item.IsWeight,
                                barcode = item.barcode,

                            };
                            ListWares.Add(dataUser);
                            break;
                        }
                        aa++;
                        if (aa == photoInfos.Count)
                        {
                            Wares dataUser = new Wares()
                            {
                                photoPath = "Images\\Spar.jpg",
                                photoFullName = photo.photoFullName,
                                kodeWares = item.code_wares,
                                nameWares = item.name_wares,
                                Articl = item.articl,
                                IsWeight = item.IsWeight,
                                barcode = item.barcode,

                            };
                            ListWares.Add(dataUser);
                            break;
                        }

                    }
                    ArtclWaresTextBox.Text = item.articl;
                    NameFindWaresTextBloc.Text = item.name_wares;
                    BarcodeWaresTextBox.Text = item.barcode;

                }
            }
            WaresList.ItemsSource = ListWares;
        }

        private void FindPhotoByActcl(object sender, RoutedEventArgs e)
        {
            //FindPhotoToPath();
            ListWares = new ObservableCollection<Wares>();
            ListWares.Clear();



            int temp = ArtclWaresTextBox.Text.Length;
            if (temp == 0)
            {
                System.Windows.MessageBox.Show("Введіть артикул!", "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (int.TryParse(ArtclWaresTextBox.Text, out int res))
                ArtclWaresTextBox.Text = res.ToString("D8");






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
                                IsWeight = item.IsWeight,
                                barcode = item.barcode,

                            };
                            ListWares.Add(dataUser);
                            break;
                        }
                        aa++;
                        if (aa == photoInfos.Count)
                        {
                            Wares dataUser = new Wares()
                            {
                                photoPath = "Images\\Spar.jpg",
                                photoFullName = photo.photoFullName,
                                kodeWares = item.code_wares,
                                nameWares = item.name_wares,
                                Articl = item.articl,
                                IsWeight = item.IsWeight,
                                barcode = item.barcode,

                            };
                            ListWares.Add(dataUser);
                            break;
                        }
                    }
                    CodeWaresTextBox.Text = item.code_wares;
                    NameFindWaresTextBloc.Text = item.name_wares;
                    BarcodeWaresTextBox.Text = item.barcode;
                }
            }
            WaresList.ItemsSource = ListWares;

        }
        private void FindPhotoBarcode(object sender, RoutedEventArgs e)
        {
            //FindPhotoToPath();
            ListWares = new ObservableCollection<Wares>();
            ListWares.Clear();



            int temp = BarcodeWaresTextBox.Text.Length;
            if (temp == 0)
            {
                System.Windows.MessageBox.Show("Введіть Штрих-код!", "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }




            foreach (var item in listWares)
            {
                var tempBarcode = item.barcode.Split(new char[] { ',' });
                for (int i = 0; i < tempBarcode.Length; i++)
                {
                    if (tempBarcode[i] == BarcodeWaresTextBox.Text)
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
                                    IsWeight = item.IsWeight,
                                    barcode = item.barcode

                                };
                                ListWares.Add(dataUser);
                                break;
                            }
                            aa++;
                            if (aa == photoInfos.Count)
                            {
                                Wares dataUser = new Wares()
                                {
                                    photoPath = "Images\\Spar.jpg",
                                    photoFullName = photo.photoFullName,
                                    kodeWares = item.code_wares,
                                    nameWares = item.name_wares,
                                    Articl = item.articl,
                                    IsWeight = item.IsWeight,
                                    barcode = item.barcode

                                };
                                ListWares.Add(dataUser);
                                break;
                            }
                        }
                        CodeWaresTextBox.Text = item.code_wares;
                        ArtclWaresTextBox.Text = item.articl;
                        NameFindWaresTextBloc.Text = item.name_wares;
                    }
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
            if (result == System.Windows.Forms.DialogResult.OK)
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
            if (Directory.Exists(pathToPhoto))
                FindPhotoToPath();
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
                photoArtcl[i].photoName = Convert.ToInt32(photoArtcl[i].photoName).ToString("D8");

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
                            System.IO.File.Copy(photo.photoPath, TextBoxCodePath.Text + item.code_wares + Path.GetExtension(photo.photoPath), true);
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
        /// <summary>
        /// RadioButton вибору групи швидких товарів
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckKasa(object sender, RoutedEventArgs e)
        {
            RadioButton ChBtn = sender as RadioButton;
            if (ChBtn.DataContext is SQLKasaList)
            {
                SQLKasaList temp = ChBtn.DataContext as SQLKasaList;
                if (ChBtn.IsChecked == true)
                {
                    SelectedExpressGoodsCode = temp._code;
                    SelectedExpressGoodsName = temp._Description;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("isSelectedExpressGoods"));
            }
        }
        /// <summary>
        /// RadioButton вибору групи ваг
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckWeightGroup(object sender, RoutedEventArgs e)
        {
            RadioButton ChBtn = sender as RadioButton;
            if (ChBtn.DataContext is SQLWeightGroups)
            {
                SQLWeightGroups temp = ChBtn.DataContext as SQLWeightGroups;
                if (ChBtn.IsChecked == true)
                {
                    SelectedWeightGroup = temp.code;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("isSelectedWeightGroup"));
            }
        }

        private void OpenToFilePathSaveCsv(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
                SaveCsvPath.Text = dialog.SelectedPath + @"\";
        }
        /// <summary>
        /// Збереження у CSV файл  всієї інформації швидких товарів по обраній групі кас
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveCsvButton(object sender, RoutedEventArgs e)
        {
            //FindPhotoToPath();
            //пошук всіх швидких товарів
            ExpressGoods = connection.Query<SQLExpressGoods>($"{query3}'{SelectedExpressGoodsCode}'").ToList();

            //сортування по групах
            SortedExpressGoods = ExpressGoods.OrderBy(n => n.Name_Button).ToList();
            LinkToPhoto();
            //створення строк для запису файлу
            List<string> StrWriteExpressGoods = new List<string>();
            StrWriteExpressGoods.Add($"Назва групи;Назва товару;Код товару;Артикул;Штрихкод;Чи присутнє фото");
            foreach (var item in SortedExpressGoods)
            {
                StrWriteExpressGoods.Add($"{item.Name_Button};{item.name_wares};{item.CodeWares};{item.articl};{item.bar_code};{item.isPhotoPresent.ToString()}");
            }

            //запис в файл
            try
            {
                File.AppendAllLines($"{SaveCsvPath.Text}{SelectedExpressGoodsName}.csv", StrWriteExpressGoods, System.Text.Encoding.GetEncoding("Windows-1251"));
                System.Windows.MessageBox.Show($"Шлях до файлу: {SaveCsvPath.Text}{SelectedExpressGoodsName}.csv", "Файл збережено!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// Кнопка друку макету підказок для касирів
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrintExpressGoodsButton(object sender, RoutedEventArgs e)
        {
            //FindPhotoToPath();
            //пошук всіх швидких товарів
            ExpressGoods = connection.Query<SQLExpressGoods>($"{query3}'{SelectedExpressGoodsCode}'").ToList();

            //сортування по групах
            SortedExpressGoods = ExpressGoods.OrderBy(n => n.Name_Button).ToList();



            LinkToPhoto();

            counter = 0;

            for (int i = 0; i < SortedExpressGoods.Count - 1; i++)
            {
                if (SortedExpressGoods[i].Name_Button != SortedExpressGoods[i + 1].Name_Button)
                {
                    lastWaresThisDoc = i + 1;

                    //Create a PrintPreviewDialog/PrintDialog object  
                    System.Windows.Forms.PrintDialog previewDlg = new System.Windows.Forms.PrintDialog();
                    //Create a PrintDocument object  
                    PrintDocument pd = new PrintDocument();
                    //Add print-page event handler
                    pd.PrintPage += pd_PrintPage;
                    //Set Document property of PrintPreviewDialog  
                    previewDlg.Document = pd;
                    //Display dialog  
                    //previewDlg.Show();
                    try
                    {
                        if (previewDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            previewDlg.Document.Print(); // печатаем
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.Message, "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        public void LinkToPhoto()  //TMP - переробити в 1 метод без дублювання коду
        {
            foreach (var expressGoodsTMP in SortedExpressGoods)
            {
                foreach (var infoPhoto in photoInfos)
                {
                    if (expressGoodsTMP.CodeWares == infoPhoto.photoName)
                    {
                        expressGoodsTMP.isPhotoPresent = true;
                        expressGoodsTMP.pathPhoto = infoPhoto.photoPath;
                        break;
                    }
                }
            }
        }
        public void WeightLinkToPhoto()
        {
            foreach (var weightGoodsTMP in WeightGoods)
            {
                foreach (var infoPhoto in photoInfos)
                {
                    if (weightGoodsTMP.CodeWares == infoPhoto.photoName)
                    {
                        weightGoodsTMP.pathPhoto = infoPhoto.photoPath;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Друк макету підказок для касирів
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void pd_PrintPage(object sender, PrintPageEventArgs e)
        {

            var barcode = new BarcodeLib.Barcode();
            int left = 20;
            int top = 20;
            int mainFontSize = 14;
            int totalFontSize = 10;
            double waresImageWidth;
            double waresImageHeight;
            double thisCounter = 0;
            while (counter < lastWaresThisDoc)
            {
                System.Drawing.Image imageBarcode;
                //Фото
                if (SortedExpressGoods[counter].pathPhoto != null)
                {
                    Graphics graphic = e.Graphics;
                    System.Drawing.Image waresImage = System.Drawing.Image.FromFile(SortedExpressGoods[counter].pathPhoto);
                    waresImageWidth = waresImage.Width;
                    waresImageHeight = waresImage.Height;
                    double coef = 0;
                    if (waresImageHeight > waresImageWidth)
                        coef = 100 / waresImageHeight;
                    else
                        coef = 100 / waresImageWidth;
                    waresImageHeight = waresImageHeight * coef;
                    waresImageWidth = waresImageWidth * coef;

                    graphic.DrawImage(waresImage, left + 450, top + 13, Convert.ToInt32(waresImageWidth), Convert.ToInt32(waresImageHeight));
                    waresImage.Dispose();
                }
                //якщо є штрих-код тоді друкуємо його
                if (SortedExpressGoods[counter].bar_code != null && SortedExpressGoods[counter].bar_code.Length == 13)
                {
                    try
                    {
                        imageBarcode = barcode.Encode(BarcodeLib.TYPE.EAN13, SortedExpressGoods[counter].bar_code, Color.Black, Color.White, 290, 120);
                        e.Graphics.DrawImage(imageBarcode, 650, top + 30, 150, 60);
                    }
                    catch (Exception)
                    {
                        System.Windows.MessageBox.Show($"{SortedExpressGoods[counter].name_wares} - не правильний штрихкод: {SortedExpressGoods[counter].bar_code}", "Увага!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                }
                else
                {
                    //якщо немає тоді генеруємо штрих-коди на ВАГОВИЙ товар
                    if (SortedExpressGoods[counter].IsWeight)
                    {
                        string EAN13String = $"22{SortedExpressGoods[counter].articl.Substring(2)}0000";
                        try
                        {
                            imageBarcode = barcode.Encode(BarcodeLib.TYPE.EAN13, EAN13String, Color.Black, Color.White, 290, 120);
                            e.Graphics.DrawImage(imageBarcode, 650, top + 30, 150, 60);
                            //e.Graphics.DrawString("Ваговий товар - введіть кількість*:", new Font("Arial", 8), Brushes.Red, 650, top + 15);
                        }
                        catch (Exception)
                        {
                            System.Windows.MessageBox.Show($"{SortedExpressGoods[counter].name_wares} - не коректні дані!", "Увага!", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }

                }


                e.Graphics.DrawString(SortedExpressGoods[counter].name_wares, new Font("Arial", mainFontSize, System.Drawing.FontStyle.Italic), Brushes.Black, left, top += 14);
                e.Graphics.DrawString("Артикул:", new Font("Arial", totalFontSize), Brushes.Black, left, top += 25);

                SolidBrush myBrush = new SolidBrush(Color.Green);
                System.Drawing.Rectangle myRectangle = new System.Drawing.Rectangle(left, top += 14, 100, 20);
                e.Graphics.FillRectangle(myBrush, myRectangle);
                e.Graphics.DrawString(SortedExpressGoods[counter].articl, new Font("Arial", mainFontSize, System.Drawing.FontStyle.Bold), Brushes.White, myRectangle);
                e.Graphics.DrawString("Назва групи товарів:", new Font("Arial", totalFontSize), Brushes.Black, left, top += 25);
                e.Graphics.DrawString(SortedExpressGoods[counter].Name_Button, new Font("Arial", mainFontSize), Brushes.Black, left, top += 14);

                Pen myPen = new Pen(System.Drawing.Color.Gray, 3);
                e.Graphics.DrawLine(myPen, 0, top + 23, 1000, top + 23);
                top += 15;
                if (counter < lastWaresThisDoc)
                {
                    counter++;
                    thisCounter++;
                }

                if (thisCounter % 10 == 0)
                {
                    break;
                }

            }
            if (counter < lastWaresThisDoc)
            {
                //Has more pages??  
                e.HasMorePages = true;
            }
        }
        /// <summary>
        /// Друк макету на ваги КАС
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PrintWeightListCAS(object sender, PrintPageEventArgs e)
        {
            int left;
            int top;
            int mainFontSize = 8;
            double waresImageWidth;
            double waresImageHeight;
            int columnWidth = 715 / 5; //e.PageBounds.Width / 5;
            int columnHeight = 1023 / 10; //e.PageBounds.Height / 10;
            int tmpcolumnWidth = columnWidth;
            int tmpcolumnHeight = columnHeight;
            int pagrWidth = e.PageBounds.Width;
            int pageHeight = e.PageBounds.Height;
            Pen myPen = new Pen(System.Drawing.Color.Gray, 3);



            while (counter <= WeightGoods.Count) //WeightGoods.Count
            {

                top = 0;
                columnWidth = 0;
                columnHeight = 0;
                for (int i = 0; i < 10; i++)
                {
                    left = 0;

                    for (int j = 0; j < 5; j++)
                    {
                        if (counter <= WeightGoods.Count)
                        {
                            if (WeightGoods[counter].pathPhoto != null)
                            {
                                System.Drawing.Image waresImage = System.Drawing.Image.FromFile(WeightGoods[counter].pathPhoto);
                                waresImageWidth = waresImage.Width;
                                waresImageHeight = waresImage.Height;
                                double coef = 0;
                                if (waresImageHeight > waresImageWidth)
                                    coef = 100 / waresImageHeight;
                                else
                                    coef = 100 / waresImageWidth;
                                waresImageHeight = waresImageHeight * coef;
                                waresImageWidth = waresImageWidth * coef;

                                e.Graphics.DrawImage(waresImage, left + 25, top + 8, Convert.ToInt32(waresImageWidth), Convert.ToInt32(waresImageHeight));
                            }
                            SolidBrush myBrush = new SolidBrush(Color.White);
                            //Для номера кнопки
                            System.Drawing.Rectangle RectangleButtonPLU = new System.Drawing.Rectangle(left + 1, top + 1, 36, 30);
                            e.Graphics.FillRectangle(myBrush, RectangleButtonPLU);
                            //Назва
                            System.Drawing.Rectangle myRectangle = new System.Drawing.Rectangle(left + 1, top + 85, 140, 15);
                            e.Graphics.FillRectangle(myBrush, myRectangle);
                            e.Graphics.DrawString(WeightGoods[counter].name_wares, new Font("Arial", mainFontSize), Brushes.Black, myRectangle);

                            //Номер кнопки
                            //e.Graphics.DrawString(WeightGoods[counter].PLU.ToString(), new Font("Arial", 20), Brushes.Black, left, top + 30);
                            left += tmpcolumnWidth;
                            counter++;
                        }
                        else break;
                    }
                    top += tmpcolumnHeight;

                }
                if (counter % 50 == 0)
                {
                    break;
                }
            }
            columnWidth = 0;
            columnHeight = 0;
            for (int i = 0; i < 11; i++)
            {
                //Горизонтальна лінії
                e.Graphics.DrawLine(myPen, columnWidth, 0, columnWidth, pageHeight);
                //вертикальна
                e.Graphics.DrawLine(myPen, 0, columnHeight, pagrWidth, columnHeight);
                columnWidth += tmpcolumnWidth;
                columnHeight += tmpcolumnHeight;
            }
            if (counter < WeightGoods.Count)
            {
                //Has more pages??  
                e.HasMorePages = true;
            }
        }
        /// <summary>
        /// Кнопка для друку макету на кас
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrintLoyoutCASButton(object sender, RoutedEventArgs e)
        {
            //FindPhotoToPath();
            List<SQLWeightGoods> WeightGoodsFromSort = new List<SQLWeightGoods>();
            WeightGoods = new List<SQLWeightGoods>();
            WeightGoodsFromSort = connection.Query<SQLWeightGoods>($"{query5}'{SelectedWeightGroup}'").ToList();
            WeightGoodsFromSort = WeightGoodsFromSort.OrderBy(n => n.PLU).ToList();
            //впорядкування по PLU
            int j = 0;
            int i = 1;
            for (int k = 0; k < 2; k++)
            {

                for (; i < 101; i++)
                {

                    if (WeightGoodsFromSort.Count >= i && WeightGoodsFromSort[j].PLU == i)
                    {
                        WeightGoods.Add(WeightGoodsFromSort[j]);
                        j++;
                    }
                    else
                    {
                        WeightGoods.Add(new SQLWeightGoods { PLU = i, articl = "", code = "", name_wares = "" });
                    }
                    if (i % 5 == 0) // від 1 до 5, 11-15  ---- розділення на 2 сторінки
                    {
                        i += 5;
                        j += 5;
                    }
                }
                i = 6;
                j = 5;
            }

            WeightLinkToPhoto();
            Console.WriteLine(WeightGoods.Count);
            //Create a PrintPreviewDialog/PrintDialog object  
            System.Windows.Forms.PrintDialog previewDlg = new System.Windows.Forms.PrintDialog();
            //Create a PrintDocument object  
            PrintDocument pd = new PrintDocument();

            // 1 cm = 4.135 пунктів
            // A4 width: 827 Height: 1169
            //pd.DefaultPageSettings.PaperSize = new PaperSize("A5", 827, 584);
            counter = 0;

            pd.PrintPage += PrintWeightListCAS;
            //Set Document property of PrintPreviewDialog  
            previewDlg.Document = pd;
            //Display dialog  
            //previewDlg.Show();
            try
            {
                if (previewDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    previewDlg.Document.Print(); // печатаем
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// Видалення дублікатів (ЛИШАЮТЬСЯ ТІЛЬКИ PNG)
        /// </summary>
        void DeleteDuplicatePhotos()
        {
            //FindPhotoToPath();
            DuplicatePhotos = new List<PhotoInfo>();
            photoInfos = photoInfos.OrderBy(n => n.photoName).ToList();
            for (int i = 1; i < photoInfos.Count; i++)
                if (photoInfos[i].photoName == photoInfos[i - 1].photoName)
                {
                    DuplicatePhotos.Add(photoInfos[i]);
                    DuplicatePhotos.Add(photoInfos[i - 1]);
                }
            MessageBox.Show(DuplicatePhotos.Count.ToString());

            int countTMP = 0;
            foreach (var item in DuplicatePhotos)
            {
                if (item.photoPath.Contains("png"))
                {
                    countTMP++;
                    continue;
                }
                else
                {
                    File.Delete(item.photoPath);
                }
            }
            MessageBox.Show(countTMP.ToString());
        }
        /// <summary>
        /// Кнопка Друку макету  А5 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrintPriceTagsButton(object sender, RoutedEventArgs e)
        {
            //FindPhotoToPath();
            System.Windows.Controls.Button btn = sender as System.Windows.Controls.Button;

            WeightGoods = connection.Query<SQLWeightGoods>($"{query5}'{SelectedWeightGroup}'").ToList();
            WeightGoods = WeightGoods.OrderBy(n => n.PLU).ToList();
            WeightLinkToPhoto();

            //Create a PrintPreviewDialog/PrintDialog object  
            System.Windows.Forms.PrintDialog previewDlg = new System.Windows.Forms.PrintDialog();
            //Create a PrintDocument object  
            PrintDocument pd = new PrintDocument();

            // 1 cm = 4.135 пунктів
            // A4 width: 827 Height: 1169
            //pd.DefaultPageSettings.PaperSize = new PaperSize("A3", 827, 584);
            counter = 0;

            switch (btn.Name)
            {
                case "PrintListWeightA5":
                    pd.PrintPage += PrintPriceTagsA5;
                    break;
                case "PrintListWeightA4":
                    pd.PrintPage += PrintPriceTagsA4;
                    break;
            }
            //Set Document property of PrintPreviewDialog  
            previewDlg.Document = pd;
            //Display dialog  
            //previewDlg.Show();
            try
            {
                if (previewDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    previewDlg.Document.Print(); // печатаем
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// Друк макету А5
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PrintPriceTagsA5(object sender, PrintPageEventArgs e)
        {
            int left;
            int top;
            double waresImageWidth;
            double waresImageHeight;
            int columnWidth = e.PageBounds.Width;
            int columnHeight = e.PageBounds.Height / 2;
            Pen myPen = new Pen(System.Drawing.Color.Gray, 3);



            while (counter < WeightGoods.Count) //WeightGoods.Count
            {

                top = 0;

                left = 0;

                for (int j = 0; j < 2; j++)
                {
                    if (counter < WeightGoods.Count)
                    {
                        if (WeightGoods[counter].pathPhoto != null)
                        {
                            System.Drawing.Image waresImage = System.Drawing.Image.FromFile(WeightGoods[counter].pathPhoto);
                            waresImageWidth = waresImage.Width;
                            waresImageHeight = waresImage.Height;
                            double coef = 0;
                            if (waresImageHeight > waresImageWidth)
                                coef = 500 / waresImageHeight;
                            else
                                coef = 500 / waresImageWidth;
                            waresImageHeight = waresImageHeight * coef;
                            waresImageWidth = waresImageWidth * coef;

                            e.Graphics.DrawImage(waresImage, left + 285, top + 8, Convert.ToInt32(waresImageWidth), Convert.ToInt32(waresImageHeight));
                        }
                        SolidBrush myBrush = new SolidBrush(Color.White);
                        SolidBrush myGreenBrush = new SolidBrush(Color.Green);
                        //Назва
                        System.Drawing.Rectangle myRectangle = new System.Drawing.Rectangle(left + 1, top + 20, 250, 180);
                        e.Graphics.FillRectangle(myBrush, myRectangle);
                        e.Graphics.DrawString(WeightGoods[counter].name_wares, new Font("Arial", 36, System.Drawing.FontStyle.Bold), Brushes.Black, myRectangle);
                        //Артикул
                        System.Drawing.Rectangle ArticlRectangle = new System.Drawing.Rectangle(left + 1, top + columnHeight - 60, 250, 40);
                        e.Graphics.FillRectangle(myGreenBrush, ArticlRectangle);
                        e.Graphics.DrawString(WeightGoods[counter].articl, new Font("Arial", 30, System.Drawing.FontStyle.Bold), Brushes.White, ArticlRectangle);
                        //Номер кнопки
                        if (WeightGoods[counter].PLU != 0)
                            e.Graphics.DrawString(WeightGoods[counter].PLU.ToString(), new Font("Arial", 80, System.Drawing.FontStyle.Bold), Brushes.Black, columnWidth - 220, top + columnHeight - 125);
                        top += columnHeight;
                        counter++;
                    }
                    else break;
                }



                if (counter % 2 == 0)
                {
                    break;
                }
            }

            //Розділювач
            e.Graphics.DrawLine(myPen, 0, columnHeight, columnWidth, columnHeight);


            if (counter < WeightGoods.Count)
            {
                //Has more pages??  
                e.HasMorePages = true;
            }
        }
        /// <summary>
        /// Друк макету А4
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PrintPriceTagsA4(object sender, PrintPageEventArgs e)
        {
            int left;
            int top;
            double waresImageWidth, leftPositionPhoto;
            double waresImageHeight, topPositionPhoto;
            double sizeImage = 800;
            int columnWidth = e.PageBounds.Width;
            int columnHeight = e.PageBounds.Height;
            Pen myPen = new Pen(System.Drawing.Color.Gray, 3);



            while (counter < WeightGoods.Count) //WeightGoods.Count
            {

                top = 0;

                left = 0;


                if (counter < WeightGoods.Count)
                {
                    if (WeightGoods[counter].pathPhoto != null)
                    {
                        System.Drawing.Image waresImage = System.Drawing.Image.FromFile(WeightGoods[counter].pathPhoto);
                        //Маштабування
                        waresImageWidth = waresImage.Width;
                        waresImageHeight = waresImage.Height;
                        double coef = 0;
                        if (waresImageHeight > waresImageWidth)
                            coef = sizeImage / waresImageHeight;
                        else
                            coef = sizeImage / waresImageWidth;
                        waresImageHeight = waresImageHeight * coef;
                        waresImageWidth = waresImageWidth * coef;

                        //вирівнювання по центру
                        leftPositionPhoto = columnWidth / 2 - waresImageWidth / 2;
                        topPositionPhoto = columnHeight / 2 - waresImageHeight / 2;
                        e.Graphics.DrawImage(waresImage, Convert.ToInt32(leftPositionPhoto), Convert.ToInt32(topPositionPhoto), Convert.ToInt32(waresImageWidth), Convert.ToInt32(waresImageHeight));
                    }
                    SolidBrush myBrush = new SolidBrush(Color.White);
                    SolidBrush myGreenBrush = new SolidBrush(Color.Green);
                    //Назва
                    System.Drawing.Rectangle myRectangle = new System.Drawing.Rectangle(left, top, columnWidth, 200);
                    e.Graphics.FillRectangle(myGreenBrush, myRectangle);
                    e.Graphics.DrawString(WeightGoods[counter].name_wares, new Font("Arial", 60, System.Drawing.FontStyle.Bold), Brushes.White, myRectangle);
                    //Якщо товар ваговий
                    if (WeightGoods[counter].IsWeight)
                    {
                        var bmp = new Bitmap(Properties.Resources.Weight);

                        e.Graphics.DrawImage(bmp, 0, 200, 33, 50);
                    }
                    //Артикул
                    System.Drawing.Rectangle ArticlRectangle = new System.Drawing.Rectangle(left, top + columnHeight - 110, columnWidth, 110);
                    e.Graphics.FillRectangle(myGreenBrush, ArticlRectangle);
                    e.Graphics.DrawString($"    {WeightGoods[counter].articl}", new Font("Arial", 80, System.Drawing.FontStyle.Bold), Brushes.White, ArticlRectangle);
                    //Номер кнопки
                    if (WeightGoods[counter].PLU != 0)
                        e.Graphics.DrawString(WeightGoods[counter].PLU.ToString(), new Font("Arial", 80, System.Drawing.FontStyle.Bold), Brushes.Black, columnWidth - 220, columnHeight - 220);
                    counter++;
                }
                else break;

                break;

            }


            if (counter < WeightGoods.Count)
            {
                //Has more pages??  
                e.HasMorePages = true;
            }
        }
        /// <summary>
        /// Кнопка друку одного макету А5/А4
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrintOnePriceTagsButton(object sender, RoutedEventArgs e)
        {
            //FindPhotoToPath();
            System.Windows.Controls.Button btn = sender as System.Windows.Controls.Button;
            if (btn.DataContext is Wares)
            {
                Wares temp = btn.DataContext as Wares;
                WeightGoods = new List<SQLWeightGoods>();
                SQLWeightGoods item = new SQLWeightGoods()
                {
                    CodeWares = temp.kodeWares,
                    name_wares = temp.nameWares,
                    articl = temp.Articl,
                    code = temp.kodeWares,
                    PLU = 0,
                    pathPhoto = temp.photoPath,

                };
                WeightGoods.Add(item);
            }


            //Create a PrintPreviewDialog/PrintDialog object  
            System.Windows.Forms.PrintDialog previewDlg = new System.Windows.Forms.PrintDialog();
            //Create a PrintDocument object  
            PrintDocument pd = new PrintDocument();

            // 1 cm = 4.135 пунктів
            // A4 width: 827 Height: 1169
            //pd.DefaultPageSettings.PaperSize = new PaperSize("A3", 827, 584);
            counter = 0;
            switch (btn.Name)
            {
                case "PrintA5":
                    pd.PrintPage += PrintPriceTagsA5;
                    break;
                case "PrintA4":
                    pd.PrintPage += PrintPriceTagsA4;
                    break;
            }

            //Set Document property of PrintPreviewDialog  
            previewDlg.Document = pd;
            //Display dialog  
            //previewDlg.Show();
            try
            {
                if (previewDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    previewDlg.Document.Print(); // печатаем
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClickReportAssortmentMatrix(object sender, RoutedEventArgs e)
        {
            //FindPhotoToPath();
            //товари з активної асортиментної матриці
            AssortmentMatrix = connection.Query<SQLAssortmentMatrix>(query6).ToList();
            int countMissingPhotos = AssortmentMatrix.Count();
            foreach (var item in AssortmentMatrix)
            {
                foreach (var infoPhoto in photoInfos)
                {
                    if (item.code == infoPhoto.photoName)
                    {
                        item.isPhotoPresent = true;
                        countMissingPhotos--;
                        break;
                    }
                }
            }

            //створення строк для запису файлу
            List<string> StrWriteAssortmentMatrix = new List<string>();
            StrWriteAssortmentMatrix.Add($"Дата створення звіту:;{DateTime.UtcNow.ToString("d")}");
            StrWriteAssortmentMatrix.Add($"Кількість позицій в асортиментній матриці:;{AssortmentMatrix.Count()};Кількість відсутніх фото:;{countMissingPhotos}; Каталог з фото по якому сформовано звіт:; {PathToPhotoTextBox.Text}");
            StrWriteAssortmentMatrix.Add($"Назва товару;Штрихкод;Внутрішній код;Внутрішній артикул;Чи ваговий товар;Чи присутнє фото");
            foreach (var item in AssortmentMatrix)
            {
                StrWriteAssortmentMatrix.Add($"{item.name};{item.barcode_last};{item.code};{item.articul};{item.is_weight};{item.isPhotoPresent}");
            }

            //запис в файл
            try
            {
                File.AppendAllLines($"{SaveAssortmentMatrixPath.Text}Звіт по асортиментній матриці.csv", StrWriteAssortmentMatrix, System.Text.Encoding.GetEncoding("Windows-1251"));
                System.Windows.MessageBox.Show($"Шлях до файлу: {SaveAssortmentMatrixPath.Text}Звіт по асортиментній матриці.csv", "Файл збережено!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenToFilePathSaveAssortmentMatrix(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
                SaveAssortmentMatrixPath.Text = dialog.SelectedPath + @"\";
        }

        private void OpenToFilePathPhotoSuppliers(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                PhotoSuppliers = dialog.SelectedPath + @"\";
                PathToPhotoSuppliers.Text = PhotoSuppliers;
            }

        }

        private void OpenToFilePathRenamedPhotos(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                RenamedPhotos = dialog.SelectedPath + @"\";
                PathToRenamedPhotos.Text = RenamedPhotos;
            }
        }

        private void ChangePhotoSuppliersPath(object sender, TextChangedEventArgs e)
        {
            PhotoSuppliers = PathToPhotoSuppliers.Text;
            if (PathToPhotoSuppliers.Text != "")
                isPhotoSuppliersPath = true;

            else
                isPhotoSuppliersPath = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("isRenamedPathOk"));
        }

        private void ChangeRenamedPhotosPath(object sender, TextChangedEventArgs e)
        {
            RenamedPhotos = PathToRenamedPhotos.Text;
            if (PathToRenamedPhotos.Text != "")
                isRenamedPhotosPath = true;

            else
                isRenamedPhotosPath = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("isRenamedPathOk"));
        }

        private void RenamePhotos(object sender, RoutedEventArgs e)
        {
            string[] fileBarcode = null;
            if (Directory.Exists(PathToPhotoSuppliers.Text))
                fileBarcode = System.IO.Directory.GetFiles(PathToPhotoSuppliers.Text);
            else
            {
                System.Windows.MessageBox.Show($"Каталог {PhotoSuppliers} відсутній", "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var photoBarcode = new List<PhotoInfo>();



            for (int i = 0; i < fileBarcode.Length; i++)
            {
                try
                {

                    photoBarcode.Add(new PhotoInfo() { photoName = Path.GetFileNameWithoutExtension(fileBarcode[i]), photoPath = Path.GetFullPath(fileBarcode[i]), photoFullName = Path.GetFileName(fileBarcode[i]) });

                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }

            }


            int countRenamedPhotos = 0;
            List<string> missingFiles = new List<string>();

            //System.Windows.MessageBox.Show(photoArtcl.Count.ToString());

            foreach (var photo in photoBarcode)
            {
                //if (!listWares.Exists( x => x.barcode == photo.photoName ))
                //{
                //    missingFiles.Add(photo.photoName);
                //}
                foreach (var item in listWares)
                {
                    var tempBarcode = item.barcode.Split(new char[] { ',' });
                    for (int i = 0; i < tempBarcode.Length; i++)
                    {

                        if (photo.photoName == tempBarcode[i])
                        {
                            try
                            {
                                if (File.Exists(RenamedPhotos + item.code_wares + Path.GetExtension(photo.photoPath)))
                                {
                                    DialogResult result = MessageBox.Show($"Фото {item.code_wares + Path.GetExtension(photo.photoPath)} існує за вказаним шляхом! Замінити його?", "Увага!", MessageBoxButtons.YesNoCancel);
                                    if (result == System.Windows.Forms.DialogResult.Yes)
                                    {
                                        File.Delete(RenamedPhotos + item.code_wares + Path.GetExtension(photo.photoPath));
                                    }
                                    else break;
                                }
                                System.IO.File.Move(photo.photoPath, RenamedPhotos + item.code_wares + Path.GetExtension(photo.photoPath));
                                countRenamedPhotos++;
                                break;
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
            MessageBox.Show($"Знайдено файлів: {photoBarcode.Count}; Перейменовано і переміщено: {countRenamedPhotos}");
        }
    }
}

