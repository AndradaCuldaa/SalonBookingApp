using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using SalonBookingApp.Models;
using SalonBookingApp.Resources.Strings;

namespace SalonBookingApp.Views;

public partial class ClientPage : ContentPage
{
    public ClientPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        bool isAdmin = (App.UserLogat != null && App.UserLogat.IsAdmin) || App.EsteAdmin;

        if (isAdmin)
        {
            StackListaAdmin.IsVisible = true;
            LabelEroare.IsVisible = false;

            try
            {
                var totiClientii = await App.Database.GetClientsAsync();

                if (totiClientii != null)
                {
                    CliențiListView.ItemsSource = totiClientii
                        .Where(c => !c.IsAdmin)
                        .OrderBy(c => c.LastName)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching clients: {ex.Message}");
                await ArataNotificareRoz(AppResources.ErrorTitle, true);
            }
        }
        else
        {
            StackListaAdmin.IsVisible = false;
            LabelEroare.IsVisible = true;

            var toolbarItem = ToolbarItems.FirstOrDefault(t => t.Text == ToolbarAddClient?.Text);
            if (toolbarItem != null)
                ToolbarItems.Remove(toolbarItem);
        }
    }

    async void OnItemAdded(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ClientEntryPage
        {
            BindingContext = new Client()
        });
    }

    async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Client selectedClient)
        {
            await Navigation.PushAsync(new ClientEntryPage
            {
                BindingContext = selectedClient
            });

            if (sender is CollectionView collectionView)
            {
                collectionView.SelectedItem = null;
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