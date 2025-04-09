using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.Maui.Storage;

namespace ThinkFast.Pages
{
    public class FlashcardResponse
    {
        public List<Flashcard> Flashcards { get; set; } = new List<Flashcard>();
        public string Title { get; set; } = string.Empty;
        public string ImageSource { get; set; }
    }

    public class Flashcard
    {
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
    }
}