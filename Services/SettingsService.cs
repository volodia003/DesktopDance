using DesktopKonata.Utility;
using Microsoft.Win32;
using System.Windows.Forms;

namespace DesktopKonata.Services
{
    /// <summary>
    /// Сервис для управления настройками приложения и автозапуском
    /// </summary>
    public class SettingsService
    {
        private const string AppName = "DesktopDance";
        private readonly AppSettings _settings;

        public AppSettings Settings => _settings;

        public SettingsService()
        {
            _settings = AppSettings.Load();
        }

        /// <summary>
        /// Проверяет, находится ли приложение в автозапуске
        /// </summary>
        public bool IsInAutoStart()
        {
            try
            {
                using RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", false);
                return key?.GetValue(AppName) != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Добавляет приложение в автозапуск
        /// </summary>
        public void AddToAutoStart()
        {
            try
            {
                using RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                string exePath = Application.ExecutablePath;
                key?.SetValue(AppName, exePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось добавить в автозапуск: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Удаляет приложение из автозапуска
        /// </summary>
        public void RemoveFromAutoStart()
        {
            try
            {
                using RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                key?.DeleteValue(AppName, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось удалить из автозапуска: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Применяет настройки к элементам управления
        /// </summary>
        public void ApplyToControls(
            TrackBar scaleTrackBar,
            Label scaleLabel,
            CheckBox flipCheckBox,
            CheckBox lockCheckBox,
            ToolStripMenuItem minimizeOnCloseMenuItem,
            ToolStripMenuItem singleCharacterModeMenuItem,
            ToolStripMenuItem showInTaskbarMenuItem,
            ToolStripMenuItem showMenuOnStartupMenuItem,
            Form form)
        {
            // Применяем масштаб
            scaleTrackBar.Value = (int)(_settings.Scale * 100);
            CharacterManager.SetGlobalScale(_settings.Scale);
            scaleLabel.Text = $"Размер: {scaleTrackBar.Value}%";

            // Применяем отзеркаливание
            flipCheckBox.Checked = _settings.IsFlipped;
            CharacterManager.SetGlobalFlip(_settings.IsFlipped);

            // Применяем блокировку
            lockCheckBox.Checked = _settings.IsLocked;
            CharacterManager.SetLocked(_settings.IsLocked);

            // Применяем настройку сворачивания
            minimizeOnCloseMenuItem.Checked = _settings.MinimizeOnClose;

            // Применяем режим: чекбокс "Много персонажей" (true=много, false=одиночный)
            // SingleCharacterMode (true=одиночный, false=много) → инвертируем
            singleCharacterModeMenuItem.Checked = !_settings.SingleCharacterMode;

            // Применяем видимость в панели задач
            showInTaskbarMenuItem.Checked = _settings.ShowInTaskbar;
            form.ShowInTaskbar = _settings.ShowInTaskbar;

            // Применяем настройку показа меню при запуске
            showMenuOnStartupMenuItem.Checked = _settings.ShowMenuOnStartup;
        }

        /// <summary>
        /// Сохраняет настройки из элементов управления
        /// </summary>
        public void SaveFromControls(
            TrackBar scaleTrackBar,
            CheckBox flipCheckBox,
            CheckBox lockCheckBox,
            ToolStripMenuItem minimizeOnCloseMenuItem,
            ToolStripMenuItem singleCharacterModeMenuItem,
            ToolStripMenuItem showInTaskbarMenuItem,
            ToolStripMenuItem showMenuOnStartupMenuItem)
        {
            _settings.Scale = scaleTrackBar.Value / 100f;
            _settings.IsFlipped = flipCheckBox.Checked;
            _settings.IsLocked = lockCheckBox.Checked;
            _settings.MinimizeOnClose = minimizeOnCloseMenuItem.Checked;
            _settings.SingleCharacterMode = singleCharacterModeMenuItem.Checked;
            _settings.ShowInTaskbar = showInTaskbarMenuItem.Checked;
            _settings.ShowMenuOnStartup = showMenuOnStartupMenuItem.Checked;
            _settings.Save();
        }

        /// <summary>
        /// Инициализирует список встроенных персонажей, если он пустой
        /// </summary>
        public void InitializeAvailableCharacters()
        {
            if (_settings.AvailableCharacters.Count == 0)
            {
                _settings.AvailableCharacters.Add(new AvailableCharacterData
                {
                    OriginalName = "blin4iik Dance",
                    DisplayName = "blin4iik Dance",
                    FilePath = "",
                    DefaultScale = 1.0f,
                    DefaultIsFlipped = false
                });
                _settings.AvailableCharacters.Add(new AvailableCharacterData
                {
                    OriginalName = "Konata Love",
                    DisplayName = "Konata Love",
                    FilePath = "",
                    DefaultScale = 1.0f,
                    DefaultIsFlipped = false
                });
                _settings.Save();
            }
        }
    }
}

