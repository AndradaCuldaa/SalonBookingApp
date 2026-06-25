using SalonBookingApp.Views;

namespace SalonBookingApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(NewAppointmentPage), typeof(NewAppointmentPage));

            if (App.UserLogat != null)
            {
                if (App.UserLogat.IsAdmin == false)
                {
                    
                    TabClienti.IsVisible = false;
                }
                else
                {
                    
                    TabClienti.IsVisible = true;
                }
            }
        }
    }
}
