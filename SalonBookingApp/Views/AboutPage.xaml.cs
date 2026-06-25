namespace SalonBookingApp.Views;

public partial class AboutPage : ContentPage
{
	public AboutPage()
	{
		InitializeComponent();
	}

    private async void OnProgrameazaClicked(object sender, EventArgs e)
    {
        
        await Shell.Current.GoToAsync(nameof(NewAppointmentPage));
    }

    
    private void OnLogoutClicked(object sender, EventArgs e)
    {
        App.UserLogat = null;
        Application.Current.MainPage = new NavigationPage(new HomePage());
    }
}