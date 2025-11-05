using DesktopKonata.Utility;
using DesktopKonata.Services;
using System.Windows.Forms;
using Microsoft.Win32;

namespace DesktopKonata.Forms
{
    public partial class Menu : Form
    {
        private const string AppName = "DesktopDance";

        private readonly TrayIconService _trayIconService;
        private readonly SettingsService _settingsService;
        private readonly CharacterService _characterService;
        private readonly CharacterManagementService _characterManagementService;
        private CharacterUIService _characterUIService = null!; // Инициализируется после InitializeComponent()
        private CharacterEntity? _selectedCharacter;
        private System.Windows.Forms.Timer _updateTimer = null!; // Инициализируется в SetupUpdateTimer()
        private string? _currentCharacterName; // Имя текущего персонажа в одиночном режиме
        private Point? _savedCharacterPosition; // Сохранённая позиция персонажа

        public Menu()
        {
            InitializeComponent();
            
            _settingsService = new SettingsService();
            _settingsService.InitializeAvailableCharacters();
            
            _characterService = new CharacterService();
            _characterManagementService = new CharacterManagementService(_settingsService.Settings);
            
            _characterUIService = new CharacterUIService(
                charactersListBox,
                activeCharactersListBox,
                activeCharactersLabel,
                _settingsService.Settings
            );
            
            _trayIconService = new TrayIconService(this, DesktopDance.Properties.Resources.blin4iikIco);
            _trayIconService.CreateContextMenu();
            _trayIconService.CharacterAddRequested += OnCharacterAddRequested;
            _trayIconService.MinimizeOnCloseChanged += OnTrayMinimizeOnCloseChanged;
            _trayIconService.AutoStartChanged += OnTrayAutoStartChanged;
            _trayIconService.SingleCharacterModeChanged += OnTraySingleCharacterModeChanged;
            _trayIconService.ShowInTaskbarChanged += OnTrayShowInTaskbarChanged;
            _trayIconService.ShowMenuOnStartupChanged += OnTrayShowMenuOnStartupChanged;
            
            LoadSettings();
            ApplySettings();
            UpdateUIForMode(); // Применяем режим персонажей при запуске
            SetupUpdateTimer();
            
            if (!_settingsService.Settings.ShowMenuOnStartup)
            {
                WindowState = FormWindowState.Minimized;
                this.Visible = false;
            }
        }

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

