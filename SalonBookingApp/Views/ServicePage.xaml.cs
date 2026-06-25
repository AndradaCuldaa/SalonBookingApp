using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using SalonBookingApp.Models;
using SalonBookingApp.Resources.Strings;
using System.Linq;

namespace SalonBookingApp.Views;

[QueryProperty(nameof(CategoriePrimita), "category")]
[QueryProperty(nameof(DisplayTitle), "displayTitle")]
public partial class ServicePage : ContentPage
{
    private string _categorieCurenta = "General";
    private string _displayTitle = "";

    public string DisplayTitle
    {
        get => _displayTitle;
        set => _displayTitle = Uri.UnescapeDataString(value ?? "");
    }

    public string CategoriePrimita
    {
        set => _categorieCurenta = Uri.UnescapeDataString(value ?? "General");
    }

    public ServicePage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        bool isRo = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ro";
        Title = isRo ? _categorieCurenta : (string.IsNullOrEmpty(_displayTitle) ? _categorieCurenta : _displayTitle);

        try
        {
            var toateServiciile = await App.Database.GetServicesAsync();
            if (toateServiciile != null)
            {
                if (App.EsteAdmin)
                {
                    listView.ItemsSource = toateServiciile.OrderBy(s => s.Category).ToList();
                }
                else
                {
                    listView.ItemsSource = toateServiciile
                        .Where(s => s.Category != null &&
                                    s.Category.Equals(_categorieCurenta, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
            }
        }
        catch (Exception)
        {
            await ArataNotificareRoz(AppResources.ErrorTitle, true);
        }

        bool poateEdita = App.EsteAdmin || (App.UserLogat != null && App.UserLogat.IsAdmin && !App.EsteStilistLogat);

        var existentPlus = ToolbarItems.FirstOrDefault(x => x.Text == "+");
        if (existentPlus != null) ToolbarItems.Remove(existentPlus);

        if (poateEdita)
        {
            var noulButon = new ToolbarItem { Text = "+" };
            noulButon.Clicked += OnItemAdded;
            ToolbarItems.Add(noulButon);
            listView.SelectionMode = SelectionMode.Single;
        }
        else
        {
            listView.SelectionMode = SelectionMode.None;
        }
    }

    async void OnItemAdded(object sender, EventArgs e)
    {
        if (!App.EsteAdmin && (App.UserLogat == null || !App.UserLogat.IsAdmin)) return;

        await Navigation.PushAsync(new ServiceEntryPage
        {
            BindingContext = new Service
            {
                Category = _categorieCurenta,
                CategoryEn = _displayTitle
            }
        });
    }

    async void OnBookArrowTapped(object sender, EventArgs e)
    {
        var label = sender as Label;
        if (label?.BindingContext is Service selectedService)
        {
            if (App.EsteAdmin || (App.UserLogat != null && App.UserLogat.IsAdmin))
            {
                await Navigation.PushAsync(new ServiceEntryPage { BindingContext = selectedService });
            }
            else
            {
                await Navigation.PushAsync(new NewAppointmentPage(selectedService.ID));
            }
        }
    }

    async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (App.EsteAdmin || (App.UserLogat != null && App.UserLogat.IsAdmin && !App.EsteStilistLogat))
        {
            if (e.CurrentSelection.FirstOrDefault() is Service selectedService)
            {
                await Navigation.PushAsync(new ServiceEntryPage { BindingContext = selectedService });
                if (sender is CollectionView cv) cv.SelectedItem = null;
            }
        }
    }

    async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
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