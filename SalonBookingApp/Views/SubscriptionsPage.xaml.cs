using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using SalonBookingApp.Resources.Strings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalonBookingApp.Views;

public partial class SubscriptionsPage : ContentPage
{
    public SubscriptionsPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        IncarcaAbonamente();
    }

    private void IncarcaAbonamente()
    {
        var abonamenteLunare = new List<MonthlyPlan>
        {
            new MonthlyPlan
            {
                Name = "BASIC",
                Concept = AppResources.BasicConcept,
                Price = 230,
                Inclusions = AppResources.BasicInclusions.Replace("\\n", Environment.NewLine),
                Perks = AppResources.BasicPerks.Replace("\\n", Environment.NewLine),
                CardColor = Color.FromArgb("#FDE0E6"),
                TextColor = Colors.Black
            },
            new MonthlyPlan
            {
                Name = "SILVER",
                Concept = AppResources.SilverConcept,
                Price = 300,
                Inclusions = AppResources.SilverInclusions.Replace("\\n", Environment.NewLine),
                Perks = AppResources.SilverPerks.Replace("\\n", Environment.NewLine),
                CardColor = Color.FromArgb("#F2AEC1"),
                TextColor = Colors.Black
            },
            new MonthlyPlan
            {
                Name = "GOLD",
                Concept = AppResources.GoldConcept,
                Price = 700,
                Inclusions = AppResources.GoldInclusions.Replace("\\n", Environment.NewLine),
                Perks = AppResources.GoldPerks.Replace("\\n", Environment.NewLine),
                CardColor = Color.FromArgb("#D96B85"),
                TextColor = Colors.White
            },
            new MonthlyPlan
            {
                Name = "DIAMOND",
                Concept = AppResources.DiamondConcept,
                Price = 1000,
                Inclusions = AppResources.DiamondInclusions.Replace("\\n", Environment.NewLine),
                Perks = AppResources.DiamondPerks.Replace("\\n", Environment.NewLine),
                CardColor = Color.FromArgb("#A33351"),
                TextColor = Colors.White
            }
        };
        ListaLunare.ItemsSource = abonamenteLunare;

        var pacheteSedinte = new List<SessionPlan>
        {
            new SessionPlan
            {
                Name = AppResources.PackHairName,
                Price = 600,
                Description = AppResources.PackHairDesc.Replace("\\n", Environment.NewLine)
            },
            new SessionPlan
            {
                Name = AppResources.PackNailsName,
                Price = 500,
                Description = AppResources.PackNailsDesc.Replace("\\n", Environment.NewLine)
            },
            new SessionPlan
            {
                Name = AppResources.PackEventName,
                Price = 1000,
                Description = AppResources.PackEventDesc.Replace("\\n", Environment.NewLine)
            }
        };
        ListaSedinte.ItemsSource = pacheteSedinte;
    }

    private void OnTabLunareTapped(object sender, EventArgs e)
    {
        TabLunare.BackgroundColor = Colors.White;
        TabLunare.HasShadow = true;
        TextLunare.TextColor = Colors.Black;
        TextLunare.FontAttributes = FontAttributes.Bold;

        TabSedinte.BackgroundColor = Colors.Transparent;
        TabSedinte.HasShadow = false;
        TextSedinte.TextColor = Color.FromArgb("#8E8E93");
        TextSedinte.FontAttributes = FontAttributes.None;

        ListaLunare.IsVisible = true;
        ListaSedinte.IsVisible = false;
    }

    private void OnTabSedinteTapped(object sender, EventArgs e)
    {
        TabLunare.BackgroundColor = Colors.Transparent;
        TabLunare.HasShadow = false;
        TextLunare.TextColor = Color.FromArgb("#8E8E93");
        TextLunare.FontAttributes = FontAttributes.None;

        TabSedinte.BackgroundColor = Colors.White;
        TabSedinte.HasShadow = true;
        TextSedinte.TextColor = Colors.Black;
        TextSedinte.FontAttributes = FontAttributes.Bold;

        ListaLunare.IsVisible = false;
        ListaSedinte.IsVisible = true;
    }

    private async void OnBuyMonthlyPlanClicked(object sender, EventArgs e)
    {
        if (App.UserLogat == null)
        {
            await ArataNotificareRoz(AppResources.ErrorTitle, true);
            return;
        }

        var plan = (sender as Button)?.BindingContext as MonthlyPlan;
        if (plan != null)
        {
            var newPackage = new Models.ClientPackage
            {
                ClientID = App.UserLogat.ID,
                PackageName = plan.Name,
                PackageType = "Lunar",
                PurchaseDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddMonths(1),
                IsActive = true
            };

            switch (plan.Name)
            {
                case "BASIC":
                    newPackage.RemainingService1 = 1;
                    newPackage.RemainingService2 = 1;
                    break;
                case "SILVER":
                    newPackage.RemainingService1 = 1;
                    newPackage.RemainingService2 = 1;
                    newPackage.RemainingService3 = 1;
                    break;
                case "GOLD":
                    newPackage.RemainingService1 = 1;
                    newPackage.RemainingService2 = 2;
                    newPackage.RemainingService3 = 2;
                    break;
                case "DIAMOND":
                    newPackage.RemainingService1 = 1;
                    newPackage.RemainingService2 = 1;
                    newPackage.RemainingService3 = 2;
                    newPackage.RemainingService4 = 1;
                    break;
            }

            await Navigation.PushAsync(new ReviewAppointmentPage(newPackage, plan.Price));
        }
    }

    private async void OnBuySessionPackageClicked(object sender, EventArgs e)
    {
        if (App.UserLogat == null)
        {
            await ArataNotificareRoz(AppResources.ErrorTitle, true);
            return;
        }

        var plan = (sender as Button)?.BindingContext as SessionPlan;
        if (plan != null)
        {
            var newPackage = new Models.ClientPackage
            {
                ClientID = App.UserLogat.ID,
                PackageName = plan.Name,
                PackageType = "Sedinte",
                PurchaseDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddYears(1),
                TotalUses = 3,
                RemainingUses = 3,
                IsActive = true
            };

            await Navigation.PushAsync(new ReviewAppointmentPage(newPackage, plan.Price));
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

    public class MonthlyPlan
    {
        public string Name { get; set; }
        public string Concept { get; set; }
        public int Price { get; set; }
        public string Inclusions { get; set; }
        public string Perks { get; set; }
        public Color CardColor { get; set; }
        public Color TextColor { get; set; }
    }

    public class SessionPlan
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }
    }
}