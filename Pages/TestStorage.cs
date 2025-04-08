using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.Maui.Storage;

namespace ThinkFast.Pages
{
    public class TestQuestion
    {
        public string Question { get; set; }
        public string Correct_Answer { get; set; }
        public List<string> Options { get; set; }
    }

    public class TestResponse
    {
        public List<TestQuestion> Test { get; set; } = new List<TestQuestion>();
        public string Title { get; set; }
        public string ImageSource { get; set; }
    }

    public static class TestStorage
    {
        private const string TestSetsKey = "TestSets";

        public static List<TestResponse> TestSets { get; private set; } = new List<TestResponse>();

        public static void Initialize()
        {
            LoadTests();
        }

        public static void AddTestSet(TestResponse testSet)
        {
            // Check if a set with the same title already exists
            var existingSet = TestSets.FirstOrDefault(ts => ts.Title == testSet.Title);

            if (existingSet != null)
            {
                // Update existing set
                existingSet.Test = testSet.Test;
            }
            else
            {
                // Add new set
                TestSets.Add(testSet);
            }

            SaveTests();
        }

        public static void RemoveTestSet(string title)
        {
            var setToRemove = TestSets.FirstOrDefault(ts => ts.Title == title);
            if (setToRemove != null)
            {
                TestSets.Remove(setToRemove);
                SaveTests();
            }
        }

        private static void LoadTests()
        {
            try
            {
                var serializedSets = Preferences.Get(TestSetsKey, string.Empty);
                if (!string.IsNullOrEmpty(serializedSets))
                {
                    TestSets = JsonSerializer.Deserialize<List<TestResponse>>(serializedSets)
                              ?? new List<TestResponse>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading tests: {ex.Message}");
                TestSets = new List<TestResponse>();
            }
        }

        private static void SaveTests()
        {
            try
            {
                var serializedSets = JsonSerializer.Serialize(TestSets);
                Preferences.Set(TestSetsKey, serializedSets);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving tests: {ex.Message}");
            }
        }
    }
}