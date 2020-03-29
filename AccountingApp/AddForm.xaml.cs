using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace AccountingApp
{
    /// <summary>
    /// Interaction logic for AddForm.xaml
    /// </summary>
    public partial class AddForm : Window
    {
        MainWindow mainWindow;
        public AddForm(MainWindow mw)
        {
            InitializeComponent();
            mainWindow = mw;
        }

        public void bt_Add_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.AddRow(mainWindow.dataTable.Rows.Count + 1, tb_Name.Text, (DateTime) Dp_Date.SelectedDate,(DateTime) Dp_DueDate.SelectedDate, int.Parse(tb_Amount.Text),
                (bool)Rb_Payable.IsChecked ? AccountType.Payable : AccountType.Receivable, tb_Info.Text);
            Close();
        }

        private void bt_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (Rb_Payable is null || Rb_Receivable is null)
            {
                return;
            }

            if (e.Source == Rb_Payable)
            {
                Rb_Receivable.IsChecked = false;
            }
            else
            {
                Rb_Payable.IsChecked = false;
            }
        }
    }
}