                if (_settingsService.Settings.SingleCharacterMode)
                {
                    if (_currentCharacterName == characterName && CharacterManager.Characters.Count > 0)
                    {
                        return;
                    }

                    if (CharacterManager.Characters.Count > 0)
                    {
                        _savedCharacterPosition = CharacterManager.Characters[0].Location;
                        CharacterManager.ClearCharacters();
                    }
                    _characterService.AddCharacter(
                        bitmap, 
                        characterName, 
                        scale,
                        isFlipped,
                        _savedCharacterPosition
                    );
                    _currentCharacterName = characterName;

                    if (CharacterManager.Characters.Count > 0)
                    {
                        _selectedCharacter = CharacterManager.Characters[0];
                        UpdateUIForSelectedCharacter();
                    }
                }
                else
                {
                    _characterService.AddCharacter(
                        bitmap, 
                        characterName, 
                        scale,
                        isFlipped
                    );
                    
                    if (CharacterManager.Characters.Count > 0)
                    {
                        _characterUIService.SetActiveCharacterSelectedIndex(CharacterManager.Characters.Count - 1);
                        _selectedCharacter = CharacterManager.Characters[CharacterManager.Characters.Count - 1];
                        UpdateUIForSelectedCharacter();
                    }
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
            singleCharacterModeToolStripMenuItem.Checked = !_trayIconService.SingleCharacterMode;
            
            if (_trayIconService.SingleCharacterMode && CharacterManager.Characters.Count > 1)
            {
                var firstCharacter = CharacterManager.Characters[0];
                var savedPosition = firstCharacter.Location;
                string characterName = firstCharacter.Name;
                float characterScale = firstCharacter.Scale;
                bool characterIsFlipped = firstCharacter.IsFlipped;
                Bitmap characterImage = (Bitmap)firstCharacter.AnimatedImage.Clone();
                
                CharacterManager.ClearCharacters();
                
                _characterService.AddCharacter(
                    characterImage, 
                    characterName, 
                    characterScale, 
                    characterIsFlipped,
                    savedPosition
                );
                
                _currentCharacterName = characterName;
                _selectedCharacter = CharacterManager.Characters[0];
                UpdateUIForSelectedCharacter();
            }
            else if (!_trayIconService.SingleCharacterMode)
            {
                _currentCharacterName = null;
                _savedCharacterPosition = null;
            }
            
            SaveSettings();
            UpdateUIForMode();
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

        private void SetupUpdateTimer()
        {
            _updateTimer = new System.Windows.Forms.Timer();
            _updateTimer.Interval = 2000; // Обновлять каждые 2 секунды (реже)
            _updateTimer.Tick += UpdateTimer_Tick;
            _updateTimer.Start();
        }

        private void UpdateTimer_Tick(object? sender, EventArgs e)
        {
            _characterUIService.UpdateActiveCharactersList(_settingsService.Settings.SingleCharacterMode);
        }


        private void LoadSettings()
        {
            bool isAutoStart = _settingsService.IsInAutoStart();
            autoStartToolStripMenuItem.Checked = isAutoStart;
            _trayIconService.AutoStart = isAutoStart;
            
            _trayIconService.SingleCharacterMode = _settingsService.Settings.SingleCharacterMode;
        }

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
            _trayIconService.SingleCharacterMode = _settingsService.Settings.SingleCharacterMode;
            _trayIconService.ShowInTaskbar = _settingsService.Settings.ShowInTaskbar;
            _trayIconService.ShowMenuOnStartup = _settingsService.Settings.ShowMenuOnStartup;
            
            UpdateUIForMode();
        }

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
        }

