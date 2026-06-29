using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using Plugin.LocalNotification;
using Plugin.LocalNotification.Core.Models;
using SalonBookingApp.Models;
using SalonBookingApp.Resources.Strings;
using System.Globalization;

namespace SalonBookingApp.Views;

public partial class ReviewAppointmentPage : ContentPage
{
    private TimeSlot _slot;
    private Stylist _stylist;
    private Service _service;
    private ClientPackage _pachetActiv;
    private int _packageSlotIndex;
    private ClientPackage _pachetNou;
    private bool _esteAchizitiePachet = false;
    private int _pretPachetNou = 0;

    public ReviewAppointmentPage(TimeSlot slot, Stylist stylist, Service service, ClientPackage pachetActiv = null, int packageSlotIndex = 0)
    {
        InitializeComponent();

        _slot = slot;
        _stylist = stylist;
        _service = service;
        _pachetActiv = pachetActiv;
        _packageSlotIndex = packageSlotIndex;
        _esteAchizitiePachet = false;

        IncarcaDateProgramare();
    }

    public ReviewAppointmentPage(ClientPackage pachetNou, int pretPachet)
    {
        InitializeComponent();
        _pachetNou = pachetNou;
        _pretPachetNou = pretPachet;
        _esteAchizitiePachet = true;

        IncarcaDateAbonament();
    }

    private void IncarcaDateProgramare()
    {
        var culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ro" ? new CultureInfo("ro-RO") : new CultureInfo("en-US");

        string dataFormatata = _slot.FullDateTime.ToString("dd MMMM yyyy", culture);
        string oraFormatata = _slot.FullDateTime.ToString("HH:mm");
        LabelDataOra.Text = $"{dataFormatata}, ora {oraFormatata}";

        LabelStilist.Text = $"{_stylist.FirstName} - {_stylist.Specialization}";
        LabelServiciu.Text = _service.DisplayName;
        LabelDurataPret.Text = $"{_service.Duration} min, {_service.Price} lei";

        if (_pachetActiv != null)
        {
            LabelTotal.Text = "0 lei";
            LabelMetodaPlataTitlu.Text = AppResources.PackageAppliedTitle;
            LabelMetodaPlataDescriere.Text = AppResources.PackageAppliedDesc;
        }

    private void IncarcaDateAbonament()
    {
        GridData.IsVisible = false;
        Separator1.IsVisible = false;

        TitluPaginaHeader.Text = _pachetNou.PackageName;
        TitluCard.Text = _pachetNou.PackageName;
        LabelServiciu.Text = AppResources.IncludedServices;

        if (_pachetNou.PackageType == "Sedinte")
            LabelDurataPret.Text = $"{_pachetNou.TotalUses} x {AppResources.LabelChooseService}";
        else
            LabelDurataPret.Text = "Abonament VIP Lunar";

        LabelMetodaPlataTitlu.Text = AppResources.PaymentLocationTitle;
        LabelMetodaPlataDescriere.Text = AppResources.SubPaymentNote;

        LabelTotal.Text = $"{_pretPachetNou} lei";
        BtnConfirmare.Text = AppResources.ConfirmSubBtn;
    }

    private async void OnConfirmClicked(object sender, EventArgs e)
    {
        try
        {
            if (_esteAchizitiePachet)
            {
                await App.Database.SaveClientPackageAsync(_pachetNou);
                await ArataNotificareRoz(AppResources.SubAddedSuccess);
            }
            else
            {
                var appt = new Appointment
                {
                    AppointmentDate = _slot.FullDateTime,
                    StylistID = _stylist.ID,
                    ServiceID = _service.ID,
                    ClientID = App.UserLogat?.ID ?? 0
                };

                await App.Database.SaveAppointmentAsync(appt);
                if (_pachetActiv != null)
                {
                    if (_pachetActiv.PackageType == "Sedinte")
                    {
                        _pachetActiv.RemainingUses--;
                    }
                    else
                    {
                        if (_packageSlotIndex == 1) _pachetActiv.RemainingService1--;
                        else if (_packageSlotIndex == 2) _pachetActiv.RemainingService2--;
                        else if (_packageSlotIndex == 3) _pachetActiv.RemainingService3--;
                        else if (_packageSlotIndex == 4) _pachetActiv.RemainingService4--;
                    await App.Database.SaveClientPackageAsync(_pachetActiv);
                }

                try
                {
                    var request = new NotificationRequest
                    {
                        NotificationId = 1000 + appt.ID,
                        Title = AppResources.SuccessTitle,
                        Description = $"{AppResources.BookNow}: {appt.AppointmentDate:HH:mm}",
                        Schedule = new NotificationRequestSchedule { NotifyTime = DateTime.Now.AddSeconds(1) }
                    };
                    await LocalNotificationCenter.Current.Show(request);
                }
                catch { }
                await ArataNotificareRoz(AppResources.AppointmentSaved);
            }

            await Task.Delay(1500);
            await Shell.Current.GoToAsync("//AppointmentPage");
        }
        catch (Exception ex)
        {
            await ArataNotificareRoz(AppResources.ErrorTitle, true);
            System.Diagnostics.Debug.WriteLine($"Error confirming: {ex.Message}");
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

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}