using SalonBookingApp.Models;
using System.Diagnostics;

namespace SalonBookingApp.Views;

public partial class StylistReviewPage : ContentPage
{
    private Stylist _stilistulCurent;

    public StylistReviewPage(Stylist stilist)
    {
        InitializeComponent();
        _stilistulCurent = stilist;
        ImageStilist.Source = _stilistulCurent.ImagePath;
        LabelNumeStilist.Text = _stilistulCurent.FullName;
        LabelSpecializare.Text = _stilistulCurent.Specialization;
        IncarcaRecenzii();
    }

    private async void IncarcaRecenzii()
    {
        try
        {
            var recenzii = await App.Database.GetReviewsForStylistAsync(_stilistulCurent.ID);
            if (recenzii != null && recenzii.Any())
            {
                ListaRecenzii.ItemsSource = recenzii.OrderByDescending(r => r.DateAdded).ToList();
                ListaRecenzii.IsVisible = true;
                LabelNoReviews.IsVisible = false;
            }
            else
            {
                ListaRecenzii.IsVisible = false;
                LabelNoReviews.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    private async void OnSubmitReviewClicked(object sender, EventArgs e)
    {
        if (PickerRating.SelectedIndex == -1 || string.IsNullOrWhiteSpace(EditorReview.Text)) return;

        try
        {
            string ratingRaw = PickerRating.SelectedItem.ToString();
            string rating = ratingRaw.Split(' ')[0];

            var reviewNou = new Review
            {
                StylistID = _stilistulCurent.ID,
                ClientID = App.UserLogat.ID,
                ClientName = $"{App.UserLogat.FirstName} {App.UserLogat.LastName}",
                RatingDisplay = rating,
                Comment = EditorReview.Text.Trim(),
                DateAdded = DateTime.Now,
                ServiceName = "Serviciu General"
            };

            await App.Database.SaveReviewAsync(reviewNou);
            EditorReview.Text = string.Empty;
            PickerRating.SelectedIndex = -1;
            IncarcaRecenzii();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    private async void OnBackClicked(object sender, EventArgs e) => await Navigation.PopAsync();
}