        private void SaveCharacters()
        {
            _settingsService.Settings.SavedCharacters.Clear();
            
            foreach (var character in CharacterManager.Characters)
            {
                var charData = new CharacterData
                {
                    Name = character.Name,
                    FilePath = "", // Для встроенных персонажей оставляем пустым
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

        // Загрузка персонажей из настроек
        private void LoadCharacters()
        {
            try
            {
                foreach (var charData in _settingsService.Settings.SavedCharacters)
                {
                    Bitmap? characterBitmap = null;
                    
                    // Если есть путь к файлу - это пользовательский GIF
                    if (!string.IsNullOrEmpty(charData.FilePath) && File.Exists(charData.FilePath))
                    {
                        characterBitmap = new Bitmap(charData.FilePath);
                    }
                    else
                    {
                        // Встроенный персонаж
                        characterBitmap = charData.Name switch
                        {
                            "blin4iik Dance" => DesktopDance.Properties.Resources.blin4iikDance,
                            "Konata Love" => DesktopDance.Properties.Resources.KonataLoveDancingGif,
                            _ => null
                        };
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
                    }
                }
                
                ForceUpdateActiveCharactersList();
            }
            catch
            {
                // Если ошибка при загрузке - игнорируем
            }
        }

        private void UpdateUIForMode()
        {
            bool isSingleMode = _settingsService.Settings.SingleCharacterMode;
            
            if (this.Controls.Contains(activeCharactersPanel))
            {
                activeCharactersPanel.Visible = !isSingleMode;
            }
            
            if (this.Controls.Contains(availableCharactersPanel))
            {
                if (isSingleMode)
                {
                    charactersLabel.Text = "🎭 Выберите персонажа";
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

        private void Menu_Load(object sender, EventArgs args)
        {
            LoadCustomGifList();
            
            // Загружаем сохранённых персонажей
            LoadCharacters();

            // Окончательно определяем видимость окна управления при запуске
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

        // Загрузка списка доступных персонажей
        private void LoadCustomGifList()
        {
            _characterUIService.LoadAvailableCharactersList();
            
            // Обновляем список пользовательских персонажей в трее
            var customCharacters = _settingsService.Settings.AvailableCharacters.Skip(2).ToList();
            _trayIconService.UpdateCustomCharacters(customCharacters);
        }

        private void charactersListBox_DoubleClick(object sender, EventArgs e)
        {
            if (charactersListBox.SelectedIndex < 0 || charactersListBox.SelectedIndex >= _settingsService.Settings.AvailableCharacters.Count)
                return;

            var charData = _settingsService.Settings.AvailableCharacters[charactersListBox.SelectedIndex];
            string newCharacterName = charData.DisplayName;
            Bitmap? newCharacterBitmap = null;

            // Получаем bitmap для персонажа
            if (string.IsNullOrEmpty(charData.FilePath))
            {
                // Встроенный персонаж
                newCharacterBitmap = charData.OriginalName switch
                {
                    "blin4iik Dance" => DesktopDance.Properties.Resources.blin4iikDance,
                    "Konata Love" => DesktopDance.Properties.Resources.KonataLoveDancingGif,
                    _ => null
                };
            }
            else
            {
                // Пользовательский GIF
                if (File.Exists(charData.FilePath))
                {
                    newCharacterBitmap = new Bitmap(charData.FilePath);
                }
            }

            if (newCharacterBitmap == null) return;

            // В режиме одного персонажа
            if (_settingsService.Settings.SingleCharacterMode)
            {
                // Если это тот же персонаж - не делаем ничего
                if (_currentCharacterName == newCharacterName && CharacterManager.Characters.Count > 0)
                {
                    return;
                }

                // Сохраняем позицию текущего персонажа перед удалением
                if (CharacterManager.Characters.Count > 0)
                {
                    _savedCharacterPosition = CharacterManager.Characters[0].Location;
                    CharacterManager.ClearCharacters();
                }

                // Добавляем нового персонажа
                _characterService.AddCharacter(
                    newCharacterBitmap, 
                    newCharacterName, 
                    charData.DefaultScale, 
                    charData.DefaultIsFlipped,
                    _savedCharacterPosition
                );
                _currentCharacterName = newCharacterName;

                // Применяем индивидуальные настройки персонажа
                if (CharacterManager.Characters.Count > 0)
                {
                    CharacterManager.Characters[0].Scale = charData.DefaultScale;
                    CharacterManager.Characters[0].IsFlipped = charData.DefaultIsFlipped;
                    
                    // Восстанавливаем позицию, если была сохранена
                    if (_savedCharacterPosition.HasValue)
                    {
                        CharacterManager.Characters[0].Location = _savedCharacterPosition.Value;
                    }
                }

                // Выбираем персонажа для настройки
                _selectedCharacter = CharacterManager.Characters[0];
                UpdateUIForSelectedCharacter();
            }
            else
            {
                // В режиме нескольких персонажей - просто добавляем
                _characterService.AddCharacter(
                    newCharacterBitmap, 
                    newCharacterName, 
                    charData.DefaultScale, 
                    charData.DefaultIsFlipped
                );
                
                // Применяем индивидуальные настройки персонажа
                if (CharacterManager.Characters.Count > 0)
                {
                    var newChar = CharacterManager.Characters[CharacterManager.Characters.Count - 1];
                    newChar.Scale = charData.DefaultScale;
                    newChar.IsFlipped = charData.DefaultIsFlipped;
                }
                
                ForceUpdateActiveCharactersList();
                
                // Автоматически выбираем добавленного персонажа
                if (CharacterManager.Characters.Count > 0)
                {
                    _characterUIService.SetActiveCharacterSelectedIndex(CharacterManager.Characters.Count - 1);
                }
            }
            
            SaveCharacters();
        }

        private void ForceUpdateActiveCharactersList()
        {
            _characterUIService.ForceUpdateActiveCharactersList(_settingsService.Settings.SingleCharacterMode);
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

        private void UpdateUIForSelectedCharacter()
        {
            if (_selectedCharacter != null)
            {
                // Отключаем обработчики временно
                scaleTrackBar.ValueChanged -= scaleTrackBar_Scroll;
                flipCheckBox.CheckedChanged -= flipCheckBox_CheckedChanged;
                lockCheckBox.CheckedChanged -= lockCheckBox_CheckedChanged;

                // Обновляем значения
                scaleTrackBar.Value = (int)(_selectedCharacter.Scale * 100);
                scaleLabel.Text = $"🎨 Размер: {(int)(_selectedCharacter.Scale * 100)}%";
                flipCheckBox.Checked = _selectedCharacter.IsFlipped;
                lockCheckBox.Checked = CharacterManager.IsLocked;

                // Включаем элементы управления
                scaleTrackBar.Enabled = true;
                flipCheckBox.Enabled = true;
                scaleLabel.Text = $"🎨 Размер: {(int)(_selectedCharacter.Scale * 100)}%";

                // Включаем обработчики обратно
                scaleTrackBar.ValueChanged += scaleTrackBar_Scroll;
                flipCheckBox.CheckedChanged += flipCheckBox_CheckedChanged;
                lockCheckBox.CheckedChanged += lockCheckBox_CheckedChanged;
            }
            else
            {
                // Нет выбранного персонажа
                scaleTrackBar.Enabled = false;
                flipCheckBox.Enabled = false;
                scaleLabel.Text = "🎨 Размер: выберите персонажа";
            }
        }

        private void removeCharacterButton_Click(object sender, EventArgs e)
        {
            RemoveSelectedCharacter();
        }

        private void HideInTray()
        {
            _trayIconService.HideParentWindow();
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
                    Application.Exit();
                }
            }
        }


        private void menuStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            HideInTray();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            CharacterManager.ClearCharacters();
            _selectedCharacter = null;
            _currentCharacterName = null;
            _savedCharacterPosition = null;
            ForceUpdateActiveCharactersList();
            UpdateUIForSelectedCharacter();
            SaveCharacters();
        }

        private void loadCustomGifToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadCustomGif();
        }

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
                    
                    // Копируем GIF в AppData
                    string copiedFilePath = AppSettings.CopyGifToAppData(sourceFilePath);
                    string fileName = Path.GetFileNameWithoutExtension(copiedFilePath);
                    
                    // Добавляем в список пользовательских GIF, если его там нет
                    string gifFileName = Path.GetFileName(copiedFilePath);
                    if (!_settingsService.Settings.CustomGifFiles.Contains(gifFileName))
                    {
                        _settingsService.Settings.CustomGifFiles.Add(gifFileName);
                        
                        // Добавляем в AvailableCharacters
                        _settingsService.Settings.AvailableCharacters.Add(new AvailableCharacterData
                        {
                            OriginalName = fileName,
                            DisplayName = fileName,
                            FilePath = copiedFilePath,
                            DefaultScale = 1.0f,
                            DefaultIsFlipped = false
                        });
                        
                        // Добавляем в список персонажей
                        _characterUIService.AddToAvailableList(fileName, "📎");
                        _settingsService.Settings.Save();
                        
                        // Обновляем список в трее
                        LoadCustomGifList();
                    }
                    
                    Bitmap gifBitmap = new(copiedFilePath);

                    // В режиме одного персонажа
                    if (_settingsService.Settings.SingleCharacterMode)
                    {
                        // Сохраняем позицию текущего персонажа
                        if (CharacterManager.Characters.Count > 0)
                        {
                            _savedCharacterPosition = CharacterManager.Characters[0].Location;
                            CharacterManager.ClearCharacters();
                        }

                        // Добавляем нового персонажа
                        _characterService.AddCharacter(
                            gifBitmap, 
                            fileName, 
                            CharacterManager.GlobalScale, 
                            CharacterManager.GlobalFlip,
                            _savedCharacterPosition
                        );
                        _currentCharacterName = fileName;

                        // Восстанавливаем позицию
                        if (_savedCharacterPosition.HasValue && CharacterManager.Characters.Count > 0)
                        {
                            CharacterManager.Characters[0].Location = _savedCharacterPosition.Value;
                        }

                        // Выбираем персонажа
                        _selectedCharacter = CharacterManager.Characters[0];
                        UpdateUIForSelectedCharacter();
                    }
                    else
                    {
                        // В режиме нескольких персонажей
                        _characterService.AddCharacter(
                            gifBitmap, 
                            fileName, 
                            CharacterManager.GlobalScale, 
                            CharacterManager.GlobalFlip
                        );
                        ForceUpdateActiveCharactersList();
                        
                        // Автоматически выбираем добавленного персонажа
                        if (CharacterManager.Characters.Count > 0)
                        {
                            _characterUIService.SetActiveCharacterSelectedIndex(CharacterManager.Characters.Count - 1);
                        }
                    }
                    
                    SaveCharacters();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось загрузить GIF: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

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

       
        private void singleCharacterModeToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            // Чекбокс "Много персонажей": Checked = true (много), false (одиночный)
            // SingleCharacterMode: true (одиночный), false (много) - инвертируем
            bool isMultipleMode = singleCharacterModeToolStripMenuItem.Checked;
            bool isSingleMode = !isMultipleMode;
            
            // Сохраняем настройку
            _settingsService.Settings.SingleCharacterMode = isSingleMode;
            _trayIconService.SingleCharacterMode = isSingleMode;
            
            // Если ВКЛЮЧАЕМ одиночный режим (выключаем много персонажей) и есть несколько персонажей
            if (isSingleMode && CharacterManager.Characters.Count > 1)
            {
                var result = MessageBox.Show(
                    "Включён режим одного персонажа.\nНа экране останется только первый персонаж.\nПродолжить?",
                    "Режим одного персонажа",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Оставляем только первого персонажа
                    var firstCharacter = CharacterManager.Characters[0];
                    var savedPosition = firstCharacter.Location;
                    string characterName = firstCharacter.Name;
                    float characterScale = firstCharacter.Scale;
                    bool characterIsFlipped = firstCharacter.IsFlipped;
                    Bitmap characterImage = (Bitmap)firstCharacter.AnimatedImage.Clone();
                    
                    CharacterManager.ClearCharacters();
                    
                    // Воссоздаём первого
                    _characterService.AddCharacter(
                        characterImage, 
                        characterName, 
                        characterScale, 
                        characterIsFlipped,
                        savedPosition
                    );
                    
                    _currentCharacterName = characterName;
                    _selectedCharacter = CharacterManager.Characters[0];
                    UpdateUIForSelectedCharacter();
                }
                else
                {
                    // Отменяем изменения
                    singleCharacterModeToolStripMenuItem.Checked = true; // Возвращаем чекбокс "Много персонажей"
                    _settingsService.Settings.SingleCharacterMode = false; // Возвращаем множественный режим
                    _trayIconService.SingleCharacterMode = false;
                    return;
                }
            }
            else if (isMultipleMode)
            {
                // Переключаемся в режим нескольких персонажей
                _currentCharacterName = null;
                _savedCharacterPosition = null;
                ForceUpdateActiveCharactersList();
            }
            
            SaveSettings();
            UpdateUIForMode();
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
            }
            else
            {
                _settingsService.RemoveFromAutoStart();
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

        // Обработчик контекстного меню - Переименовать
        private void renameMenuItem_Click(object? sender, EventArgs e)
        {
            RenameSelectedCharacter();
        }

        // Обработчик контекстного меню - Удалить
        private void deleteMenuItem_Click(object? sender, EventArgs e)
        {
            RemoveSelectedCharacter();
        }

        // Обработчик горячих клавиш
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

        // Метод для переименования персонажа
        private void RenameSelectedCharacter()
        {
            int selectedIndex = _characterUIService.GetActiveCharacterSelectedIndex();
            if (selectedIndex < 0)
                return;

            if (_characterManagementService.ShowRenameActiveCharacterDialog(selectedIndex, out string newName))
            {
                if (_characterManagementService.RenameActiveCharacter(selectedIndex, newName))
                {
                    ForceUpdateActiveCharactersList();
                    SaveCharacters();
                }
            }
        }

        // Метод для удаления персонажа
        private void RemoveSelectedCharacter()
        {
            if (activeCharactersListBox.SelectedIndex >= 0)
            {
                int indexToRemove = activeCharactersListBox.SelectedIndex;
                CharacterManager.RemoveCharacter(indexToRemove);
                _selectedCharacter = null;
                ForceUpdateActiveCharactersList();
                UpdateUIForSelectedCharacter();
                SaveCharacters();
            }
        }

        // Обработчик контекстного меню - Удалить GIF
        private void deleteGifMenuItem_Click(object? sender, EventArgs e)
        {
            DeleteSelectedGif();
        }

        // Обработчик горячей клавиши Delete для списка доступных GIF
        private void charactersListBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteSelectedGif();
                e.Handled = true;
            }
        }

        // Метод для удаления пользовательского GIF
        private void DeleteSelectedGif()
        {
            int selectedIndex = _characterUIService.GetAvailableCharacterSelectedIndex();
            if (selectedIndex < 2) // Первые 2 - встроенные персонажи
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
                    MessageBox.Show("GIF успешно удалён!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"Не удалось удалить GIF: {errorMessage}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Обработчик переименования доступного персонажа
        private void renameAvailableCharacterMenuItem_Click(object? sender, EventArgs e)
        {
            RenameAvailableCharacter();
        }

        // Обработчик настроек персонажа
        private void characterSettingsMenuItem_Click(object? sender, EventArgs e)
        {
            ShowCharacterSettings();
        }

        // Метод для переименования доступного персонажа
        private void RenameAvailableCharacter()
        {
            int selectedIndex = _characterUIService.GetAvailableCharacterSelectedIndex();
            if (selectedIndex < 0)
                return;

            if (_characterManagementService.ShowRenameDialog(selectedIndex, out string newName))
            {
                if (_characterManagementService.RenameAvailableCharacter(selectedIndex, newName))
                {
                    LoadCustomGifList();
                }
            }
        }

        // Метод для настройки персонажа (масштаб и отзеркаливание по умолчанию)
        private void ShowCharacterSettings()
        {
            int selectedIndex = _characterUIService.GetAvailableCharacterSelectedIndex();
            if (selectedIndex < 0)
                return;

            if (_characterManagementService.ShowCharacterSettingsDialog(selectedIndex, out float scale, out bool isFlipped))
            {
                if (_characterManagementService.UpdateCharacterDefaultSettings(selectedIndex, scale, isFlipped))
                {
                    MessageBox.Show("Настройки сохранены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void settingsButton_Click(object? sender, EventArgs e)
        {
            ContextMenuStrip settingsMenu = new ContextMenuStrip();
            
            ToolStripMenuItem singleModeItem = new ToolStripMenuItem("👥 Много персонажей")
            {
                Checked = !_settingsService.Settings.SingleCharacterMode,
                CheckOnClick = true
            };
            singleModeItem.Click += (s, ev) =>
            {
                _settingsService.Settings.SingleCharacterMode = !singleModeItem.Checked;
                _trayIconService.SingleCharacterMode = !singleModeItem.Checked;
                OnTraySingleCharacterModeChanged(_trayIconService, EventArgs.Empty);
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
            
            // Сворачивать при закрытии
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
            autoStartItem.Click += (s, e) => autoStartToolStripMenuItem_Click(s!, e);
            settingsMenu.Items.Add(autoStartItem);
            
            // Показываем меню около кнопки
            if (sender is Button btn)
            {
                settingsMenu.Show(btn, new Point(0, btn.Height));
            }
        }
    }
}
