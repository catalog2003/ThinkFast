using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.Maui.Storage;

namespace ThinkFast.Pages
{
    // Static class to store flashcard sets across the application
    public static class FlashcardStorage
    {
        private const string FlashcardSetsKey = "FlashcardSets";

        public static List<FlashcardResponse> FlashcardSets { get; private set; } = new List<FlashcardResponse>();

        public static void Initialize()
        {
            LoadFlashcards();
        }

        public static void AddFlashcardSet(FlashcardResponse flashcardSet)
        {
            if (flashcardSet == null)
                throw new ArgumentNullException(nameof(flashcardSet));

            // Check if a set with the same title already exists
            var existingSet = FlashcardSets.FirstOrDefault(fs => fs.Title == flashcardSet.Title);

            if (existingSet != null)
            {
                // Update existing set
                existingSet.Flashcards = flashcardSet.Flashcards;
            }
            else
            {
                // Add new set
                FlashcardSets.Add(flashcardSet);
            }

            SaveFlashcards();
        }

        public static void RemoveFlashcardSet(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return;

            var setToRemove = FlashcardSets.FirstOrDefault(fs => fs.Title == title);
            if (setToRemove != null)
            {
                FlashcardSets.Remove(setToRemove);
                SaveFlashcards();
            }
        }

        private static void LoadFlashcards()
        {
            try
            {
                var serializedSets = Preferences.Get(FlashcardSetsKey, string.Empty);
                if (!string.IsNullOrEmpty(serializedSets))
                {
                    var sets = JsonSerializer.Deserialize<List<FlashcardResponse>>(serializedSets);
                    FlashcardSets = sets ?? new List<FlashcardResponse>();
                }
                else
                {
                    FlashcardSets = new List<FlashcardResponse>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading flashcards: {ex.Message}");
                FlashcardSets = new List<FlashcardResponse>();
            }
        }

        private static void SaveFlashcards()
        {
            try
            {
                var serializedSets = JsonSerializer.Serialize(FlashcardSets);
                Preferences.Set(FlashcardSetsKey, serializedSets);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving flashcards: {ex.Message}");
            }
        }
    }
}