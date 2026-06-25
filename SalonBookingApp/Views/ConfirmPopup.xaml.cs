using CommunityToolkit.Maui.Views;
using SalonBookingApp.Resources.Strings;

namespace SalonBookingApp.Views;

public partial class ConfirmPopup : Popup
{
    public bool IsConfirmed { get; private set; } = false;

    public ConfirmPopup(string title, string message)
    {
        InitializeComponent();

        LabelTitle.Text = title;
        LabelMessage.Text = message;
        BtnYes.Text = AppResources.YesBtn;
        BtnNo.Text = AppResources.NoBtn;
    }

    private async void OnYesClicked(object sender, EventArgs e)
    {
        IsConfirmed = true; 
        await CloseAsync();
    }

    private async void OnNoClicked(object sender, EventArgs e)
    {
        IsConfirmed = false; 
        await CloseAsync();
    }
}