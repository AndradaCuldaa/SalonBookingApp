using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;
using SalonBookingApp.Models;
using SalonBookingApp.Resources.Strings;
using System.Diagnostics;

namespace SalonBookingApp.Views;

public partial class EditProfilePage : ContentPage
{
    private string _currentImagePath = "";

    public EditProfilePage()
    {
        InitializeComponent();

        if (App.UserLogat != null)
            BindingContext = App.UserLogat;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (App.UserLogat != null)
        {
            string savedPicPath = Preferences.Get($"ProfilePic_{App.UserLogat.ID}", "");
            if (!string.IsNullOrEmpty(savedPicPath) && File.Exists(savedPicPath))
            {
                _currentImagePath = savedPicPath;
                ImageProfilePic.Source = ImageSource.FromFile(_currentImagePath);
            }
            else
            {
                ImageProfilePic.Source = "profile_placeholder.png";
            }
        }
    }

    private async void OnChangePhotoClicked(object sender, EventArgs e)
    {
        ContextMenuOverlay.IsVisible = true;
        await ContextMenuFrame.ScaleTo(1, 150, Easing.SpringOut);
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await ContextMenuFrame.ScaleTo(0, 100, Easing.SinIn);
        ContextMenuOverlay.IsVisible = false;
    }

    private async void OnOptionSelected(object sender, EventArgs e)
    {
        var grid = sender as Grid;
        var tap = grid?.GestureRecognizers.FirstOrDefault() as TapGestureRecognizer;
        string action = tap?.CommandParameter as string;

        await ContextMenuFrame.ScaleTo(0, 100, Easing.SinIn);
        ContextMenuOverlay.IsVisible = false;

        try
        {
            if (action == "Remove")
            {
                _currentImagePath = "";
                ImageProfilePic.Source = "profile_placeholder.png";
                return;
            }

            FileResult result = null;
            if (action == "Camera")
            {
                if (await Permissions.RequestAsync<Permissions.Camera>() == PermissionStatus.Granted)
                    result = await MediaPicker.Default.CapturePhotoAsync();
            }
            else if (action == "Library")
            {
                result = await MediaPicker.Default.PickPhotoAsync();
            }
            else if (action == "Files")
            {
                result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = AppResources.ChooseFile,
                    FileTypes = FilePickerFileType.Images
                });
            }

            if (result != null)
            {
                _currentImagePath = result.FullPath;
                ImageProfilePic.Source = ImageSource.FromFile(_currentImagePath);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Eroare selecție foto: {ex.Message}");
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (App.UserLogat == null) return;

        try
        {
            await App.Database.SaveClientAsync(App.UserLogat);

            Preferences.Set($"ProfilePic_{App.UserLogat.ID}", _currentImagePath);

            WeakReferenceMessenger.Default.Send(new ProfilePicChangedMessage(_currentImagePath));

            await ArataNotificareRoz(AppResources.SuccessUpdate);

            await Task.Delay(1500);
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Eroare salvare profil: {ex.Message}");
            await ArataNotificareRoz(AppResources.ErrorTitle, true);
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