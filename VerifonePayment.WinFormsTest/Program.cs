using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace VerifonePayment.WinFormsTest
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Global exception handling for enhanced monitoring
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            Debug.WriteLine($"Application starting - PID: {Process.GetCurrentProcess().Id}");
            Debug.WriteLine($"CLR Version: {Environment.Version}");
            Debug.WriteLine($"OS Version: {Environment.OSVersion}");
            
            try
            {
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Application.Run exception: {ex}");
                MessageBox.Show($"Application fatal error: {ex.Message}", "Fatal Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            Debug.WriteLine("Application ending");
        }
        
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Debug.WriteLine($"Thread exception: {e.Exception}");
            LogException("ThreadException", e.Exception);
        }
        
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Debug.WriteLine($"Unhandled exception: {e.ExceptionObject}");
            LogException("UnhandledException", e.ExceptionObject as Exception);
        }
        
        private static void LogException(string type, Exception ex)
        {
            if (ex == null) return;
            
            var details = $"{type} - {ex.GetType().Name}: {ex.Message}\nStack: {ex.StackTrace}";
            Debug.WriteLine(details);
            
            // Also log inner exceptions
            var innerEx = ex.InnerException;
            while (innerEx != null)
            {
                Debug.WriteLine($"Inner Exception: {innerEx.GetType().Name}: {innerEx.Message}");
                innerEx = innerEx.InnerException;
            }
        }
    }
}