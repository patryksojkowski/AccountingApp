using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.IO;
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
using System.ComponentModel;
using Microsoft.Win32;
using System.Globalization;

namespace AccountingApp
{
     public enum AccountType
    {
        [Description("Accounts Payable")]
        Payable,
        [Description("Accounts Receivable")]
        Receivable,
        [Description("All")]
        All
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow mainWindow;
        
        public DataTable dataTable = new DataTable();
        public MainWindow()
        {
            InitializeComponent();
            FillDataTable();
            mainWindow = this;
        }

        public void FillDataTable()
        {
            dataTable.Columns.Add(new DataColumn
            {
                ColumnName = "ID",
                ReadOnly = true
            });
            dataTable.Columns.Add("Name");
            dataTable.Columns.Add("Date");
            dataTable.Columns.Add("DueDate");
            dataTable.Columns.Add("Amount");
            dataTable.Columns.Add("Type");
            dataTable.Columns.Add("Info");

            var rand = new Random();
            var dateTime = DateTime.Now;
            var dueDateTime = DateTime.Now.AddMonths(1);
            AddRow(1, "John Smith", dateTime.AddDays(-rand.Next()%30), dueDateTime.AddDays(rand.Next()%60), rand.Next()%10000, AccountType.Payable, "");
            AddRow(2, "Margaret Johnson", dateTime.AddDays(-rand.Next() % 30), dueDateTime.AddDays(rand.Next() % 60), rand.Next() % 10000, AccountType.Payable, "");
            AddRow(3, "Emily Miller", dateTime.AddDays(-rand.Next() % 30), dueDateTime.AddDays(rand.Next() % 60), rand.Next() % 10000, AccountType.Receivable, "");
            AddRow(4, "Harry Williams", dateTime.AddDays(-rand.Next() % 30), dueDateTime.AddDays(rand.Next() % 60), rand.Next() % 10000, AccountType.Receivable, "");
            AddRow(5, "Oscar Brown", dateTime.AddDays(-rand.Next() % 30), dueDateTime.AddDays(rand.Next() % 60), rand.Next() % 10000, AccountType.Receivable, "");
            AddRow(6, "Sophia Davies", dateTime.AddDays(-rand.Next() % 30), dueDateTime.AddDays(rand.Next() % 60), rand.Next() % 10000, AccountType.Receivable, "");
            AddRow(7, "Michael Wilson", dateTime.AddDays(-rand.Next() % 30), dueDateTime.AddDays(rand.Next() % 60), rand.Next() % 10000, AccountType.Receivable, "");
            AddRow(8, "Megan Jones", dateTime.AddDays(-rand.Next() % 30), dueDateTime.AddDays(rand.Next() % 60), rand.Next() % 10000, AccountType.Receivable, "");
            AddRow(9, "Isabella King", dateTime.AddDays(-rand.Next() % 30), dueDateTime.AddDays(rand.Next() % 60), rand.Next() % 10000, AccountType.Receivable, "");
            AddRow(10, "Daniel Wilson", dateTime.AddDays(-rand.Next() % 30), dueDateTime.AddDays(rand.Next() % 60), rand.Next() % 10000, AccountType.Receivable, "");


            DataGrid.ItemsSource = dataTable.AsDataView();
        }
        public void AddRow(int id, string name, DateTime date, DateTime duedate, int amount, AccountType type, string info)
        {
            var row = dataTable.NewRow();
            row["ID"] = id;
            row["Name"] = name;
            row["Date"] = date.ToShortDateString();
            row["DueDate"] = duedate.ToShortDateString();
            row["Amount"] = amount;
            row["Type"] = type.ToString();
            row["Info"] = info;
            dataTable.Rows.Add(row);
        }

        private void bt_Add_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new AddForm(this);
            if (dlg.ShowDialog() == true)
            {
                
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (Rb_All is null || Rb_AP is null || Rb_AR is null)
            {
                return;
            }

            if (e.Source == Rb_All)
            {
                Rb_AP.IsChecked = Rb_AR.IsChecked = false;
                FilterDataGrid(AccountType.All);
            }
            else if (e.Source == Rb_AP)
            {
                Rb_All.IsChecked = Rb_AR.IsChecked = false;
                FilterDataGrid(AccountType.Payable);
            }
            else // rb_AR
            {
                Rb_All.IsChecked = Rb_AP.IsChecked = false;
                FilterDataGrid(AccountType.Receivable);
            }

            void FilterDataGrid(AccountType type)
            {
                if (type == AccountType.All)
                {
                    DataGrid.ItemsSource = dataTable.AsDataView();
                }
                else
                {
                    DataGrid.ItemsSource = dataTable.AsEnumerable().Where(x => (string)x["Type"] == type.ToString()).AsDataView();
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var saveDlg = new SaveFileDialog
            {
                Filter = "CSV file (*.csv)|*.csv"
            };
            if(saveDlg.ShowDialog() == true)
            {
                SaveAsCSV();
            }

            void SaveAsCSV()
            {
                var sb = new StringBuilder();
                foreach(var row in dataTable.AsEnumerable())
                {
                    sb.AppendLine($"{row["ID"]},{row["Name"]},{row["Date"]},{row["DueDate"]},{row["Amount"]},{row["Type"]},{row["Info"]}");
                }
                File.WriteAllText(saveDlg.FileName, sb.ToString());
            }
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            var printDlg = new PrintDialog()
            {
               
            };
            printDlg.PrintVisual(DataGrid, "Printing...");
            printDlg.ShowDialog();
        }

        public void Open_Click(object sender, RoutedEventArgs e)
        {
            if(!DataGrid.Items.IsEmpty)
            {
                var result = MessageBox.Show("Do you want to save current work?", "Save current data", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    Save_Click(null, null);
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    return;
                }
            }
            var openDlg = new OpenFileDialog
            {
                Filter = "CSV file (*.csv)|*.csv"
            };
            if(openDlg.ShowDialog() == true)
            {
                dataTable.Clear();
                var contentArr = File.ReadAllLines(openDlg.FileName);
                foreach(var item in contentArr)
                {
                    var itemSplit = item.Split(',');
                    try
                    {
                        AddRow(int.Parse(itemSplit[0]), itemSplit[1], DateTime.Parse(itemSplit[2]), DateTime.Parse(itemSplit[3]), int.Parse(itemSplit[4]), (AccountType)Enum.Parse(typeof(AccountType), itemSplit[5]), itemSplit[6]);
                    }
                    catch (Exception ex)
                    {
                        Debug.Write(ex.Message);
                    }
                }
                DataGrid.ItemsSource = dataTable.AsDataView();
            }
        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var header = e.Column.Header.ToString();
            if(header == "Info")
            {
                e.Column.Header = "Additional info";
            }
            
        }

        private void bt_Remove_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
