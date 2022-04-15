using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Dapper;
using System.Data.SqlClient;
using Microsoft.Win32;
using System.Collections.Generic;

namespace PhotoCheck
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public ObservableCollection<Wares> ListWares { get; set; }
        public eTypeCommit TypeCommit { get; set; }
        public string pathPhoto { get; set; } = @"d:\Pictures\Products\";
        public string query1 = @"SELECT w.code_wares,w.name_wares,w.Code_Direction FROM dbo.Wares w WHERE w.Code_Direction="; //000148259
        public string varConectionString = @"Server=10.1.0.22;Database=DW;Uid=dwreader;Pwd=DW_Reader;Connect Timeout=180;";
        public SqlConnection connection = null;
        public string SerchCode { get; set; }


        public MainWindow()
        {
            InitializeComponent();

            
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
                        MessageBox.Show("Error");
                        break;
                }
                //if (rbtn.Content.ToString() == "Залишити фото")
                //{
                //    temp.savePhotoStatus = true;
                //}
                //else temp.savePhotoStatus = false;
                //MessageBox.Show(temp.savePhotoStatus.ToString());
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
            MessageBox.Show(dialog.SelectedPath);
            //OpenFileDialog openFileDialog = new OpenFileDialog();
            //if (openFileDialog.ShowDialog() == true)
            //    MessageBox.Show(openFileDialog.FileName);
        }

        private void RunButton(object sender, RoutedEventArgs e)
        {
            
            string[] files = System.IO.Directory.GetFiles(pathPhoto);
            int[] photoName = new int[files.Length];
            ListWares = new ObservableCollection<Wares>();
            ListWares.Clear();
            WaresList.ItemsSource = ListWares;
            //MessageBox.Show(query1 + SerchCode);
            for (int i = 0; i < files.Length; i++)
            {
                try
                {
                    photoName[i] = Convert.ToInt32(System.IO.Path.GetFileNameWithoutExtension(files[i]));
                    //MessageBox.Show(files[i]);
                    //MessageBox.Show(photoName[i].ToString());

                }
                catch (Exception)
                {
                    MessageBox.Show(System.IO.Path.GetFileNameWithoutExtension(files[i]) + " - назвіть фото кодом або це взагалі не фото(");
                }

            }
            string aa = query1 + SerchCode;
            connection = new SqlConnection(varConectionString);
            connection.Open();
            var listWares = connection.Query<SQLWares>(aa).ToList();
            foreach (var item in listWares)
            {
                for (int j = 0; j < photoName.Length; j++)
                {
                    
                    //MessageBox.Show(photoName[j].ToString());
                    if (item.code_wares == photoName[j])
                    {
                        Wares dataUser = new Wares()
                        {
                            photo = files[j],
                            kodeWares = item.code_wares,
                            nameWares = item.name_wares
                        };
                        //MessageBox.Show(item.code_wares.ToString());
                        ListWares.Add(dataUser);
                        //RadioButtonList.Items.Add(dataUser);
                        break;
                    }
                }
            }
            
            

            //MessageBox.Show(ListWares.Count().ToString());
            WaresList.ItemsSource = ListWares;
            
        }
    }

    public enum eTypeCommit
    {
        Auto,
        Manual
    }

    public class SQLWares
    {
        public int code_wares { get; set; }
        public string name_wares { get; set; }
        public string Code_Direction { get; set; }
    }
    public class CodeGroup
    {
        public string Code_Direction { get; set; }
        public string name { get; set; }
        public bool Show { get; set; }
    }
    public class Wares
    {
        public string photo { get; set; }
        public int kodeWares { get; set; }
        public string nameWares { get; set; }
        public int savePhotoStatus { get; set; } // 0-лишити фото; 1-невірне фото; 2-невірний код

    }
}
