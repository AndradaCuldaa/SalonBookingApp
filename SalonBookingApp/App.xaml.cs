using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using SalonBookingApp.Data;
using SalonBookingApp.Models;
using SalonBookingApp.Resources.Strings;
using SalonBookingApp.Views;
using System.Globalization;

namespace SalonBookingApp
{
    public partial class App : Application
    {
        public static Client UserLogat { get; set; }
        public static Stylist StilistLogat { get; set; }
        public static bool EsteStilistLogat { get; set; }
        public static int IdStilistLogat { get; set; }
        public static bool EsteAdmin { get; set; }

        public static SalonDatabase Database { get; private set; }

        public App(SalonDatabase database)
        {
            InitializeComponent();

            Database = database;

            string savedLanguage = Preferences.Get("SelectedLanguage", CultureInfo.CurrentCulture.Name);
            var culture = new CultureInfo(savedLanguage);

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            AppResources.Culture = culture;

            MainPage = new NavigationPage(new WelcomePage());
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(MainPage);
        }

        public static async Task ShowToastAsync(string message, bool isError = false)
        {
            var options = new SnackbarOptions
            {
                BackgroundColor = isError ? Color.FromArgb("#FF3B30") : Color.FromArgb("#EAB8C1"),
                TextColor = Colors.White,
                ActionButtonTextColor = Colors.White,
                CornerRadius = new CornerRadius(15),
                Font = Microsoft.Maui.Font.Default
            };

            var snackbar = Snackbar.Make(message, null, "", TimeSpan.FromSeconds(3), options);
            await snackbar.Show();
        }
    }
}