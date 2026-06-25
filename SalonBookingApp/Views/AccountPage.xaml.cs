using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.Maui.Controls.Shapes;
using SalonBookingApp.Models;
using SalonBookingApp.Resources.Strings;
using System.Diagnostics;
using System.Globalization;

namespace SalonBookingApp.Views;

public class ProfilePicChangedMessage : ValueChangedMessage<string>
{
    public ProfilePicChangedMessage(string value) : base(value) { }
}

public partial class AccountPage : ContentPage
{
    public AccountPage()
    {
        InitializeComponent();

        WeakReferenceMessenger.Default.Register<ProfilePicChangedMessage>(this, (r, m) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (!string.IsNullOrEmpty(m.Value) && File.Exists(m.Value))
                    ImageProfilePic.Source = ImageSource.FromFile(m.Value);
                else
                    ImageProfilePic.Source = "profile_placeholder.png";
            });
        });
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (App.UserLogat != null)
        {
            LabelUserName.Text = $"{App.UserLogat.FirstName} {App.UserLogat.LastName}";

            string savedPicPath = Preferences.Get($"ProfilePic_{App.UserLogat.ID}", "");
            if (!string.IsNullOrEmpty(savedPicPath) && File.Exists(savedPicPath))
                ImageProfilePic.Source = ImageSource.FromFile(savedPicPath);
            else
                ImageProfilePic.Source = "profile_placeholder.png";
        }
    }

    private async void OnEditProfileTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EditProfilePage());
    }

    private async void OnAccountInfoTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AccountInfoPage());
    }

    private async void OnLanguageTapped(object sender, EventArgs e)
    {
        string action = await DisplayActionSheet(
            AppResources.LanguageTitle,
            AppResources.Cancel,
            null,
            "Română",
            "English");

        if (action == AppResources.Cancel || string.IsNullOrEmpty(action)) return;

        string cultureCode = action == "Română" ? "ro-RO" : "en-US";
        var newCulture = new CultureInfo(cultureCode);

        Preferences.Set("SelectedLanguage", cultureCode);

        CultureInfo.DefaultThreadCurrentCulture = newCulture;
        CultureInfo.DefaultThreadCurrentUICulture = newCulture;
        CultureInfo.CurrentCulture = newCulture;
        CultureInfo.CurrentUICulture = newCulture;
        AppResources.Culture = newCulture;
        Application.Current.MainPage = new AppShell();
    }

    private async void OnSignOutClicked(object sender, EventArgs e)
    {
        var popup = new ConfirmPopup(AppResources.ConfirmTitle, AppResources.SignOutConfirmation);
        await this.ShowPopupAsync(popup);

        if (popup.IsConfirmed)
        {
            App.UserLogat = null;
            App.EsteStilistLogat = false;
            Application.Current.MainPage = new NavigationPage(new WelcomePage());
        }
    }

    private async Task ArataNotificareRoz(string mesaj, bool esteEroare = false)
    {
        var bgColor = esteEroare ? Color.FromArgb("#FF3B30") : Color.FromArgb("#EAB8C1");

        var border = new Border
        {
            BackgroundColor = bgColor,
            StrokeThickness = 0,
            Padding = new Thickness(20, 15, 20, 15),
            Margin = new Thickness(0),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            Opacity = 0,
            StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(0, 0, 15, 15) },
            Shadow = new Shadow { Brush = Colors.Black, Offset = new Point(0, 4), Opacity = 0.2f, Radius = 5 },
            ZIndex = 999
        };

        var gridContent = new Grid();
        gridContent.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        gridContent.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var labelMesaj = new Label
        {
            Text = mesaj,
            TextColor = Colors.White,
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            VerticalOptions = LayoutOptions.Center
        };
        gridContent.Children.Add(labelMesaj);
        Grid.SetColumn(labelMesaj, 0);

        var labelClose = new Label
        {
            Text = "✕",
            TextColor = Colors.White,
            FontSize = 20,
            FontAttributes = FontAttributes.Bold,
            VerticalOptions = LayoutOptions.Center
        };
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += async (s, e) => { await border.FadeTo(0, 200); };
        labelClose.GestureRecognizers.Add(tapGesture);
        gridContent.Children.Add(labelClose);
        Grid.SetColumn(labelClose, 1);

        border.Content = gridContent;

        if (!(this.Content is Grid wrapperGrid && wrapperGrid.StyleId == "NotificareWrapper"))
        {
            var continutVechi = this.Content;
            wrapperGrid = new Grid { StyleId = "NotificareWrapper" };
            wrapperGrid.Children.Add(continutVechi);
            this.Content = wrapperGrid;
        }

        var gridPrincipal = (Grid)this.Content;
        gridPrincipal.Children.Add(border);

        border.TranslationY = -150;
        await border.FadeTo(1, 100);
        await border.TranslateTo(0, 0, 400, Easing.SpringOut);

        await Task.Delay(3000);

        if (border.Opacity > 0)
        {
            await border.TranslateTo(0, -150, 300, Easing.CubicIn);
            await border.FadeTo(0, 100);
        }

        gridPrincipal.Children.Remove(border);
    }
}