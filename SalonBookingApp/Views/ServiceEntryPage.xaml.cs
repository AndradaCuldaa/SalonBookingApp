using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using SalonBookingApp.Models;
using SalonBookingApp.Resources.Strings;

namespace SalonBookingApp.Views;

public partial class ServiceEntryPage : ContentPage
{
    public ServiceEntryPage()
    {
        InitializeComponent();
        if (BindingContext == null)
        {
            BindingContext = new Service();
        }
    }

    async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        if (BindingContext is Service service)
        {
            if (string.IsNullOrWhiteSpace(service.Name) || string.IsNullOrWhiteSpace(service.NameEn))
            {
                await ArataNotificareRoz(AppResources.FieldsRequired, true);
                return;
            }

            try
            {
                await App.Database.SaveServiceAsync(service);
                await ArataNotificareRoz(AppResources.SuccessUpdate);

                await Task.Delay(1500);
                if (Navigation.NavigationStack.Count > 1)
                    await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving service: {ex.Message}");
                await ArataNotificareRoz(AppResources.ErrorTitle, true);
            }
        }
    }

    async void OnDeleteButtonClicked(object sender, EventArgs e)
    {
        if (BindingContext is Service service)
        {
            if (service.ID != 0)
            {
                var popup = new ConfirmPopup(AppResources.ConfirmTitle, AppResources.ConfirmCancel);
                await this.ShowPopupAsync(popup);

                if (popup.IsConfirmed)
                {
                    try
                    {
                        await App.Database.DeleteServiceAsync(service);
                        await ArataNotificareRoz(AppResources.BtnCancel);

                        await Task.Delay(1500);
                        if (Navigation.NavigationStack.Count > 1)
                            await Navigation.PopAsync();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error deleting service: {ex.Message}");
                        await ArataNotificareRoz(AppResources.ErrorTitle, true);
                    }
                }
            }
            else
            {
                if (Navigation.NavigationStack.Count > 1)
                    await Navigation.PopAsync();
            }
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