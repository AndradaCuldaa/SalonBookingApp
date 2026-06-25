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
        
        var client = (Client)BindingContext;


        await App.Database.SaveClientAsync(client);

        
        await Navigation.PopAsync();
    }

    
    async void OnDeleteButtonClicked(object sender, EventArgs e)
    {
        var client = (Client)BindingContext;

        
        if (client.ID != 0)
        {
            await App.Database.DeleteClientAsync(client);
        }

        
        await Navigation.PopAsync();
    }
}
