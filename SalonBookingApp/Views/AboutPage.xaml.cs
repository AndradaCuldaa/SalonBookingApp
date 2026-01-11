namespace SalonBookingApp.Views;

public partial class AboutPage : ContentPage
{
	public AboutPage()
	{
		InitializeComponent();
	}

    private async void OnProgrameazaClicked(object sender, EventArgs e)
    {
        // Această comandă spune aplicației să comute pe Tab-ul cu ruta "AppointmentPage"
        // (Verifică în AppShell dacă route-ul tab-ului de programări este "AppointmentPage")
        await Shell.Current.GoToAsync(nameof(NewAppointmentPage));
    }

    // Funcția de Log Out (o vom lega imediat de un buton nou)
    private void OnLogoutClicked(object sender, EventArgs e)
    {
        App.UserLogat = null;
        Application.Current.MainPage = new NavigationPage(new HomePage());
    }
}