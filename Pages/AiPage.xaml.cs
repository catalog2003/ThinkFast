using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System;

using System.Net.Http;

using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Collections.Generic;

namespace ThinkFast.Pages
{
    public partial class AiPage : ContentPage
    {
        // ObservableCollection to store chat messages
        public ObservableCollection<ChatMessage> ChatHistory { get; set; } = new ObservableCollection<ChatMessage>();
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string ApiUrl = "https://aiedun.azurewebsites.net/api/geminichatbot";


        public AiPage()
        {
            InitializeComponent();

            // Set the BindingContext to this page
            BindingContext = this;

            // Scroll to the latest message when collection changes
            ChatHistory.CollectionChanged += (s, e) => ScrollToEnd();
            FlashcardStorage.Initialize();
            TestStorage.Initialize();
        }

        // Event handler for the Send button
        private async void OnSendButtonClicked(object sender, EventArgs e)
        {
            string userMessage = UserInput.Text;
            if (string.IsNullOrWhiteSpace(userMessage))
                return;

            ContentScrollView.IsVisible = false;
            ChatLayout.IsVisible = true;
            ChatHistory.Add(new ChatMessage { Text = userMessage, IsUserMessage = true });
            UserInput.Text = string.Empty;

            if (userMessage.Contains("flashcard", StringComparison.OrdinalIgnoreCase))
            {
                var response = await GenerateFlashcards(userMessage);
                FlashcardStorage.AddFlashcardSet(response);
                ChatHistory.Add(new ChatMessage { Text = $"Generated {response.Flashcards.Count} flashcards on {response.Title}", IsUserMessage = false });
               
            }
            else if (userMessage.Contains("test", StringComparison.OrdinalIgnoreCase))
            {
                var response = await GenerateTest(userMessage);
                TestStorage.AddTestSet(response);
                ChatHistory.Add(new ChatMessage { Text = $"Generated a test with {response.Test.Count} questions on {response.Title}", IsUserMessage = false });
               
            }
            else
            {
                string botResponse = await SendMessageToBot(userMessage);
                ChatHistory.Add(new ChatMessage { Text = botResponse, IsUserMessage = false });
            }
        }

        // Method to send the user's message to the Azure Function API
        private async Task<string> SendMessageToBot(string message)
        {
            var requestBody = new { message };
            string json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(ApiUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    string responseJson = await response.Content.ReadAsStringAsync();
                    var responseData = JsonSerializer.Deserialize<Dictionary<string, string>>(responseJson);
                    return responseData["response"];
                }
                else
                {
                    return "Error: Unable to get a response from the bot.";
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        // Method to scroll the chat history to the latest message
        private void ScrollToEnd()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (ChatHistoryView.ItemsSource != null && ChatHistory.Count > 0)
                {
                    ChatHistoryView.ScrollTo(ChatHistory[ChatHistory.Count - 1], position: ScrollToPosition.End, animate: true);
                }
            });
        }

        // Event handler for label tap
        private async void OnLabelTapped(object sender, EventArgs e)
        {
            var label = sender as Label;
            if (label != null)
            {
                // Hide the content and show the chat interface
                ContentScrollView.IsVisible = false;
                ChatLayout.IsVisible = true;

                // Submit the label's text to the chat
                string message = label.Text;
                ChatHistory.Add(new ChatMessage { Text = message, IsUserMessage = true });

                // Send message to the bot and get the response
                string botResponse = await SendMessageToBot(message);

                // Add bot's response to chat history
                ChatHistory.Add(new ChatMessage { Text = botResponse, IsUserMessage = false });
            }
        }

        private async Task<FlashcardResponse> GenerateFlashcards(string topic)
        {
            var requestData = new { message = $"generate flashcards on {topic}" };
            var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(ApiUrl, content);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<FlashcardResponse>(jsonResponse, options);
        }

        private async Task<TestResponse> GenerateTest(string topic)
        {
            var requestData = new { message = $"generate a test on {topic}" };
            var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(ApiUrl, content);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<TestResponse>(jsonResponse, options);
        }
    }

    // Model for chat messages

    public class ChatMessage
    {
        public string Text { get; set; }
        public bool IsUserMessage { get; set; }

        public FormattedString FormattedText
        {
            get
            {
                var formatted = MarkdownFormatter.Parse(Text);

                // Set color for all spans based on message type
                Color textColor = IsUserMessage ? Colors.White : Colors.Black;
                foreach (var span in formatted.Spans)
                {
                    span.TextColor = textColor;
                }

                return formatted;
            }
        }
    }


    // MessageColorConverter implementation
    public class MessageColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isUserMessage = (bool)value;
            return isUserMessage ? Color.FromArgb("#007AFF") : Color.FromArgb("#F2F2F7");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // MessageAlignmentConverter implementation
    public class MessageAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isUserMessage = (bool)value;
            return isUserMessage ? LayoutOptions.End : LayoutOptions.Start;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // MessageTextColorConverter implementation
    public class MessageTextColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isUserMessage = (bool)value;
            return isUserMessage ? Colors.White : Colors.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}