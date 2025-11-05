using System.Text;

namespace DesktopDance.Utility
{
    /// <summary>
    /// Простая система логирования для Desktop Dance
    /// Логи сохраняются в AppData/DesktopDance/Logs
    /// </summary>
    public static class Logger
    {
        private static readonly string LogsFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "DesktopDance",
            "Logs"
        );

        private static readonly string LogFilePath = Path.Combine(
            LogsFolder,
            $"DesktopDance_{DateTime.Now:yyyy-MM-dd}.log"
        );

        private static readonly object _lock = new object();
        private static bool _isInitialized = false;

        public enum LogLevel
        {
            Debug,
            Info,
            Warning,
            Error,
            Critical
        }

        /// <summary>
        /// Инициализирует папку для логов
        /// </summary>
        private static void Initialize()
        {
            if (_isInitialized) return;

            try
            {
                if (!Directory.Exists(LogsFolder))
                {
                    Directory.CreateDirectory(LogsFolder);
                }

                // Очистка старых логов (старше 7 дней)
                CleanOldLogs();

                _isInitialized = true;
            }
            catch
            {
                // Если не удалось создать папку логов, продолжаем без логирования
                _isInitialized = false;
            }
        }

        /// <summary>
        /// Удаляет логи старше 7 дней
        /// </summary>
        private static void CleanOldLogs()
        {
            try
            {
                var logFiles = Directory.GetFiles(LogsFolder, "DesktopDance_*.log");
                var weekAgo = DateTime.Now.AddDays(-7);

                foreach (var file in logFiles)
                {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.CreationTime < weekAgo)
                    {
                        File.Delete(file);
                    }
                }
            }
            catch
            {
                // Игнорируем ошибки при очистке
            }
        }

        /// <summary>
        /// Записывает сообщение в лог
        /// </summary>
        private static void Log(LogLevel level, string message, Exception? exception = null)
        {
            if (!_isInitialized)
            {
                Initialize();
            }

            if (!_isInitialized) return; // Не удалось инициализировать

            try
            {
                lock (_lock)
                {
                    var logMessage = FormatLogMessage(level, message, exception);
                    File.AppendAllText(LogFilePath, logMessage, Encoding.UTF8);
                }
            }
            catch
            {
                // Если не удалось записать в лог, молча игнорируем
            }
        }

        /// <summary>
        /// Форматирует сообщение лога
        /// </summary>
        private static string FormatLogMessage(LogLevel level, string message, Exception? exception)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{level}] {message}");

            if (exception != null)
            {
                sb.AppendLine($"Exception: {exception.GetType().Name}");
                sb.AppendLine($"Message: {exception.Message}");
                sb.AppendLine($"StackTrace: {exception.StackTrace}");

                if (exception.InnerException != null)
                {
                    sb.AppendLine($"InnerException: {exception.InnerException.Message}");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Логирование отладочной информации
        /// </summary>
        public static void Debug(string message)
        {
            Log(LogLevel.Debug, message);
        }

        /// <summary>
        /// Логирование информационного сообщения
        /// </summary>
        public static void Info(string message)
        {
            Log(LogLevel.Info, message);
        }

        /// <summary>
        /// Логирование предупреждения
        /// </summary>
        public static void Warning(string message, Exception? exception = null)
        {
            Log(LogLevel.Warning, message, exception);
        }

        /// <summary>
        /// Логирование ошибки
        /// </summary>
        public static void Error(string message, Exception? exception = null)
        {
            Log(LogLevel.Error, message, exception);
        }

        /// <summary>
        /// Логирование критической ошибки
        /// </summary>
        public static void Critical(string message, Exception? exception = null)
        {
            Log(LogLevel.Critical, message, exception);
        }

        /// <summary>
        /// Получает путь к текущему файлу лога
        /// </summary>
        public static string GetLogFilePath()
        {
            return LogFilePath;
        }

        /// <summary>
        /// Получает путь к папке с логами
        /// </summary>
        public static string GetLogsFolder()
        {
            return LogsFolder;
        }
    }
}

