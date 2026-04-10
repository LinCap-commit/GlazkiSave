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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace ГлазаЗеркалоДуши
{
    /// <summary>
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        private Agent currentAgent = new Agent();
        public AddEditPage(Agent SelAgent)
        {
            InitializeComponent();
            if (SelAgent != null)
            {
                currentAgent = SelAgent;
                ComboType.SelectedIndex = currentAgent.AgentTypeID - 1;
            }
            else
                AllofProduct.Visibility = Visibility.Hidden;
            
            DataContext = currentAgent;

            var PSales = MuzafarovGlazkiEntities.GetContext().ProductSale.Where(ps => ps.AgentID == currentAgent.ID).ToList();
            if (PSales.Count > 0)            
                AgentsProducts.ItemsSource = PSales;            
            else 
                Sell.Text = "Продажи отсутствуют";
            
            UpdateCB();
        }
        private void ChangeLogoBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog=new OpenFileDialog();
            if (fileDialog.ShowDialog() == true)
            {
                currentAgent.Logo = fileDialog.FileName;
                LogoImage.Source=new BitmapImage(new Uri(fileDialog.FileName));
                int cut = currentAgent.Logo.IndexOf("agents");
                currentAgent.Logo = currentAgent.Logo.Remove(0,cut-1);
        //        MessageBox.Show(currentAgent.Logo);
            }
        }
        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(currentAgent.Title))
                errors.AppendLine("Укажите наименование агента");
            if (string.IsNullOrWhiteSpace(currentAgent.Address))
                errors.AppendLine("Укажите адрес агента");
            if (string.IsNullOrWhiteSpace(currentAgent.DirectorName))
                errors.AppendLine("Укажите имя директора агента");
            if (string.IsNullOrWhiteSpace(currentAgent.Priority.ToString()))
                errors.AppendLine("Укажите приоритет агента");
            if (string.IsNullOrWhiteSpace(currentAgent.INN))
                errors.AppendLine("Укажите ИНН агента");
            if (string.IsNullOrWhiteSpace(currentAgent.KPP))
                errors.AppendLine("Укажите КПП агента");
            if (string.IsNullOrWhiteSpace(currentAgent.Phone))
                errors.AppendLine("Укажите телефон агента");
            else
            {
                string ph = currentAgent.Phone.Replace("(", "").Replace(")", "").Replace("-", "").Replace("+", "");
                if ((ph[1] == '9' || ph[1] == '4' || ph[1] == '8' && ph.Length != 11)
                    || (ph[1] == '3' && ph.Length != 12))
                    errors.AppendLine("Укажите правильно телефон агента");
            }
            if (ComboType.SelectedItem == null)
                errors.AppendLine("Укажите тип агента");
            else
                currentAgent.AgentTypeID = ComboType.SelectedIndex + 1;
            if (currentAgent.Priority <= 0)
                errors.AppendLine("Укажите подходящий приоритет агента");
            
            if (string.IsNullOrWhiteSpace(currentAgent.Email))
                errors.AppendLine("Укажите почту агента");
            else if (!currentAgent.Email.Contains("@") || (!currentAgent.Email.Contains(".ru")) && !currentAgent.Email.Contains(".net") 
                && !currentAgent.Email.Contains(".com") && !currentAgent.Email.Contains(".org"))
                errors.AppendLine("Укажите почту агента правильно");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }
            if (currentAgent.ID == 0)
                MuzafarovGlazkiEntities.GetContext().Agent.Add(currentAgent);
            try
            {
                MuzafarovGlazkiEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex) { 
                MessageBox.Show(ex.InnerException.ToString());
            }
        }
        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var SoldProducts = MuzafarovGlazkiEntities.GetContext().ProductSale.ToList();
            SoldProducts = SoldProducts.Where(p => p.AgentID == currentAgent.ID).ToList();

            if (SoldProducts.Count != 0)            
                MessageBox.Show("Невозможно удалить агента, существует " + SoldProducts.Count.ToString() + " реализованных(-ый) прод.");
            else
            {
                if (MessageBox.Show("Вы точно хотите удалить агента?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        //еще нужно связанные записи какието удалять
                        MuzafarovGlazkiEntities.GetContext().Agent.Remove(currentAgent);
                        var agentPH = MuzafarovGlazkiEntities.GetContext().AgentPriorityHistory.ToList();
                        foreach (AgentPriorityHistory aph in agentPH)
                            if (aph.AgentID == currentAgent.ID)
                                MuzafarovGlazkiEntities.GetContext().AgentPriorityHistory.Remove(aph);
                        MuzafarovGlazkiEntities.GetContext().SaveChanges();
                      //  MessageBox.Show("Агент удален");
                        Manager.MainFrame.GoBack();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }    
            }
        }






        private void AddProd_Click(object sender, RoutedEventArgs e)
        {
            if (currentAgent.ID != 0)
            {
                Product prod = (Product)AllProdCB.SelectedItem;
                if (AllProdCB.SelectedIndex != -1 && CountTB.Text != null && int.TryParse(CountTB.Text, out int i))
                {
                    ProductSale PS = new ProductSale
                    {
                        ProductID = prod.ID,
                        AgentID = currentAgent.ID,
                        SaleDate = DateTime.Today,
                        ProductCount = Convert.ToInt32(CountTB.Text)
                    };
                    MuzafarovGlazkiEntities.GetContext().ProductSale.Add(PS);
                    MuzafarovGlazkiEntities.GetContext().SaveChanges();
                    AgentsProducts.Items.Refresh();
                    AllProdCB.SelectedIndex = -1;
                    CountTB.Text = "";
                    Sell.Text = "Продажи";
                }
                else
                    MessageBox.Show("Выберите продукт для добавления");
            }
            else
                MessageBox.Show("Добавление продаж недоступно", "Сначала добавьте Агента!");
        }

        private void DelProd_Click(object sender, RoutedEventArgs e)
        {
            var psToDel = (sender as Button).DataContext as ProductSale;
            if (MessageBox.Show("Вы точно хотите удалить информацию о реализации?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    MuzafarovGlazkiEntities.GetContext().ProductSale.Remove(psToDel);
                    MuzafarovGlazkiEntities.GetContext().SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            AgentsProducts.Items.Refresh();
        }
        private void UpdateCB()
        {
            var Prods = MuzafarovGlazkiEntities.GetContext().Product.Where(p => p.Title.Contains(SearchProd.Text)).ToList();
            AllProdCB.ItemsSource = Prods;
        }

        private void SearchProd_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateCB();
        }
    }
}
