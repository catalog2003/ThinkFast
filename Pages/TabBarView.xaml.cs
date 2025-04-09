using ThinkFast.Pages;
using Microsoft.Maui.Controls;

namespace ThinkFast.Controls
{
    public partial class TabBarView : ContentView
    {
        public TabBarView()
        {
            InitializeComponent();
            BindingContext = new TabBarViewModel();
        }
    }
}