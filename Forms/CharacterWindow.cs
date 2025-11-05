using DesktopDance.Utility;

namespace DesktopDance.Forms
{
    public partial class CharacterWindow : Form
    {
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // Убираем окно из Alt+Tab
                cp.ExStyle |= 0x80; // WS_EX_TOOLWINDOW
                return cp;
            }
        }

        public CharacterWindow(Screen screen)
        {
            InitializeComponent();
            Controls.Add(new WindowPictureBox(screen.Bounds));
        }
    }
}