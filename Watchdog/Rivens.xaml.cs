using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Watchdog.Data;

namespace Watchdog
{
    public partial class Rivens : Page
    {
        public Rivens()
        {
            InitializeComponent();
            LoadContents();
        }

        private void LoadContents()
        {
            RivensDataGrid.ItemsSource = AppManager.RivenList;
        }

        private void RivenSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var foundItems = new List<Riven>();
            foreach (var riven in AppManager.RivenList)
            {
                if (!string.IsNullOrEmpty(riven.Compatibility) && riven.Compatibility.ToLower().Contains(RivenSearch.Text.ToLower()))
                {
                    foundItems.Add(riven);
                }
            }
            if (!string.IsNullOrEmpty(RivenSearch.Text))
            {
                RivensDataGrid.ItemsSource = foundItems;
            }
            else
            {
                RivensDataGrid.ItemsSource = AppManager.RivenList;
            }
        }
    }
}
