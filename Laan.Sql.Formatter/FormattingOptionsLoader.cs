using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Laan.Sql.Formatter
{
    /// <summary>
    /// Loads formatting options from .sqlformat.json files
    /// </summary>
    public static class FormattingOptionsLoader
    {
        private const string ConfigFileName = ".sqlformat.json";

        /// <summary>
        /// Loads options from a specific file path
        /// </summary>
        public static FormattingOptions LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Configuration file not found: {filePath}");

            var json = File.ReadAllText(filePath);

#if NET6_0_OR_GREATER
            var jsonOptions = new JsonSerializerOptions(FormattingOptionsJsonContext.Default.Options)
            {
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };
            jsonOptions.Converters.Add(new JsonStringEnumConverter());

            var options = JsonSerializer.Deserialize(json, typeof(FormattingOptions), jsonOptions) as FormattingOptions;
#else
            var jsonOptions = new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            jsonOptions.Converters.Add(new JsonStringEnumConverter());
            var options = JsonSerializer.Deserialize(json, typeof(FormattingOptions), jsonOptions) as FormattingOptions;
#endif

            if (options == null)
                throw new InvalidOperationException($"Failed to parse configuration file: {filePath}");

            options.Validate();
            return options;
        }

        /// <summary>
        /// Searches for .sqlformat.json in a hierarchy: startDirectory -> workspace root -> user home
        /// </summary>
        public static FormattingOptions LoadFromHierarchy(string startDirectory = null)
        {
            startDirectory = startDirectory ?? Directory.GetCurrentDirectory();

            // Search upward from start directory
            var configPath = FindConfigFileUpward(startDirectory);
            if (configPath != null)
                return LoadFromFile(configPath);

            // Try user home directory
            var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var homeConfigPath = Path.Combine(homeDir, ConfigFileName);
            if (File.Exists(homeConfigPath))
                return LoadFromFile(homeConfigPath);

            // Return defaults if no config found
            return new FormattingOptions();
        }

        /// <summary>
        /// Tries to load options from hierarchy, returns default options if not found
        /// </summary>
        public static FormattingOptions TryLoadFromHierarchy(string startDirectory = null)
        {
            try
            {
                return LoadFromHierarchy(startDirectory);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading formatting options: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Searches upward through directory hierarchy for config file
        /// </summary>
        private static string FindConfigFileUpward(string startDirectory)
        {
            var directory = new DirectoryInfo(startDirectory);

            while (directory != null)
            {
                var configPath = Path.Combine(directory.FullName, ConfigFileName);
                if (File.Exists(configPath))
                    return configPath;

                directory = directory.Parent;
            }

            return null;
        }

        /// <summary>
        /// Saves options to a file
        /// </summary>
        public static void SaveToFile(FormattingOptions options, string filePath)
        {
            options.Validate();
#if NET6_0_OR_GREATER
            var json = JsonSerializer.Serialize(options, typeof(FormattingOptions), FormattingOptionsJsonContext.Default);
#else
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
            jsonOptions.Converters.Add(new JsonStringEnumConverter());
            var json = JsonSerializer.Serialize(options, typeof(FormattingOptions), jsonOptions);
#endif

            File.WriteAllText(filePath, json);
        }
    }
}
