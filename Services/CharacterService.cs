using DesktopKonata.Forms;
using DesktopKonata.Utility;
using System.Drawing;

namespace DesktopKonata.Services
{
    /// <summary>
    /// Сервис для работы с персонажами (загрузка ресурсов, создание окон)
    /// </summary>
    public class CharacterService
    {
        private readonly List<CharacterWindow> _windows = new();
        private readonly Screen[] _allScreens = Screen.AllScreens;

        /// <summary>
        /// Получает Bitmap встроенного персонажа по имени
        /// </summary>
        public Bitmap? GetBuiltInCharacterBitmap(string characterName)
        {
            return characterName switch
            {
                "blin4iik Dance" => DesktopDance.Properties.Resources.blin4iikDance,
                "Konata Love" => DesktopDance.Properties.Resources.KonataLoveDancingGif,
                _ => null
            };
        }

        /// <summary>
        /// Загружает Bitmap персонажа (встроенного или пользовательского)
        /// </summary>
        public Bitmap? LoadCharacterBitmap(AvailableCharacterData charData)
        {
            if (string.IsNullOrEmpty(charData.FilePath))
            {
                // Встроенный персонаж
                return GetBuiltInCharacterBitmap(charData.OriginalName);
            }
            else if (File.Exists(charData.FilePath))
            {
                // Пользовательский GIF
                return new Bitmap(charData.FilePath);
            }
            return null;
        }

        /// <summary>
        /// Добавляет нового персонажа на экран
        /// </summary>
        public void AddCharacter(Bitmap characterBitmap, string characterName, float scale, bool isFlipped, Point? position = null)
        {
            // Создаём окна для всех экранов, если их ещё нет
            InitializeWindows();

            // Создаём и добавляем персонажа через CharacterManager
            CharacterManager.AddCharacter(characterBitmap, characterName);
            
            // Получаем только что добавленного персонажа
            var character = CharacterManager.Characters.LastOrDefault();
            if (character != null)
            {
                character.Scale = scale;
                character.IsFlipped = isFlipped;
                
                if (position.HasValue)
                {
                    character.Location = position.Value;
                }
            }
        }

        /// <summary>
        /// Удаляет персонажа по имени
        /// </summary>
        public bool RemoveCharacter(string characterName)
        {
            var index = CharacterManager.Characters.FindIndex(c => c.Name == characterName);
            if (index >= 0)
            {
                CharacterManager.RemoveCharacter(index);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Удаляет всех персонажей
        /// </summary>
        public void ClearAllCharacters()
        {
            CharacterManager.ClearCharacters();
        }

        /// <summary>
        /// Скрывает все окна с персонажами
        /// </summary>
        public void HideAllWindows()
        {
            foreach (var window in _windows)
            {
                window.Hide();
            }
        }

        /// <summary>
        /// Показывает все окна с персонажами
        /// </summary>
        public void ShowAllWindows()
        {
            foreach (var window in _windows)
            {
                window.Show();
            }
        }

        /// <summary>
        /// Закрывает все окна с персонажами
        /// </summary>
        public void CloseAllWindows()
        {
            foreach (var window in _windows)
            {
                window.Close();
            }
            _windows.Clear();
        }

        /// <summary>
        /// Инициализирует окна для всех экранов
        /// </summary>
        private void InitializeWindows()
        {
            if (_windows.Count == 0)
            {
                foreach (var screen in _allScreens)
                {
                    var window = new CharacterWindow(screen);
                    _windows.Add(window);
                    window.Show();
                }
            }
        }

        /// <summary>
        /// Проверяет, инициализированы ли окна
        /// </summary>
        public bool AreWindowsInitialized()
        {
            return _windows.Count > 0;
        }
    }
}

