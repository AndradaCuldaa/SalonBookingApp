using SalonBookingApp.Models;

namespace SalonBookingApp.Views;

public partial class ServicePage : ContentPage
{
	public ServicePage()
	{
		InitializeComponent();
	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        listView.ItemsSource = await App.Database.GetServicesAsync();
        bool esteAdmin = App.UserLogat != null && App.UserLogat.IsAdmin;

        // 3. Gestionăm butonul "+" din Toolbar
        // Căutăm butonul (presupunând că în XAML i-ai pus x:Name="ToolbarAddService")
        // Dacă nu i-ai pus nume, îl găsim după poziție sau text
        var butonPlus = ToolbarItems.FirstOrDefault(x => x.Text == "+");

        if (!esteAdmin)
        {
            // Dacă nu e admin, scoatem butonul de adăugare
            if (butonPlus != null)
            {
                ToolbarItems.Remove(butonPlus);
            }
            // Dezactivăm posibilitatea de a selecta (pentru a nu edita)
            listView.SelectionMode = SelectionMode.None;
        }
        else
        {
            // Dacă e admin, ne asigurăm că butonul este acolo
            if (butonPlus == null)
            {
                var noulButon = new ToolbarItem { Text = "+" };
                // Aici era eroarea: folosim un handler de eveniment normal, fără await în declararea Command-ului
                noulButon.Clicked += OnItemAdded;
                ToolbarItems.Add(noulButon);
            }
            listView.SelectionMode = SelectionMode.Single;
        }
    }

    async void OnItemAdded(object sender, EventArgs e)
    {
        if (App.UserLogat == null || !App.UserLogat.IsAdmin) return;
        await Navigation.PushAsync(new ServiceEntryPage
        {
            BindingContext = new Service()
        });
    }

    async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (App.UserLogat == null || !App.UserLogat.IsAdmin)
        {
            // Resetăm selecția ca să nu rămână marcat rândul
            listView.SelectedItem = null;
            return;
        }
        if (e.CurrentSelection is Service selectedService)
        {
            await Navigation.PushAsync(new ServiceEntryPage
            {
                BindingContext = selectedService
            });
        }
    }
}