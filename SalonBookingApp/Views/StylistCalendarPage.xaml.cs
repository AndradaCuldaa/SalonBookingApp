using SalonBookingApp.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Maui.Layouts;
using Microsoft.Maui.Controls.Shapes;

namespace SalonBookingApp.Views
{
    public partial class StylistCalendarPage : ContentPage
    {
        private ObservableCollection<ModelZiCalendar> _zileleLunii;
        private ObservableCollection<ModelStilistSelectabil> _stilisti;
        private DateTime _dataSelectata;
        private int _idStilistSelectat;
        private bool _seInitializeaza = true;

        private readonly int _startHour = 8;
        private readonly int _endHour = 20;
        private readonly double _inaltimeOra = 90;

        public StylistCalendarPage()
        {
            InitializeComponent();
            _zileleLunii = new ObservableCollection<ModelZiCalendar>();
            _stilisti = new ObservableCollection<ModelStilistSelectabil>();

            ListaZile.ItemsSource = _zileleLunii;
            ListaStilisti.ItemsSource = _stilisti;

            _dataSelectata = DateTime.Today;
            PopuleazaPickerLuni();
            ActualizeazaHeaderLuna(_dataSelectata);

            if (App.StilistLogat != null)
            {
                _idStilistSelectat = App.StilistLogat.ID;
            }

            GenereazaZilePentruLuna(_dataSelectata);
            _seInitializeaza = false;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await IncarcaStilisti();

            if (App.EsteAdmin && _idStilistSelectat == 0 && _stilisti.Count > 0)
            {
                var primul = _stilisti.First();
                primul.IsSelected = true;
                _idStilistSelectat = primul.Stilist.ID;
            }

            IncarcaProgramari(_dataSelectata);
        }

        private void PopuleazaPickerLuni()
        {
            var luni = DateTimeFormatInfo.CurrentInfo.MonthNames
                .Where(m => !string.IsNullOrEmpty(m))
                .Select(m => char.ToUpper(m[0]) + m.Substring(1))
                .ToList();
            PickerLuni.ItemsSource = luni;
            PickerLuni.SelectedIndex = _dataSelectata.Month - 1;
        }

        private void ActualizeazaHeaderLuna(DateTime data)
        {
            string numeLuna = data.ToString("MMMM", CultureInfo.CurrentCulture);
            LabelLunaAn.Text = char.ToUpper(numeLuna[0]) + numeLuna.Substring(1) + " " + data.Year;
        }

        private void OnHeaderTapped(object sender, EventArgs e) => PickerLuni.Focus();

        private void OnMonthPickerChanged(object sender, EventArgs e)
        {
            if (_seInitializeaza) return;
            int lunaNoua = PickerLuni.SelectedIndex + 1;
            _dataSelectata = new DateTime(_dataSelectata.Year, lunaNoua, 1);
            ActualizeazaHeaderLuna(_dataSelectata);
            GenereazaZilePentruLuna(_dataSelectata);
            IncarcaProgramari(_dataSelectata);
        }

