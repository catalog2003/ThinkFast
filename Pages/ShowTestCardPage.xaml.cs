using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
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
        public ICommand GoBackCommand { get; }

        // In constructor:
       public ShowTestCardPage(TestResponse testData)
        {
            InitializeComponent();
            _testData = testData;
            Title = _testData.Title;

            NextQuestionCommand = new Command(NavigateToNextQuestion);
     
        // In constructor:
        GoBackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
            BindingContext = this;

            DisplayCurrentQuestion();
        }
        private void DisplayCurrentQuestion()
        {
            if (_testData.Test.Count == 0) return;

            OptionsLayout.Children.Clear();
            _questionAnswered = false;
            NextButton.IsEnabled = false;
            NextButton.TextColor = Color.FromArgb("#FFFFFF");
            NextButton.BackgroundColor = Color.FromArgb("#007CFF");
            var currentQuestion = _testData.Test[_currentIndex];
            CurrentQuestionText = currentQuestion.Question;
            CounterText = $"Question {_currentIndex + 1} of {_testData.Test.Count}";
            OnPropertyChanged(nameof(CurrentQuestionText));
            OnPropertyChanged(nameof(CounterText));

            var options = ShuffleOptions(new List<string>(currentQuestion.Options));
            char optionLetter = 'A';
            foreach (var option in options)
            {
                // Create the option container
                var border = new Border
                {
                    Stroke = Colors.LightGray,
                    StrokeThickness = 1,
                    StrokeShape = new RoundRectangle { CornerRadius = 8 },
                    BackgroundColor = Colors.White,
                    Padding = 0,
                    Margin = new Thickness(0, 5)
                };

                // Create the inner layout
                var grid = new Grid
                {
                    ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Star }
            },
                    Padding = new Thickness(15, 12)
                };

                // Add letter label with right margin
                var letterLabel = new Label
                {
                    Text = $"{optionLetter}.",
                    FontSize = 16,
                    TextColor = Colors.Black,
                    VerticalOptions = LayoutOptions.Start,
                    FontAttributes = FontAttributes.Bold,
                    Margin = new Thickness(0, 0, 15, 0) // Added right margin here
                };
                Grid.SetColumn(letterLabel, 0);
                grid.Children.Add(letterLabel);

                // Add option text label
                var optionLabel = new Label
                {
                    Text = option,
                    FontSize = 16,
                    TextColor = Colors.Black,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start,
                    LineBreakMode = LineBreakMode.WordWrap
                };
                Grid.SetColumn(optionLabel, 1);
                grid.Children.Add(optionLabel);

                border.Content = grid;

                // Add tap gesture
                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += (s, e) => CheckAnswer(border, option, currentQuestion.Correct_Answer);
                border.GestureRecognizers.Add(tapGesture);

                OptionsLayout.Children.Add(border);
                optionLetter++;
            }
        }

        private void CheckAnswer(Border border, string selectedAnswer, string correctAnswer)
        {
            if (_questionAnswered) return;

            _questionAnswered = true;
            NextButton.IsEnabled = true;
            NextButton.BackgroundColor = Color.FromArgb("#007CFF");
            bool isCorrect = string.Equals(selectedAnswer, correctAnswer, StringComparison.OrdinalIgnoreCase);

            // Color definitions
            var lightGreen = Color.FromArgb("#E8F5E9");
            var lightRed = Color.FromArgb("#FFEBEE");
            var borderGreen = Color.FromArgb("#81C784");
            var borderRed = Color.FromArgb("#EF9A9A");

            // Update selected answer
            if (isCorrect)
            {
                border.BackgroundColor = lightGreen;
                border.Stroke = borderGreen;
                border.StrokeThickness = 2;
            }
            else
            {
                border.BackgroundColor = lightRed;
                border.Stroke = borderRed;
                border.StrokeThickness = 2;
            }

            // Highlight correct answer if wrong was selected
            if (!isCorrect)
            {
                foreach (var child in OptionsLayout.Children)
                {
                    if (child is Border correctBorder &&
                        correctBorder.Content is HorizontalStackLayout stack &&
                        stack.Children[1] is Label label &&
                        label.Text == correctAnswer)
                    {
                        correctBorder.BackgroundColor = lightGreen;
                        correctBorder.Stroke = borderGreen;
                        correctBorder.StrokeThickness = 2;
                        break;
                    }
                }
            }

            // Disable all options
            foreach (var child in OptionsLayout.Children)
            {
                if (child is Border optionBorder)
                {
                    optionBorder.GestureRecognizers.Clear();
                }
            }

            // Update score
            if (isCorrect)
            {
                _score++;
                ScoreLabel.Text = $"Score: {_score}";
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

      

        private void NavigateToNextQuestion()
        {
            _currentIndex++;
            NextButton.IsEnabled = false;
            NextButton.BackgroundColor = Color.FromArgb("#007CFF");
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
