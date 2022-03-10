using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using Watchdog.Data;

namespace Watchdog
{
    public partial class Items : Page
    {
        public static Items Instance;
        public Items()
        {
            InitializeComponent();
            Instance = this;
            LoadContents();
        }

        private void LoadContents()
        {
            ItemsDataGrid.ItemsSource = AppManager.ItemList;
        }

        private void ItemSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var foundItems = new List<Item>();
            foreach (var item in AppManager.ItemList)
            {
                if (!string.IsNullOrEmpty(item.Name) && item.Name.ToLower().Contains(ItemSearch.Text.ToLower()))
                {
                    foundItems.Add(item);
                }
            }
            if (!string.IsNullOrEmpty(ItemSearch.Text))
            {
                ItemsDataGrid.ItemsSource = foundItems;
            }
            else
            {
                ItemsDataGrid.ItemsSource = AppManager.ItemList;
            }
        }
    }
}
