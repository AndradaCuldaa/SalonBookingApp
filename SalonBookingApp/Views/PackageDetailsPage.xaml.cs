using SalonBookingApp.Models;
using SalonBookingApp.Resources.Strings;
using Microsoft.Maui.Controls.Shapes;

namespace SalonBookingApp.Views;

public partial class PackageDetailsPage : ContentPage
{
    private ClientPackage _pachet;

    public PackageDetailsPage(ClientPackage pachet)
    {
        InitializeComponent();
        _pachet = pachet;
        IncarcaDetalii();
    }

    private void IncarcaDetalii()
    {
        LabelNumePachet.Text = _pachet.PackageName;
        LabelValabilitate.Text = $"{AppResources.ExpiresOn} {_pachet.ExpiryDate:dd MMM yyyy}";

        ContainerServicii.Children.Clear();

        if (_pachet.PackageType == "Sedinte")
        {
            if (_pachet.PackageName == AppResources.PackHairName)
                AdaugaCardServiciu(AppResources.PackHairName, _pachet.RemainingUses, 0, "Retușare Rădăcini ", "Tuns Damă");

            else if (_pachet.PackageName == AppResources.PackNailsName)
                AdaugaCardServiciu(AppResources.PackNailsName, _pachet.RemainingUses, 0, "Manichiură", "Pedichiură");

            else if (_pachet.PackageName == AppResources.PackEventName)
                AdaugaCardServiciu(AppResources.PackEventName, _pachet.RemainingUses, 0, "Aranjare Păr", "Machiaj de seară");
        }
        else
        {
            switch (_pachet.PackageName)
            {
                case "BASIC":
                    if (_pachet.RemainingService1 > 0)
                        AdaugaCardServiciu(AppResources.ServiceRoots, _pachet.RemainingService1, 1, "Retușare Rădăcini ");
                    if (_pachet.RemainingService2 > 0)
                        AdaugaCardServiciu(AppResources.ServiceManicure, _pachet.RemainingService2, 2, "Manichiură");
                    break;

                case "SILVER":
                    if (_pachet.RemainingService1 > 0)
                        AdaugaCardServiciu(AppResources.ServiceNails, _pachet.RemainingService1, 1, "Manichiură", "Construcție gel");
                    if (_pachet.RemainingService2 > 0)
                        AdaugaCardServiciu(AppResources.ServiceHairstyle, _pachet.RemainingService2, 2, "Aranjare Păr");
                    if (_pachet.RemainingService3 > 0)
                        AdaugaCardServiciu(AppResources.ServiceEyebrows, _pachet.RemainingService3, 3, "Stilizare sprâncene", "Laminare sprâncene");
                    break;

                case "GOLD":
                    if (_pachet.RemainingService1 > 0)
                        AdaugaCardServiciu(AppResources.ServiceHairorFacial, _pachet.RemainingService1, 1, "Vopsit clasic", "Tratamente faciale");
                    if (_pachet.RemainingService2 > 0)
                        AdaugaCardServiciu(AppResources.ServiceManiPedi, _pachet.RemainingService2, 2, "Manichiură", "Pedichiură");
                    if (_pachet.RemainingService3 > 0)
                        AdaugaCardServiciu(AppResources.ServiceHairstyle, _pachet.RemainingService3, 3, "Aranjare Păr");
                    break;

                case "DIAMOND":
                    if (_pachet.RemainingService1 > 0)
                        AdaugaCardServiciu(AppResources.ServicePremiumHair, _pachet.RemainingService1, 1, "Balayage", "Ombre", "Vopsit clasic", "Tuns Damă");
                    if (_pachet.RemainingService2 > 0)
                        AdaugaCardServiciu(AppResources.ServiceFacials, _pachet.RemainingService2, 2, "Tratamente faciale");
                    if (_pachet.RemainingService3 > 0)
                        AdaugaCardServiciu(AppResources.ServiceManiPedi, _pachet.RemainingService3, 3, "Manichiură", "Pedichiură");
                    if (_pachet.RemainingService4 > 0)
                        AdaugaCardServiciu(AppResources.ServiceMakeup, _pachet.RemainingService4, 4, "Machiaj de seară", "Machiaj de mireasă");
                    break;
            }
        }
    }

    private void AdaugaCardServiciu(string displayTitle, int ramase, int slotIndex, params string[] allowedDbNames)
    {
        var border = new Border
        {
            BackgroundColor = Color.FromArgb("#FFF0F2"),
            Padding = 20,
            StrokeShape = new RoundRectangle { CornerRadius = 20 },
            StrokeThickness = 0,
            Margin = new Thickness(0, 0, 0, 10)
        };

        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var infoLayout = new VerticalStackLayout { Spacing = 2, VerticalOptions = LayoutOptions.Center };

        infoLayout.Children.Add(new Label { Text = displayTitle, FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Colors.Black });
        infoLayout.Children.Add(new Label { Text = $"{ramase} {AppResources.Remaining}", FontSize = 14, TextColor = Color.FromArgb("#EAB8C1") });

        var btn = new Button
        {
            Text = "+ " + AppResources.BtnNewAppoint,
            BackgroundColor = Color.FromArgb("#EAB8C1"),
            TextColor = Colors.White,
            CornerRadius = 10,
            HeightRequest = 40,
            Padding = new Thickness(15, 0)
        };

        btn.Clicked += async (s, e) =>
        {
            var allowedServicesList = allowedDbNames.ToList();
            await Navigation.PushAsync(new NewAppointmentPage(allowedServicesList, _pachet, slotIndex));
        };

        grid.Children.Add(infoLayout);
        grid.Children.Add(btn);
        Grid.SetColumn(btn, 1);

        border.Content = grid;
        ContainerServicii.Children.Add(border);
    }
}