using DesktopDance.Utility;
using DesktopDance.Services;

namespace DesktopDance.Forms
{
    /// <summary>
    /// Partial класс Menu - обработчики событий UI
    /// </summary>
    public partial class Menu
    {
        #region Обработчики событий формы

        private void Menu_Load(object sender, EventArgs args)
        {
            LoadCustomGifList();
            
            LoadCharacters();

            if (_settingsService.Settings.ShowMenuOnStartup)
            {
                this.Visible = true;
                WindowState = FormWindowState.Normal;
                Show();
            }
            else
            {
                this.Visible = false;
                Hide();
            }
        }

        private void Menu_FormClosing(object sender, FormClosingEventArgs args)
        {
            if (args.CloseReason == CloseReason.UserClosing)
            {
                if (minimizeOnCloseToolStripMenuItem.Checked)
                {
                    args.Cancel = true;
                    HideInTray();
                }
                else
                {
                    Logger.Info("Завершение работы приложения");
                    Application.Exit();
                }
            }
        }

        #endregion

        #region Обработчики событий персонажей

        private void charactersListBox_DoubleClick(object sender, EventArgs e)
        {
            if (charactersListBox.SelectedIndex < 0 || charactersListBox.SelectedIndex >= _settingsService.Settings.AvailableCharacters.Count)
                return;

            var charData = _settingsService.Settings.AvailableCharacters[charactersListBox.SelectedIndex];
            string newCharacterName = charData.DisplayName;
            Bitmap? newCharacterBitmap = null;

            newCharacterBitmap = CharacterResourceProvider.LoadCharacterBitmap(charData);

            if (newCharacterBitmap == null) return;

            Logger.Info($"Добавление персонажа: {newCharacterName}");
            _characterModeService.AddCharacter(
                newCharacterBitmap, 
                newCharacterName, 
                charData.DefaultScale, 
                charData.DefaultIsFlipped
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
        }

        private void activeCharactersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = _characterUIService.GetActiveCharacterSelectedIndex();
            if (selectedIndex >= 0 && selectedIndex < CharacterManager.Characters.Count)
            {
                _selectedCharacter = CharacterManager.Characters[selectedIndex];
                UpdateUIForSelectedCharacter();
            }
            else
            {
                _selectedCharacter = null;
                UpdateUIForSelectedCharacter();
            }
        }

