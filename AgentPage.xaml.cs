using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace ГлазаЗеркалоДуши
{
    /// <summary>
    /// Логика взаимодействия для AgentPage.xaml
    /// </summary>
    public partial class AgentPage : Page
    {
        int CountPage;
        int CurrentPage = 0;
        List<Agent> AgentsViewed = new List<Agent>();
        public List<Agent> CurrentAgents { get; set; } = MuzafarovGlazkiEntities.GetContext().Agent.ToList();
        public AgentPage()
        {
            InitializeComponent();
            SortBox.SelectedIndex = 0;
            FilterBox.SelectedIndex = 0;          
            PriorChangeBtn.Visibility = Visibility.Hidden;
            UpdateAgentPage();
        }

        private void UpdateAgentPage()
        {
            var currentAgents = MuzafarovGlazkiEntities.GetContext().Agent.ToList();

            //сортировка

            if (SortBox.SelectedIndex == 1)
                currentAgents = currentAgents.OrderBy(a => a.Title).ToList();
            if (SortBox.SelectedIndex == 2)
                currentAgents = currentAgents.OrderByDescending(a => a.Title).ToList();
            if (SortBox.SelectedIndex == 3)
                currentAgents = currentAgents.OrderBy(a => a.Discount).ToList();
            if (SortBox.SelectedIndex == 4)
                currentAgents = currentAgents.OrderByDescending(a => a.Discount).ToList();
            if (SortBox.SelectedIndex == 5)
                currentAgents = currentAgents.OrderBy(a => a.Priority).ToList();
            if (SortBox.SelectedIndex == 6)
                currentAgents = currentAgents.OrderByDescending(a => a.Priority).ToList();
          //  MessageBox.Show("sortol");
            //фильтр

            if (FilterBox.SelectedIndex == 1)
                currentAgents = currentAgents.Where(a => a.AgentTypeID == 1).ToList();
            if (FilterBox.SelectedIndex == 2)
                currentAgents = currentAgents.Where(a => a.AgentTypeID == 2).ToList();
            if (FilterBox.SelectedIndex == 3)
                currentAgents = currentAgents.Where(a => a.AgentTypeID == 3).ToList();
            if (FilterBox.SelectedIndex == 4)
                currentAgents = currentAgents.Where(a => a.AgentTypeID == 4).ToList();
            if (FilterBox.SelectedIndex == 5)
                currentAgents = currentAgents.Where(a => a.AgentTypeID == 5).ToList();
            if (FilterBox.SelectedIndex == 6)
                currentAgents = currentAgents.Where(a => a.AgentTypeID == 6).ToList();
         //   MessageBox.Show("filtol");
            //поиск

            currentAgents = currentAgents.Where(a => a.Title.ToLower().Contains(SearchBox.Text.ToLower())   //по имени

            || a.Phone.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "")
            .Contains(SearchBox.Text.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", ""))     //по телефону

            || a.Email.ToLower().Contains(SearchBox.Text.ToLower())).ToList();    //по емайл

            AgentListView.ItemsSource = currentAgents.ToList();
            AgentListView.Items.Refresh();

            LeftAg = currentAgents;

        // MessageBox.Show("searol");
            
            ChangePage(0, 0);
        }
        List<Agent> LeftAg;
        private void ChangePage(int dir, int? selPage)
        {            
            int AgentsOnPage = 10;
            
            CountPage = LeftAg.Count / AgentsOnPage; if (CurrentAgents.Count % AgentsOnPage > 0) CountPage++;
            
            if (selPage.HasValue) {
                if (selPage >= 0 && selPage < CountPage)               
                    CurrentPage = (int)selPage;
            }
            else
            {
                switch (dir)
                {
                    case -1:
                        if (CurrentPage > 0) CurrentPage--; break;
                    case 1:
                        if (CurrentPage < CountPage - 1) CurrentPage++; break;
                }
            }
            AgentsViewed.Clear();
            AgentsViewed.AddRange(LeftAg.Skip(AgentsOnPage * CurrentPage).Take(AgentsOnPage));
            PageList.Items.Clear();
            for (int i = 1; i <= CountPage; i++)
                PageList.Items.Add(i);

            if (LeftAg.Count > AgentsOnPage)
            {
                PageForwBtn.Visibility = Visibility.Visible;
                PageBackBtn.Visibility = Visibility.Visible;
                PageList.Visibility = Visibility.Visible;
            }
            else
            {
                PageForwBtn.Visibility = Visibility.Hidden;
                PageBackBtn.Visibility = Visibility.Hidden;
                PageList.Visibility = Visibility.Hidden;
            }

            PageList.SelectedIndex = CurrentPage;
            AgentListView.ItemsSource = AgentsViewed;

            AgentListView.Items.Refresh();
            
        }
        
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateAgentPage();
    //        MessageBox.Show("lol");
        }

        private void SortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgentPage();
    //        MessageBox.Show("lol");
        }

        private void FilterBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgentPage();
       //     MessageBox.Show("lol");
        }

        private void PageBackBtn_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(-1, null);
        }

        private void PageForwBtn_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(1, null);
        }

        private void PageList_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangePage(0, Convert.ToInt32(PageList.SelectedItem.ToString()) - 1);
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null));
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage((sender as Button).DataContext as Agent));
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateAgentPage();
        }

        private void PriorChangeBtn_Click(object sender, RoutedEventArgs e)
        {
            int MaxPrior = 0;
            foreach (Agent ag in AgentListView.SelectedItems)
            {
                if (ag.Priority > MaxPrior)
                    MaxPrior = ag.Priority;
            }
            PriorityDialogWindow PDW = new PriorityDialogWindow(MaxPrior);

            if (PDW.ShowDialog() == true)
            {
                foreach (Agent ag in AgentListView.SelectedItems)
                {
          /*          AgentPriorityHistory history = new AgentPriorityHistory();
                    history.AgentID = ag.ID;
                    history.ChangeDate = DateTime.Today;
                    history.PriorityValue = ag.Priority;

                    ag.AgentPriorityHistory.Add(history);*/
                    ag.Priority = PDW.prior;      
                    
                }
                MuzafarovGlazkiEntities.GetContext().SaveChanges();
                AgentListView.SelectedItems.Clear();
                PriorChangeBtn.Visibility = Visibility.Hidden;
            }
            UpdateAgentPage();
        }
        private void AgentListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AgentListView.SelectedItem == null)
                PriorChangeBtn.Visibility = Visibility.Hidden;
            else            
                PriorChangeBtn.Visibility = Visibility.Visible;           
        }
    }
}
