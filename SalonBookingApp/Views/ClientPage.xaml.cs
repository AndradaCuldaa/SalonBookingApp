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
            
            StackListaAdmin.IsVisible = true;
            LabelEroare.IsVisible = false;

            
            var totiClientii = await App.Database.GetClientsAsync();

            
            CliențiListView.ItemsSource = totiClientii.Where(c => c.IsAdmin == false).ToList();
        }
        else
        {
           
            StackListaAdmin.IsVisible = false;
            LabelEroare.IsVisible = true;
        }
    }
}