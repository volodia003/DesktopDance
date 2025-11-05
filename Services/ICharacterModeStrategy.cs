namespace DesktopKonata.Services
{
    /// <summary>
    /// Интерфейс стратегии для работы с режимами персонажей (одиночный/множественный)
    /// </summary>
    public interface ICharacterModeStrategy
    {
        /// <summary>
        /// Добавляет персонажа в соответствии с логикой режима
        /// </summary>
        void AddCharacter(Bitmap bitmap, string characterName, float scale, bool isFlipped, Point? position = null);

        /// <summary>
        /// Удаляет персонажа по индексу
        /// </summary>
        void RemoveCharacter(int index);

        /// <summary>
        /// Вызывается при активации данного режима
        /// </summary>
        void OnModeActivated();

        /// <summary>
        /// Проверяет, можно ли добавить ещё персонажей
        /// </summary>
        bool CanAddMoreCharacters();

        /// <summary>
        /// Получает максимальное количество персонажей для данного режима
        /// </summary>
        int GetMaxCharacters();

        /// <summary>
        /// Название режима для UI
        /// </summary>
        string ModeName { get; }
    }
}