        private void activeCharactersListBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                RemoveSelectedCharacter();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F2)
            {
                RenameSelectedCharacter();
                e.Handled = true;
            }
        }

        private void charactersListBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteSelectedGif();
                e.Handled = true;
            }
        }

        #endregion

        #region Обработчики событий настроек персонажа

        private void scaleTrackBar_Scroll(object? sender, EventArgs e)
        {
            if (_selectedCharacter != null)
            {
                float scale = scaleTrackBar.Value / 100f;
                _selectedCharacter.Scale = scale;
                scaleLabel.Text = $"🎨 Размер: {scaleTrackBar.Value}%";
                SaveCharacters();
                SaveSettings();
            }
        }

        private void flipCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            if (_selectedCharacter != null)
            {
                _selectedCharacter.IsFlipped = flipCheckBox.Checked;
                SaveCharacters();
                SaveSettings();
            }
        }

        private void lockCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            CharacterManager.SetLocked(lockCheckBox.Checked);
            SaveSettings();
        }

        #endregion

        #region Обработчики кнопок

        private void button1_Click(object sender, EventArgs e)
        {
            Logger.Info("Завершение работы (кнопка Выход)");
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            HideInTray();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Logger.Info("Очистка всех персонажей");
            CharacterManager.ClearCharacters();
            _selectedCharacter = null;
            _characterModeService.ResetSingleModeState();
            ForceUpdateActiveCharactersList();
            UpdateUIForSelectedCharacter();
            SaveCharacters();
        }

        private void removeCharacterButton_Click(object sender, EventArgs e)
        {
            RemoveSelectedCharacter();
        }

        private void settingsButton_Click(object? sender, EventArgs e)
        {
            ShowSettingsMenu(sender);
        }

        #endregion

        #region Обработчики событий меню

        private void loadCustomGifToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadCustomGif();
        }

        private void singleCharacterModeToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            bool isMultipleMode = singleCharacterModeToolStripMenuItem.Checked;
            bool isSingleMode = !isMultipleMode;
            
            bool previousCheckedState = singleCharacterModeToolStripMenuItem.Checked;
            
            SwitchCharacterMode(isSingleMode);
            
            if (_characterModeService.IsSingleCharacterMode != isSingleMode)
            {
                singleCharacterModeToolStripMenuItem.Checked = previousCheckedState;
            }
        }

        private void minimizeOnCloseToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            _trayIconService.MinimizeOnClose = minimizeOnCloseToolStripMenuItem.Checked;
            SaveSettings();
        }

        private void autoStartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (autoStartToolStripMenuItem.Checked)
            {
                _settingsService.AddToAutoStart();
                Logger.Info("Автозапуск включен");
            }
            else
            {
                _settingsService.RemoveFromAutoStart();
                Logger.Info("Автозапуск выключен");
            }
            _trayIconService.AutoStart = autoStartToolStripMenuItem.Checked;
        }

        private void showTrayIconToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            ShowInTaskbar = showTrayIconToolStripMenuItem.Checked;
            SaveSettings();
        }

        private void showMenuOnStartupToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            SaveSettings();
        }

        private void renameMenuItem_Click(object? sender, EventArgs e)
        {
            RenameSelectedCharacter();
        }

        private void deleteMenuItem_Click(object? sender, EventArgs e)
        {
            RemoveSelectedCharacter();
        }

        private void deleteGifMenuItem_Click(object? sender, EventArgs e)
        {
            DeleteSelectedGif();
        }

        private void renameAvailableCharacterMenuItem_Click(object? sender, EventArgs e)
        {
            RenameAvailableCharacter();
        }

        private void characterSettingsMenuItem_Click(object? sender, EventArgs e)
        {
            ShowCharacterSettings();
        }

        private void menuStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            // Пустой обработчик
        }

        #endregion

        #region Обработчики событий от TrayIconService

        private void OnCharacterAddRequested(object? sender, string characterName)
        {
            var charData = _settingsService.Settings.AvailableCharacters.FirstOrDefault(c => 
                c.DisplayName == characterName || c.OriginalName == characterName);

            Bitmap? bitmap = null;

            if (charData != null)
            {
                bitmap = _characterService.LoadCharacterBitmap(charData);
            }

            if (bitmap != null)
            {
                float scale = charData?.DefaultScale ?? CharacterManager.GlobalScale;
                bool isFlipped = charData?.DefaultIsFlipped ?? CharacterManager.GlobalFlip;

                Logger.Info($"Добавление персонажа из трея: {characterName}");
                _characterModeService.AddCharacter(bitmap, characterName, scale, isFlipped);
                
                if (CharacterManager.Characters.Count > 0)
                {
                    int lastIndex = CharacterManager.Characters.Count - 1;
                    if (!_characterModeService.IsSingleCharacterMode)
                    {
                        _characterUIService.SetActiveCharacterSelectedIndex(lastIndex);
                    }
                    else
                    {
                        lastIndex = 0; // В одиночном режиме всегда первый
                    }
                    _selectedCharacter = CharacterManager.Characters[lastIndex];
                    UpdateUIForSelectedCharacter();
                }
                
                ForceUpdateActiveCharactersList();
                SaveCharacters();
            }
        }

        private void OnTrayMinimizeOnCloseChanged(object? sender, EventArgs e)
        {
            minimizeOnCloseToolStripMenuItem.Checked = _trayIconService.MinimizeOnClose;
            SaveSettings();
        }

        private void OnTrayAutoStartChanged(object? sender, EventArgs e)
        {
            if (_trayIconService.AutoStart)
            {
                _settingsService.AddToAutoStart();
            }
            else
            {
                _settingsService.RemoveFromAutoStart();
            }
            autoStartToolStripMenuItem.Checked = _trayIconService.AutoStart;
        }

        private void OnTraySingleCharacterModeChanged(object? sender, EventArgs e)
        {
            SwitchCharacterMode(_trayIconService.SingleCharacterMode);
        }

        private void OnTrayShowInTaskbarChanged(object? sender, EventArgs e)
        {
            showTrayIconToolStripMenuItem.Checked = _trayIconService.ShowInTaskbar;
            ShowInTaskbar = _trayIconService.ShowInTaskbar;
            SaveSettings();
        }

        private void OnTrayShowMenuOnStartupChanged(object? sender, EventArgs e)
        {
            showMenuOnStartupToolStripMenuItem.Checked = _trayIconService.ShowMenuOnStartup;
            SaveSettings();
        }

        #endregion

        #region Вспомогательные методы

        private void HideInTray()
        {
            _trayIconService.HideParentWindow();
        }

        /// <summary>
        /// Показывает меню настроек
        /// </summary>
        private void ShowSettingsMenu(object? sender)
        {
            ContextMenuStrip settingsMenu = new ContextMenuStrip();
            
            ToolStripMenuItem singleModeItem = new ToolStripMenuItem("👥 Много персонажей")
            {
                Checked = !_characterModeService.IsSingleCharacterMode,
                CheckOnClick = true
            };
            singleModeItem.Click += (s, ev) =>
            {
                bool newSingleMode = !singleModeItem.Checked;
                SwitchCharacterMode(newSingleMode);
                singleModeItem.Checked = !_characterModeService.IsSingleCharacterMode;
            };
            settingsMenu.Items.Add(singleModeItem);
            
            settingsMenu.Items.Add(new ToolStripSeparator());
            
            ToolStripMenuItem showInTaskbarItem = new ToolStripMenuItem("Показывать в панели задач")
            {
                Checked = _settingsService.Settings.ShowInTaskbar,
                CheckOnClick = true
            };
            showInTaskbarItem.Click += (s, ev) =>
            {
                showTrayIconToolStripMenuItem.Checked = showInTaskbarItem.Checked;
                _settingsService.Settings.ShowInTaskbar = showInTaskbarItem.Checked;
                showTrayIconToolStripMenuItem_Click(showInTaskbarItem, ev);
            };
            settingsMenu.Items.Add(showInTaskbarItem);
            
            ToolStripMenuItem showMenuOnStartupItem = new ToolStripMenuItem("Открывать меню при запуске")
            {
                Checked = _settingsService.Settings.ShowMenuOnStartup,
                CheckOnClick = true
            };
            showMenuOnStartupItem.Click += (s, ev) =>
            {
                showMenuOnStartupToolStripMenuItem.Checked = showMenuOnStartupItem.Checked;
                _settingsService.Settings.ShowMenuOnStartup = showMenuOnStartupItem.Checked;
                showMenuOnStartupToolStripMenuItem_Click(showMenuOnStartupItem, ev);
            };
            settingsMenu.Items.Add(showMenuOnStartupItem);
            
            ToolStripMenuItem minimizeOnCloseItem = new ToolStripMenuItem("Сворачивать при закрытии")
            {
                Checked = _settingsService.Settings.MinimizeOnClose,
                CheckOnClick = true
            };
            minimizeOnCloseItem.Click += (s, ev) =>
            {
                minimizeOnCloseToolStripMenuItem.Checked = minimizeOnCloseItem.Checked;
                _settingsService.Settings.MinimizeOnClose = minimizeOnCloseItem.Checked;
                minimizeOnCloseToolStripMenuItem_Click(minimizeOnCloseItem, ev);
            };
            settingsMenu.Items.Add(minimizeOnCloseItem);
            
            ToolStripMenuItem autoStartItem = new ToolStripMenuItem("Автозапуск")
            {
                Checked = autoStartToolStripMenuItem.Checked,
                CheckOnClick = true
            };
            autoStartItem.Click += (s, ev) =>
            {
                autoStartToolStripMenuItem.Checked = autoStartItem.Checked;
                autoStartToolStripMenuItem_Click(autoStartItem, ev);
            };
            settingsMenu.Items.Add(autoStartItem);
            
            settingsMenu.Items.Add(new ToolStripSeparator());
            
            ToolStripMenuItem themeMenuItem = new ToolStripMenuItem("🎨 Тема");
            
            ToolStripMenuItem lightThemeItem = new ToolStripMenuItem("☀️ Светлая")
            {
                Checked = _themeService.CurrentTheme == ThemeService.ThemeMode.Light,
                CheckOnClick = false
            };
            lightThemeItem.Click += (s, ev) => ChangeTheme(ThemeService.ThemeMode.Light);
            themeMenuItem.DropDownItems.Add(lightThemeItem);
            
            ToolStripMenuItem darkThemeItem = new ToolStripMenuItem("🌙 Темная")
            {
                Checked = _themeService.CurrentTheme == ThemeService.ThemeMode.Dark,
                CheckOnClick = false
            };
            darkThemeItem.Click += (s, ev) => ChangeTheme(ThemeService.ThemeMode.Dark);
            themeMenuItem.DropDownItems.Add(darkThemeItem);
            
            ToolStripMenuItem blin4iikThemeItem = new ToolStripMenuItem("🎭 Blin4iik")
            {
                Checked = _themeService.CurrentTheme == ThemeService.ThemeMode.Blin4iik,
                CheckOnClick = false
            };
            blin4iikThemeItem.Click += (s, ev) => ChangeTheme(ThemeService.ThemeMode.Blin4iik);
            themeMenuItem.DropDownItems.Add(blin4iikThemeItem);
            
            ToolStripMenuItem systemThemeItem = new ToolStripMenuItem("💻 Системная")
            {
                Checked = _themeService.CurrentTheme == ThemeService.ThemeMode.System,
                CheckOnClick = false
            };
            systemThemeItem.Click += (s, ev) => ChangeTheme(ThemeService.ThemeMode.System);
            themeMenuItem.DropDownItems.Add(systemThemeItem);
            
            settingsMenu.Items.Add(themeMenuItem);
            
            if (sender is Button btn)
            {
                settingsMenu.Show(btn, new Point(0, btn.Height));
            }
        }

        #endregion
    }
}

