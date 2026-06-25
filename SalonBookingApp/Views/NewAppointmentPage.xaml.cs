using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using Plugin.LocalNotification;
using Plugin.LocalNotification.Core.Models;
using SalonBookingApp.Models;
using SalonBookingApp.Resources.Strings;
using System.Collections.ObjectModel;
using System.Globalization;

namespace SalonBookingApp.Views;

public class MonthItem
{
    public string DisplayName { get; set; }
    public DateTime DateValue { get; set; }
}

public partial class NewAppointmentPage : ContentPage
{
    private bool _isBusy = false;
    private int? _preselectedServiceId = null;
    private List<string> _allowedServiceNames = null;
    private ClientPackage _pachetActiv = null;
    private int _packageSlotIndex = 0;

    public NewAppointmentPage(object parameter = null, ClientPackage pachetActiv = null, int packageSlotIndex = 0)
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);

        if (parameter is int id)
        {
            _preselectedServiceId = id;
        }
        else if (parameter is List<string> names)
        {
            _allowedServiceNames = names;
        }

        _pachetActiv = pachetActiv;
        _packageSlotIndex = packageSlotIndex;
    }

    private CultureInfo GetCurrentCultureInfo()
    {
        bool isRo = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ro";
        return isRo ? new CultureInfo("ro-RO") : new CultureInfo("en-US");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        PopulateCalendar(DateTime.Today);

        var servicii = await App.Database.GetServicesAsync();

        if (_allowedServiceNames != null && _allowedServiceNames.Any())
        {
            servicii = servicii.Where(s => _allowedServiceNames.Contains(s.DisplayName) || _allowedServiceNames.Contains(s.Name)).ToList();

            if (servicii.Count == 1)
            {
                var targetService = servicii.First();
                SelectedServiceLabel.Text = $"{targetService.DisplayName} ({AppResources.BtnUsePackage})";
                SelectedServiceLabel.TextColor = Color.FromArgb("#EAB8C1");
                ServiceDropdownList.IsVisible = false;

                foreach (var s in servicii) s.IsSelected = true;
                StylistSelectionList.ItemsSource = await GetStylistiFiltrati(targetService.ID);
                await RefreshAvailableSlots();
            }
            else if (servicii.Count > 1)
            {
                SelectedServiceLabel.Text = $"{AppResources.LabelChooseService} ({AppResources.BtnUsePackage})";
                SelectedServiceLabel.TextColor = Color.FromArgb("#EAB8C1");
                ServiceDropdownList.IsVisible = false;
            }
            ServicesSelectionList.ItemsSource = servicii;
        }
        else if (_preselectedServiceId.HasValue)
        {
            var targetService = servicii.FirstOrDefault(s => s.ID == _preselectedServiceId.Value);
            if (targetService != null)
            {
                SelectedServiceLabel.Text = targetService.DisplayName;
                SelectedServiceLabel.TextColor = Colors.Black;
                ServiceDropdownList.IsVisible = false;

                foreach (var s in servicii) s.IsSelected = (s.ID == targetService.ID);
                StylistSelectionList.ItemsSource = await GetStylistiFiltrati(targetService.ID);
                await RefreshAvailableSlots();
            }
            ServicesSelectionList.ItemsSource = servicii;
        }
        else
        {
            ServicesSelectionList.ItemsSource = servicii;
        }

        await ValideazaSiIncarca(true);
    }

    private void PopulateCalendar(DateTime startDate)
    {
        var zile = new List<CalendarDay>();
        int daysInMonth = DateTime.DaysInMonth(startDate.Year, startDate.Month);
        int daysToAdd = daysInMonth - startDate.Day + 1;
        var culture = GetCurrentCultureInfo();

        for (int i = 0; i < daysToAdd; i++)
        {
            var targetDate = startDate.AddDays(i);
            zile.Add(new CalendarDay { Date = targetDate, IsSelected = false });
        }

        if (zile.Any()) zile.First().IsSelected = true;
        CalendarList.ItemsSource = zile;

        string monthName = startDate.ToString("MMMM yyyy", culture);
        monthName = char.ToUpper(monthName[0]) + monthName.Substring(1);
        LabelLunaAn.Text = monthName + " ▼";
    }

    private DateTime GetMaxAllowedDate(Stylist stylist)
    {
        DateTime maxDate = DateTime.Today.AddMonths(6);
        if (stylist != null && !string.IsNullOrEmpty(stylist.FutureLimit))
        {
            if (stylist.FutureLimit == "Maxim 30 de zile în avans")
                maxDate = DateTime.Today.AddDays(30);
            else if (stylist.FutureLimit == "Maxim 60 de zile în avans")
                maxDate = DateTime.Today.AddDays(60);
            else if (stylist.FutureLimit == "Maxim 120 de zile în avans")
                maxDate = DateTime.Today.AddDays(120);
            else if (stylist.FutureLimit == "Maxim 6 luni în avans")
                maxDate = DateTime.Today.AddMonths(6);
        }
        return maxDate;
    }

    private void OnMonthTapped(object sender, EventArgs e)
    {
        var months = new List<MonthItem>();
        var culture = GetCurrentCultureInfo();
        var stilistCurent = ((IEnumerable<Stylist>)StylistSelectionList.ItemsSource)?.FirstOrDefault(s => s.IsSelected);
        DateTime maxDate = GetMaxAllowedDate(stilistCurent);
        DateTime maxMonthStart = new DateTime(maxDate.Year, maxDate.Month, 1);

        for (int i = 0; i < 6; i++)
        {
            var targetDate = DateTime.Today.AddMonths(i);
            DateTime targetMonthStart = new DateTime(targetDate.Year, targetDate.Month, 1);

            if (targetMonthStart <= maxMonthStart)
            {
                string monthName = targetDate.ToString("MMMM yyyy", culture);
                monthName = char.ToUpper(monthName[0]) + monthName.Substring(1);
                months.Add(new MonthItem { DisplayName = monthName, DateValue = targetDate });
            }
        }

        MonthSelectionList.ItemsSource = months;
        MonthPickerOverlay.IsVisible = true;
    }

    private async void OnSpecificMonthTapped(object sender, EventArgs e)
    {
        if ((sender as Grid)?.BindingContext is MonthItem selectedMonth)
        {
            DateTime targetMonth = selectedMonth.DateValue;
            DateTime newStartDate = (targetMonth.Month == DateTime.Today.Month && targetMonth.Year == DateTime.Today.Year)
                                    ? DateTime.Today
                                    : new DateTime(targetMonth.Year, targetMonth.Month, 1);

            PopulateCalendar(newStartDate);
            MonthPickerOverlay.IsVisible = false;
            await ValideazaSiIncarca();
        }
    }

    private void CloseMonthPicker(object sender, EventArgs e) => MonthPickerOverlay.IsVisible = false;

    private async void OnToggleServiceList(object sender, EventArgs e)
    {
        if (_allowedServiceNames != null && _allowedServiceNames.Count == 1) return;
        ServiceDropdownList.IsVisible = !ServiceDropdownList.IsVisible;
        var arrow = this.FindByName<Label>("ArrowLabel");
        if (arrow != null) await arrow.RotateTo(ServiceDropdownList.IsVisible ? 180 : 0, 200);
    }

    private async void OnServiceTapped(object sender, EventArgs e)
    {
        if (_allowedServiceNames != null && _allowedServiceNames.Count == 1) return;

        Service selectedService = (sender as View)?.BindingContext as Service;
        if (e is TappedEventArgs tapped && tapped.Parameter is Service srv) selectedService = srv;

        if (selectedService != null)
        {
            SelectedServiceLabel.Text = selectedService.DisplayName;
            SelectedServiceLabel.TextColor = Colors.Black;
            ServiceDropdownList.IsVisible = false;

            var arrow = this.FindByName<Label>("ArrowLabel");
            if (arrow != null) await arrow.RotateTo(0, 200);

            if (ServicesSelectionList.ItemsSource is IEnumerable<Service> services)
            {
                foreach (var s in services) s.IsSelected = (s.ID == selectedService.ID);
            }

            StylistSelectionList.ItemsSource = await GetStylistiFiltrati(selectedService.ID);
            await ValideazaSiIncarca();
        }
    }

    private async Task<List<Stylist>> GetStylistiFiltrati(int serviceId)
    {
        return await App.Database.GetStylistsForServiceAsync(serviceId);
    }

    private async void OnDateTapped(object sender, EventArgs e)
    {
        if ((sender as View)?.GestureRecognizers[0] is TapGestureRecognizer tap && tap.CommandParameter is CalendarDay day)
        {
            if (day.Date.DayOfWeek == DayOfWeek.Sunday)
            {
                await ArataNotificareRoz(AppResources.SalonClosedMessage, true);
                return;
            }

            var stilistCurent = ((IEnumerable<Stylist>)StylistSelectionList.ItemsSource)?.FirstOrDefault(s => s.IsSelected);

            if (stilistCurent != null)
            {
                DateTime maxDate = GetMaxAllowedDate(stilistCurent);
                if (day.Date > maxDate.Date)
                {
                    await ArataNotificareRoz(string.Format(AppResources.StylistUnavailableMessage, stilistCurent.FirstName), true);
                    return;
                }

                bool lucreaza = await VerificaProgramStilistAsync(stilistCurent.ID, day.Date);
                if (!lucreaza)
                {
                    await ArataNotificareRoz(string.Format(AppResources.StylistUnavailableMessage, stilistCurent.FirstName), true);
                    return;
                }
            }

            foreach (var d in (IEnumerable<CalendarDay>)CalendarList.ItemsSource) d.IsSelected = false;
            day.IsSelected = true;

            var culture = GetCurrentCultureInfo();
            string monthName = day.Date.ToString("MMMM yyyy", culture);
            LabelLunaAn.Text = char.ToUpper(monthName[0]) + monthName.Substring(1) + " ▼";

            await RefreshAvailableSlots();
        }
    }

    private async void OnStylistTapped(object sender, EventArgs e)
    {
        Stylist stylist = (sender as View)?.BindingContext as Stylist;
        if (e is TappedEventArgs tapped && tapped.Parameter is Stylist stl) stylist = stl;

        if (stylist != null)
        {
            if (StylistSelectionList.ItemsSource is IEnumerable<Stylist> stylists)
            {
                foreach (var s in stylists) s.IsSelected = (s.ID == stylist.ID);
            }
            await ValideazaSiIncarca();
        }
    }

    private async Task ValideazaSiIncarca(bool laInitializare = false)
    {
        var selectedDay = ((IEnumerable<CalendarDay>)CalendarList.ItemsSource)?.FirstOrDefault(d => d.IsSelected);
        var stilistCurent = ((IEnumerable<Stylist>)StylistSelectionList.ItemsSource)?.FirstOrDefault(s => s.IsSelected);

        if (selectedDay != null)
        {
            DateTime maxDate = GetMaxAllowedDate(stilistCurent);
            bool lucreaza = true;

            if (stilistCurent != null)
            {
                lucreaza = await VerificaProgramStilistAsync(stilistCurent.ID, selectedDay.Date);
            }

            bool trebuieSchimbataZiua = (selectedDay.Date.DayOfWeek == DayOfWeek.Sunday) ||
                                        (!lucreaza) ||
                                        (selectedDay.Date > maxDate.Date);

            if (trebuieSchimbataZiua)
            {
                CalendarDay urmatoareaZi = null;
                foreach (var d in (IEnumerable<CalendarDay>)CalendarList.ItemsSource)
                {
                    if (d.Date > selectedDay.Date && d.Date <= maxDate.Date && d.Date.DayOfWeek != DayOfWeek.Sunday)
                    {
                        if (stilistCurent == null)
                        {
                            urmatoareaZi = d;
                            break;
                        }

                        bool ziBuna = await VerificaProgramStilistAsync(stilistCurent.ID, d.Date);
                        if (ziBuna)
                        {
                            urmatoareaZi = d;
                            break;
                        }
                    }
                }

                if (urmatoareaZi != null)
                {
                    foreach (var d in (IEnumerable<CalendarDay>)CalendarList.ItemsSource) d.IsSelected = false;
                    urmatoareaZi.IsSelected = true;
                    var culture = GetCurrentCultureInfo();
                    string monthName = urmatoareaZi.Date.ToString("MMMM yyyy", culture);
                    LabelLunaAn.Text = char.ToUpper(monthName[0]) + monthName.Substring(1) + " ▼";
                }
            }
        }
        await RefreshAvailableSlots();
    }

    private async Task<bool> VerificaProgramStilistAsync(int stylistId, DateTime dataAleasa)
    {
        if (dataAleasa.DayOfWeek == DayOfWeek.Sunday) return false;

        var schedule = await App.Database.GetWorkScheduleAsync(stylistId, dataAleasa.Date);
        if (schedule != null)
        {
            return !schedule.IsOff;
        }

        return true;
    }

    private void OnSlotTapped(object sender, EventArgs e)
    {
        if ((sender as View)?.GestureRecognizers[0] is TapGestureRecognizer tap && tap.CommandParameter is TimeSlot slot)
        {
            var morning = MorningSlotsList.ItemsSource as IEnumerable<TimeSlot>;
            var afternoon = AfternoonSlotsList.ItemsSource as IEnumerable<TimeSlot>;
            if (morning != null) foreach (var s in morning) s.IsSelected = false;
            if (afternoon != null) foreach (var s in afternoon) s.IsSelected = false;
            slot.IsSelected = true;
        }
    }

    private async Task RefreshAvailableSlots()
    {
        if (_isBusy) return;
        _isBusy = true;
        try
        {
            var service = ((IEnumerable<Service>)ServicesSelectionList.ItemsSource)?.FirstOrDefault(s => s.IsSelected);
            var stylist = ((IEnumerable<Stylist>)StylistSelectionList.ItemsSource)?.FirstOrDefault(s => s.IsSelected);
            var selectedDay = ((IEnumerable<CalendarDay>)CalendarList.ItemsSource)?.FirstOrDefault(d => d.IsSelected);

            if (service != null && stylist != null && selectedDay != null)
            {
                var allSlots = await GetAvailableSlots(stylist.ID, selectedDay.Date, service);
                MainThread.BeginInvokeOnMainThread(() => {
                    MorningSlotsList.ItemsSource = allSlots.Where(s => s.FullDateTime.Hour < 12).ToList();
                    AfternoonSlotsList.ItemsSource = allSlots.Where(s => s.FullDateTime.Hour >= 12).ToList();
                });
            }
        }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex.Message); }
        finally { _isBusy = false; }
    }

    public async Task<List<TimeSlot>> GetAvailableSlots(int stylistId, DateTime date, Service selectedService)
    {
        var slots = new List<TimeSlot>();
        int duration = selectedService.Duration > 0 ? selectedService.Duration : 30;
        var allAppointments = await App.Database.GetAppointmentsAsync();
        var appsForToday = allAppointments.Where(a => a.StylistID == stylistId && a.AppointmentDate.Date == date.Date).ToList();

        var schedule = await App.Database.GetWorkScheduleAsync(stylistId, date.Date);

        if (schedule != null && schedule.IsOff)
            return slots;

        TimeSpan start1 = schedule?.StartTime1 ?? new TimeSpan(9, 0, 0);
        TimeSpan end1 = schedule?.EndTime1 ?? new TimeSpan(13, 0, 0);
        TimeSpan start2 = schedule?.StartTime2 ?? new TimeSpan(14, 0, 0);
        TimeSpan end2 = schedule?.EndTime2 ?? new TimeSpan(18, 0, 0);

        AdaugaIntervalOrare(start1, end1, duration, date, appsForToday, slots);
        AdaugaIntervalOrare(start2, end2, duration, date, appsForToday, slots);

        return slots;
    }

    private void AdaugaIntervalOrare(TimeSpan start, TimeSpan end, int duration, DateTime date, List<Appointment> appsForToday, List<TimeSlot> slots)
    {
        TimeSpan currentTime = start;
        while (currentTime + TimeSpan.FromMinutes(duration) <= end)
        {
            DateTime startPotential = date.Date.Add(currentTime);
            DateTime endPotential = startPotential.AddMinutes(duration);
            bool isOverlapping = appsForToday.Any(appt => startPotential < appt.AppointmentDate.AddMinutes(30) && endPotential > appt.AppointmentDate);

            if (!isOverlapping && startPotential > DateTime.Now)
            {
                slots.Add(new TimeSlot { FullDateTime = startPotential });
            }
            currentTime = currentTime.Add(TimeSpan.FromMinutes(30));
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var selectedService = ((IEnumerable<Service>)ServicesSelectionList.ItemsSource)?.FirstOrDefault(s => s.IsSelected);
        var selectedStylist = ((IEnumerable<Stylist>)StylistSelectionList.ItemsSource)?.FirstOrDefault(s => s.IsSelected);
        var selectedSlot = ((IEnumerable<TimeSlot>)MorningSlotsList.ItemsSource)?.FirstOrDefault(s => s.IsSelected)
                           ?? ((IEnumerable<TimeSlot>)AfternoonSlotsList.ItemsSource)?.FirstOrDefault(s => s.IsSelected);

        if (selectedSlot == null || selectedStylist == null || selectedService == null)
        {
            await ArataNotificareRoz(AppResources.SelectAllError, true);
            return;
        }

        await Navigation.PushAsync(new ReviewAppointmentPage(selectedSlot, selectedStylist, selectedService, _pachetActiv, _packageSlotIndex));
    }

    private async void OnBackClicked(object sender, EventArgs e) => await Navigation.PopAsync();

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
        if (border.Opacity > 0) { await border.TranslateTo(0, -150, 300, Easing.CubicIn); await border.FadeTo(0, 100); }
        gridPrincipal.Children.Remove(border);
    }
}