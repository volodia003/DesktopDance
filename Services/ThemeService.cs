using System.Drawing;
using System.Windows.Forms;

namespace DesktopDance.Services
{
    /// <summary>
    /// Сервис для управления темами приложения
    /// </summary>
    public class ThemeService
    {
        public enum ThemeMode
        {
            Light,
            Dark,
            System  // Следует за системной темой Windows
        }

        // Цвета для светлой темы
        public static class LightTheme
        {
            public static readonly Color Background = Color.FromArgb(240, 240, 240);
            public static readonly Color Surface = Color.White;
            public static readonly Color Primary = Color.FromArgb(0, 120, 215);
            public static readonly Color PrimaryHover = Color.FromArgb(0, 102, 204);
            public static readonly Color Secondary = Color.FromArgb(118, 118, 118);
            public static readonly Color Text = Color.FromArgb(51, 51, 51);
            public static readonly Color TextSecondary = Color.FromArgb(118, 118, 118);
            public static readonly Color Border = Color.FromArgb(200, 200, 200);
            public static readonly Color Success = Color.FromArgb(16, 124, 16);
            public static readonly Color Error = Color.FromArgb(196, 43, 28);
        }

        // Цвета для темной темы
        public static class DarkTheme
        {
            public static readonly Color Background = Color.FromArgb(32, 32, 32);
            public static readonly Color Surface = Color.FromArgb(45, 45, 45);
            public static readonly Color Primary = Color.FromArgb(0, 120, 215);
            public static readonly Color PrimaryHover = Color.FromArgb(0, 140, 235);
            public static readonly Color Secondary = Color.FromArgb(180, 180, 180);
            public static readonly Color Text = Color.FromArgb(255, 255, 255);
            public static readonly Color TextSecondary = Color.FromArgb(180, 180, 180);
            public static readonly Color Border = Color.FromArgb(60, 60, 60);
            public static readonly Color Success = Color.FromArgb(16, 185, 16);
            public static readonly Color Error = Color.FromArgb(232, 65, 50);
        }

        private ThemeMode _currentTheme;
        
        public ThemeMode CurrentTheme
        {
            get => _currentTheme;
            set
            {
                _currentTheme = value;
                ThemeChanged?.Invoke(this, value);
            }
        }

        public event EventHandler<ThemeMode>? ThemeChanged;

        public ThemeService(ThemeMode initialTheme = ThemeMode.Light)
        {
            _currentTheme = initialTheme;
        }

