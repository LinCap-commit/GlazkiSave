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

namespace ГлазаЗеркалоДуши
{
    /// <summary>
    /// Логика взаимодействия для PriorityDialogWindow.xaml
    /// </summary>
    public partial class PriorityDialogWindow : Window
    {
        public PriorityDialogWindow(int Maxed)
        {
            InitializeComponent();
            PriorBox.Text = Maxed.ToString();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(PriorBox.Text, out int i) && PriorBox.Text != null)
                this.DialogResult = true;
            else
                MessageBox.Show("Введите число");
        }
        public int prior
        {
            get { return Convert.ToInt32(PriorBox.Text); }
        }
    }
}
