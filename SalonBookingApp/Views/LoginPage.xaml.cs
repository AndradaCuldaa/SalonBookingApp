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
        string usernameIntrodus = EntryLoginUsername.Text?.Trim() ?? "";
        string parolaIntrodusa = EntryLoginPassword.Text ?? "";

        if (string.IsNullOrWhiteSpace(usernameIntrodus) || string.IsNullOrWhiteSpace(parolaIntrodusa))
        {
            await DisplayAlert("Eroare", "Te rog introdu numele de utilizator și parola.", "OK");
            return;
        }

        // 1. Verificare ADMIN
        if (usernameIntrodus.ToLower() == "admin" && parolaIntrodusa == "admin123")
        {
            App.UserLogat = new Client
            {
                Username = "admin",
                IsAdmin = true,
                FirstName = "Admin",
                LastName = "Sistem"
            };
            App.EsteStilistLogat = false; // Ne asigurăm că nu e marcat ca stilist
            await DisplayAlert("Succes", "Te-ai autentificat ca Admin!", "OK");
            Application.Current.MainPage = new AppShell();
            return;
        }

        // 2. Verificare STILIST
        var listaStilisti = await App.Database.GetStylistsAsync();
        var stilistGasit = listaStilisti.FirstOrDefault(s =>
            s.Username == usernameIntrodus &&
            s.Password == parolaIntrodusa);
        
        var listaClienti = await App.Database.GetClientsAsync();

     
        var userGasit = listaClienti.FirstOrDefault(c =>
            c.Username == usernameIntrodus &&
            c.Password == parolaIntrodusa);

        if (userGasit != null)
        {
            
            App.UserLogat = userGasit;

            await DisplayAlert("Succes", $"Bine ai revenit, {userGasit.FirstName}!", "OK");

            
            Application.Current.MainPage = new AppShell();
        }
        else
        {
            BtnGoToRegister.IsVisible = true;
            await DisplayAlert("Eroare", "Date incorecte. Nu ai cont?", "OK");
        }
    }
    private async void OnRegisterClicked(object sender, EventArgs e)
    {
       
        await Navigation.PushAsync(new RegisterPage());
    }
}