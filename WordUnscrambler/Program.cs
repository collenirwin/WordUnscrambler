using ConsoleUtils;
using Newtonsoft.Json;
using RegularExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WordUnscrambler
{
    static class Program
    {
        private static bool _initialized = false;
        private static readonly StandardConsole _console = new StandardConsole();
        private static HashSet<string> _words;
        private static Settings _settings;

        private static void Main()
        {
            if (!_initialized)
            {
                _console.Write("Loading... ");

                string settingsFileName = "settings.json";
                if (!File.Exists(settingsFileName))
                {
                    _settings = new Settings();
                    File.WriteAllText(settingsFileName, JsonConvert.SerializeObject(_settings));
                }
                else
                {
                    _settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(settingsFileName));
                }

                _words = File.ReadAllText("words.txt")
                    .RegexSplit("\r?\n")
                    .Where(word => word.Length >= _settings.MinimumWordLength)
                    .Select(word => word.ToLower())
                    .ToHashSet();

                _console.WriteLine("Done!");
                _initialized = true;
            }

            string letters = _console
                .PromptUntil("Enter letters: ", entry => entry.Any())
                .ToLower();

            var matchingWords = _words
                .Where(word => word.CanBeMadeFrom(letters))
                .GroupBy(word => word.Length)
                .OrderByDescending(word => word.Key);

            string indent = new string(' ', _settings.OutputIndentWidth);
            foreach (var group in matchingWords)
            {
                _console.WriteLine($"{group.Key} letter words:");

                using var innerScope = new ColorScope(ConsoleColor.Cyan, Console.BackgroundColor);
                foreach (var word in group.OrderBy(word => word))
                {
                    _console.WriteLine($"{indent}{(_settings.OutputIsLowerCase ? word : word.ToUpper())}");
                }
            }

            if (!matchingWords.Any())
            {
                using var scope = new ColorScope(ConsoleColor.Red, Console.BackgroundColor);
                _console.WriteLine("No matching words found.");
            }

            Main();
        }

        private static bool CanBeMadeFrom(this string word, string letters)
        {
            foreach (var index in word.Select(letter => letters.IndexOf(letter)))
            {
                if (index == -1)
                {
                    return false;
                }

                letters = letters.Remove(index, 1);
            }

            return true;
        }
    }
}
