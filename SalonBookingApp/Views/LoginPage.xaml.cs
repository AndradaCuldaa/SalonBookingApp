using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using SalonBookingApp.Models;
using SalonBookingApp.Resources.Strings;
using System.Diagnostics;

namespace SalonBookingApp.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string usernameIntrodus = EntryLoginUsername.Text?.Trim() ?? "";
        string parolaIntrodusa = EntryLoginPassword.Text ?? "";

        if (string.IsNullOrWhiteSpace(usernameIntrodus) || string.IsNullOrWhiteSpace(parolaIntrodusa))
        {
            await ArataNotificareRoz(AppResources.FieldsRequired, true);
            return;
        }

        try
        {
            if (usernameIntrodus.ToLower() == "admin" && parolaIntrodusa == "admin123")
            {
                App.EsteAdmin = true;
                App.UserLogat = new Client { Username = "admin", FirstName = "Admin", LastName = "Sistem" };
                Application.Current.MainPage = new AdminShell();
                return;
            }

            var stilistGasit = await App.Database.GetStylistByLoginAsync(usernameIntrodus, parolaIntrodusa);
            if (stilistGasit != null)
            {
                App.EsteStilistLogat = true;
                App.IdStilistLogat = stilistGasit.ID;
                App.StilistLogat = stilistGasit;
                App.UserLogat = null;
                App.EsteAdmin = false;
                Application.Current.MainPage = new StylistAppShell();
                return;
            }

            var clientGasit = await App.Database.GetClientByLoginAsync(usernameIntrodus, parolaIntrodusa);
            if (clientGasit != null)
            {
                App.UserLogat = clientGasit;
                App.EsteStilistLogat = false;
                App.StilistLogat = null;
                App.EsteAdmin = false;
                Application.Current.MainPage = new AppShell();
                return;
            }

            await ArataNotificareRoz(AppResources.InvalidLogin, true);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Login error: {ex.Message}");
            await ArataNotificareRoz(AppResources.ErrorTitle, true);
        }
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage());
    }

    private async Task ArataNotificareRoz(string mesaj, bool esteEroare = false)
    {
        var bgColor = esteEroare ? Color.FromArgb("#FF3B30") : Color.FromArgb("#EAB8C1");
        var border = new Border
        {
            BackgroundColor = bgColor,
            StrokeThickness = 0,
            Padding = new Thickness(20, 15, 20, 15),
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

        var labelMesaj = new Label { Text = mesaj, TextColor = Colors.White, FontSize = 16, FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center };
        gridContent.Children.Add(labelMesaj);
        Grid.SetColumn(labelMesaj, 0);

        var labelClose = new Label { Text = "✕", TextColor = Colors.White, FontSize = 20, FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center };
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