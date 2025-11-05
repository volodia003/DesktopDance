using System.Windows.Forms;

namespace DesktopKonata.Services
{
    /// <summary>
    /// Сервис для управления иконкой в системном трее и её контекстным меню
    /// </summary>
    public class TrayIconService : IDisposable
    {
        private readonly NotifyIcon _notifyIcon;
        private readonly Form _parentForm;
        private ContextMenuStrip? _trayContextMenu;
        private ToolStripMenuItem? _charactersMenuItem;
        private ToolStripMenuItem? _minimizeOnCloseMenuItem;
        private ToolStripMenuItem? _autoStartMenuItem;
        private ToolStripMenuItem? _singleCharacterModeMenuItem;
        private ToolStripMenuItem? _showInTaskbarMenuItem;
        private ToolStripMenuItem? _showMenuOnStartupMenuItem;

        public event EventHandler? MinimizeOnCloseChanged;
        public event EventHandler? AutoStartChanged;
        public event EventHandler? SingleCharacterModeChanged;
        public event EventHandler? ShowInTaskbarChanged;
        public event EventHandler? ShowMenuOnStartupChanged;
        public event EventHandler<string>? CharacterAddRequested;

        public bool MinimizeOnClose
        {
            get => _minimizeOnCloseMenuItem?.Checked ?? false;
            set
            {
                if (_minimizeOnCloseMenuItem != null)
                    _minimizeOnCloseMenuItem.Checked = value;
            }
        }

        public bool AutoStart
        {
            get => _autoStartMenuItem?.Checked ?? false;
            set
            {
                if (_autoStartMenuItem != null)
                    _autoStartMenuItem.Checked = value;
            }
        }

        public bool SingleCharacterMode
        {
            get => !(_singleCharacterModeMenuItem?.Checked ?? true);
            set
            {
                if (_singleCharacterModeMenuItem != null)
                    _singleCharacterModeMenuItem.Checked = !value;
            }
        }