        private async Task IncarcaStilisti()
        {
            try
            {
                var totiStilistii = await App.Database.GetStylistsAsync();
                var listaOrdonata = App.EsteAdmin
                    ? totiStilistii.OrderBy(s => s.FirstName).ToList()
                    : totiStilistii.OrderByDescending(s => s.ID == App.StilistLogat?.ID).ThenBy(s => s.FirstName).ToList();

                _stilisti.Clear();
                foreach (var s in listaOrdonata)
                {
                    _stilisti.Add(new ModelStilistSelectabil
                    {
                        Stilist = s,
                        IsSelected = s.ID == _idStilistSelectat,
                        PozaPath = s.FirstName.ToLower() + ".png"
                    });
                }
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
        }

        private void OnStilistTapped(object sender, TappedEventArgs e)
        {
            if (sender is View view && view.BindingContext is ModelStilistSelectabil stilistSelectat)
            {
                foreach (var s in _stilisti) { s.IsSelected = false; }
                stilistSelectat.IsSelected = true;
                _idStilistSelectat = stilistSelectat.Stilist.ID;
                IncarcaProgramari(_dataSelectata);
            }
        }

        private void OnZiTapped(object sender, TappedEventArgs e)
        {
            if (sender is View view && view.BindingContext is ModelZiCalendar ziSelectata)
            {
                foreach (var zi in _zileleLunii) { zi.IsSelected = false; }
                ziSelectata.IsSelected = true;
                _dataSelectata = ziSelectata.DataCompleta;
                IncarcaProgramari(_dataSelectata);
            }
        }

        private void GenereazaZilePentruLuna(DateTime dataDinLuna)
        {
            _zileleLunii.Clear();
            int zileInLuna = DateTime.DaysInMonth(dataDinLuna.Year, dataDinLuna.Month);
            for (int i = 1; i <= zileInLuna; i++)
            {
                var dataCurenta = new DateTime(dataDinLuna.Year, dataDinLuna.Month, i);
                _zileleLunii.Add(new ModelZiCalendar
                {
                    DataCompleta = dataCurenta,
                    NumeZi = dataCurenta.ToString("dddd").Substring(0, 1).ToUpper(),
                    NumarZi = i.ToString(),
                    IsSelected = dataCurenta.Date == _dataSelectata.Date
                });
            }
            FaScrollLaZiuaCurenta();
        }

        private async void FaScrollLaZiuaCurenta()
        {
            await Task.Delay(200);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var ziCurenta = _zileleLunii.FirstOrDefault(z => z.IsSelected);
                if (ziCurenta != null) ListaZile.ScrollTo(ziCurenta, position: ScrollToPosition.Start, animate: false);
            });
        }

