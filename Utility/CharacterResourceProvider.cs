using System.Drawing;

namespace DesktopDance.Utility
{
    /// <summary>
    /// Провайдер для централизованной загрузки ресурсов встроенных персонажей
    /// Устраняет дублирование кода загрузки персонажей
    /// </summary>
    public static class CharacterResourceProvider
    {
        public const string BLIN4IIK_DANCE = "blin4iik Dance";
        public const string KONATA_LOVE = "Konata Love";
        
        public const int BUILT_IN_CHARACTERS_COUNT = 2;

        /// <summary>
        /// Получает Bitmap встроенного персонажа по имени
        /// </summary>
        /// <param name="characterName">Имя персонажа</param>
        /// <returns>Bitmap персонажа или null, если персонаж не найден</returns>
        public static Bitmap? GetBuiltInCharacterBitmap(string characterName)
        {
            return characterName switch
            {
                BLIN4IIK_DANCE => Properties.Resources.blin4iikDance,
                KONATA_LOVE => Properties.Resources.KonataLoveDancingGif,
                _ => null
            };
        }

        /// <summary>
        /// Проверяет, является ли персонаж встроенным
        /// </summary>
        /// <param name="characterName">Имя персонажа</param>
        /// <returns>true, если персонаж встроенный</returns>
        public static bool IsBuiltInCharacter(string characterName)
        {
            return characterName == BLIN4IIK_DANCE || characterName == KONATA_LOVE;
        }

        /// <summary>
        /// Получает список всех встроенных персонажей
        /// </summary>
        /// <returns>Список данных встроенных персонажей</returns>
        public static List<AvailableCharacterData> GetBuiltInCharacters()
        {
            return new List<AvailableCharacterData>
            {
                new AvailableCharacterData
                {
                    OriginalName = BLIN4IIK_DANCE,
                    DisplayName = BLIN4IIK_DANCE,
                    FilePath = "",
                    DefaultScale = 1.0f,
                    DefaultIsFlipped = false
                },
                new AvailableCharacterData
                {
                    OriginalName = KONATA_LOVE,
                    DisplayName = KONATA_LOVE,
                    FilePath = "",
                    DefaultScale = 1.0f,
                    DefaultIsFlipped = false
                }
            };
        }

        /// <summary>
        /// Получает иконку для персонажа по индексу
        /// </summary>
        /// <param name="index">Индекс персонажа</param>
        /// <returns>Строка с эмодзи-иконкой</returns>
        public static string GetCharacterIcon(int index)
        {
            return index switch
            {
                0 => "🎭",  // blin4iik Dance
                1 => "💝",  // Konata Love
                _ => "📎"   // Пользовательский персонаж
            };
        }

        /// <summary>
        /// Получает Bitmap персонажа (встроенного или пользовательского)
        /// </summary>
        /// <param name="charData">Данные персонажа</param>
        /// <returns>Bitmap персонажа или null</returns>
        public static Bitmap? LoadCharacterBitmap(AvailableCharacterData charData)
        {
            if (string.IsNullOrEmpty(charData.FilePath))
            {
                return GetBuiltInCharacterBitmap(charData.OriginalName);
            }
            else if (File.Exists(charData.FilePath))
            {
                try
                {
                    return new Bitmap(charData.FilePath);
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }
    }
}

