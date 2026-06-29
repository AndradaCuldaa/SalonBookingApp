using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using SalonBookingApp.Models;
using SalonBookingApp.Resources.Strings;

namespace SalonBookingApp.Views;

public partial class AppointmentPage : ContentPage
{
    public Client UserLogat => App.UserLogat;

    public AppointmentPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await IncarcaProgramari();
    }

    private async Task IncarcaProgramari()
    {
        try
        {
            if (App.UserLogat == null) return;

            var toateProgramarile = await App.Database.GetAppointmentsAsync();
            foreach (var p in toateProgramarile) p.AppointmentDate = p.AppointmentDate.ToLocalTime();

            var programarileUserului = toateProgramarile.Where(p => p.ClientID == App.UserLogat.ID).OrderBy(p => p.AppointmentDate).ToList();
            DateTime dataCurenta = DateTime.Now;

            var viitoare = programarileUserului.Where(p => p.AppointmentDate >= dataCurenta).ToList();
            var trecute = programarileUserului.Where(p => p.AppointmentDate < dataCurenta).OrderByDescending(p => p.AppointmentDate).ToList();

            ListaProgramari.ItemsSource = viitoare;
            ListaIstoric.ItemsSource = trecute;

            TitluViitoare.IsVisible = viitoare.Any();
            ListaProgramari.IsVisible = viitoare.Any();
            TitluIstoric.IsVisible = trecute.Any();
            ListaIstoric.IsVisible = trecute.Any();

            var pachete = await App.Database.GetPackagesForClientAsync(App.UserLogat.ID);
            var pacheteActive = pachete?.Where(p => p.IsActive).ToList();

            if (pacheteActive != null && pacheteActive.Any())
            {
                TitluPachete.IsVisible = true;
                ListaPachete.IsVisible = true;
                LinieDespartitoare.IsVisible = true;
                ListaPachete.ItemsSource = pacheteActive;
            }
            else
            {
                TitluPachete.IsVisible = false;
                ListaPachete.IsVisible = false;
                LinieDespartitoare.IsVisible = false;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
    }

    private async void OnNewAppointmentClicked(object sender, EventArgs e) => await Navigation.PushAsync(new NewAppointmentPage());

    private async void OnCancelTapped(object sender, EventArgs e)
    {
        var label = sender as Label;
        var programare = label?.BindingContext as Appointment;
        if (programare == null) return;

        var popup = new ConfirmPopup(AppResources.ConfirmTitle, AppResources.ConfirmCancel);
        await this.ShowPopupAsync(popup);

        if (popup.IsConfirmed)
        {
            await App.Database.DeleteAppointmentAsync(programare);

            try
            {
                string emailDestinatar = App.UserLogat?.Email ?? "andaanduta18@yahoo.ro";
                string numeDestinatar = App.UserLogat?.FirstName ?? "Client";
                string emailStilist = "andaanduta18@yahoo.ro";

                using var client = new System.Net.Http.HttpClient();
                var url = "https://iufctragcodrtzfdwkoq.supabase.co/functions/v1/send-email";
                client.DefaultRequestHeaders.Add("Authorization", "Bearer sb_publishable_b7r2dhT1u58cvW8c8D1WFw_z56qqm02");

                var payload = new
                {
                    action = "cancel",
                    client_email = emailDestinatar,
                    client_name = numeDestinatar,
                    stylist_email = emailStilist,
                    date = programare.AppointmentDate.ToString("dd.MM.yyyy"),
                    time = programare.AppointmentDate.ToString("HH:mm"),
                    service_name = programare.Service?.DisplayName ?? "Necunoscut"
                };
                var json = System.Text.Json.JsonSerializer.Serialize(payload);
                var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");
                await client.PostAsync(url, content);
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex.Message); }

            await IncarcaProgramari();
            await ArataNotificareRoz(AppResources.BtnCancel);
        }
    }

    private async void OnCancelPackageTapped(object sender, EventArgs e)
    {
        var label = sender as Label;
        var pachet = label?.BindingContext as ClientPackage;
        if (pachet == null) return;
        var popup = new ConfirmPopup(AppResources.ConfirmTitle, AppResources.CancelSubConfirmMsg);
        await this.ShowPopupAsync(popup);
        if (popup.IsConfirmed)
        {
            await App.Database.DeleteClientPackageAsync(pachet);
            await IncarcaProgramari();
            await ArataNotificareRoz(AppResources.CancelSubSuccess);
        }
    }

    private async void OnFolosestePachetClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var pachet = button?.CommandParameter as ClientPackage;
        if (pachet != null) await Navigation.PushAsync(new PackageDetailsPage(pachet));
    }

    private async void OnLeaveReviewTapped(object sender, EventArgs e)
    {
        var label = sender as Label;
        var stilistPentruRecenzie = ((TapGestureRecognizer)label.GestureRecognizers[0]).CommandParameter as Stylist;
        if (stilistPentruRecenzie != null) await Navigation.PushAsync(new StylistReviewPage(stilistPentruRecenzie));
    }

    private async Task ArataNotificareRoz(string mesaj, bool esteEroare = false)
    {
        var bgColor = esteEroare ? Color.FromArgb("#FF3B30") : Color.FromArgb("#EAB8C1");
        var border = new Border { BackgroundColor = bgColor, StrokeThickness = 0, Padding = new Thickness(20, 15, 20, 15), HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Start, Opacity = 0, StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(0, 0, 15, 15) }, ZIndex = 999 };
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