        public void TriggerSingleCharacterModeChanged()
        {
            SingleCharacterModeChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool ShowInTaskbar
        {
            get => _showInTaskbarMenuItem?.Checked ?? false;
            set
            {
                if (_showInTaskbarMenuItem != null)
                    _showInTaskbarMenuItem.Checked = value;
            }
        }

        public bool ShowMenuOnStartup
        {
            get => _showMenuOnStartupMenuItem?.Checked ?? false;
            set
            {
                if (_showMenuOnStartupMenuItem != null)
                    _showMenuOnStartupMenuItem.Checked = value;
            }
        }

        public TrayIconService(Form parentForm, Icon icon)
        {
            _parentForm = parentForm;
            
            _notifyIcon = new NotifyIcon
            {
                Visible = true,
                Icon = icon,
                Text = "Konata is here!",
                BalloonTipIcon = ToolTipIcon.Info,
                BalloonTipTitle = "Hey",
                BalloonTipText = "I`ll be here!"
            };

            _notifyIcon.MouseClick += NotifyIcon_MouseClick;
            _notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
        }

        public void CreateContextMenu()
        {
            _trayContextMenu = new ContextMenuStrip();

            // Показать окно управления
            var showMenuItem = new ToolStripMenuItem("🎮 Управление");
            var defaultFont = SystemFonts.MessageBoxFont;
            if (defaultFont != null)
            {
                showMenuItem.Font = new Font(defaultFont, FontStyle.Bold);
            }
            showMenuItem.Click += (s, e) => ShowParentWindow();
            _trayContextMenu.Items.Add(showMenuItem);

            _trayContextMenu.Items.Add(new ToolStripSeparator());

            // Персонажи
            _charactersMenuItem = new ToolStripMenuItem("Персонажи");
            
            var blin4iikMenuItem = new ToolStripMenuItem("🎭 blin4iik Dance");
            blin4iikMenuItem.Click += (s, e) => CharacterAddRequested?.Invoke(this, "blin4iik Dance");
            _charactersMenuItem.DropDownItems.Add(blin4iikMenuItem);

            var konataLoveMenuItem = new ToolStripMenuItem("💝 Konata Love");
            konataLoveMenuItem.Click += (s, e) => CharacterAddRequested?.Invoke(this, "Konata Love");
            _charactersMenuItem.DropDownItems.Add(konataLoveMenuItem);

            _trayContextMenu.Items.Add(_charactersMenuItem);

            // Настройки
            var settingsMenuItem = new ToolStripMenuItem("⚙️ Настройки");

            _singleCharacterModeMenuItem = new ToolStripMenuItem("👥 Много персонажей")
            {
                CheckOnClick = true,
                Checked = false // По умолчанию одиночный режим (чекбокс снят)
            };
            _singleCharacterModeMenuItem.Click += (s, e) => SingleCharacterModeChanged?.Invoke(this, EventArgs.Empty);
            settingsMenuItem.DropDownItems.Add(_singleCharacterModeMenuItem);

            settingsMenuItem.DropDownItems.Add(new ToolStripSeparator());

            _minimizeOnCloseMenuItem = new ToolStripMenuItem("Сворачивать при закрытии")
            {
                CheckOnClick = true,
                Checked = true
            };
            _minimizeOnCloseMenuItem.Click += (s, e) => MinimizeOnCloseChanged?.Invoke(this, EventArgs.Empty);
            settingsMenuItem.DropDownItems.Add(_minimizeOnCloseMenuItem);

            _autoStartMenuItem = new ToolStripMenuItem("Автозапуск")
            {
                CheckOnClick = true
            };
            _autoStartMenuItem.Click += (s, e) => AutoStartChanged?.Invoke(this, EventArgs.Empty);
            settingsMenuItem.DropDownItems.Add(_autoStartMenuItem);

            _showInTaskbarMenuItem = new ToolStripMenuItem("Показывать в панели задач")
            {
                CheckOnClick = true
            };
            _showInTaskbarMenuItem.Click += (s, e) => ShowInTaskbarChanged?.Invoke(this, EventArgs.Empty);
            settingsMenuItem.DropDownItems.Add(_showInTaskbarMenuItem);

            _showMenuOnStartupMenuItem = new ToolStripMenuItem("Открывать меню при запуске")
            {
                CheckOnClick = true
            };
            _showMenuOnStartupMenuItem.Click += (s, e) => ShowMenuOnStartupChanged?.Invoke(this, EventArgs.Empty);
            settingsMenuItem.DropDownItems.Add(_showMenuOnStartupMenuItem);

            _trayContextMenu.Items.Add(settingsMenuItem);

            _trayContextMenu.Items.Add(new ToolStripSeparator());

            // Очистить
            var clearMenuItem = new ToolStripMenuItem("🗑️ Очистить");
            clearMenuItem.Click += (s, e) => Utility.CharacterManager.ClearCharacters();
            _trayContextMenu.Items.Add(clearMenuItem);

            _trayContextMenu.Items.Add(new ToolStripSeparator());

            // Выход
            var exitMenuItem = new ToolStripMenuItem("❌ Выход");
            exitMenuItem.Click += (s, e) => 
            { 
                _notifyIcon.Visible = false; 
                Application.Exit(); 
            };
            _trayContextMenu.Items.Add(exitMenuItem);

            _notifyIcon.ContextMenuStrip = _trayContextMenu;
        }

        /// <summary>
        /// Обновляет список пользовательских персонажей в меню трея
        /// </summary>
        public void UpdateCustomCharacters(List<Utility.AvailableCharacterData> customCharacters)
        {
            if (_charactersMenuItem == null)
                return;

            // Удаляем все пункты после встроенных персонажей
            while (_charactersMenuItem.DropDownItems.Count > 2)
            {
                _charactersMenuItem.DropDownItems.RemoveAt(2);
            }

            // Если есть пользовательские персонажи, добавляем разделитель
            if (customCharacters.Count > 0)
            {
                _charactersMenuItem.DropDownItems.Add(new ToolStripSeparator());

                // Добавляем пользовательских персонажей
                foreach (var charData in customCharacters)
                {
                    var customMenuItem = new ToolStripMenuItem($"📎 {charData.DisplayName}");
                    string characterName = charData.DisplayName;
                    customMenuItem.Click += (s, e) => CharacterAddRequested?.Invoke(this, characterName);
                    _charactersMenuItem.DropDownItems.Add(customMenuItem);
                }
            }
        }

        private void NotifyIcon_MouseClick(object? sender, MouseEventArgs args)
        {
            // Контекстное меню открывается автоматически при правом клике
        }

        private void NotifyIcon_DoubleClick(object? sender, EventArgs e)
        {
            ShowParentWindow();
        }

        public void ShowParentWindow()
        {
            _parentForm.Show();
            _parentForm.WindowState = FormWindowState.Normal;
            _parentForm.TopMost = true;
            _parentForm.Activate();
            _parentForm.BringToFront();
            _parentForm.Focus();
            _parentForm.TopMost = false;
        }

        public void HideParentWindow()
        {
            _parentForm.Hide();
            _notifyIcon.ShowBalloonTip(500);
        }

        public void Dispose()
        {
            _notifyIcon.MouseClick -= NotifyIcon_MouseClick;
            _notifyIcon.DoubleClick -= NotifyIcon_DoubleClick;
            _trayContextMenu?.Dispose();
            _notifyIcon.Dispose();
        }
    }
}

