using DesktopDance.Forms;
using DesktopDance.Utility;

namespace DesktopDance
{
    public static class Program
    {
        private static SingleInstanceManager? _singleInstanceManager;

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
                // Проверка единственного экземпляра приложения
                _singleInstanceManager = new SingleInstanceManager();

                if (!_singleInstanceManager.IsFirstInstance)
                {
                    Logger.Info("Обнаружен запущенный экземпляр приложения. Активация существующего окна...");
                    SingleInstanceManager.ActivateFirstInstance();
                    return;
                }

                Logger.Info("Первый экземпляр приложения - продолжаем запуск");

                ApplicationConfiguration.Initialize();
                
                var mainForm = new Menu();
                _singleInstanceManager.RegisterMainForm(mainForm);
                
                Application.Run(mainForm);
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
                _singleInstanceManager?.Dispose();
                Logger.Info("Desktop Dance Application Closed");
                Logger.Info("==============================================");
            }
        }
    }
}