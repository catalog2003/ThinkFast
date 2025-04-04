namespace ThinkFast.Pages;

public partial class FirstPage : ContentPage
{
	public FirstPage()
	{
		InitializeComponent();
       

    }

    private async void OnGetStartedClicked(object sender, EventArgs e)
	{
        await Navigation.PushAsync(new SecondPage());


    }
}