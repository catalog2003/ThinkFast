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
        public List<string> Options { get; set; } = new List<string>();
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
        private static readonly object _lock = new object();

        public static List<TestResponse> TestSets { get; private set; } = new List<TestResponse>();

        public static void Initialize()
        {
            lock (_lock)
            {
                LoadTests();
            }
        }

        public static void AddTestSet(TestResponse testSet)
        {
            lock (_lock)
            {
                var existingSet = TestSets.FirstOrDefault(ts => ts.Title == testSet.Title);
                if (existingSet != null)
                {
                    existingSet.Test = testSet.Test;
                }
                else
                {
                    TestSets.Add(testSet);
                }
                SaveTests();
            }
        }

        public static void RemoveTestSet(string title)
        {
            lock (_lock)
            {
                var setToRemove = TestSets.FirstOrDefault(ts => ts.Title == title);
                if (setToRemove != null)
                {
                    TestSets.Remove(setToRemove);
                    SaveTests();
                }
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
                else
                {
                    TestSets = new List<TestResponse>();
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