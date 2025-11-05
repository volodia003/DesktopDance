using DesktopDance.Utility;
using DesktopDance.Services;

namespace DesktopDance.Forms
{
    /// <summary>
    /// Partial класс Menu - работа с персонажами
    /// </summary>
    public partial class Menu
    {
        /// <summary>
        /// Загружает сохраненных персонажей при старте приложения
        /// </summary>
        private void LoadCharacters()
        {
            try
            {
                Logger.Info($"Загрузка сохраненных персонажей: {_settingsService.Settings.SavedCharacters.Count} шт.");
                
                foreach (var charData in _settingsService.Settings.SavedCharacters)
                {
                    try
                    {
                        Bitmap? characterBitmap = null;
                        
                        var availableChar = _settingsService.Settings.AvailableCharacters.FirstOrDefault(ac => 
                            ac.DisplayName == charData.Name || ac.OriginalName == charData.Name);
                        
                        if (availableChar != null)
                        {
                            characterBitmap = CharacterResourceProvider.LoadCharacterBitmap(availableChar);
                        }
                        
                        if (characterBitmap != null)
                        {
                            _characterService.AddCharacter(
                                characterBitmap, 
                                charData.Name, 
                                charData.Scale, 
                                charData.IsFlipped,
                                new Point(charData.PositionX, charData.PositionY)
                            );
                            Logger.Debug($"Персонаж загружен: {charData.Name}");
                        }
                        else
                        {
                            Logger.Warning($"Не удалось загрузить персонажа: {charData.Name}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Ошибка загрузки персонажа '{charData.Name}'", ex);
                    }
                }
                
                ForceUpdateActiveCharactersList();
                
                if (CharacterManager.Characters.Count > 0)
                {
                    _selectedCharacter = CharacterManager.Characters[0];
                    UpdateUIForSelectedCharacter();
                    
                    if (!_settingsService.Settings.SingleCharacterMode)
                    {
                        _characterUIService.SetActiveCharacterSelectedIndex(0);
                    }
                }
                
                Logger.Info($"Загружено персонажей: {CharacterManager.Characters.Count}");
            }
            catch (Exception ex)
            {
                Logger.Error("Ошибка при загрузке персонажей", ex);
                MessageBox.Show(
                    $"Не удалось загрузить сохраненных персонажей:\n{ex.Message}", 
                    "Ошибка", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Warning
                );
            }
        }

        /// <summary>
        /// Сохраняет текущих персонажей в настройки
        /// </summary>
        private void SaveCharacters()
        {
            _settingsService.Settings.SavedCharacters.Clear();
            
            foreach (var character in CharacterManager.Characters)
            {
                var charData = new CharacterData
                {
                    Name = character.Name,
                    FilePath = "",
                    Scale = character.Scale,
                    IsFlipped = character.IsFlipped,
                    PositionX = character.Location.X,
                    PositionY = character.Location.Y
                };
                
                // Проверяем, если это пользовательский GIF (не встроенный)
                // Ищем персонажа в списке доступных
                var availableChar = _settingsService.Settings.AvailableCharacters.FirstOrDefault(ac => 
                    ac.DisplayName == character.Name || ac.OriginalName == character.Name);
                
                if (availableChar != null && !string.IsNullOrEmpty(availableChar.FilePath))
                {
                    // Это пользовательский GIF
                    charData.FilePath = availableChar.FilePath;
                }
                else if (availableChar == null)
                {
                    // Если не нашли в AvailableCharacters, ищем в CustomGifs по имени
                    foreach (var gifFileName in _settingsService.Settings.CustomGifFiles)
                    {
                        if (Path.GetFileNameWithoutExtension(gifFileName) == character.Name)
                        {
                            charData.FilePath = Path.Combine(AppSettings.CustomGifsFolder, gifFileName);
                            break;
                        }
                    }
                }
                
                _settingsService.Settings.SavedCharacters.Add(charData);
            }
            
            _settingsService.Settings.Save();
        }

        /// <summary>
        /// Загружает пользовательский GIF файл
        /// </summary>
        private void LoadCustomGif()
        {
            using OpenFileDialog openFileDialog = new()
            {
                Title = "Выберите GIF файл",
                Filter = "GIF файлы (*.gif)|*.gif|Все файлы (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string sourceFilePath = openFileDialog.FileName;
                    Logger.Info($"Загрузка пользовательского GIF: {sourceFilePath}");
                    
                    string copiedFilePath = AppSettings.CopyGifToAppData(sourceFilePath);
                    string fileName = Path.GetFileNameWithoutExtension(copiedFilePath);
                    
                    string gifFileName = Path.GetFileName(copiedFilePath);
                    if (!_settingsService.Settings.CustomGifFiles.Contains(gifFileName))
                    {
                        _settingsService.Settings.CustomGifFiles.Add(gifFileName);
                        
                        _settingsService.Settings.AvailableCharacters.Add(new AvailableCharacterData
                        {
                            OriginalName = fileName,
                            DisplayName = fileName,
                            FilePath = copiedFilePath,
                            DefaultScale = 1.0f,
                            DefaultIsFlipped = false
                        });
                        
                        _characterUIService.AddToAvailableList(fileName, "📎");
                        _settingsService.Settings.Save();
                        
                        LoadCustomGifList();
                    }
                    
                    Bitmap gifBitmap = new(copiedFilePath);

                    _characterModeService.AddCharacter(
                        gifBitmap, 
                        fileName, 
                        CharacterManager.GlobalScale, 
                        CharacterManager.GlobalFlip
                    );

                    if (CharacterManager.Characters.Count > 0)
                    {
                        if (_characterModeService.IsSingleCharacterMode)
                        {
                            _selectedCharacter = CharacterManager.Characters[0];
                        }
                        else
                        {
                            _characterUIService.SetActiveCharacterSelectedIndex(CharacterManager.Characters.Count - 1);
                            _selectedCharacter = CharacterManager.Characters[CharacterManager.Characters.Count - 1];
                        }
                        UpdateUIForSelectedCharacter();
                    }
                    
                    ForceUpdateActiveCharactersList();
                    SaveCharacters();
                    Logger.Info($"GIF успешно загружен: {fileName}");
                }
                catch (Exception ex)
                {
                    Logger.Error($"Не удалось загрузить GIF '{openFileDialog.FileName}'", ex);
                    MessageBox.Show(
                        $"Не удалось загрузить GIF:\n{ex.Message}\n\nПроверьте, что файл:\n- Является корректным GIF\n- Не поврежден\n- Не слишком большой", 
                        "Ошибка", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        /// <summary>
        /// Удаляет выбранного персонажа
        /// </summary>
        private void RemoveSelectedCharacter()
        {
            if (activeCharactersListBox.SelectedIndex >= 0)
            {
                int indexToRemove = activeCharactersListBox.SelectedIndex;
                string characterName = CharacterManager.Characters[indexToRemove].Name;
                
                Logger.Info($"Удаление персонажа: {characterName}");
                CharacterManager.RemoveCharacter(indexToRemove);
                _selectedCharacter = null;
                ForceUpdateActiveCharactersList();
                UpdateUIForSelectedCharacter();
                SaveCharacters();
            }
        }

        /// <summary>
        /// Переименовывает выбранного персонажа
        /// </summary>
        private void RenameSelectedCharacter()
        {
            int selectedIndex = _characterUIService.GetActiveCharacterSelectedIndex();
            if (selectedIndex < 0)
                return;

            if (_characterManagementService.ShowRenameActiveCharacterDialog(selectedIndex, out string newName))
            {
                if (_characterManagementService.RenameActiveCharacter(selectedIndex, newName))
                {
                    Logger.Info($"Персонаж переименован в: {newName}");
                    ForceUpdateActiveCharactersList();
                    SaveCharacters();
                }
            }
        }

        /// <summary>
        /// Удаляет выбранный GIF из списка доступных
        /// </summary>
        private void DeleteSelectedGif()
        {
            int selectedIndex = _characterUIService.GetAvailableCharacterSelectedIndex();
            if (selectedIndex < CharacterResourceProvider.BUILT_IN_CHARACTERS_COUNT)
                return;

            var charData = _characterManagementService.GetAvailableCharacter(selectedIndex);
            if (charData == null)
                return;

            var result = MessageBox.Show(
                $"Вы действительно хотите удалить GIF '{charData.DisplayName}'?\n\nФайл будет удалён из AppData.",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                if (_characterManagementService.DeleteCustomGif(selectedIndex, out string errorMessage))
                {
                    _characterUIService.RemoveFromAvailableList(selectedIndex);
                    Logger.Info($"GIF удален: {charData.DisplayName}");
                    MessageBox.Show("GIF успешно удалён!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    Logger.Error($"Не удалось удалить GIF: {errorMessage}");
                    MessageBox.Show($"Не удалось удалить GIF: {errorMessage}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Переименовывает доступного персонажа
        /// </summary>
        private void RenameAvailableCharacter()
        {
            int selectedIndex = _characterUIService.GetAvailableCharacterSelectedIndex();
            if (selectedIndex < 0)
                return;

            if (_characterManagementService.ShowRenameDialog(selectedIndex, out string newName))
            {
                if (_characterManagementService.RenameAvailableCharacter(selectedIndex, newName))
                {
                    Logger.Info($"Доступный персонаж переименован в: {newName}");
                    LoadCustomGifList();
                }
            }
        }

        /// <summary>
        /// Показывает настройки персонажа
        /// </summary>
        private void ShowCharacterSettings()
        {
            int selectedIndex = _characterUIService.GetAvailableCharacterSelectedIndex();
            if (selectedIndex < 0)
                return;

            if (_characterManagementService.ShowCharacterSettingsDialog(selectedIndex, out float scale, out bool isFlipped))
            {
                if (_characterManagementService.UpdateCharacterDefaultSettings(selectedIndex, scale, isFlipped))
                {
                    Logger.Info($"Настройки персонажа обновлены: scale={scale}, flipped={isFlipped}");
                    MessageBox.Show("Настройки сохранены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// Загружает список пользовательских GIF
        /// </summary>
        private void LoadCustomGifList()
        {
            _characterUIService.LoadAvailableCharactersList();
            
            var customCharacters = _settingsService.Settings.AvailableCharacters
                .Skip(CharacterResourceProvider.BUILT_IN_CHARACTERS_COUNT).ToList();
            _trayIconService.UpdateCustomCharacters(customCharacters);
        }

        /// <summary>
        /// Принудительно обновляет список активных персонажей
        /// </summary>
        private void ForceUpdateActiveCharactersList()
        {
            _characterUIService.ForceUpdateActiveCharactersList(_characterModeService.IsSingleCharacterMode);
        }

        /// <summary>
        /// Обновляет UI для выбранного персонажа
        /// </summary>
        private void UpdateUIForSelectedCharacter()
        {
            if (_selectedCharacter != null)
            {
                scaleTrackBar.ValueChanged -= scaleTrackBar_Scroll;
                flipCheckBox.CheckedChanged -= flipCheckBox_CheckedChanged;
                lockCheckBox.CheckedChanged -= lockCheckBox_CheckedChanged;

                scaleTrackBar.Value = (int)(_selectedCharacter.Scale * 100);
                scaleLabel.Text = $"🎨 Размер: {(int)(_selectedCharacter.Scale * 100)}%";
                flipCheckBox.Checked = _selectedCharacter.IsFlipped;
                lockCheckBox.Checked = CharacterManager.IsLocked;

                scaleTrackBar.Enabled = true;
                flipCheckBox.Enabled = true;
                scaleLabel.Text = $"🎨 Размер: {(int)(_selectedCharacter.Scale * 100)}%";

                scaleTrackBar.ValueChanged += scaleTrackBar_Scroll;
                flipCheckBox.CheckedChanged += flipCheckBox_CheckedChanged;
                lockCheckBox.CheckedChanged += lockCheckBox_CheckedChanged;
            }
            else
            {
                scaleTrackBar.Enabled = false;
                flipCheckBox.Enabled = false;
                scaleLabel.Text = "🎨 Размер: выберите персонажа";
            }
        }
    }
}

