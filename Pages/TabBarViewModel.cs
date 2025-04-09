namespace ThinkFast.Pages;

using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;



    public class TabBarViewModel
    {
        public ICommand GoToHomeCommand { get; }
        public ICommand GoToCalendarCommand { get; }
        public ICommand GoToFiltersCommand { get; }

        public TabBarViewModel()
        {
            GoToHomeCommand = new RelayCommand(GoToHome);
            GoToCalendarCommand = new RelayCommand(GoToCalendar);
            GoToFiltersCommand = new RelayCommand(GoToFilters);
        }

        private async void GoToHome()
        {
            await Shell.Current.GoToAsync("//AiPage");
        }

        private async void GoToCalendar()
        {
            await Shell.Current.GoToAsync("//FlashcardPage");
        }

        private async void GoToFilters()
        {
            await Shell.Current.GoToAsync("//TestCardPage");
        }
    }
