using SalonBookingApp.Models;

namespace SalonBookingApp.Views;

public partial class StylistPage : ContentPage
{
	public StylistPage()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        listView.ItemsSource = await App.Database.GetStylistsAsync();

        bool esteAdmin = App.UserLogat != null && App.UserLogat.IsAdmin;

        // 3. Gestionăm butonul de adăugare (+)
        var butonPlus = ToolbarItems.FirstOrDefault(x => x.Text == "+");

        if (!esteAdmin)
        {
            // Dacă e client, scoatem butonul și blocăm selecția
            if (butonPlus != null)
            {
                ToolbarItems.Remove(butonPlus);
            }
            listView.SelectionMode = SelectionMode.None;
        }
        else
        {
            // Dacă e admin, ne asigurăm că butonul există și activăm selecția
            if (butonPlus == null)
            {
                var noulButon = new ToolbarItem { Text = "+" };
                noulButon.Clicked += OnItemAdded;
                ToolbarItems.Add(noulButon);
            }
            listView.SelectionMode = SelectionMode.Single;
        }
    }

    async void OnItemAdded(object sender, EventArgs e)
    {
        if (App.UserLogat == null || !App.UserLogat.IsAdmin) return;

        await Navigation.PushAsync(new StylistEntryPage
        {
            BindingContext = new Stylist()
        });
    }

    async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (App.UserLogat == null || !App.UserLogat.IsAdmin)
        {
            listView.SelectedItem = null; // Deselectăm rândul
            return;
        }

        if (e.CurrentSelection.FirstOrDefault() is Stylist selectedStylist)
        {
            await Navigation.PushAsync(new StylistEntryPage
            {
                BindingContext = selectedStylist
            });
        }
    }
}