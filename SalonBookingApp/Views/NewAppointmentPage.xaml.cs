using SalonBookingApp.Models;
using Plugin.LocalNotification;

namespace SalonBookingApp.Views;

public partial class NewAppointmentPage : ContentPage
{
	public NewAppointmentPage()
	{
		InitializeComponent();
	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        bool permisiuneAcordata = await LocalNotificationCenter.Current.AreNotificationsEnabled();
        if (!permisiuneAcordata)
        {
            await LocalNotificationCenter.Current.RequestNotificationPermission();
        }
        
        PickerStilist.ItemsSource = await App.Database.GetStylistsAsync();
        PickerServiciu.ItemsSource = await App.Database.GetServicesAsync();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (PickerStilist.SelectedIndex == -1 || PickerServiciu.SelectedIndex == -1)
        {
            await DisplayAlert("Eroare", "Selectează stilistul și serviciul!", "OK");
            return;
        }

        var appt = new Appointment();
        
        if (App.UserLogat != null) appt.ClientID = App.UserLogat.ID;

       
        var d = DatePickerData.Date;
        var t = TimePickerOra.Time;
        if (d.HasValue && t.HasValue)
        {
            
            appt.AppointmentDate = new DateTime(
                d.Value.Year,
                d.Value.Month,
                d.Value.Day,
                t.Value.Hours,
                t.Value.Minutes,
                0);
        }
        else
        {
            
            appt.AppointmentDate = DateTime.Now;
        }

        
        var stilist = (Stylist)PickerStilist.SelectedItem;
        var serviciu = (Service)PickerServiciu.SelectedItem;
        appt.StylistID = stilist.ID;
        appt.ServiceID = serviciu.ID;

        await App.Database.SaveAppointmentAsync(appt);

        try
        {
         
            var notifyTime = appt.AppointmentDate.AddDays(-1);

            
            if (notifyTime <= DateTime.Now)
            {
                notifyTime = DateTime.Now.AddSeconds(10);
            }

            var request = new NotificationRequest
            {
                NotificationId = 1000 + appt.ID,
                Title = "Reminder Programare Salon",
                Description = $"Ai o programare mâine pentru {serviciu.Name} la ora {appt.AppointmentDate:HH:mm}!",
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = notifyTime
                }
            };

            await LocalNotificationCenter.Current.Show(request);
        }
        catch (Exception ex)
        {
            
            System.Diagnostics.Debug.WriteLine($"Eroare notificare: {ex.Message}");
        }

        await DisplayAlert("Succes", "Programare creată!", "OK");

        
        await Navigation.PopAsync();
    }
}