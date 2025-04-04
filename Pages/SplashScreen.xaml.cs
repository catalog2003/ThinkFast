namespace ThinkFast.Pages;

public partial class SplashScreen : ContentPage
{
	public SplashScreen()
	{
		InitializeComponent();

		NavigateToFirstPage();
       
    }
	private async void NavigateToFirstPage()
	{
		await Task.Delay(3000);
     

        // Navigate to the FirstPage
        await Navigation.PushAsync(new FirstPage());

    }

}