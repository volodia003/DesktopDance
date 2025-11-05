using DesktopDance.Utility;
using DesktopDance.Services;
using System.Windows.Forms;
using Microsoft.Win32;

namespace DesktopDance.Forms
{
    /// <summary>
    /// Главная форма управления Desktop Dance
    /// Разбита на partial классы для лучшей организации:
    /// - Menu.Main.cs - конструктор и основная инициализация
    /// - Menu.Characters.cs - работа с персонажами
    /// - Menu.Settings.cs - работа с настройками
    /// - Menu.Events.cs - обработчики событий UI
    /// </summary>
    public partial class Menu : Form
    {
        private const string AppName = "DesktopDance";

        // Сервисы приложения
        private readonly TrayIconService _trayIconService;
        private readonly SettingsService _settingsService;
        private readonly CharacterService _characterService;
        private readonly CharacterManagementService _characterManagementService;
        private readonly CharacterModeService _characterModeService;
        private readonly ThemeService _themeService;
        private CharacterUIService _characterUIService = null!;
        
        // Состояние
        private CharacterEntity? _selectedCharacter;
        private System.Windows.Forms.Timer _updateTimer = null!;

        /// <summary>
        /// Конструктор главной формы
        /// </summary>
        public Menu()
        {
            try
            {
                Logger.Info("=== Запуск Desktop Dance ===");
                InitializeComponent();
                
                // Инициализация сервисов
                _settingsService = new SettingsService();
                _settingsService.InitializeAvailableCharacters();
            
                // Определение темы
                var themeMode = _settingsService.Settings.Theme switch
                {
                    "Dark" => ThemeService.ThemeMode.Dark,
                    "Blin4iik" => ThemeService.ThemeMode.Blin4iik,
                    "System" => ThemeService.ThemeMode.System,
                    _ => ThemeService.ThemeMode.Light
                };
                _themeService = new ThemeService(themeMode);
                _themeService.ThemeChanged += OnThemeChanged;
                
                // Инициализация остальных сервисов
                _characterService = new CharacterService();
                _characterManagementService = new CharacterManagementService(_settingsService.Settings);
                _characterModeService = new CharacterModeService(_characterService, _settingsService.Settings);
                
                _characterUIService = new CharacterUIService(
                    charactersListBox,
                    activeCharactersListBox,
                    activeCharactersLabel,
                    _settingsService.Settings
                );
                
                // Инициализация трея
                _trayIconService = new TrayIconService(this, DesktopDance.Properties.Resources.blin4iikIco);
                _trayIconService.CreateContextMenu();
                _trayIconService.CharacterAddRequested += OnCharacterAddRequested;
                _trayIconService.MinimizeOnCloseChanged += OnTrayMinimizeOnCloseChanged;
                _trayIconService.AutoStartChanged += OnTrayAutoStartChanged;
                _trayIconService.SingleCharacterModeChanged += OnTraySingleCharacterModeChanged;
                _trayIconService.ShowInTaskbarChanged += OnTrayShowInTaskbarChanged;
                _trayIconService.ShowMenuOnStartupChanged += OnTrayShowMenuOnStartupChanged;
                
                // Применение настроек
                LoadSettings();
                ApplySettings();
                UpdateUIForMode();
                SetupUpdateTimer();
                
                // Применение темы
                _themeService.ApplyTheme(this);
                
                // Настройка видимости окна
                if (!_settingsService.Settings.ShowMenuOnStartup)
                {
                    WindowState = FormWindowState.Minimized;
                    this.Visible = false;
                }
                
                Logger.Info("Menu инициализировано успешно");
            }
            catch (Exception ex)
            {
                Logger.Critical("Критическая ошибка при инициализации Menu", ex);
                MessageBox.Show(
                    $"Произошла критическая ошибка при запуске приложения:\n{ex.Message}\n\nЛог сохранен в: {Logger.GetLogFilePath()}", 
                    "Ошибка", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error
                );
                throw;
            }
        }
    }
}

