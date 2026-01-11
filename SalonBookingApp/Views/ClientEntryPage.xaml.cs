using SalonBookingApp.Models;

namespace SalonBookingApp.Views;


public partial class ClientEntryPage : ContentPage
{
	public ClientEntryPage()
	{
		InitializeComponent();
	}

    async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        // Preluăm clientul din BindingContext (datele de pe ecran)
        var client = (Client)BindingContext;

        // Setăm data creării (opțional, dacă ai câmpul în model)
        // client.Date = DateTime.UtcNow; 

        // Salvăm în baza de date folosind "Singleton-ul" din App
        await App.Database.SaveClientAsync(client);

        // Ne întoarcem la pagina anterioară (lista)
        await Navigation.PopAsync();
    }

    // Se apelează când apeși ȘTERGE
    async void OnDeleteButtonClicked(object sender, EventArgs e)
    {
        var client = (Client)BindingContext;

        // Ștergem doar dacă clientul există deja în bază (are ID > 0)
        if (client.ID != 0)
        {
            await App.Database.DeleteClientAsync(client);
        }

        // Ne întoarcem la pagina anterioară
        await Navigation.PopAsync();
    }
}
