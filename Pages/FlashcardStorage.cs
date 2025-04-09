using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.Maui.Storage;

namespace ThinkFast.Pages
{
    public static class FlashcardStorage
    {
        private const string FlashcardSetsKey = "FlashcardSets";
        private static readonly object _lock = new object();

        public static List<FlashcardResponse> FlashcardSets { get; private set; } = new List<FlashcardResponse>();

        public static void Initialize()
        {
            lock (_lock)
            {
                LoadFlashcards();
            }
        }

        public static void AddFlashcardSet(FlashcardResponse flashcardSet)
        {
            lock (_lock)
            {
                if (flashcardSet == null)
                    throw new ArgumentNullException(nameof(flashcardSet));

                var existingSet = FlashcardSets.FirstOrDefault(fs => fs.Title == flashcardSet.Title);

                if (existingSet != null)
                {
                    existingSet.Flashcards = flashcardSet.Flashcards;
                }
                else
                {
                    FlashcardSets.Add(flashcardSet);
                }

                SaveFlashcards();
            }
        }

        public static void RemoveFlashcardSet(string title)
        {
            lock (_lock)
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
        }

        private static void LoadFlashcards()
        {
            try
            {
                var serializedSets = Preferences.Get(FlashcardSetsKey, string.Empty);
                if (!string.IsNullOrEmpty(serializedSets))
                {
                    FlashcardSets = JsonSerializer.Deserialize<List<FlashcardResponse>>(serializedSets)
                                  ?? new List<FlashcardResponse>();
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