using SalonBookingApp.Models;
using System;

namespace SalonBookingApp.Views;

public partial class AppointmentPage : ContentPage
{
    public Client UserLogat => App.UserLogat;
    public AppointmentPage()
	{
		InitializeComponent();
        BindingContext = this;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await IncarcaProgramari();
    }

    private async Task IncarcaProgramari()
    {
        // 1. Luăm toate programările din bază
        // NOTĂ: Trebuie să implementăm o funcție care aduce și datele despre Service/Stylist (relații)
        var toateProgramarile = await App.Database.GetAppointmentsWithChildrenAsync();

        if (App.UserLogat.IsAdmin)
        {
            // Adminul vede tot
            ListaProgramari.ItemsSource = toateProgramarile;
        }
        else
        {
            // Clientul vede doar ale lui
            var programarileMele = toateProgramarile.Where(p => p.ClientID == App.UserLogat.ID).ToList();
            ListaProgramari.ItemsSource = programarileMele;
        }
    }

    // Butonul care te duce la formularul de creare
    private async void OnNewAppointmentClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new NewAppointmentPage());
    }

    // Butonul mic de Anulare din interiorul dreptunghiului
    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var programare = button.CommandParameter as Appointment;

        bool confirm = await DisplayAlert("Confirmare", "Sigur vrei să anulezi?", "Da", "Nu");
        if (confirm)
        {
            await App.Database.DeleteAppointmentAsync(programare);
            await IncarcaProgramari(); // Reîmprospătăm lista
        }
    }
}