using SalonBookingApp.Models;

namespace SalonBookingApp.Views;

public partial class StylistAppointmentSettingsPage : ContentPage
{
    private List<string> _cancelOptions = new List<string>
    {
        "1h înainte de programare",
        "2h înainte de programare",
        "3h înainte de programare",
        "4h înainte de programare",
        "5h înainte de programare",
        "24h înainte de programare",
        "48h înainte de programare"
    };

    private List<string> _bookingOptions = new List<string>
    {
        "0h înainte de ora curentă",
        "30m înainte de ora curentă",
        "1h înainte de ora curentă",
        "2h înainte de ora curentă",
        "24h înainte de ora curentă",
        "48h înainte de ora curentă"
    };

    private List<string> _maxBookingsOptions = new List<string>
    {
        "Fără limită",
        "Maxim 1 programare pe client",
        "Maxim 2 programări pe client",
        "Maxim 3 programări pe client",
        "Maxim 4 programări pe client",
        "Maxim 5 programări pe client",
        "Maxim 6 programări pe client"
    };

    private List<string> _futureLimitOptions = new List<string>
    {
        "Maxim 30 de zile în avans",
        "Maxim 60 de zile în avans",
        "Maxim 120 de zile în avans",
        "Maxim 6 luni în avans"
    };

    public StylistAppointmentSettingsPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        PickerCancelLimit.ItemsSource = _cancelOptions;
        PickerBookingLimit.ItemsSource = _bookingOptions;
        PickerMaxBookings.ItemsSource = _maxBookingsOptions;
        PickerFutureLimit.ItemsSource = _futureLimitOptions;

        IncarcaSetarileCurente();
    }

    private void IncarcaSetarileCurente()
    {
        if (App.StilistLogat != null)
        {
            PickerCancelLimit.SelectedItem = _cancelOptions.Contains(App.StilistLogat.CancelLimit)
                ? App.StilistLogat.CancelLimit : "24h înainte de programare";

            PickerBookingLimit.SelectedItem = _bookingOptions.Contains(App.StilistLogat.BookingLimit)
                ? App.StilistLogat.BookingLimit : "1h înainte de ora curentă";

            PickerMaxBookings.SelectedItem = _maxBookingsOptions.Contains(App.StilistLogat.MaxBookings)
                ? App.StilistLogat.MaxBookings : "Fără limită";

            PickerFutureLimit.SelectedItem = _futureLimitOptions.Contains(App.StilistLogat.FutureLimit)
                ? App.StilistLogat.FutureLimit : "Maxim 6 luni în avans";
        }
        else
        {
            PickerCancelLimit.SelectedIndex = 5;
            PickerBookingLimit.SelectedIndex = 2;
            PickerMaxBookings.SelectedIndex = 0;
            PickerFutureLimit.SelectedIndex = 3;
        }
    }

    private async void OnSaveSettingsClicked(object sender, EventArgs e)
    {
        if (App.StilistLogat == null) return;

        string selectedCancel = (string)PickerCancelLimit.SelectedItem;
        string selectedBooking = (string)PickerBookingLimit.SelectedItem;
        string selectedMax = (string)PickerMaxBookings.SelectedItem;
        string selectedFuture = (string)PickerFutureLimit.SelectedItem;

        try
        {
            App.StilistLogat.CancelLimit = selectedCancel;
            App.StilistLogat.BookingLimit = selectedBooking;
            App.StilistLogat.MaxBookings = selectedMax;
            App.StilistLogat.FutureLimit = selectedFuture;

            await App.Database.UpdateStylistAsync(App.StilistLogat);

            await Navigation.PopAsync();
        }
        catch (Exception)
        {
        }
    }
}