using SalonBookingApp.Models;
using System.Globalization;
using Microsoft.Maui.Controls.Shapes;

namespace SalonBookingApp.Views;

public partial class StylistWorkSchedulePage : ContentPage
{
    private DateTime _luniCurent;
    private List<WorkDaySchedule> _orarSaptamanal;

    public StylistWorkSchedulePage()
    {
        InitializeComponent();
        _luniCurent = GetStartOfWeek(DateTime.Today);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await IncarcaSaptamanaDinDbAsync();
    }

    private DateTime GetStartOfWeek(DateTime dt)
    {
        int diff = (7 + (dt.DayOfWeek - DayOfWeek.Monday)) % 7;
        return dt.AddDays(-1 * diff).Date;
    }

    private async Task IncarcaSaptamanaDinDbAsync()
    {
        DateTime sambata = _luniCurent.AddDays(5);
        LabelSaptamana.Text = $"{_luniCurent:dd MMM} - {sambata:dd MMM}";

        _orarSaptamanal = new List<WorkDaySchedule>();

        for (int i = 0; i < 6; i++)
        {
            DateTime dataZi = _luniCurent.AddDays(i).Date;
            var zi = new WorkDaySchedule { Date = dataZi };
            zi.IsOff = VerificaDacaEFree(dataZi);

            if (App.StilistLogat != null)
            {
                var dbSchedule = await App.Database.GetWorkScheduleAsync(App.StilistLogat.ID, dataZi);
                if (dbSchedule != null)
                {
                    zi.DbId = dbSchedule.Id;
                    zi.IsOff = dbSchedule.IsOff;
                    if (dbSchedule.StartTime1.HasValue) zi.StartTime1 = dbSchedule.StartTime1.Value;
                    if (dbSchedule.EndTime1.HasValue) zi.EndTime1 = dbSchedule.EndTime1.Value;
                    if (dbSchedule.StartTime2.HasValue) zi.StartTime2 = dbSchedule.StartTime2.Value;
                    if (dbSchedule.EndTime2.HasValue) zi.EndTime2 = dbSchedule.EndTime2.Value;
                }
            }
            _orarSaptamanal.Add(zi);
        }
        RandareCartonase();
    }

    private bool VerificaDacaEFree(DateTime data)
    {
        if (App.StilistLogat == null) return false;
        return App.StilistLogat.FirstName switch
        {
            "Ana" => data.DayOfWeek == DayOfWeek.Monday,
            "Elena" => data.DayOfWeek == DayOfWeek.Tuesday,
            "Simona" => data.DayOfWeek == DayOfWeek.Wednesday,
            "Andrada" => data.DayOfWeek == DayOfWeek.Thursday,
            _ => false
        };
    }

    private void RandareCartonase()
    {
        ContainerZile.Children.Clear();
        foreach (var zi in _orarSaptamanal)
        {
            var border = new Border { BackgroundColor = Color.FromRgba(255, 255, 255, 220), Padding = 15, StrokeThickness = 0, StrokeShape = new RoundRectangle { CornerRadius = 15 } };
            var grid = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) } };

            var infoStanga = new VerticalStackLayout { VerticalOptions = LayoutOptions.Center };
            infoStanga.Children.Add(new Label { Text = zi.DayName, FontAttributes = FontAttributes.Bold, FontSize = 16, TextColor = Colors.Black });
            var lblOre = new Label { Text = zi.TotalHours, FontSize = 13, TextColor = Colors.Gray };
            infoStanga.Children.Add(lblOre);

            var switchLiber = new Switch { IsToggled = zi.IsOff, OnColor = Color.FromArgb("#EAB8C1") };
            switchLiber.Toggled += (s, e) => { zi.IsOff = e.Value; RandareCartonase(); };
            var stackLiber = new HorizontalStackLayout { Spacing = 5, Children = { switchLiber, new Label { Text = "Zi liberă", VerticalOptions = LayoutOptions.Center } } };
            infoStanga.Children.Add(stackLiber);

            var infoDreapta = new VerticalStackLayout { HorizontalOptions = LayoutOptions.End };
            if (!zi.IsOff)
            {
                infoDreapta.Children.Add(CreateTimeRange((TimeSpan)zi.StartTime1, (TimeSpan)zi.EndTime1, t => { zi.StartTime1 = t; lblOre.Text = zi.TotalHours; }, t => { zi.EndTime1 = t; lblOre.Text = zi.TotalHours; }));
                infoDreapta.Children.Add(CreateTimeRange((TimeSpan)zi.StartTime2, (TimeSpan)zi.EndTime2, t => { zi.StartTime2 = t; lblOre.Text = zi.TotalHours; }, t => { zi.EndTime2 = t; lblOre.Text = zi.TotalHours; }));
            }
            else
            {
                infoDreapta.Children.Add(new Label { Text = "Liber", TextColor = Color.FromArgb("#EAB8C1"), FontAttributes = FontAttributes.Bold, FontSize = 16 });
            }

            grid.Children.Add(infoStanga);
            Grid.SetColumn(infoDreapta, 1);
            grid.Children.Add(infoDreapta);
            border.Content = grid;
            ContainerZile.Children.Add(border);
        }
    }

    private View CreateTimeRange(TimeSpan start, TimeSpan end, Action<TimeSpan> setStart, Action<TimeSpan> setEnd)
    {
        var stack = new HorizontalStackLayout { Spacing = 5 };
        var tpStart = new TimePicker { Time = start, Format = "HH:mm" };
        var tpEnd = new TimePicker { Time = end, Format = "HH:mm" };

        tpStart.PropertyChanged += (s, e) => { if (e.PropertyName == "Time") setStart((TimeSpan)tpStart.Time); };
        tpEnd.PropertyChanged += (s, e) => { if (e.PropertyName == "Time") setEnd((TimeSpan)tpEnd.Time); };

        stack.Children.Add(tpStart);
        stack.Children.Add(new Label { Text = "-", VerticalOptions = LayoutOptions.Center });
        stack.Children.Add(tpEnd);
        return stack;
    }

    private async void OnSaveScheduleClicked(object sender, EventArgs e)
    {
        if (App.StilistLogat == null) return;
        foreach (var zi in _orarSaptamanal)
        {
            await App.Database.SaveWorkScheduleAsync(new WorkScheduleDb
            {
                Id = zi.DbId ?? 0,
                StylistId = App.StilistLogat.ID,
                ScheduleDate = zi.Date,
                IsOff = zi.IsOff,
                StartTime1 = zi.IsOff ? null : (TimeSpan?)zi.StartTime1,
                EndTime1 = zi.IsOff ? null : (TimeSpan?)zi.EndTime1,
                StartTime2 = zi.IsOff ? null : (TimeSpan?)zi.StartTime2,
                EndTime2 = zi.IsOff ? null : (TimeSpan?)zi.EndTime2
            });
        }
        await Navigation.PopAsync();
    }

    private async void OnPrevWeekClicked(object sender, EventArgs e) { _luniCurent = _luniCurent.AddDays(-7); await IncarcaSaptamanaDinDbAsync(); }
    private async void OnNextWeekClicked(object sender, EventArgs e) { _luniCurent = _luniCurent.AddDays(7); await IncarcaSaptamanaDinDbAsync(); }
}