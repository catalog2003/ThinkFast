using Microsoft.Maui.Controls;
using System.Collections.Generic;

namespace ThinkFast.Pages
{
    public partial class FlashcardPage : ContentPage
    {
        public List<FlashcardResponse> FlashcardSets { get; set; }

        public FlashcardPage()
        {
            InitializeComponent();
            LoadFlashcardSets();
            BindingContext = this;
        }

        private void LoadFlashcardSets()
        {
            FlashcardSets = FlashcardStorage.FlashcardSets;

            if (FlashcardSets.Count == 0)
            {
                NoSetsLabel.IsVisible = true;
                FlashcardsCollectionView.IsVisible = false;
            }
            else
            {
                NoSetsLabel.IsVisible = false;
                FlashcardsCollectionView.IsVisible = true;
            }
        }

        private async void OnViewFlashcardsClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is FlashcardResponse flashcardSet)
            {
                await Navigation.PushAsync(new ShowFlashcardPage(flashcardSet));
            }
        }

        private void OnDeleteSetClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is FlashcardResponse flashcardSet)
            {
                FlashcardStorage.RemoveFlashcardSet(flashcardSet.Title);
                LoadFlashcardSets();
            }
        }
    }
}