using DesktopDance.Utility;

namespace DesktopDance.Services
{
    /// <summary>
    /// Стратегия для режима нескольких персонажей
    /// В этом режиме на экране может быть неограниченное количество персонажей.
    /// </summary>
    public class MultipleCharacterModeStrategy : ICharacterModeStrategy
    {
        private readonly CharacterService _characterService;

        public string ModeName => "Режим нескольких персонажей";

        public MultipleCharacterModeStrategy(CharacterService characterService)
        {
            _characterService = characterService;
        }

        public void AddCharacter(Bitmap bitmap, string characterName, float scale, bool isFlipped, Point? position = null)
        {
            _characterService.AddCharacter(bitmap, characterName, scale, isFlipped, position);
        }

        public void RemoveCharacter(int index)
        {
            CharacterManager.RemoveCharacter(index);
        }

        public void OnModeActivated()
        {
            // При переключении в режим нескольких персонажей
            // ничего особенного делать не нужно - все персонажи остаются
        }

        public bool CanAddMoreCharacters()
        {
            return true;
        }

        public int GetMaxCharacters()
        {
            return int.MaxValue;
        }
    }
}

