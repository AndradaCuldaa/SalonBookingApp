using SalonBookingApp.Models;

namespace SalonBookingApp.Views;

public partial class StylistEntryPage : ContentPage
{
	public StylistEntryPage()
	{
		InitializeComponent();
	}

    async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        var stylist = (Stylist)BindingContext;
        await App.Database.SaveStylistAsync(stylist);
        await Navigation.PopAsync();
    }

    async void OnDeleteButtonClicked(object sender, EventArgs e)
    {
        var stylist = (Stylist)BindingContext;
        if (stylist.ID != 0)
        {
            await App.Database.DeleteStylistAsync(stylist);
        }
        await Navigation.PopAsync();
    }
}