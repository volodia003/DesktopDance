using System.Text.Json;

namespace DesktopDance.Utility
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
        public bool MinimizeOnClose { get; set; } = false; // По умолчанию НЕ сворачивать при закрытии
        public bool SingleCharacterMode { get; set; } = true; // По умолчанию одиночный режим (true = одиночный, false = много)
        public bool ShowInTaskbar { get; set; } = true; // Показывать в панели задач
        public bool ShowMenuOnStartup { get; set; } = true; // Открывать меню при запуске
        public string Theme { get; set; } = "Light"; // Тема: Light, Dark, System
        
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
                if (!Directory.Exists(AppDataFolder))
                {
                    Directory.CreateDirectory(AppDataFolder);
                    Logger.Info($"Создана папка AppData: {AppDataFolder}");
                }
                
                if (!Directory.Exists(CustomGifsFolder))
                {
                    Directory.CreateDirectory(CustomGifsFolder);
                    Logger.Info($"Создана папка CustomGifs: {CustomGifsFolder}");
                }

                if (File.Exists(SettingsFilePath))
                {
                    string json = File.ReadAllText(SettingsFilePath);
                    var settings = JsonSerializer.Deserialize<AppSettings>(json);
                    
                    if (settings != null)
                    {
                        Logger.Info("Настройки успешно загружены");
                        return settings;
                    }
                    else
                    {
                        Logger.Warning("Не удалось десериализовать настройки, используются значения по умолчанию");
                    }
                }
                else
                {
                    Logger.Info("Файл настроек не найден, создаются настройки по умолчанию");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Ошибка при загрузке настроек", ex);
            }
            return new AppSettings();
        }

        public static string CopyGifToAppData(string sourceFilePath)
        {
            try
            {
                if (!Directory.Exists(CustomGifsFolder))
                {
                    Directory.CreateDirectory(CustomGifsFolder);
                    Logger.Info($"Создана папка CustomGifs: {CustomGifsFolder}");
                }

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
                Logger.Info($"GIF скопирован в AppData: {fileName}");
                return destFilePath;
            }
            catch (Exception ex)
            {
                Logger.Error($"Не удалось скопировать GIF '{sourceFilePath}' в AppData", ex);
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
                    Logger.Info($"Создана папка для настроек: {directory}");
                }

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                string json = JsonSerializer.Serialize(this, options);
                File.WriteAllText(SettingsFilePath, json);
                Logger.Debug("Настройки успешно сохранены");
            }
            catch (Exception ex)
            {
                Logger.Error("Ошибка при сохранении настроек", ex);
            }
        }
    }
}

