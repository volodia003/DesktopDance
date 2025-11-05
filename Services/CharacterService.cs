using DesktopDance.Forms;
using DesktopDance.Utility;
using System.Drawing;

namespace DesktopDance.Services
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
            return CharacterResourceProvider.GetBuiltInCharacterBitmap(characterName);
        }

        /// <summary>
        /// Загружает Bitmap персонажа (встроенного или пользовательского)
        /// </summary>
        public Bitmap? LoadCharacterBitmap(AvailableCharacterData charData)
        {
            return CharacterResourceProvider.LoadCharacterBitmap(charData);
        }

        /// <summary>
        /// Добавляет нового персонажа на экран
        /// </summary>
        public void AddCharacter(Bitmap characterBitmap, string characterName, float scale, bool isFlipped, Point? position = null)
        {
            InitializeWindows();

            CharacterManager.AddCharacter(characterBitmap, characterName);
            
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

