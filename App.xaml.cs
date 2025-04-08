using ThinkFast.Pages;
using Microsoft.Maui.Controls;

namespace ThinkFast
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            FlashcardStorage.Initialize();
            TestStorage.Initialize();
            // Set SplashScreen as the main page
            MainPage = new NavigationPage(new Pages.SplashScreen());
        }

        // Method to navigate to the main app (AppShell)
        public static void NavigateToMainApp()
        {
            Current.MainPage = new AppShell();
        }

        // Method to navigate to a specific page (e.g., FirstPage)
        public static async Task NavigateToPage(Page page)
        {
            if (Current.MainPage is NavigationPage navigationPage)
            {
                await navigationPage.PushAsync(page);
            }
        }
    }
}