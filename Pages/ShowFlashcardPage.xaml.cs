using Microsoft.Maui.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ThinkFast.Pages
{
    public partial class ShowFlashcardPage : ContentPage, INotifyPropertyChanged
    {
        private FlashcardResponse _flashcardData;
        private int _currentIndex = 0;
        private bool _showingAnswer = false;

        public string Title { get; private set; }
        public string CurrentText { get; private set; }
        public string CounterText { get; private set; }
        public string FlipButtonText { get; private set; } = "Show Answer";

        public ICommand FlipCardCommand { get; }
        public ICommand PreviousCardCommand { get; }
        public ICommand NextCardCommand { get; }

        public ShowFlashcardPage(FlashcardResponse flashcardData)
        {
            InitializeComponent();
            _flashcardData = flashcardData;
            Title = _flashcardData.Title;

            // Initialize commands
            FlipCardCommand = new Command(FlipCard);
            PreviousCardCommand = new Command(() => NavigateCards(-1));
            NextCardCommand = new Command(() => NavigateCards(1));

            // Initialize first card
            UpdateCardDisplay();

            BindingContext = this;
        }

        private void FlipCard()
        {
            if (_flashcardData.Flashcards.Count == 0) return;

            _showingAnswer = !_showingAnswer;
            UpdateCardDisplay();
        }

        private void NavigateCards(int direction)
        {
            if (_flashcardData.Flashcards.Count == 0) return;

            _currentIndex = (_currentIndex + direction + _flashcardData.Flashcards.Count) % _flashcardData.Flashcards.Count;
            _showingAnswer = false;
            UpdateCardDisplay();
        }

        private void UpdateCardDisplay()
        {
            if (_flashcardData.Flashcards.Count == 0)
            {
                CurrentText = "No flashcards available";
                CounterText = string.Empty;
                FlipButtonText = string.Empty;
            }
            else
            {
                CurrentText = _showingAnswer
                    ? _flashcardData.Flashcards[_currentIndex].Answer
                    : _flashcardData.Flashcards[_currentIndex].Question;

                CounterText = $"Card {_currentIndex + 1} of {_flashcardData.Flashcards.Count}";
                FlipButtonText = _showingAnswer ? "Show Question" : "Show Answer";
            }

            OnPropertyChanged(nameof(CurrentText));
            OnPropertyChanged(nameof(CounterText));
            OnPropertyChanged(nameof(FlipButtonText));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}