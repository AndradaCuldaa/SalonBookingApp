using SalonBookingApp.Models;
using System.Text.RegularExpressions;

namespace SalonBookingApp.Views;

public partial class RegisterPage : ContentPage
{
	public RegisterPage()
	{
		InitializeComponent();
	}
    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        
        if (string.IsNullOrWhiteSpace(EntryNume.Text) ||
            string.IsNullOrWhiteSpace(EntryPrenume.Text) ||
            string.IsNullOrWhiteSpace(EntryPhone.Text) ||
            string.IsNullOrWhiteSpace(EntryUsername.Text) ||
            string.IsNullOrWhiteSpace(EntryEmail.Text) ||
            string.IsNullOrWhiteSpace(EntryPassword.Text))
        {
            await DisplayAlert("Eroare", "Te rog completează toate câmpurile", "OK");
            return;
        }
        if (EntryPassword.Text.Length < 8)
        {
            await DisplayAlert("Eroare Parolă", "Parola trebuie să conțină cel puțin 8 caractere.", "OK");
            return;
        }

        
        string telPattern = @"^07\d{8}$";
        string telefonCurat = EntryPhone.Text.Replace(" ", ""); 

        if (!Regex.IsMatch(telefonCurat, telPattern))
        {
            await DisplayAlert("Eroare Telefon", "Numărul de telefon trebuie să fie de forma 07xxxxxxxx (10 cifre).", "OK");
            return;
        }

        
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        if (!Regex.IsMatch(EntryEmail.Text ?? "", emailPattern))
        {
            await DisplayAlert("Eroare", "Te rugăm să introduci o adresă de email validă.", "OK");
            return;
        }

        var noulClient = new Client
        {
            LastName = EntryNume.Text.Trim(),     
            FirstName = EntryPrenume.Text.Trim(),  
            Email = EntryEmail.Text,
            Phone = telefonCurat,
            Username = EntryUsername.Text,
            Password = EntryPassword.Text,
            IsAdmin = false
        };

        
        await App.Database.SaveClientAsync(noulClient);

        var clienti = await App.Database.GetClientsAsync();
        var clientLogat = clienti.FirstOrDefault(c => c.Username == noulClient.Username);

        if (clientLogat != null)
        {
            App.UserLogat = clientLogat;

            await DisplayAlert("Succes", $"Cont creat cu succes! Bine ai venit, {clientLogat.FirstName}!", "OK");

            
            
            Application.Current.MainPage = new AppShell();
        }
    }
    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}