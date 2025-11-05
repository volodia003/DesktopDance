using System.Text.Json;

namespace DesktopKonata.Utility
{
    // Класс для хранения данных о персонаже на экране
    public class CharacterData
    {
        public string Name { get; set; } = "";
        public string FilePath { get; set; } = ""; // Путь к GIF файлу (пустой для встроенных)
        public float Scale { get; set; } = 1.0f;
        public bool IsFlipped { get; set; } = false;
        public int PositionX { get; set; } = 0;
        public int PositionY { get; set; } = 0;
    }

    // Класс для хранения настроек доступного персонажа (шаблон)
    public class AvailableCharacterData
    {
        public string OriginalName { get; set; } = ""; // Оригинальное имя для идентификации
        public string DisplayName { get; set; } = ""; // Отображаемое имя
        public string FilePath { get; set; } = ""; // Путь к GIF файлу (пустой для встроенных)
        public float DefaultScale { get; set; } = 1.0f; // Масштаб по умолчанию
        public bool DefaultIsFlipped { get; set; } = false; // Отзеркаливание по умолчанию
    }

    public class AppSettings
    {
        public float Scale { get; set; } = 1.0f;
        public bool IsFlipped { get; set; } = false;
        public bool IsLocked { get; set; } = false;
        public bool MinimizeOnClose { get; set; } = true;
        public bool SingleCharacterMode { get; set; } = true; // По умолчанию одиночный режим (true = одиночный, false = много)
        public bool ShowInTaskbar { get; set; } = false; // Показывать в панели задач
        public bool ShowMenuOnStartup { get; set; } = false; // Открывать меню при запуске (по умолчанию скрыто)
        
        // Список сохранённых персонажей на экране
        public List<CharacterData> SavedCharacters { get; set; } = new List<CharacterData>();
        
        // Список доступных персонажей с настройками
        public List<AvailableCharacterData> AvailableCharacters { get; set; } = new List<AvailableCharacterData>();
        
        // Список пользовательских GIF файлов (имена файлов в папке CustomGifs)
        public List<string> CustomGifFiles { get; set; } = new List<string>();

        private static readonly string AppDataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "DesktopDance"
        );

        private static readonly string SettingsFilePath = Path.Combine(AppDataFolder, "settings.json");
        
        public static readonly string CustomGifsFolder = Path.Combine(AppDataFolder, "CustomGifs");

        public static AppSettings Load()
        {
            try
            {
                // Создаём папки, если их нет
                if (!Directory.Exists(AppDataFolder))
                    Directory.CreateDirectory(AppDataFolder);
                if (!Directory.Exists(CustomGifsFolder))
                    Directory.CreateDirectory(CustomGifsFolder);

                if (File.Exists(SettingsFilePath))
                {
                    string json = File.ReadAllText(SettingsFilePath);
                    var settings = JsonSerializer.Deserialize<AppSettings>(json);
                    return settings ?? new AppSettings();
                }
            }
            catch
            {
                // Если не удалось загрузить, возвращаем настройки по умолчанию
            }
            return new AppSettings();
        }

        // Метод для копирования GIF в AppData
        public static string CopyGifToAppData(string sourceFilePath)
        {
            try
            {
                if (!Directory.Exists(CustomGifsFolder))
                    Directory.CreateDirectory(CustomGifsFolder);

                string fileName = Path.GetFileName(sourceFilePath);
                string destFilePath = Path.Combine(CustomGifsFolder, fileName);

                // Если файл с таким именем уже существует, добавляем номер
                int counter = 1;
                while (File.Exists(destFilePath))
                {
                    string fileNameWithoutExt = Path.GetFileNameWithoutExtension(sourceFilePath);
                    string extension = Path.GetExtension(sourceFilePath);
                    fileName = $"{fileNameWithoutExt}_{counter}{extension}";
                    destFilePath = Path.Combine(CustomGifsFolder, fileName);
                    counter++;
                }

                File.Copy(sourceFilePath, destFilePath, false);
                return destFilePath;
            }
            catch
            {
                return sourceFilePath; // Если не удалось скопировать, используем исходный путь
            }
        }

        public void Save()
        {
            try
            {
                string directory = Path.GetDirectoryName(SettingsFilePath)!;
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                string json = JsonSerializer.Serialize(this, options);
                File.WriteAllText(SettingsFilePath, json);
            }
            catch
            {
                // Игнорируем ошибки сохранения
            }
        }
    }
}

