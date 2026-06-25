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

        
        var butonPlus = ToolbarItems.FirstOrDefault(x => x.Text == "+");

        if (!esteAdmin)
        {
           
            if (butonPlus != null)
            {
                ToolbarItems.Remove(butonPlus);
            }
            
            listView.SelectionMode = SelectionMode.None;
        }
        else
        {
            
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
        await Navigation.PushAsync(new ServiceEntryPage
        {
            BindingContext = new Service()
        });
    }

    async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (App.UserLogat == null || !App.UserLogat.IsAdmin)
        {
            
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