namespace SalonBookingApp.Views;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();
	}
    private async void OnSignUpClicked(object sender, EventArgs e)
    {
       
         await Navigation.PushAsync(new RegisterPage());

       
    }


    private async void OnLoginClicked(object sender, EventArgs e)
    {
        
        await Navigation.PushAsync(new LoginPage());

        
    }
}