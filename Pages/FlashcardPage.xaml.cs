using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace ThinkFast.Pages
{
    public partial class FlashcardPage : ContentPage, INotifyPropertyChanged
    {
        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        private static readonly string[] CardImages = {
            "i1.png",
            "i2.png",
            "i3.png",
            "i4.png",
            "i5.png"
        };

        private List<FlashcardResponse> _flashcardSets;
        public List<FlashcardResponse> FlashcardSets
        {
            get => _flashcardSets;
            set
            {
                _flashcardSets = value;
                OnPropertyChanged(nameof(FlashcardSets));
            }
        }

        public ICommand ViewFlashcardsCommand { get; }
        public ICommand DeleteSetCommand { get; }
        public ICommand RefreshCommand { get; }

        public FlashcardPage()
        {
            InitializeComponent();

            ViewFlashcardsCommand = new Command<FlashcardResponse>(async (flashcardSet) =>
            {
                await Navigation.PushAsync(new ShowFlashcardPage(flashcardSet));
            });

            DeleteSetCommand = new Command<string>(async (title) =>
            {
                bool answer = await DisplayAlert("Confirm Delete", $"Are you sure you want to delete '{title}'?", "Yes", "No");
                if (answer)
                {
                    FlashcardStorage.RemoveFlashcardSet(title);
                    LoadFlashcardSets();
                }
            });

            RefreshCommand = new Command(() =>
            {
                IsRefreshing = true;
                LoadFlashcardSets();
                IsRefreshing = false;
            });

            LoadFlashcardSets();
            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadFlashcardSets();
        }

        private void LoadFlashcardSets()
        {
            FlashcardStorage.Initialize();
            FlashcardSets = new List<FlashcardResponse>(FlashcardStorage.FlashcardSets);

            for (int i = 0; i < FlashcardSets.Count; i++)
            {
                FlashcardSets[i].ImageSource = CardImages[i % CardImages.Length];
            }

            NoSetsLabel.IsVisible = FlashcardSets.Count == 0;
            FlashcardsCollectionView.IsVisible = FlashcardSets.Count > 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}