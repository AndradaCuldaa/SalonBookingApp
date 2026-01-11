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
                    // Daca e client simplu, ASCUNDEM tab-ul de clienti
                    // Nota: 'TabClienti' trebuie sa fie x:Name-ul pus in XAML
                    TabClienti.IsVisible = false;
                }
                else
                {
                    // Daca e admin, vede tot
                    TabClienti.IsVisible = true;
                }
            }
        }
    }
}
