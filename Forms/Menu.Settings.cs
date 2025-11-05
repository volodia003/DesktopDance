using DesktopDance.Utility;
using DesktopDance.Services;

namespace DesktopDance.Forms
{
    /// <summary>
    /// Partial класс Menu - работа с настройками
    /// </summary>
    public partial class Menu
    {
        /// <summary>
        /// Загружает настройки из файла
        /// </summary>
        private void LoadSettings()
        {
            bool isAutoStart = _settingsService.IsInAutoStart();
            autoStartToolStripMenuItem.Checked = isAutoStart;
            _trayIconService.AutoStart = isAutoStart;
            
            _trayIconService.SingleCharacterMode = _characterModeService.IsSingleCharacterMode;
            
            Logger.Debug("Настройки загружены");
        }

        /// <summary>
        /// Применяет настройки к UI элементам
        /// </summary>
        private void ApplySettings()
        {
            _settingsService.ApplyToControls(
                scaleTrackBar,
                scaleLabel,
                flipCheckBox,
                lockCheckBox,
                minimizeOnCloseToolStripMenuItem,
                singleCharacterModeToolStripMenuItem,
                showTrayIconToolStripMenuItem,
                showMenuOnStartupToolStripMenuItem,
                this
            );
            
            _trayIconService.MinimizeOnClose = _settingsService.Settings.MinimizeOnClose;
            _trayIconService.SingleCharacterMode = _characterModeService.IsSingleCharacterMode;
            _trayIconService.ShowInTaskbar = _settingsService.Settings.ShowInTaskbar;
            _trayIconService.ShowMenuOnStartup = _settingsService.Settings.ShowMenuOnStartup;
            
            UpdateUIForMode();
            Logger.Debug("Настройки применены к UI");
        }

        /// <summary>
        /// Сохраняет текущие настройки
        /// </summary>
        private void SaveSettings()
        {
            _settingsService.SaveFromControls(
                scaleTrackBar,
                flipCheckBox,
                lockCheckBox,
                minimizeOnCloseToolStripMenuItem,
                singleCharacterModeToolStripMenuItem,
                showTrayIconToolStripMenuItem,
                showMenuOnStartupToolStripMenuItem
            );
            Logger.Debug("Настройки сохранены");
        }

        /// <summary>
        /// Переключает режим персонажей (одиночный/множественный)
        /// </summary>
        private void SwitchCharacterMode(bool singleMode)
        {
            if (_characterModeService.IsSingleCharacterMode == singleMode)
                return; // Режим не изменился

            // Если включаем одиночный режим и есть несколько персонажей
            if (singleMode && CharacterManager.Characters.Count > 1)
            {
                var result = MessageBox.Show(
                    "Включён режим одного персонажа.\nНа экране останется только первый персонаж.\nПродолжить?",
                    "Режим одного персонажа",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    // Пользователь отменил переключение
                    return;
                }
            }

            Logger.Info($"Переключение режима персонажей: {(singleMode ? "одиночный" : "множественный")}");
            _characterModeService.SwitchMode(singleMode, saveSettings: true);

            UpdateUIForMode();
            ForceUpdateActiveCharactersList();
            
            if (CharacterManager.Characters.Count > 0)
            {
                _selectedCharacter = CharacterManager.Characters[0];
                UpdateUIForSelectedCharacter();
            }

            // Синхронизируем состояние чекбоксов
            singleCharacterModeToolStripMenuItem.Checked = !singleMode; // Инвертировано: чекбокс "Много персонажей"
            _trayIconService.SingleCharacterMode = singleMode;

            SaveCharacters();
        }

        /// <summary>
        /// Обновляет UI в соответствии с режимом персонажей
        /// </summary>
        private void UpdateUIForMode()
        {
            bool isSingleMode = _characterModeService.IsSingleCharacterMode;
            
            if (this.Controls.Contains(activeCharactersPanel))
            {
                activeCharactersPanel.Visible = !isSingleMode;
            }
            
            if (this.Controls.Contains(availableCharactersPanel))
            {
                if (isSingleMode)
                {
                    charactersLabel.Text = "🎭 Персонажи";
                    availableCharactersPanel.Size = new Size(265, 375);
                    charactersListBox.Size = new Size(240, 325);
                }
                else
                {
                    charactersLabel.Text = "🎭 Персонажи";
                    availableCharactersPanel.Size = new Size(265, 190);
                    charactersListBox.Size = new Size(240, 136);
                    activeCharactersLabel.Text = $"👥 Активные ({CharacterManager.Characters.Count})";
                }
            }
        }

        /// <summary>
        /// Изменяет тему приложения
        /// </summary>
        private void ChangeTheme(ThemeService.ThemeMode newTheme)
        {
            Logger.Info($"Изменение темы на: {newTheme}");
            _themeService.CurrentTheme = newTheme;
            _settingsService.Settings.Theme = newTheme switch
            {
                ThemeService.ThemeMode.Dark => "Dark",
                ThemeService.ThemeMode.Blin4iik => "Blin4iik",
                ThemeService.ThemeMode.System => "System",
                _ => "Light"
            };
            SaveSettings();
        }

        /// <summary>
        /// Обработчик изменения темы
        /// </summary>
        private void OnThemeChanged(object? sender, ThemeService.ThemeMode newTheme)
        {
            _themeService.ApplyTheme(this);
        }

        /// <summary>
        /// Настройка таймера обновления
        /// </summary>
        private void SetupUpdateTimer()
        {
            _updateTimer = new System.Windows.Forms.Timer();
            _updateTimer.Interval = 2000;
            _updateTimer.Tick += UpdateTimer_Tick;
            _updateTimer.Start();
        }

        /// <summary>
        /// Обработчик тика таймера обновления
        /// </summary>
        private void UpdateTimer_Tick(object? sender, EventArgs e)
        {
            _characterUIService.UpdateActiveCharactersList(_characterModeService.IsSingleCharacterMode);
        }
    }
}

