using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using SalonBookingApp.Models;
using SalonBookingApp.Resources.Strings;
using System.Text.RegularExpressions;

namespace SalonBookingApp.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterPage()
    {
        InitializeComponent();
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EntryLastName.Text) ||
            string.IsNullOrWhiteSpace(EntryFirstName.Text) ||
            string.IsNullOrWhiteSpace(EntryPhone.Text) ||
            string.IsNullOrWhiteSpace(EntryUsername.Text) ||
            string.IsNullOrWhiteSpace(EntryEmail.Text) ||
            string.IsNullOrWhiteSpace(EntryPassword.Text))
        {
            await ArataNotificareRoz(AppResources.FieldsRequired, true);
            return;
        }

        if (EntryPassword.Text.Length < 8)
        {
            await ArataNotificareRoz(AppResources.PassLengthError, true);
            return;
        }

        string telPattern = @"^07\d{8}$";
        string telefonCurat = EntryPhone.Text.Replace(" ", "");
        if (!Regex.IsMatch(telefonCurat, telPattern))
        {
            await ArataNotificareRoz(AppResources.PhoneFormatError, true);
            return;
        }

        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        if (!Regex.IsMatch(EntryEmail.Text ?? "", emailPattern))
        {
            await ArataNotificareRoz(AppResources.EmailFormatError, true);
            return;
        }

        var noulClient = new Client
        {
            LastName = EntryLastName.Text.Trim(),
            FirstName = EntryFirstName.Text.Trim(),
            Email = EntryEmail.Text.Trim(),
            Phone = telefonCurat,
            Username = EntryUsername.Text.Trim(),
            Password = EntryPassword.Text,
            IsAdmin = false
        };

        try
        {
            var clientSalvat = await App.Database.SaveClientAsync(noulClient);

            if (clientSalvat != null)
            {
                App.UserLogat = clientSalvat;
                App.EsteStilistLogat = false;

                await ArataNotificareRoz(AppResources.AccountSaved);

                await Task.Delay(1500);
                Application.Current.MainPage = new AppShell();
            }
        }
        catch (Exception ex)
        {
            await ArataNotificareRoz(ex.Message, true);
        }
    }

    private async void OnBackToLoginClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
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