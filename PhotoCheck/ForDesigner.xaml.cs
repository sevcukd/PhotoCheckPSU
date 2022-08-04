using System.Windows.Controls;
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
using System.Drawing;
using System.Drawing.Printing;
using RadioButton = System.Windows.Controls.RadioButton;
using Font = System.Drawing.Font;
using MessageBox = System.Windows.Forms.MessageBox;

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
        public List<SQLKasaList> KasaList { get; set; }
        public List<SQLExpressGoods> ExpressGoods { get; set; }
        List<SQLExpressGoods> SortedExpressGoods { get; set; }
        public List<PhotoInfo> DuplicatePhotos { get; set; }
        public string SelectedExpressGoodsCode { get; set; }
        public string SelectedExpressGoodsName { get; set; }
        int counter = 0;
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
        public bool isSelectedExpressGoods
        {
            get
            {
                if (SelectedExpressGoodsCode != null)
                    return true;
                else return false;
            }
        }

        public string query1 = @"SELECT w.code_wares,w.name_wares,w.Code_Direction, w.articl FROM dbo.Wares w "; //000148259
        public string query2 = @"SELECT _code, _CASH_place._Description FROM DW.dbo.V1C_DIM_OPTION_WPC _CASH_place";
        public string query3 = @"SELECT  g.Order_Button , g.Name_Button, w1.code_wares AS CodeWares ,w1.name_wares,w1.articl,
  (SELECT max(bc.bar_code)  FROM barcode bc WHERE bc.code_wares=w1.code_wares) AS bar_code,w1.IsWeight
  FROM DW.dbo.V1C_DIM_OPTION_WPC O  
  JOIN DW.dbo.V1C_DIM_OPTION_WPC_FAST_GROUP G ON o._IDRRef=G._Reference18850_IDRRef
  JOIN DW.dbo.V1C_DIM_OPTION_WPC_FAST_WARES W ON o._IDRRef = W._Reference18850_IDRRef AND G.Order_Button_wares = W.Order_Button
  JOIN dw.dbo.Wares w1 ON w.Wares_RRef=w1._IDRRef
    WHERE o._Code=";
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
            KasaList = connection.Query<SQLKasaList>(query2).ToList();
            KasaListShow.ItemsSource = KasaList;


            //System.Windows.MessageBox.Show(listWares[0].articl);
            //FindPhotoToPath();

        }


        private void FindPhotoToPath()
        {
            string[] files = null;
            try
            {
                files = System.IO.Directory.GetFiles(pathToPhoto);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Вкажіть правильний шлях! {ex.Message}", "Увага!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            FindPhotoToPath();
            ListWares = new ObservableCollection<Wares>();
            ListWares.Clear();



            int temp = CodeWaresTextBox.Text.Length;
            if (temp == 0)
            {
                System.Windows.MessageBox.Show("Введіть код!", "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
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
            FindPhotoToPath();
            ListWares = new ObservableCollection<Wares>();
            ListWares.Clear();



            int temp = ArtclWaresTextBox.Text.Length;
            if (temp == 0)
            {
                System.Windows.MessageBox.Show("Введіть артикул!", "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
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

        private void OpenToFilePathSaveCsv(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            SaveCsvPath.Text = dialog.SelectedPath + @"\";
        }

        private void SaveCsvButton(object sender, RoutedEventArgs e)
        {
            FindPhotoToPath();
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

        private void PrintExpressGoodsButton(object sender, RoutedEventArgs e)
        {
            FindPhotoToPath();
            //пошук всіх швидких товарів
            ExpressGoods = connection.Query<SQLExpressGoods>($"{query3}'{SelectedExpressGoodsCode}'").ToList();

            //сортування по групах
            SortedExpressGoods = ExpressGoods.OrderBy(n => n.Name_Button).ToList();

            LinkToPhoto();

            //Create a PrintPreviewDialog/PrintDialog object  
            System.Windows.Forms.PrintDialog previewDlg = new System.Windows.Forms.PrintDialog();
            //Create a PrintDocument object  
            PrintDocument pd = new PrintDocument();
            //Add print-page event handler
            counter = 0;
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



            // объект для печати
            //PrintDocument printDocument = new PrintDocument();

            //// обработчик события печати
            //printDocument.PrintPage += PrintPageHandler;

            //// диалог настройки печати
            //System.Windows.Forms.PrintDialog printDialog = new System.Windows.Forms.PrintDialog();

            //// установка объекта печати для его настройки
            //printDialog.Document = printDocument;
            // если в диалоге было нажато ОК
            //try
            //{
            //    if (printDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //        printDialog.Document.Print(); // печатаем
            //}
            //catch (Exception ex)
            //{
            //    System.Windows.MessageBox.Show(ex.Message, "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
            //}

        }

        public void LinkToPhoto()
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
        // обработчик события печати
        //void PrintPageHandler(object sender, PrintPageEventArgs e)
        //{
        //    while (current != 10)
        //    {
        //        current++;
        //        PrintPages(SortedExpressGoods, e);

        //    }
        //}
        //void PrintPages(List<SQLExpressGoods> sortExpressGoods, PrintPageEventArgs e)
        //{

        //    var barcode = new BarcodeLib.Barcode();
        //    int left = 20;
        //    int top = 10;
        //    int mainFontSize = 14;
        //    int totalFontSize = 10;
        //    int counter = 0;
        //    System.Drawing.Image imageBarcode;
        //    foreach (var expressGoods in sortExpressGoods)
        //    {
        //        //Pen myPen = new Pen(System.Drawing.Color.Red, 5);
        //        //System.Drawing.Rectangle myRectangle = new System.Drawing.Rectangle(left, top, 200, 160);
        //        //e.Graphics.DrawRectangle(myPen, myRectangle);

        //        //Фото
        //        if (expressGoods.pathPhoto != null)
        //            e.Graphics.DrawImage(System.Drawing.Image.FromFile(expressGoods.pathPhoto), left + 350, top + 10, 100, 100);

        //        if (expressGoods.bar_code != null && expressGoods.bar_code.Length == 13)
        //        {
        //            try
        //            {
        //                imageBarcode = barcode.Encode(BarcodeLib.TYPE.EAN13, expressGoods.bar_code, Color.Black, Color.White, 290, 120);
        //                e.Graphics.DrawImage(imageBarcode, 650, top + 20, 150, 60);
        //            }
        //            catch (Exception)
        //            {
        //                System.Windows.MessageBox.Show($"{expressGoods.name_wares} - не правильний штрихкод: {expressGoods.bar_code}", "Увага!", MessageBoxButton.OK, MessageBoxImage.Error);
        //            }

        //        }


        //        //e.Graphics.DrawString("Назва товару:", new Font("Arial", totalFontSize), Brushes.Black, left, top);
        //        e.Graphics.DrawString(expressGoods.name_wares, new Font("Arial", mainFontSize, System.Drawing.FontStyle.Bold), Brushes.Black, left, top += 14);
        //        e.Graphics.DrawString("Артикул:", new Font("Arial", totalFontSize), Brushes.Black, left, top += 25);

        //        SolidBrush myBrush = new SolidBrush(Color.Green);
        //        System.Drawing.Rectangle myRectangle = new System.Drawing.Rectangle(left, top += 14, 90, 20);
        //        e.Graphics.FillRectangle(myBrush, myRectangle);
        //        e.Graphics.DrawString(expressGoods.articl, new Font("Arial", mainFontSize, System.Drawing.FontStyle.Bold), Brushes.White, myRectangle);
        //        e.Graphics.DrawString("Назва групи товарів:", new Font("Arial", totalFontSize), Brushes.Black, left, top += 25);
        //        e.Graphics.DrawString(expressGoods.Name_Button, new Font("Arial", mainFontSize), Brushes.Black, left, top += 14);

        //        //top += 20;
        //        Pen myPen = new Pen(System.Drawing.Color.Gray, 3);
        //        e.Graphics.DrawLine(myPen, 0, top + 23, 1000, top + 23);
        //        top += 15;
        //        counter++;
        //        if (counter >= 11)
        //        {
        //            current = 0;
        //            e.HasMorePages = true;
        //        }
        //    }
        //}

        public void pd_PrintPage(object sender, PrintPageEventArgs e)
        {

            var barcode = new BarcodeLib.Barcode();
            int left = 20;
            int top = 20;
            int mainFontSize = 14;
            int totalFontSize = 10;
            double waresImageWidth;
            double waresImageHeight;
            while (counter < SortedExpressGoods.Count)
            {
                System.Drawing.Image imageBarcode;

                //Pen myPen = new Pen(System.Drawing.Color.Red, 5);
                //System.Drawing.Rectangle myRectangle = new System.Drawing.Rectangle(left, top, 200, 160);
                //e.Graphics.DrawRectangle(myPen, myRectangle);

                //Фото
                if (SortedExpressGoods[counter].pathPhoto != null)
                {
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

                    e.Graphics.DrawImage(waresImage, left + 450, top + 13, Convert.ToInt32(waresImageWidth), Convert.ToInt32(waresImageHeight));
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
                    //else
                    //{
                    //    string Code128String = Convert.ToInt32(SortedExpressGoods[counter].articl).ToString();
                    //    try
                    //    {
                    //        imageBarcode = barcode.Encode(BarcodeLib.TYPE.CODE128, Code128String, Color.Red, Color.White, 290, 120);
                    //        e.Graphics.DrawImage(imageBarcode, 650, top + 30, 150, 60);
                    //        e.Graphics.DrawString("***", new Font("Arial", 8), Brushes.Red, 650, top+15);
                    //    }
                    //    catch (Exception)
                    //    {
                    //        System.Windows.MessageBox.Show($"{SortedExpressGoods[counter].name_wares} - не коректні дані!", "Увага!", MessageBoxButton.OK, MessageBoxImage.Error);
                    //    }
                    //}

                }


                //e.Graphics.DrawString("Назва товару:", new Font("Arial", totalFontSize), Brushes.Black, left, top);
                e.Graphics.DrawString(SortedExpressGoods[counter].name_wares, new Font("Arial", mainFontSize, System.Drawing.FontStyle.Italic), Brushes.Black, left, top += 14);
                e.Graphics.DrawString("Артикул:", new Font("Arial", totalFontSize), Brushes.Black, left, top += 25);

                SolidBrush myBrush = new SolidBrush(Color.Green);
                System.Drawing.Rectangle myRectangle = new System.Drawing.Rectangle(left, top += 14, 100, 20);
                e.Graphics.FillRectangle(myBrush, myRectangle);
                e.Graphics.DrawString(SortedExpressGoods[counter].articl, new Font("Arial", mainFontSize, System.Drawing.FontStyle.Bold), Brushes.White, myRectangle);
                e.Graphics.DrawString("Назва групи товарів:", new Font("Arial", totalFontSize), Brushes.Black, left, top += 25);
                e.Graphics.DrawString(SortedExpressGoods[counter].Name_Button, new Font("Arial", mainFontSize), Brushes.Black, left, top += 14);

                //top += 20;
                Pen myPen = new Pen(System.Drawing.Color.Gray, 3);
                e.Graphics.DrawLine(myPen, 0, top + 23, 1000, top + 23);
                top += 15;
                if (counter < SortedExpressGoods.Count)
                    counter++;

                if (counter % 10 == 0)
                {
                    break;
                }

            }
            if (counter < SortedExpressGoods.Count)
            {
                //Has more pages??  
                e.HasMorePages = true;
            }
        }
        public void PrintWeightListCAS(object sender, PrintPageEventArgs e)
        {
            int left;
            int top;
            int mainFontSize = 14;
            double waresImageWidth;
            double waresImageHeight;
            int columnWidth = e.PageBounds.Width / 5;
            int columnHeight = e.PageBounds.Height / 10;
            int tmpcolumnWidth = columnWidth;
            int tmpcolumnHeight = columnHeight;
            int pagrWidth = e.PageBounds.Width;
            int pageHeight = e.PageBounds.Height;
            while (counter < SortedExpressGoods.Count)
            {
                Pen myPen = new Pen(System.Drawing.Color.Gray, 3);
                top = 0;
                columnWidth = 0;
                columnHeight = 0;
                for (int i = 0; i < 10; i++)
                {
                    left = 0;
                    //Горизонтальна лінії
                    e.Graphics.DrawLine(myPen, columnWidth, 0, columnWidth, pageHeight);
                    //вертикальна
                    e.Graphics.DrawLine(myPen, 0, columnHeight, pagrWidth, columnHeight);
                    columnWidth += tmpcolumnWidth;
                    columnHeight += tmpcolumnHeight;
                    for (int j = 0; j < 5; j++)
                    {

                        if (counter < SortedExpressGoods.Count)
                        {
                            if (SortedExpressGoods[counter].pathPhoto != null)
                            {
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

                                e.Graphics.DrawImage(waresImage, left + 25, top + 8, Convert.ToInt32(waresImageWidth), Convert.ToInt32(waresImageHeight));
                            }
                            SolidBrush myBrush = new SolidBrush(Color.Green);
                            System.Drawing.Rectangle myRectangle = new System.Drawing.Rectangle(left + 15, top, 100, 20);
                            e.Graphics.FillRectangle(myBrush, myRectangle);
                            e.Graphics.DrawString(SortedExpressGoods[counter].articl, new Font("Arial", mainFontSize, System.Drawing.FontStyle.Bold), Brushes.White, myRectangle);
                            left += tmpcolumnWidth;
                            counter++;
                        }
                        else break;
                    }
                    top += tmpcolumnHeight;

                }

                //top += 15;
                if (counter % 50 == 0)
                {
                    break;
                }

            }
            if (counter < SortedExpressGoods.Count)
            {
                //Has more pages??  
                e.HasMorePages = true;
            }
        }

        private void PrintLoyoutCASButton(object sender, RoutedEventArgs e)
        {
            FindPhotoToPath();

           // пошук всіх швидких товарів
            ExpressGoods = connection.Query<SQLExpressGoods>($"{query3}'{SelectedExpressGoodsCode}'").ToList();

            //сортування по групах
            SortedExpressGoods = ExpressGoods.OrderBy(n => n.Name_Button).ToList();

            LinkToPhoto();

            //Create a PrintPreviewDialog/PrintDialog object  
            System.Windows.Forms.PrintDialog previewDlg = new System.Windows.Forms.PrintDialog();
            //Create a PrintDocument object  
            PrintDocument pd = new PrintDocument();
            // A4 width: 827 Height: 1169
            //pd.DefaultPageSettings.PaperSize = new PaperSize("A3", 827, 584);
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
            FindPhotoToPath();
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
    }
}

