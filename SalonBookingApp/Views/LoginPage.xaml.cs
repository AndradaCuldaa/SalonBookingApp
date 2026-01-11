using SalonBookingApp.Models;

namespace SalonBookingApp.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}
    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string usernameIntrodus = EntryLoginUsername.Text.Trim();
        string parolaIntrodusa = EntryLoginPassword.Text;

        // 1. Verificăm să nu fie câmpurile goale
        if (string.IsNullOrWhiteSpace(EntryLoginUsername.Text) ||
            string.IsNullOrWhiteSpace(EntryLoginPassword.Text))
        {
            await DisplayAlert("Eroare", "Te rog introdu numele de utilizator și parola.", "OK");
            return;
        }
        if (usernameIntrodus.ToLower() == "admin" && parolaIntrodusa == "admin123")
        {
            App.UserLogat = new Client
            {
                Username = "admin",
                IsAdmin = true,
                FirstName = "Admin",
                LastName = "Sistem"
            };
            await DisplayAlert("Succes", "Te-ai autentificat ca Admin!", "OK");
            Application.Current.MainPage = new AppShell();
            return;
        }

        // --- LOGICA PENTRU CLIENȚI REALI (Căutare în DB) ---
        // Luăm toți clienții din baza de date
        var listaClienti = await App.Database.GetClientsAsync();

        // Căutăm dacă există un client cu acest username și această parolă
        var userGasit = listaClienti.FirstOrDefault(c =>
            c.Username == usernameIntrodus &&
            c.Password == parolaIntrodusa);

        if (userGasit != null)
        {
            // Dacă l-am găsit, îl salvăm în variabila globală a aplicației
            App.UserLogat = userGasit;

            await DisplayAlert("Succes", $"Bine ai revenit, {userGasit.FirstName}!", "OK");

            // Intrăm în aplicație
            Application.Current.MainPage = new AppShell();
        }
        else
        {
            BtnGoToRegister.IsVisible = true;

            // 2. Mesaj de eroare
            await DisplayAlert("Eroare", "Date incorecte. Nu ai cont?", "OK");
        }

    }
    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        // Te trimite la pagina de înregistrare
        await Navigation.PushAsync(new RegisterPage());
    }
}