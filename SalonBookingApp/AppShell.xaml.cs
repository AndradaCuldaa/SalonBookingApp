using SalonBookingApp.Views;

namespace SalonBookingApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(NewAppointmentPage), typeof(NewAppointmentPage));
        Routing.RegisterRoute(nameof(ServicePage), typeof(ServicePage));
        Routing.RegisterRoute(nameof(ServiceEntryPage), typeof(ServiceEntryPage));
        Routing.RegisterRoute(nameof(StylistEntryPage), typeof(StylistEntryPage));
        Routing.RegisterRoute(nameof(ClientEntryPage), typeof(ClientEntryPage));

        ConfigureTabs();
    }

    private void ConfigureTabs()
    {
        if (App.UserLogat != null)
        {
            if (App.UserLogat.IsAdmin)
            {
                TabClienti.IsVisible = true;
                TabStilisti.IsVisible = true;
            }
            else
            {
                TabClienti.IsVisible = false;
                TabStilisti.IsVisible = false;
            }
        }
    }
}