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
        // 1. Validare simplă (să nu fie câmpurile goale)
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

        // 3. Validare Format Telefon (07xx xxx xxx)
        // Acest Regex verifică să înceapă cu 07 și să aibă în total 10 cifre
        string telPattern = @"^07\d{8}$";
        string telefonCurat = EntryPhone.Text.Replace(" ", ""); // Eliminăm spațiile dacă utilizatorul a pus

        if (!Regex.IsMatch(telefonCurat, telPattern))
        {
            await DisplayAlert("Eroare Telefon", "Numărul de telefon trebuie să fie de forma 07xxxxxxxx (10 cifre).", "OK");
            return;
        }

        // Adaugă în OnRegisterClicked:
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        if (!Regex.IsMatch(EntryEmail.Text ?? "", emailPattern))
        {
            await DisplayAlert("Eroare", "Te rugăm să introduci o adresă de email validă.", "OK");
            return;
        }

        var noulClient = new Client
        {
            LastName = EntryNume.Text.Trim(),      // Nume de familie
            FirstName = EntryPrenume.Text.Trim(),  // Prenume
            Email = EntryEmail.Text,
            Phone = telefonCurat,
            Username = EntryUsername.Text,
            Password = EntryPassword.Text,
            IsAdmin = false
        };

        // 3. Salvare în baza de date
        await App.Database.SaveClientAsync(noulClient);

        var clienti = await App.Database.GetClientsAsync();
        var clientLogat = clienti.FirstOrDefault(c => c.Username == noulClient.Username);

        if (clientLogat != null)
        {
            App.UserLogat = clientLogat;

            await DisplayAlert("Succes", $"Cont creat cu succes! Bine ai venit, {clientLogat.FirstName}!", "OK");

            // 4. Îl trimitem direct în interfața principală a aplicației
            // Înlocuim pagina de login/signup cu meniul principal
            Application.Current.MainPage = new AppShell();
        }
    }
    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}