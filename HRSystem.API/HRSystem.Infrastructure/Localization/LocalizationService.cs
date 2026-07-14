using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using HRSystem.Application.Localization;

namespace HRSystem.Infrastructure.Localization
{
    public class LocalizationService : ILocalizationService
    {
        private readonly IDictionary<string, IDictionary<string, string>> _messages =
            new Dictionary<string, IDictionary<string, string>>();

        public LocalizationService()
        {
            LoadMessages("en");
            LoadMessages("ar");
        }
        private void LoadMessages(string languageCode)
        {
            try
            {
                var filePath = Path.Combine(AppContext.BaseDirectory, "Localization", "Resources", $"{languageCode}.json");
                if (File.Exists(filePath))
                {
                    var json = File.ReadAllText(filePath);
                    var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                    if (dict != null)
                    {
                        _messages[languageCode] = dict;
                    }
                }
                else
                {
                    _messages[languageCode] = new Dictionary<string, string>();
                }
            }
            catch
            {
                _messages[languageCode] = new Dictionary<string, string>();
            }
        }
        public string Get(string key)
        {
            var language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            if (!_messages.ContainsKey(language))
            {
                language = "en";
            }

            return _messages[language].ContainsKey(key) ? _messages[language][key] : key;
        }
    }
}
