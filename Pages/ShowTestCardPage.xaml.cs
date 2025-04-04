using Microsoft.Maui;
using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ThinkFast.Pages
{
    public partial class ShowTestCardPage : ContentPage, INotifyPropertyChanged
    {
        private TestResponse _testData;
        private int _currentIndex = 0;
        private int _score = 0;
        private bool _questionAnswered = false;
        private Random _random = new Random();

        public string Title { get; private set; }
        public string CounterText { get; private set; }
        public string CurrentQuestionText { get; private set; }
        public ICommand NextQuestionCommand { get; }

        public ShowTestCardPage(TestResponse testData)
        {
            InitializeComponent();
            _testData = testData;
            Title = _testData.Title;

            NextQuestionCommand = new Command(NavigateToNextQuestion);
            BindingContext = this;

            DisplayCurrentQuestion();
        }

        private void DisplayCurrentQuestion()
        {
            if (_testData.Test.Count == 0) return;

            // Clear previous options
            OptionsLayout.Children.Clear();
            _questionAnswered = false;
            NextButton.IsEnabled = false;

            // Update current question info
            var currentQuestion = _testData.Test[_currentIndex];
            CurrentQuestionText = currentQuestion.Question;
            CounterText = $"Question {_currentIndex + 1} of {_testData.Test.Count}";
            OnPropertyChanged(nameof(CurrentQuestionText));
            OnPropertyChanged(nameof(CounterText));

            // Shuffle and add options
            var options = ShuffleOptions(new List<string>(currentQuestion.Options));
            foreach (var option in options)
            {
                var optionButton = new Button
                {
                    Text = option,
                    HorizontalOptions = LayoutOptions.Fill,
                    Margin = new Thickness(0, 5)
                };

                optionButton.Clicked += (sender, e) => CheckAnswer(sender as Button, option, currentQuestion.Correct_Answer);
                OptionsLayout.Children.Add(optionButton);
            }
        }

        private List<string> ShuffleOptions(List<string> options)
        {
            int n = options.Count;
            while (n > 1)
            {
                n--;
                int k = _random.Next(n + 1);
                string value = options[k];
                options[k] = options[n];
                options[n] = value;
            }
            return options;
        }

        private void CheckAnswer(Button button, string selectedAnswer, string correctAnswer)
        {
            if (_questionAnswered) return;

            _questionAnswered = true;
            NextButton.IsEnabled = true;

            bool isCorrect = string.Equals(selectedAnswer, correctAnswer, StringComparison.OrdinalIgnoreCase);

            if (isCorrect)
            {
                button.BackgroundColor = Colors.Green;
                button.TextColor = Colors.White;
                _score++;
                ScoreLabel.Text = $"Score: {_score}";
            }
            else
            {
                button.BackgroundColor = Colors.Red;
                button.TextColor = Colors.White;

                foreach (var child in OptionsLayout.Children)
                {
                    if (child is Button optionButton &&
                        string.Equals(optionButton.Text, correctAnswer, StringComparison.OrdinalIgnoreCase))
                    {
                        optionButton.BackgroundColor = Colors.Green;
                        optionButton.TextColor = Colors.White;
                        break;
                    }
                }
            }

            foreach (var child in OptionsLayout.Children)
            {
                if (child is Button optionButton)
                {
                    optionButton.IsEnabled = false;
                }
            }
        }

        private void NavigateToNextQuestion()
        {
            _currentIndex++;

            if (_currentIndex >= _testData.Test.Count)
            {
                ShowTestResults();
            }
            else
            {
                DisplayCurrentQuestion();
            }
        }

        private void ShowTestResults()
        {
            double percentage = ((double)_score / _testData.Test.Count) * 100;

            var mainLayout = new VerticalStackLayout
            {
                Margin = new Thickness(20),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Spacing = 20
            };

            mainLayout.Children.Add(new Label
            {
                Text = "Test Complete!",
                FontSize = 24,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center
            });

            mainLayout.Children.Add(new Label
            {
                Text = $"Your Score: {_score} / {_testData.Test.Count}",
                FontSize = 20,
                HorizontalOptions = LayoutOptions.Center
            });

            mainLayout.Children.Add(new Label
            {
                Text = $"{percentage:F1}%",
                FontSize = 36,
                HorizontalOptions = LayoutOptions.Center,
                TextColor = percentage >= 70 ? Colors.Green : (percentage >= 50 ? Colors.Orange : Colors.Red)
            });

            var returnButton = new Button
            {
                Text = "Return to Test List",
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 20, 0, 0)
            };

            returnButton.Clicked += async (sender, e) =>
            {
                await Navigation.PopAsync();
            };

            mainLayout.Children.Add(returnButton);

            Content = mainLayout;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}