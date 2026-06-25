using SalonBookingApp.Models;
using SalonBookingApp.Resources.Strings;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace SalonBookingApp.Views;

public partial class StylistProfilePage : ContentPage, INotifyPropertyChanged
{
    private string _numeStilist;
    public string NumeStilist
    {
        get => _numeStilist;
        set { _numeStilist = value; OnPropertyChanged(); }
    }

    private string _pozaStilist;
    public string PozaStilist
    {
        get => _pozaStilist;
        set { _pozaStilist = value; OnPropertyChanged(); }
    }

    public StylistProfilePage()
    {
        InitializeComponent();

        if (App.StilistLogat != null)
        {
            NumeStilist = $"{App.StilistLogat.FirstName} {App.StilistLogat.LastName}";

            string pathImagine = $"{App.StilistLogat.FirstName.ToLower()}.png";

            if (File.Exists(FileSystem.AppDataDirectory + "/" + pathImagine) || ImageSource.FromFile(pathImagine) != null)
            {
                PozaStilist = pathImagine;
            }
            else
            {
                PozaStilist = "profile_placeholder.png";
            }
        }
        else
        {
            NumeStilist = "Administrator";
            PozaStilist = "profile_placeholder.png";
        }

        BindingContext = this;
    }

    public new event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private async void OnVeziProfilTapped(object sender, EventArgs e) => await Navigation.PushAsync(new HomePage());

    private async void OnDetaliiProfilTapped(object sender, EventArgs e) => await Navigation.PushAsync(new StylistEditProfilePage());

    private async void OnSetariProgramariTapped(object sender, EventArgs e) => await Navigation.PushAsync(new StylistAppointmentSettingsPage());

    private async void OnProgramLucruTapped(object sender, EventArgs e) => await Shell.Current.GoToAsync(nameof(StylistWorkSchedulePage));

    private async void OnRecenziiTapped(object sender, EventArgs e)
    {
        if (App.StilistLogat != null)
        {
            var stilist = new Stylist
            {
                ID = App.StilistLogat.ID,
                FirstName = App.StilistLogat.FirstName,
                LastName = App.StilistLogat.LastName,
                Specialization = App.StilistLogat.Specialization ?? "Stilist",
                ImagePath = PozaStilist
            };
            await Navigation.PushAsync(new StylistReviewPage(stilist));
        }
    }

    private async void OnLimbaTapped(object sender, EventArgs e)
    {
        string actiune = await DisplayActionSheet(AppResources.LanguageTitle, AppResources.BtnCancel, null, AppResources.LangRomanian, AppResources.LangEnglish);

        if (actiune == AppResources.LangRomanian) SeteazaLimbaAplicatiei("ro-RO");
        else if (actiune == AppResources.LangEnglish) SeteazaLimbaAplicatiei("en-US");
    }

    private void SeteazaLimbaAplicatiei(string codLimba)
    {
        var culturaNoua = new CultureInfo(codLimba);
        CultureInfo.CurrentCulture = culturaNoua;
        CultureInfo.CurrentUICulture = culturaNoua;
        AppResources.Culture = culturaNoua;
        Application.Current.MainPage = new StylistAppShell();
    }

    private void OnDeconectareTapped(object sender, EventArgs e)
    {
        App.StilistLogat = null;
        App.EsteStilistLogat = false;
        App.IdStilistLogat = 0;
        Application.Current.MainPage = new NavigationPage(new LoginPage());
    }
}