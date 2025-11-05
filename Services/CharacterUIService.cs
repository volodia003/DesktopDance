using DesktopDance.Utility;
using System.Windows.Forms;

namespace DesktopDance.Services
{
    /// <summary>
    /// Сервис для управления отображением списков персонажей в UI
    /// </summary>
    public class CharacterUIService
    {
        private readonly ListBox _availableCharactersListBox;
        private readonly ListBox _activeCharactersListBox;
        private readonly Label _activeCharactersLabel;
        private readonly AppSettings _settings;

        public CharacterUIService(
            ListBox availableCharactersListBox,
            ListBox activeCharactersListBox,
            Label activeCharactersLabel,
            AppSettings settings)
        {
            _availableCharactersListBox = availableCharactersListBox;
            _activeCharactersListBox = activeCharactersListBox;
            _activeCharactersLabel = activeCharactersLabel;
            _settings = settings;
        }

        /// <summary>
        /// Загружает список доступных персонажей в ListBox
        /// </summary>
        public void LoadAvailableCharactersList()
        {
            try
            {
                _availableCharactersListBox.Items.Clear();
                
                for (int i = 0; i < _settings.AvailableCharacters.Count; i++)
                {
                    var charData = _settings.AvailableCharacters[i];
                    string icon = CharacterResourceProvider.GetCharacterIcon(i);
                    _availableCharactersListBox.Items.Add($"{icon} {charData.DisplayName}");
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Обновляет список активных персонажей на экране
        /// </summary>
        public void UpdateActiveCharactersList(bool singleCharacterMode)
        {
            // Оптимизация: обновляем только если количество изменилось
            if (_activeCharactersListBox.Items.Count != CharacterManager.Characters.Count)
            {
                int previousSelectedIndex = _activeCharactersListBox.SelectedIndex;
                
                _activeCharactersListBox.BeginUpdate();
                _activeCharactersListBox.Items.Clear();
                
                foreach (var character in CharacterManager.Characters)
                {
                    string displayText = $"{character.Name} (⚖{(int)(character.Scale * 100)}%)";
                    if (character.IsFlipped)
                        displayText += " 🔄";
                    _activeCharactersListBox.Items.Add(displayText);
                }
                
                _activeCharactersListBox.EndUpdate();
                
                if (singleCharacterMode)
                {
                    _activeCharactersLabel.Text = $"На экране (макс. 1):";
                }
                else
                {
                    _activeCharactersLabel.Text = $"На экране ({CharacterManager.Characters.Count}):";
                }

                // Восстанавливаем выбор, если возможно
                if (previousSelectedIndex >= 0 && previousSelectedIndex < _activeCharactersListBox.Items.Count)
                {
                    _activeCharactersListBox.SelectedIndex = previousSelectedIndex;
                }
            }
        }

        /// <summary>
        /// Принудительно обновляет список активных персонажей
        /// </summary>
        public void ForceUpdateActiveCharactersList(bool singleCharacterMode)
        {
            _activeCharactersListBox.BeginUpdate();
            _activeCharactersListBox.Items.Clear();
            
            foreach (var character in CharacterManager.Characters)
            {
                string displayText = $"{character.Name} (⚖{(int)(character.Scale * 100)}%)";
                if (character.IsFlipped)
                    displayText += " 🔄";
                _activeCharactersListBox.Items.Add(displayText);
            }
            
            _activeCharactersListBox.EndUpdate();
            
            if (singleCharacterMode)
            {
                _activeCharactersLabel.Text = $"На экране (макс. 1):";
            }
            else
            {
                _activeCharactersLabel.Text = $"На экране ({CharacterManager.Characters.Count}):";
            }
        }

        /// <summary>
        /// Добавляет персонажа в список доступных
        /// </summary>
        public void AddToAvailableList(string displayName, string icon = "📎")
        {
            _availableCharactersListBox.Items.Add($"{icon} {displayName}");
        }

        /// <summary>
        /// Удаляет персонажа из списка доступных по индексу
        /// </summary>
        public void RemoveFromAvailableList(int index)
        {
            if (index >= 0 && index < _availableCharactersListBox.Items.Count)
            {
                _availableCharactersListBox.Items.RemoveAt(index);
            }
        }

        /// <summary>
        /// Получает индекс выбранного персонажа в списке активных
        /// </summary>
        public int GetActiveCharacterSelectedIndex()
        {
            return _activeCharactersListBox.SelectedIndex;
        }

        /// <summary>
        /// Получает индекс выбранного персонажа в списке доступных
        /// </summary>
        public int GetAvailableCharacterSelectedIndex()
        {
            return _availableCharactersListBox.SelectedIndex;
        }

        /// <summary>
        /// Устанавливает выбранный индекс в списке активных персонажей
        /// </summary>
        public void SetActiveCharacterSelectedIndex(int index)
        {
            if (index >= 0 && index < _activeCharactersListBox.Items.Count)
            {
                _activeCharactersListBox.SelectedIndex = index;
            }
        }
    }
}

