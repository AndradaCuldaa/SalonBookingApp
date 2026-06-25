namespace SalonBookingApp.Views;

public partial class WelcomePage : ContentPage
{
    public WelcomePage()
    {
        InitializeComponent();
    }

    private async void OnGetStartedClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LoginPage());
    }
}