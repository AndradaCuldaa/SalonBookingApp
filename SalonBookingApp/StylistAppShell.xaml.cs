using SalonBookingApp.Views;

namespace SalonBookingApp;

public partial class StylistAppShell : Shell
{
    public StylistAppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(StylistWorkSchedulePage), typeof(StylistWorkSchedulePage));
    }
}