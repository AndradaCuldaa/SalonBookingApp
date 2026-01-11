
using Microsoft.Extensions.DependencyInjection;
using SalonBookingApp.Data;
using SalonBookingApp.Models;
using SalonBookingApp.Views;
using System.IO;
namespace SalonBookingApp
{
    public partial class App : Application
    {
        public static Client UserLogat { get; set; }

        static SalonDatabase database;
        public static SalonDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new SalonDatabase(Constants.DatabasePath);
                }
                return database;
            }
        }
        public App()
        {
            InitializeComponent();
           
            
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new NavigationPage(new HomePage()));
        }
    }
}