        /// <summary>
        /// Определяет, используется ли темная тема в системе Windows
        /// </summary>
        public static bool IsSystemDarkMode()
        {
            try
            {
                using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
                var value = key?.GetValue("AppsUseLightTheme");
                return value is int i && i == 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Получает активную тему с учетом системной настройки
        /// </summary>
        public ThemeMode GetEffectiveTheme()
        {
            if (_currentTheme == ThemeMode.System)
            {
                return IsSystemDarkMode() ? ThemeMode.Dark : ThemeMode.Light;
            }
            return _currentTheme;
        }

        /// <summary>
        /// Применяет тему к форме
        /// </summary>
        public void ApplyTheme(Form form)
        {
            var isDark = GetEffectiveTheme() == ThemeMode.Dark;
            
            form.BackColor = isDark ? DarkTheme.Background : LightTheme.Background;
            form.ForeColor = isDark ? DarkTheme.Text : LightTheme.Text;
            
            ApplyThemeToControls(form.Controls, isDark);
        }

        /// <summary>
        /// Применяет тему к элементам управления рекурсивно
        /// </summary>
        private void ApplyThemeToControls(Control.ControlCollection controls, bool isDark)
        {
            foreach (Control control in controls)
            {
                ApplyThemeToControl(control, isDark);
                
                if (control.HasChildren)
                {
                    ApplyThemeToControls(control.Controls, isDark);
                }
            }
        }

        /// <summary>
        /// Применяет тему к конкретному элементу управления
        /// </summary>
        private void ApplyThemeToControl(Control control, bool isDark)
        {
            switch (control)
            {
                case Button button:
                    button.BackColor = isDark ? DarkTheme.Primary : LightTheme.Primary;
                    button.ForeColor = Color.White;
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = 0;
                    button.FlatAppearance.MouseOverBackColor = isDark ? DarkTheme.PrimaryHover : LightTheme.PrimaryHover;
                    break;

                case TextBox textBox:
                    textBox.BackColor = isDark ? DarkTheme.Surface : LightTheme.Surface;
                    textBox.ForeColor = isDark ? DarkTheme.Text : LightTheme.Text;
                    textBox.BorderStyle = BorderStyle.None;
                    break;

                case ListBox listBox:
                    listBox.BackColor = isDark ? DarkTheme.Surface : LightTheme.Surface;
                    listBox.ForeColor = isDark ? DarkTheme.Text : LightTheme.Text;
                    listBox.BorderStyle = BorderStyle.None;
                    break;

                case CheckBox checkBox:
                    checkBox.BackColor = Color.Transparent;
                    checkBox.ForeColor = isDark ? DarkTheme.Text : LightTheme.Text;
                    break;

                case Label label:
                    if (label.Parent != null)
                    {
                        label.BackColor = Color.Transparent;
                    }
                    label.ForeColor = isDark ? DarkTheme.Text : LightTheme.Text;
                    break;

                case Panel panel:
                    // Проверяем, если это панель с особым стилем (по имени)
                    if (panel.Name.Contains("Panel") || panel.BorderStyle != BorderStyle.None)
                    {
                        panel.BackColor = isDark ? DarkTheme.Surface : LightTheme.Surface;
                        panel.ForeColor = isDark ? DarkTheme.Text : LightTheme.Text;
                    }
                    break;

                case GroupBox groupBox:
                    groupBox.ForeColor = isDark ? DarkTheme.Text : LightTheme.Text;
                    break;

                case TrackBar trackBar:
                    trackBar.BackColor = isDark ? DarkTheme.Background : LightTheme.Background;
                    break;

                case MenuStrip menuStrip:
                    menuStrip.BackColor = isDark ? DarkTheme.Surface : LightTheme.Surface;
                    menuStrip.ForeColor = isDark ? DarkTheme.Text : LightTheme.Text;
                    ApplyThemeToMenuItems(menuStrip.Items, isDark);
                    break;

                case ContextMenuStrip contextMenu:
                    contextMenu.BackColor = isDark ? DarkTheme.Surface : LightTheme.Surface;
                    contextMenu.ForeColor = isDark ? DarkTheme.Text : LightTheme.Text;
                    ApplyThemeToMenuItems(contextMenu.Items, isDark);
                    break;

                default:
                    // Для остальных контролов применяем базовые цвета
                    if (control.BackColor != Color.Transparent)
                    {
                        control.BackColor = isDark ? DarkTheme.Background : LightTheme.Background;
                    }
                    control.ForeColor = isDark ? DarkTheme.Text : LightTheme.Text;
                    break;
            }
        }

        /// <summary>
        /// Применяет тему к элементам меню
        /// </summary>
        private void ApplyThemeToMenuItems(ToolStripItemCollection items, bool isDark)
        {
            foreach (ToolStripItem item in items)
            {
                item.BackColor = isDark ? DarkTheme.Surface : LightTheme.Surface;
                item.ForeColor = isDark ? DarkTheme.Text : LightTheme.Text;

                if (item is ToolStripMenuItem menuItem && menuItem.HasDropDownItems)
                {
                    menuItem.DropDown.BackColor = isDark ? DarkTheme.Surface : LightTheme.Surface;
                    ApplyThemeToMenuItems(menuItem.DropDownItems, isDark);
                }
            }
        }

        /// <summary>
        /// Получает цвет фона для текущей темы
        /// </summary>
        public Color GetBackgroundColor()
        {
            return GetEffectiveTheme() == ThemeMode.Dark ? DarkTheme.Background : LightTheme.Background;
        }

        /// <summary>
        /// Получает цвет текста для текущей темы
        /// </summary>
        public Color GetTextColor()
        {
            return GetEffectiveTheme() == ThemeMode.Dark ? DarkTheme.Text : LightTheme.Text;
        }

        /// <summary>
        /// Получает цвет поверхности для текущей темы
        /// </summary>
        public Color GetSurfaceColor()
        {
            return GetEffectiveTheme() == ThemeMode.Dark ? DarkTheme.Surface : LightTheme.Surface;
        }

        /// <summary>
        /// Получает основной цвет для текущей темы
        /// </summary>
        public Color GetPrimaryColor()
        {
            return GetEffectiveTheme() == ThemeMode.Dark ? DarkTheme.Primary : LightTheme.Primary;
        }
    }
}

