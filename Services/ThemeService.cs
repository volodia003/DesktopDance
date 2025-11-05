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
            Blin4iik,  // Особая тема в стиле blin4iik
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

        // Цвета для темы Blin4iik (из расписания и персонажа)
        public static class Blin4iikTheme
        {
            public static readonly Color Background = Color.FromArgb(45, 35, 40); // Темно-коричневый фон
            public static readonly Color Surface = Color.FromArgb(65, 45, 50); // Коричневый для карточек
            public static readonly Color Primary = Color.FromArgb(220, 140, 80); // Теплый оранжевый
            public static readonly Color PrimaryHover = Color.FromArgb(240, 160, 100); // Светлее оранжевый
            public static readonly Color Secondary = Color.FromArgb(200, 180, 160); // Бежевый
            public static readonly Color Text = Color.FromArgb(245, 230, 210); // Теплый светло-бежевый
            public static readonly Color TextSecondary = Color.FromArgb(210, 180, 150); // Золотисто-бежевый
            public static readonly Color Border = Color.FromArgb(85, 65, 70); // Темно-коричневая рамка
            public static readonly Color Success = Color.FromArgb(200, 160, 100); // Золотистый
            public static readonly Color Error = Color.FromArgb(220, 100, 100); // Теплый красный
            public static readonly Color Accent = Color.FromArgb(230, 180, 120); // Золотой акцент
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
        /// Проверяет, является ли текущая тема темной или Blin4iik
        /// </summary>
        private bool IsDarkOrBlin4iikTheme()
        {
            var effective = GetEffectiveTheme();
            return effective == ThemeMode.Dark || effective == ThemeMode.Blin4iik;
        }

        /// <summary>
        /// Применяет тему к форме
        /// </summary>
        public void ApplyTheme(Form form)
        {
            var effectiveTheme = GetEffectiveTheme();
            
            form.BackColor = GetBackgroundColor();
            form.ForeColor = GetTextColor();
            
            ApplyThemeToControls(form.Controls, effectiveTheme);
        }

        /// <summary>
        /// Применяет тему к элементам управления рекурсивно
        /// </summary>
        private void ApplyThemeToControls(Control.ControlCollection controls, ThemeMode theme)
        {
            foreach (Control control in controls)
            {
                ApplyThemeToControl(control, theme);
                
                if (control.HasChildren)
                {
                    ApplyThemeToControls(control.Controls, theme);
                }
            }
        }

        /// <summary>
        /// Применяет тему к конкретному элементу управления
        /// </summary>
        private void ApplyThemeToControl(Control control, ThemeMode theme)
        {
            var background = GetBackgroundColorForTheme(theme);
            var surface = GetSurfaceColorForTheme(theme);
            var text = GetTextColorForTheme(theme);
            var primary = GetPrimaryColorForTheme(theme);
            var primaryHover = GetPrimaryHoverColorForTheme(theme);

            switch (control)
            {
                case Button button:
                    button.BackColor = primary;
                    button.ForeColor = Color.White;
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = 0;
                    button.FlatAppearance.MouseOverBackColor = primaryHover;
                    break;

                case TextBox textBox:
                    textBox.BackColor = surface;
                    textBox.ForeColor = text;
                    textBox.BorderStyle = BorderStyle.None;
                    break;

                case ListBox listBox:
                    listBox.BackColor = surface;
                    listBox.ForeColor = text;
                    listBox.BorderStyle = BorderStyle.None;
                    break;

                case CheckBox checkBox:
                    checkBox.BackColor = Color.Transparent;
                    checkBox.ForeColor = text;
                    break;

                case Label label:
                    if (label.Parent != null)
                    {
                        label.BackColor = Color.Transparent;
                    }
                    label.ForeColor = text;
                    break;

                case Panel panel:
                    // Проверяем, если это панель с особым стилем (по имени)
                    if (panel.Name.Contains("Panel") || panel.BorderStyle != BorderStyle.None)
                    {
                        panel.BackColor = surface;
                        panel.ForeColor = text;
                    }
                    break;

                case GroupBox groupBox:
                    groupBox.ForeColor = text;
                    break;

                case TrackBar trackBar:
                    trackBar.BackColor = background;
                    break;

                case MenuStrip menuStrip:
                    menuStrip.BackColor = surface;
                    menuStrip.ForeColor = text;
                    ApplyThemeToMenuItems(menuStrip.Items, theme);
                    break;

                case ContextMenuStrip contextMenu:
                    contextMenu.BackColor = surface;
                    contextMenu.ForeColor = text;
                    ApplyThemeToMenuItems(contextMenu.Items, theme);
                    break;

                default:
                    if (control.BackColor != Color.Transparent)
                    {
                        control.BackColor = background;
                    }
                    control.ForeColor = text;
                    break;
            }
        }

        /// <summary>
        /// Применяет тему к элементам меню
        /// </summary>
        private void ApplyThemeToMenuItems(ToolStripItemCollection items, ThemeMode theme)
        {
            var surface = GetSurfaceColorForTheme(theme);
            var text = GetTextColorForTheme(theme);

            foreach (ToolStripItem item in items)
            {
                item.BackColor = surface;
                item.ForeColor = text;

                if (item is ToolStripMenuItem menuItem && menuItem.HasDropDownItems)
                {
                    menuItem.DropDown.BackColor = surface;
                    ApplyThemeToMenuItems(menuItem.DropDownItems, theme);
                }
            }
        }

        private Color GetBackgroundColorForTheme(ThemeMode theme)
        {
            return theme switch
            {
                ThemeMode.Dark => DarkTheme.Background,
                ThemeMode.Blin4iik => Blin4iikTheme.Background,
                _ => LightTheme.Background
            };
        }

        private Color GetSurfaceColorForTheme(ThemeMode theme)
        {
            return theme switch
            {
                ThemeMode.Dark => DarkTheme.Surface,
                ThemeMode.Blin4iik => Blin4iikTheme.Surface,
                _ => LightTheme.Surface
            };
        }

        private Color GetTextColorForTheme(ThemeMode theme)
        {
            return theme switch
            {
                ThemeMode.Dark => DarkTheme.Text,
                ThemeMode.Blin4iik => Blin4iikTheme.Text,
                _ => LightTheme.Text
            };
        }

        private Color GetPrimaryColorForTheme(ThemeMode theme)
        {
            return theme switch
            {
                ThemeMode.Dark => DarkTheme.Primary,
                ThemeMode.Blin4iik => Blin4iikTheme.Primary,
                _ => LightTheme.Primary
            };
        }

        private Color GetPrimaryHoverColorForTheme(ThemeMode theme)
        {
            return theme switch
            {
                ThemeMode.Dark => DarkTheme.PrimaryHover,
                ThemeMode.Blin4iik => Blin4iikTheme.PrimaryHover,
                _ => LightTheme.PrimaryHover
            };
        }

        /// <summary>
        /// Получает цвет фона для текущей темы
        /// </summary>
        public Color GetBackgroundColor()
        {
            return GetBackgroundColorForTheme(GetEffectiveTheme());
        }

        /// <summary>
        /// Получает цвет текста для текущей темы
        /// </summary>
        public Color GetTextColor()
        {
            return GetTextColorForTheme(GetEffectiveTheme());
        }

        /// <summary>
        /// Получает цвет поверхности для текущей темы
        /// </summary>
        public Color GetSurfaceColor()
        {
            return GetSurfaceColorForTheme(GetEffectiveTheme());
        }

        /// <summary>
        /// Получает основной цвет для текущей темы
        /// </summary>
        public Color GetPrimaryColor()
        {
            return GetPrimaryColorForTheme(GetEffectiveTheme());
        }
    }
}