        private async void IncarcaProgramari(DateTime data)
        {
            if (_idStilistSelectat == 0) return;
            try
            {
                var toateProgramarile = await App.Database.GetAppointmentsAsync();
                var programariZi = toateProgramarile
                    .Where(p => p.StylistID == _idStilistSelectat && p.AppointmentDate.ToLocalTime().Date == data.Date)
                    .OrderBy(p => p.AppointmentDate)
                    .ToList();

                var clienti = await App.Database.GetClientsAsync();
                foreach (var p in programariZi)
                {
                    p.AppointmentDate = p.AppointmentDate.ToLocalTime();
                    var client = clienti.FirstOrDefault(c => c.ID == p.ClientID);
                    if (client != null) p.ClientNameDisplay = $"{client.FirstName} {client.LastName}";
                }
                DeseneazaTimeline(programariZi);
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
        }

        private void DeseneazaTimeline(List<Appointment> programari)
        {
            TimelineGrid.Children.Clear();
            TimelineGrid.RowDefinitions.Clear();

            int totalHours = _endHour - _startHour + 1;
            for (int i = 0; i < totalHours; i++)
            {
                TimelineGrid.RowDefinitions.Add(new RowDefinition { Height = _inaltimeOra });
                var timeLabel = new Label { Text = $"{_startHour + i:D2}:00", TextColor = Color.FromArgb("#8E8E93"), FontSize = 14, FontFamily = "Lora", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Start, Margin = new Thickness(0, 0, 10, 0), HorizontalTextAlignment = TextAlignment.End };
                Grid.SetRow(timeLabel, i); Grid.SetColumn(timeLabel, 0); TimelineGrid.Children.Add(timeLabel);

                var emptySlot = new Border { BackgroundColor = Color.FromArgb("#F9F9F9"), StrokeThickness = 1, Stroke = Color.FromArgb("#EEEEEE"), Margin = new Thickness(0, 0, 0, 2), StrokeShape = new RoundRectangle { CornerRadius = 10 } };
                Grid.SetRow(emptySlot, i); Grid.SetColumn(emptySlot, 1); TimelineGrid.Children.Add(emptySlot);
            }

            var panouSuprapus = new AbsoluteLayout();
            Grid.SetRow(panouSuprapus, 0); Grid.SetRowSpan(panouSuprapus, totalHours); Grid.SetColumn(panouSuprapus, 1);
            TimelineGrid.Children.Add(panouSuprapus);

            foreach (var p in programari)
            {
                double oreDeLaStart = (p.AppointmentDate.TimeOfDay.TotalHours - _startHour);
                if (oreDeLaStart < 0) continue;

                double topY = oreDeLaStart * _inaltimeOra;
                double inaltime = (p.Service.Duration / 60.0) * _inaltimeOra;
                bool isPast = p.AppointmentDate.AddMinutes(p.Service.Duration) < DateTime.Now;

                var card = new Border { BackgroundColor = isPast ? Color.FromArgb("#F2F2F2") : Color.FromArgb("#FFF0F2"), StrokeThickness = 0, Padding = new Thickness(10, 5), StrokeShape = new RoundRectangle { CornerRadius = 10 } };
                var gridCard = new Grid { ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition(4), new ColumnDefinition(GridLength.Star) } };
                var dungaAccent = new BoxView { Color = isPast ? Color.FromArgb("#8E8E93") : Color.FromArgb("#EAB8C1"), WidthRequest = 4, CornerRadius = 2, HorizontalOptions = LayoutOptions.Start };
                Grid.SetColumn(dungaAccent, 0);

                var vStack = new VerticalStackLayout { Spacing = 2, Margin = new Thickness(8, 0, 0, 0) };
                Grid.SetColumn(vStack, 1);
                vStack.Children.Add(new Label { Text = p.ClientNameDisplay, FontSize = 16, FontAttributes = FontAttributes.Bold, TextColor = isPast ? Color.FromArgb("#555555") : Colors.Black, FontFamily = "Lora" });
                if (p.Service.Duration >= 45) vStack.Children.Add(new Label { Text = p.Service.DisplayName, FontSize = 14, TextColor = isPast ? Color.FromArgb("#888888") : Color.FromArgb("#444444"), FontFamily = "Lora" });
                vStack.Children.Add(new Label { Text = $"{p.AppointmentDate:HH:mm} - {p.AppointmentDate.AddMinutes(p.Service.Duration):HH:mm}", FontSize = 13, TextColor = isPast ? Color.FromArgb("#666666") : Color.FromArgb("#D28A99"), FontAttributes = FontAttributes.Bold, FontFamily = "Lora" });

                gridCard.Children.Add(dungaAccent); gridCard.Children.Add(vStack); card.Content = gridCard;
                AbsoluteLayout.SetLayoutBounds(card, new Rect(0, topY, 1, inaltime - 2));
                AbsoluteLayout.SetLayoutFlags(card, AbsoluteLayoutFlags.WidthProportional);
                panouSuprapus.Children.Add(card);
            }
        }
    }

    public class ModelStilistSelectabil : INotifyPropertyChanged
    {
        private bool _isSelected;
        public Stylist Stilist { get; set; }
        public string PozaPath { get; set; }
        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(nameof(IsSelected)); OnPropertyChanged(nameof(TextColor)); OnPropertyChanged(nameof(FontStyle)); }
        }
        public Color TextColor => IsSelected ? Color.FromArgb("#EAB8C1") : Color.FromArgb("#8E8E93");
        public FontAttributes FontStyle => IsSelected ? FontAttributes.Bold : FontAttributes.None;
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class ModelZiCalendar : INotifyPropertyChanged
    {
        private bool _isSelected;
        public DateTime DataCompleta { get; set; }
        public string NumeZi { get; set; }
        public string NumarZi { get; set; }
        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(nameof(IsSelected)); OnPropertyChanged(nameof(BackgroundColor)); OnPropertyChanged(nameof(TextColor)); }
        }
        public Color BackgroundColor => IsSelected ? Color.FromArgb("#EAB8C1") : Colors.Transparent;
        public Color TextColor => IsSelected ? Colors.White : Color.FromArgb("#8E8E93");
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}