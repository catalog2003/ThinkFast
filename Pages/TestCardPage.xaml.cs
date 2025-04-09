using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace ThinkFast.Pages
{
    public partial class TestCardPage : ContentPage, INotifyPropertyChanged
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

        public List<TestResponse> TestSets { get; private set; } = new List<TestResponse>();

        public ICommand StartTestCommand { get; }
        public ICommand DeleteSetCommand { get; }
        public ICommand RefreshCommand { get; }

        private static readonly string[] CardImages = {
            "i6.png",
            "i7.png",
            "i8.png",
            "i9.png",
            "i10.png"
        };

        public TestCardPage()
        {
            InitializeComponent();

            // Initialize commands
            StartTestCommand = new Command<TestResponse>(async (testSet) =>
                await Navigation.PushAsync(new ShowTestCardPage(testSet)));

            DeleteSetCommand = new Command<string>(async (title) =>
            {
                bool answer = await DisplayAlert("Confirm", $"Delete {title}?", "Yes", "No");
                if (answer)
                {
                    TestStorage.RemoveTestSet(title);
                    LoadTestSets();
                }
            });

            RefreshCommand = new Command(() =>
            {
                IsRefreshing = true;
                LoadTestSets();
                IsRefreshing = false;
            });

            // Load initial data
            LoadTestSets();
            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadTestSets(); // Refresh when page appears
        }

        private void LoadTestSets()
        {
            TestStorage.Initialize(); // Ensure data is loaded
            TestSets = new List<TestResponse>(TestStorage.TestSets); // Create new list for binding

            // Assign images
            for (int i = 0; i < TestSets.Count; i++)
            {
                TestSets[i].ImageSource = CardImages[i % CardImages.Length];
            }

            // Update UI
            NoSetsLabel.IsVisible = TestSets.Count == 0;
            TestSetsCollectionView.IsVisible = TestSets.Count > 0;

            OnPropertyChanged(nameof(TestSets));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}