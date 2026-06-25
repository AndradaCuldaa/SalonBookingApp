using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls.Shapes;
using SalonBookingApp.Models;
using System.Diagnostics;

namespace SalonBookingApp.Views;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();

        WeakReferenceMessenger.Default.Register<ProfilePicChangedMessage>(this, (r, m) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (!string.IsNullOrEmpty(m.Value) && File.Exists(m.Value))
                    HomePageProfilePic.Source = ImageSource.FromFile(m.Value);
                else
                    HomePageProfilePic.Source = "profile_placeholder.png";
            });
        });
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (App.EsteStilistLogat)
        {
            HeaderPrevizualizareStilist.IsVisible = true;
            if (App.StilistLogat != null)
            {
                LabelNumeClient.Text = App.StilistLogat.FirstName;
                HomePageProfilePic.Source = $"{App.StilistLogat.FirstName.ToLower()}.png";
            }
        }
        else
        {
            HeaderPrevizualizareStilist.IsVisible = false;
            if (App.UserLogat != null)
            {
                LabelNumeClient.Text = App.UserLogat.FirstName;
                string savedPicPath = Preferences.Get($"ProfilePic_{App.UserLogat.ID}", "");
                if (!string.IsNullOrEmpty(savedPicPath) && File.Exists(savedPicPath))
                    HomePageProfilePic.Source = ImageSource.FromFile(savedPicPath);
                else
                    HomePageProfilePic.Source = "profile_placeholder.png";
            }
        }

        await IncarcaStilisti();
    }

    private async void OnÎnapoiLaStilistTapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async Task IncarcaStilisti()
    {
        try
        {
            var stilisti = await App.Database.GetStylistsAsync();

            if (stilisti != null)
            {
                foreach (var s in stilisti)
                {
                    var recenzii = await App.Database.GetReviewsForStylistAsync(s.ID);
                    if (recenzii != null && recenzii.Any())
                    {
                        double average = recenzii.Average(r => r.RatingDisplay.Count(c => c == '⭐'));
                        int starCount = (int)Math.Round(average);
                        s.StarsDisplay = new string('⭐', Math.Max(1, starCount));
                    }
                    else
                    {
                        s.StarsDisplay = "Nou";
                    }
                }

                StylistsList.ItemsSource = stilisti;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    private async void OnCategoryClicked(object sender, EventArgs e)
    {
        if (App.EsteStilistLogat) return;

        var frame = sender as Frame;
        var tap = frame?.GestureRecognizers.FirstOrDefault() as TapGestureRecognizer;
        string categorieSelectata = tap?.CommandParameter as string;

        if (!string.IsNullOrEmpty(categorieSelectata))
        {
            string dbCategory = categorieSelectata;
            if (categorieSelectata == "Hair") dbCategory = "Coafor";
            else if (categorieSelectata == "Nails") dbCategory = "Manichiură";
            else if (categorieSelectata == "Facial") dbCategory = "Cosmetică";
            else if (categorieSelectata == "Makeup") dbCategory = "Machiaj";

            await Shell.Current.GoToAsync($"{nameof(ServicePage)}?category={Uri.EscapeDataString(dbCategory)}&displayTitle={Uri.EscapeDataString(categorieSelectata)}");
        }
    }

    private async void OnMembershipTapped(object sender, EventArgs e)
    {
        if (App.EsteStilistLogat) return;

        await Navigation.PushAsync(new SubscriptionsPage());
    }

    private async void OnStylistTapped(object sender, EventArgs e)
    {
        if (App.EsteStilistLogat) return;

        var view = sender as View;
        var stilistSelectat = view?.BindingContext as Stylist;

        if (stilistSelectat != null)
        {
            await Navigation.PushAsync(new StylistReviewPage(stilistSelectat));
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