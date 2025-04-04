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