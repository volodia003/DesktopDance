using DesktopDance.Utility;
using System.Windows.Forms;

namespace DesktopDance.Services
{
    /// <summary>
    /// Сервис для управления операциями с персонажами (переименование, удаление, настройки)
    /// </summary>
    public class CharacterManagementService
    {
        private readonly AppSettings _settings;

        public CharacterManagementService(AppSettings settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// Переименовывает персонажа в списке доступных
        /// </summary>
        public bool RenameAvailableCharacter(int characterIndex, string newName)
        {
            if (characterIndex < 0 || characterIndex >= _settings.AvailableCharacters.Count)
                return false;

            if (string.IsNullOrWhiteSpace(newName))
                return false;

            _settings.AvailableCharacters[characterIndex].DisplayName = newName.Trim();
            _settings.Save();
            return true;
        }

        /// <summary>
        /// Переименовывает активного персонажа на экране
        /// </summary>
        public bool RenameActiveCharacter(int characterIndex, string newName)
        {
            if (characterIndex < 0 || characterIndex >= CharacterManager.Characters.Count)
                return false;

            if (string.IsNullOrWhiteSpace(newName))
                return false;

            CharacterManager.Characters[characterIndex].Name = newName.Trim();
            return true;
        }

        /// <summary>
        /// Удаляет пользовательский GIF из настроек и файловой системы
        /// </summary>
        public bool DeleteCustomGif(int characterIndex, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (characterIndex < CharacterResourceProvider.BUILT_IN_CHARACTERS_COUNT)
            {
                errorMessage = "Невозможно удалить встроенного персонажа.";
                return false;
            }

            if (characterIndex >= _settings.AvailableCharacters.Count)
            {
                errorMessage = "Неверный индекс персонажа.";
                return false;
            }

            try
            {
                var charData = _settings.AvailableCharacters[characterIndex];

                // Удаляем файл
                if (!string.IsNullOrEmpty(charData.FilePath) && File.Exists(charData.FilePath))
                {
                    File.Delete(charData.FilePath);
                }

                // Удаляем из списка пользовательских GIF
                int customIndex = characterIndex - CharacterResourceProvider.BUILT_IN_CHARACTERS_COUNT;
                if (customIndex >= 0 && customIndex < _settings.CustomGifFiles.Count)
                {
                    _settings.CustomGifFiles.RemoveAt(customIndex);
                }

                // Удаляем из доступных персонажей
                _settings.AvailableCharacters.RemoveAt(characterIndex);
                _settings.Save();

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Добавляет пользовательский GIF в настройки
        /// </summary>
        public bool AddCustomGif(string sourceFilePath, out string copiedFilePath, out string errorMessage)
        {
            copiedFilePath = string.Empty;
            errorMessage = string.Empty;

            try
            {
                // Копируем GIF в AppData
                copiedFilePath = AppSettings.CopyGifToAppData(sourceFilePath);
                string fileName = Path.GetFileNameWithoutExtension(copiedFilePath);
                string gifFileName = Path.GetFileName(copiedFilePath);

                // Проверяем, нет ли уже такого GIF
                if (_settings.CustomGifFiles.Contains(gifFileName))
                {
                    errorMessage = "Этот GIF уже добавлен.";
                    return false;
                }

                // Добавляем в список пользовательских GIF
                _settings.CustomGifFiles.Add(gifFileName);

                // Добавляем в доступные персонажи
                _settings.AvailableCharacters.Add(new AvailableCharacterData
                {
                    OriginalName = fileName,
                    DisplayName = fileName,
                    FilePath = copiedFilePath,
                    DefaultScale = 1.0f,
                    DefaultIsFlipped = false
                });

                _settings.Save();
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Обновляет настройки персонажа по умолчанию
        /// </summary>
        public bool UpdateCharacterDefaultSettings(int characterIndex, float scale, bool isFlipped)
        {
            if (characterIndex < 0 || characterIndex >= _settings.AvailableCharacters.Count)
                return false;

            var charData = _settings.AvailableCharacters[characterIndex];
            charData.DefaultScale = scale;
            charData.DefaultIsFlipped = isFlipped;
            _settings.Save();

            return true;
        }

        /// <summary>
        /// Получает данные персонажа по индексу
        /// </summary>
        public AvailableCharacterData? GetAvailableCharacter(int index)
        {
            if (index < 0 || index >= _settings.AvailableCharacters.Count)
                return null;

            return _settings.AvailableCharacters[index];
        }

        /// <summary>
        /// Показывает диалог переименования персонажа в списке доступных
        /// </summary>
        public bool ShowRenameDialog(int characterIndex, out string newName)
        {
            newName = string.Empty;

            var charData = GetAvailableCharacter(characterIndex);
            if (charData == null)
                return false;

            using var inputDialog = new Form
            {
                Text = "Переименовать персонажа",
                Width = 400,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var label = new Label
            {
                Text = "Новое имя:",
                Left = 20,
                Top = 20,
                Width = 100
            };

            var textBox = new TextBox
            {
                Text = charData.DisplayName,
                Left = 20,
                Top = 45,
                Width = 340
            };
            textBox.SelectAll();

            var okButton = new Button
            {
                Text = "OK",
                Left = 200,
                Top = 75,
                DialogResult = DialogResult.OK
            };

            var cancelButton = new Button
            {
                Text = "Отмена",
                Left = 285,
                Top = 75,
                DialogResult = DialogResult.Cancel
            };

            inputDialog.Controls.Add(label);
            inputDialog.Controls.Add(textBox);
            inputDialog.Controls.Add(okButton);
            inputDialog.Controls.Add(cancelButton);
            inputDialog.AcceptButton = okButton;
            inputDialog.CancelButton = cancelButton;

            if (inputDialog.ShowDialog() == DialogResult.OK)
            {
                newName = textBox.Text.Trim();
                return !string.IsNullOrEmpty(newName);
            }

            return false;
        }

        /// <summary>
        /// Показывает диалог переименования активного персонажа
        /// </summary>
        public bool ShowRenameActiveCharacterDialog(int characterIndex, out string newName)
        {
            newName = string.Empty;

            if (characterIndex < 0 || characterIndex >= CharacterManager.Characters.Count)
                return false;

            var character = CharacterManager.Characters[characterIndex];

            using var inputDialog = new Form
            {
                Text = "Переименовать персонажа",
                Width = 400,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var label = new Label
            {
                Text = "Новое имя:",
                Left = 20,
                Top = 20,
                Width = 100
            };

            var textBox = new TextBox
            {
                Text = character.Name,
                Left = 20,
                Top = 45,
                Width = 340
            };
            textBox.SelectAll();

            var okButton = new Button
            {
                Text = "OK",
                Left = 200,
                Top = 75,
                DialogResult = DialogResult.OK
            };

            var cancelButton = new Button
            {
                Text = "Отмена",
                Left = 285,
                Top = 75,
                DialogResult = DialogResult.Cancel
            };

            inputDialog.Controls.Add(label);
            inputDialog.Controls.Add(textBox);
            inputDialog.Controls.Add(okButton);
            inputDialog.Controls.Add(cancelButton);
            inputDialog.AcceptButton = okButton;
            inputDialog.CancelButton = cancelButton;

            if (inputDialog.ShowDialog() == DialogResult.OK)
            {
                newName = textBox.Text.Trim();
                return !string.IsNullOrEmpty(newName);
            }

            return false;
        }

        /// <summary>
        /// Показывает диалог настроек персонажа
        /// </summary>
        public bool ShowCharacterSettingsDialog(int characterIndex, out float scale, out bool isFlipped)
        {
            scale = 1.0f;
            isFlipped = false;

            var charData = GetAvailableCharacter(characterIndex);
            if (charData == null)
                return false;

            using var settingsDialog = new Form
            {
                Text = $"Настройки: {charData.DisplayName}",
                Width = 400,
                Height = 200,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var scaleLabel = new Label
            {
                Text = "Размер по умолчанию:",
                Left = 20,
                Top = 20,
                Width = 150
            };

            var scaleTrack = new TrackBar
            {
                Minimum = 10,
                Maximum = 300,
                Value = (int)(charData.DefaultScale * 100),
                Left = 20,
                Top = 45,
                Width = 340,
                TickFrequency = 10
            };

            var scaleValueLabel = new Label
            {
                Text = $"{scaleTrack.Value}%",
                Left = 170,
                Top = 20,
                Width = 50
            };

            scaleTrack.Scroll += (s, e) => scaleValueLabel.Text = $"{scaleTrack.Value}%";

            var flipCheck = new CheckBox
            {
                Text = "Отзеркалить по умолчанию",
                Left = 20,
                Top = 90,
                Width = 200,
                Checked = charData.DefaultIsFlipped
            };

            var okButton = new Button
            {
                Text = "Сохранить",
                Left = 200,
                Top = 120,
                DialogResult = DialogResult.OK
            };

            var cancelButton = new Button
            {
                Text = "Отмена",
                Left = 285,
                Top = 120,
                DialogResult = DialogResult.Cancel
            };

            settingsDialog.Controls.Add(scaleLabel);
            settingsDialog.Controls.Add(scaleValueLabel);
            settingsDialog.Controls.Add(scaleTrack);
            settingsDialog.Controls.Add(flipCheck);
            settingsDialog.Controls.Add(okButton);
            settingsDialog.Controls.Add(cancelButton);
            settingsDialog.AcceptButton = okButton;
            settingsDialog.CancelButton = cancelButton;

            if (settingsDialog.ShowDialog() == DialogResult.OK)
            {
                scale = scaleTrack.Value / 100f;
                isFlipped = flipCheck.Checked;
                return true;
            }

            return false;
        }
    }
}

