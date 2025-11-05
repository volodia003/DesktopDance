using DesktopDance.Forms;

namespace DesktopDance
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new Menu());
        }
    }
}