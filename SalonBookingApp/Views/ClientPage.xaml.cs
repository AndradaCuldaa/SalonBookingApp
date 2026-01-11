using SalonBookingApp.Models;

namespace SalonBookingApp.Views;

public partial class ClientPage : ContentPage
{
	public ClientPage()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (App.UserLogat != null && App.UserLogat.IsAdmin)
        {
            // Dacă e admin, arătăm lista
            StackListaAdmin.IsVisible = true;
            LabelEroare.IsVisible = false;

            // 2. Încărcăm clienții reali din baza de date
            var totiClientii = await App.Database.GetClientsAsync();

            // 3. Afișăm în listă doar clienții (fără admin)
            CliențiListView.ItemsSource = totiClientii.Where(c => c.IsAdmin == false).ToList();
        }
        else
        {
            // Dacă nu e admin, arătăm mesajul de eroare
            StackListaAdmin.IsVisible = false;
            LabelEroare.IsVisible = true;
        }
    }
}