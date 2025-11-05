using DesktopDance.Forms;
using DesktopDance.Utility;

namespace DesktopDance
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            // Инициализация логгера
            Logger.Info("==============================================");
            Logger.Info("Desktop Dance Application Started");
            Logger.Info($"Version: 2.0.0");
            Logger.Info($"OS: {Environment.OSVersion}");
            Logger.Info($".NET: {Environment.Version}");
            Logger.Info("==============================================");

            try
            {
                ApplicationConfiguration.Initialize();
                Application.Run(new Menu());
            }
            catch (Exception ex)
            {
                Logger.Critical("Необработанное исключение в Main", ex);
                MessageBox.Show(
                    $"Критическая ошибка:\n{ex.Message}\n\nЛог сохранен в:\n{Logger.GetLogFilePath()}",
                    "Критическая ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                throw;
            }
            finally
            {
                Logger.Info("Desktop Dance Application Closed");
                Logger.Info("==============================================");
            }
        }
    }
}