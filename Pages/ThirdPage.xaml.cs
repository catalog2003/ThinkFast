namespace ThinkFast.Pages;

public partial class ThirdPage : ContentPage
{
	public ThirdPage()
	{
		InitializeComponent();
	}
    private async void OnGetStartedClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new FourthPage());


    }
}