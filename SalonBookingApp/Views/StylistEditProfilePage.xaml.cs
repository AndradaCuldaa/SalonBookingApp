using SalonBookingApp.Models;

namespace SalonBookingApp.Views;

public partial class StylistEditProfilePage : ContentPage
{
    public Stylist StilistulMeu { get; set; }

    public StylistEditProfilePage()
    {
        InitializeComponent();

        if (App.StilistLogat != null)
        {
            StilistulMeu = new Stylist
            {
                ID = App.StilistLogat.ID,
                FirstName = App.StilistLogat.FirstName,
                LastName = App.StilistLogat.LastName,
                Specialization = App.StilistLogat.Specialization,
                Username = App.StilistLogat.Username,
                Password = App.StilistLogat.Password
            };

            ImageProfilePic.Source = $"{StilistulMeu.FirstName.ToLower()}.png";
        }

        BindingContext = StilistulMeu;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(StilistulMeu.FirstName) ||
            string.IsNullOrWhiteSpace(StilistulMeu.LastName) ||
            string.IsNullOrWhiteSpace(StilistulMeu.Username))
        {
            return;
        }

        try
        {
            await App.Database.UpdateStylistAsync(StilistulMeu);

            App.StilistLogat.FirstName = StilistulMeu.FirstName;
            App.StilistLogat.LastName = StilistulMeu.LastName;
            App.StilistLogat.Specialization = StilistulMeu.Specialization;
            App.StilistLogat.Username = StilistulMeu.Username;
            App.StilistLogat.Password = StilistulMeu.Password;

            await Navigation.PopAsync();
        }
        catch (Exception)
        {
        }
    }

    private async void OnChangePhotoClicked(object sender, EventArgs e)
    {
        ContextMenuOverlay.IsVisible = true;
        await ContextMenuFrame.ScaleTo(1, 250, Easing.SpringOut);
    }

    private async void OnCancelClicked(object sender, TappedEventArgs e)
    {
        await ContextMenuFrame.ScaleTo(0, 200, Easing.CubicIn);
        ContextMenuOverlay.IsVisible = false;
    }

    private async void OnOptionSelected(object sender, TappedEventArgs e)
    {
        await ContextMenuFrame.ScaleTo(0, 200, Easing.CubicIn);
        ContextMenuOverlay.IsVisible = false;

        if (sender is Grid grid && grid.GestureRecognizers.FirstOrDefault() is TapGestureRecognizer tap && tap.CommandParameter is string option)
        {
            try
            {
                if (option == "Library")
                {
                    var photo = await MediaPicker.Default.PickPhotoAsync();
                    if (photo != null)
                    {
                        ImageProfilePic.Source = ImageSource.FromFile(photo.FullPath);
                    }
                }
                else if (option == "Camera")
                {
                    if (MediaPicker.Default.IsCaptureSupported)
                    {
                        var photo = await MediaPicker.Default.CapturePhotoAsync();
                        if (photo != null)
                        {
                            ImageProfilePic.Source = ImageSource.FromFile(photo.FullPath);
                        }
                    }
                }
                else if (option == "Files")
                {
                    var result = await FilePicker.Default.PickAsync(new PickOptions
                    {
                        PickerTitle = "Selectează o poză",
                        FileTypes = FilePickerFileType.Images
                    });

                    if (result != null)
                    {
                        ImageProfilePic.Source = ImageSource.FromFile(result.FullPath);
                    }
                }
                else if (option == "Remove")
                {
                    ImageProfilePic.Source = "profile_placeholder.png";
                }
            }
            catch (Exception)
            {
            }
        }
    }
}