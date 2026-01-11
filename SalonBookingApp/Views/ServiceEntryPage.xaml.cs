using SalonBookingApp.Models;

namespace SalonBookingApp.Views;

public partial class ServiceEntryPage : ContentPage
{
	public ServiceEntryPage()
	{
		InitializeComponent();
	}
    async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        var service = (Service)BindingContext;
        await App.Database.SaveServiceAsync(service);
        await Navigation.PopAsync();
    }

    async void OnDeleteButtonClicked(object sender, EventArgs e)
    {
        var service = (Service)BindingContext;
        if (service.ID != 0)
        {
            await App.Database.DeleteServiceAsync(service);
        }
        await Navigation.PopAsync();
    }
}