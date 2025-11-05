using DesktopDance.Utility;

namespace DesktopDance.Services
{
    /// <summary>
    /// Стратегия для режима одного персонажа
    /// В этом режиме на экране может быть только один персонаж.
    /// При добавлении нового персонажа предыдущий заменяется.
    /// </summary>
    public class SingleCharacterModeStrategy : ICharacterModeStrategy
    {
        private readonly CharacterService _characterService;
        private string? _currentCharacterName;
        private Point? _savedPosition;

        public string ModeName => "Одиночный режим";

        public SingleCharacterModeStrategy(CharacterService characterService)
        {
            _characterService = characterService;
        }

        public void AddCharacter(Bitmap bitmap, string characterName, float scale, bool isFlipped, Point? position = null)
        {
            // Если это тот же персонаж - не делаем ничего
            if (_currentCharacterName == characterName && CharacterManager.Characters.Count > 0)
            {
                return;
            }

            // Сохраняем позицию текущего персонажа перед удалением
            if (CharacterManager.Characters.Count > 0)
            {
                _savedPosition = CharacterManager.Characters[0].Location;
                CharacterManager.ClearCharacters();
            }

            // Используем переданную позицию или сохранённую
            Point? finalPosition = position ?? _savedPosition;

            // Добавляем нового персонажа
            _characterService.AddCharacter(bitmap, characterName, scale, isFlipped, finalPosition);
            _currentCharacterName = characterName;

            // Обновляем сохранённую позицию
            if (CharacterManager.Characters.Count > 0)
            {
                _savedPosition = CharacterManager.Characters[0].Location;
            }
        }

        public void RemoveCharacter(int index)
        {
            if (index == 0 && CharacterManager.Characters.Count > 0)
            {
                _savedPosition = CharacterManager.Characters[0].Location;
                CharacterManager.RemoveCharacter(index);
                _currentCharacterName = null;
            }
        }

        public void OnModeActivated()
        {
            // При активации одиночного режима оставляем только первого персонажа
            if (CharacterManager.Characters.Count > 1)
            {
                var firstCharacter = CharacterManager.Characters[0];
                var savedPosition = firstCharacter.Location;
                string characterName = firstCharacter.Name;
                float characterScale = firstCharacter.Scale;
                bool characterIsFlipped = firstCharacter.IsFlipped;
                Bitmap characterImage = (Bitmap)firstCharacter.AnimatedImage.Clone();

                CharacterManager.ClearCharacters();

                // Воссоздаём первого персонажа
                _characterService.AddCharacter(
                    characterImage,
                    characterName,
                    characterScale,
                    characterIsFlipped,
                    savedPosition
                );

                _currentCharacterName = characterName;
                _savedPosition = savedPosition;
            }
            else if (CharacterManager.Characters.Count == 1)
            {
                // Сохраняем информацию о единственном персонаже
                _currentCharacterName = CharacterManager.Characters[0].Name;
                _savedPosition = CharacterManager.Characters[0].Location;
            }
            else
            {
                // Нет персонажей
                _currentCharacterName = null;
                _savedPosition = null;
            }
        }

        public bool CanAddMoreCharacters()
        {
            // В одиночном режиме всегда можно "добавить" (заменить)
            return true;
        }

        public int GetMaxCharacters()
        {
            return 1;
        }

        /// <summary>
        /// Получает имя текущего персонажа
        /// </summary>
        public string? GetCurrentCharacterName()
        {
            return _currentCharacterName;
        }

        /// <summary>
        /// Получает сохранённую позицию персонажа
        /// </summary>
        public Point? GetSavedPosition()
        {
            return _savedPosition;
        }

        /// <summary>
        /// Сбрасывает состояние при очистке всех персонажей
        /// </summary>
        public void Reset()
        {
            _currentCharacterName = null;
            _savedPosition = null;
        }
    }
}

