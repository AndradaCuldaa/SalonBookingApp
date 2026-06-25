using SalonBookingApp.Models;

namespace SalonBookingApp.Views;

public partial class AppointmentListPage : ContentPage
{
	public AppointmentListPage()
	{
		InitializeComponent();
	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();
       
        listView.ItemsSource = await App.Database.GetAppointmentsAsync();
    }

    async void OnItemAdded(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AppointmentPage
        {
            BindingContext = new Appointment()
        });
    }

    async void OnListItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem != null)
        {
            await Navigation.PushAsync(new AppointmentPage
            {
                BindingContext = e.SelectedItem as Appointment
            });
        }
    }
}