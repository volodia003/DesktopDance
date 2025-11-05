using DesktopKonata.Utility;

namespace DesktopKonata.Services
{
    /// <summary>
    /// Сервис для управления режимами персонажей (одиночный/множественный)
    /// Использует паттерн Strategy для переключения между режимами
    /// </summary>
    public class CharacterModeService
    {
        private ICharacterModeStrategy _currentStrategy;
        private readonly SingleCharacterModeStrategy _singleModeStrategy;
        private readonly MultipleCharacterModeStrategy _multipleModeStrategy;
        private readonly AppSettings _settings;

        /// <summary>
        /// Событие, вызываемое при смене режима
        /// </summary>
        public event EventHandler<bool>? ModeChanged;

        /// <summary>
        /// Текущий режим: true = одиночный, false = множественный
        /// </summary>
        public bool IsSingleCharacterMode { get; private set; }

        public CharacterModeService(CharacterService characterService, AppSettings settings)
        {
            _settings = settings;
            _singleModeStrategy = new SingleCharacterModeStrategy(characterService);
            _multipleModeStrategy = new MultipleCharacterModeStrategy(characterService);

            // Устанавливаем стратегию в соответствии с настройками
            IsSingleCharacterMode = _settings.SingleCharacterMode;
            _currentStrategy = IsSingleCharacterMode ? _singleModeStrategy : _multipleModeStrategy;
        }

        /// <summary>
        /// Переключает режим персонажей
        /// </summary>
        /// <param name="singleMode">true - одиночный режим, false - множественный</param>
        /// <param name="saveSettings">Сохранить настройки после переключения</param>
        public void SwitchMode(bool singleMode, bool saveSettings = true)
        {
            if (IsSingleCharacterMode == singleMode)
                return; // Режим не изменился

            IsSingleCharacterMode = singleMode;
            _currentStrategy = singleMode ? _singleModeStrategy : _multipleModeStrategy;

            // Вызываем логику активации режима
            _currentStrategy.OnModeActivated();

            // Сохраняем в настройках
            _settings.SingleCharacterMode = singleMode;
            if (saveSettings)
            {
                _settings.Save();
            }

            // Уведомляем подписчиков о смене режима
            ModeChanged?.Invoke(this, singleMode);
        }

        /// <summary>
        /// Добавляет персонажа используя текущую стратегию
        /// </summary>
        public void AddCharacter(Bitmap bitmap, string characterName, float scale, bool isFlipped, Point? position = null)
        {
            _currentStrategy.AddCharacter(bitmap, characterName, scale, isFlipped, position);
        }

        /// <summary>
        /// Удаляет персонажа по индексу используя текущую стратегию
        /// </summary>
        public void RemoveCharacter(int index)
        {
            _currentStrategy.RemoveCharacter(index);
        }

        /// <summary>
        /// Проверяет, можно ли добавить ещё персонажей
        /// </summary>
        public bool CanAddMoreCharacters()
        {
            return _currentStrategy.CanAddMoreCharacters();
        }

        /// <summary>
        /// Получает максимальное количество персонажей для текущего режима
        /// </summary>
        public int GetMaxCharacters()
        {
            return _currentStrategy.GetMaxCharacters();
        }

        /// <summary>
        /// Получает название текущего режима
        /// </summary>
        public string GetCurrentModeName()
        {
            return _currentStrategy.ModeName;
        }

        /// <summary>
        /// Получает текущую стратегию
        /// </summary>
        public ICharacterModeStrategy GetCurrentStrategy()
        {
            return _currentStrategy;
        }

        /// <summary>
        /// Получает стратегию одиночного режима (для специфичных операций)
        /// </summary>
        public SingleCharacterModeStrategy GetSingleModeStrategy()
        {
            return _singleModeStrategy;
        }

        /// <summary>
        /// Сбрасывает состояние одиночного режима
        /// </summary>
        public void ResetSingleModeState()
        {
            _singleModeStrategy.Reset();
        }
    }
}

