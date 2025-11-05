using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DesktopDance.Utility
{
    /// <summary>
    /// Управляет единственным экземпляром приложения.
    /// Если приложение уже запущено, активирует существующее окно.
    /// </summary>
    public class SingleInstanceManager : IDisposable
    {
        private const string MUTEX_NAME = "DesktopDance_SingleInstance_Mutex_B4E8F9A2";
        private const string PIPE_NAME = "DesktopDance_SingleInstance_Pipe";
        private const string ACTIVATE_COMMAND = "ACTIVATE";

        private readonly Mutex _mutex;
        private readonly bool _isFirstInstance;
        private NamedPipeServerStream? _pipeServer;
        private CancellationTokenSource? _cancellationTokenSource;
        private Form? _mainForm;

        // Win32 API для активации окна
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        private const int SW_RESTORE = 9;
        private const int SW_SHOW = 5;

        public SingleInstanceManager()
        {
            _mutex = new Mutex(true, MUTEX_NAME, out _isFirstInstance);
        }

        /// <summary>
        /// Проверяет, является ли текущий процесс первым экземпляром.
        /// </summary>
        public bool IsFirstInstance => _isFirstInstance;

        /// <summary>
        /// Регистрирует главную форму для активации при запуске второго экземпляра.
        /// </summary>
        public void RegisterMainForm(Form mainForm)
        {
            _mainForm = mainForm;

            if (_isFirstInstance)
            {
                _cancellationTokenSource = new CancellationTokenSource();
                Task.Run(() => StartPipeServer(_cancellationTokenSource.Token));
            }
        }

        /// <summary>
        /// Активирует существующий экземпляр приложения.
        /// </summary>
        public static void ActivateFirstInstance()
        {
            try
            {
                using var client = new NamedPipeClientStream(".", PIPE_NAME, PipeDirection.Out);
                client.Connect(1000); // Таймаут 1 секунда

                using var writer = new StreamWriter(client);
                writer.WriteLine(ACTIVATE_COMMAND);
                writer.Flush();

                Logger.Info("Команда активации отправлена существующему экземпляру");
            }
            catch (Exception ex)
            {
                Logger.Warning($"Не удалось отправить команду активации: {ex.Message}");
            }
        }

        /// <summary>
        /// Запускает сервер для прослушивания команд от других экземпляров.
        /// </summary>
        private async Task StartPipeServer(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    _pipeServer = new NamedPipeServerStream(
                        PIPE_NAME,
                        PipeDirection.In,
                        1,
                        PipeTransmissionMode.Message,
                        PipeOptions.Asynchronous
                    );

                    await _pipeServer.WaitForConnectionAsync(cancellationToken);

                    using var reader = new StreamReader(_pipeServer);
                    var command = await reader.ReadLineAsync();

                    if (command == ACTIVATE_COMMAND && _mainForm != null)
                    {
                        _mainForm.Invoke(new Action(() =>
                        {
                            ActivateMainForm();
                        }));

                        Logger.Info("Главное окно активировано по запросу второго экземпляра");
                    }

                    _pipeServer.Dispose();
                    _pipeServer = null;
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Logger.Warning($"Ошибка в pipe сервере: {ex.Message}");
                    await Task.Delay(100, cancellationToken);
                }
            }
        }

        /// <summary>
        /// Выводит главное окно поверх всех остальных окон.
        /// </summary>
        private void ActivateMainForm()
        {
            if (_mainForm == null || _mainForm.IsDisposed)
                return;

            try
            {
                IntPtr handle = _mainForm.Handle;

                // Если окно свернуто, восстанавливаем его
                if (IsIconic(handle))
                {
                    ShowWindow(handle, SW_RESTORE);
                }
                else
                {
                    ShowWindow(handle, SW_SHOW);
                }

                // Выводим окно на передний план
                SetForegroundWindow(handle);

                // Также используем стандартные методы WinForms
                _mainForm.WindowState = FormWindowState.Normal;
                _mainForm.Activate();
                _mainForm.BringToFront();

                Logger.Info("Главное окно успешно активировано");
            }
            catch (Exception ex)
            {
                Logger.Error("Ошибка при активации главного окна", ex);
            }
        }

        public void Dispose()
        {
            try
            {
                _cancellationTokenSource?.Cancel();
                _pipeServer?.Dispose();
                _mutex?.Dispose();
            }
            catch (Exception ex)
            {
                Logger.Warning($"Ошибка при освобождении ресурсов SingleInstanceManager: {ex.Message}");
            }
        }
    }
}

