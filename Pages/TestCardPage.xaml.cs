using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Windows.Input;

namespace ThinkFast.Pages
{
    public partial class TestCardPage : ContentPage
    {
        public List<TestResponse> TestSets { get; private set; }

        public ICommand StartTestCommand { get; }
        public ICommand DeleteSetCommand { get; }
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

            StartTestCommand = new Command<TestResponse>(async (testSet) =>
                await Navigation.PushAsync(new ShowTestCardPage(testSet)));

            DeleteSetCommand = new Command<string>((title) =>
            {
                TestStorage.RemoveTestSet(title);
                LoadTestSets();
            });

            LoadTestSets();
            BindingContext = this;
        }

        private void LoadTestSets()
        {
            TestSets = TestStorage.TestSets;
            for (int i = 0; i < TestSets.Count; i++)
            {
                TestSets[i].ImageSource = CardImages[i % CardImages.Length];
            }
            if (TestSets.Count == 0)
            {
                NoSetsLabel.IsVisible = true;
                TestSetsCollectionView.IsVisible = false;
            }
            else
            {
                NoSetsLabel.IsVisible = false;
                TestSetsCollectionView.IsVisible = true;
            }

            OnPropertyChanged(nameof(TestSets));
        }
    }
}