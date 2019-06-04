using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic.ApplicationServices;
using System.Windows;
using System.Text;
using System.Threading.Tasks;
using NLog;


namespace Friday
{
    public class SApp
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                SingleInstanceManager manager = new SingleInstanceManager();
                manager.Run(args);
            }
            catch (Exception ex)
            {
                logger.Error(String.Format("Ошибка старта приложения {0}. {1}. {2}.", ex.Message, ex.StackTrace, ex.HelpLink));
            }

        }
    }
    public class SingleInstanceManager : WindowsFormsApplicationBase
    {
        public static SingleInstanceApplication app;
        Logger logger = LogManager.GetCurrentClassLogger();

        public SingleInstanceManager()
        {
            this.IsSingleInstance = true;
        }

        protected override bool OnStartup(Microsoft.VisualBasic.ApplicationServices.StartupEventArgs e)
        {
            try
            {
                // First time app is launched
                app = new SingleInstanceApplication();
                app.DispatcherUnhandledException += App_DispatcherUnhandledException;
                app.Run();
                return false;
            }
            catch (Exception ex)
            {
                logger.Error(String.Format("Критическая ошибка приложения: {0}. {1}. {2}.", ex.Message, ex.StackTrace, ex.HelpLink));
                return false;
            }
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            logger.Error("Unhandled exception: {0}. {1}. {2}.", e.Exception.Message, e.Exception.StackTrace, e.Exception.HelpLink);
        }
       
        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs eventArgs)
        {
            try
            {
                // Subsequent launches
                base.OnStartupNextInstance(eventArgs);                
                app.Activate();
                app.MainWindow.WindowState = System.Windows.WindowState.Normal;
                //app.MainWindow.Show();
            }
            catch (Exception ex)
            {
                logger.Error(String.Format("Ошибка чтения ключей второго экземпляра приложения: {0}", ex.Message));
            }
        }
    }
    public class SingleInstanceApplication : System.Windows.Application
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        protected override void OnStartup(System.Windows.StartupEventArgs e)
        {
            try
            {
                base.OnStartup(e);
                DevExpress.Xpf.Core.ApplicationThemeHelper.UpdateApplicationThemeName();
                // Create and show the application's main window
                MainWindow window = new MainWindow();
                window.Show();
            }
            catch (Exception ex)
            {
                logger.Error(String.Format("Ошибка старта первого экземпляра приложения: {0}. {1}. {2}.", ex.Message, ex.StackTrace, ex.HelpLink));
            }
        }

        public void Activate()
        {
            try
            {
                // Reactivate application's main window
                this.MainWindow.Show();
                this.MainWindow.Activate();
            }
            catch (Exception ex)
            {
                logger.Error(String.Format("Ошибка активации экземпляра приложения: {0}. {1}. {2}.", ex.Message, ex.StackTrace, ex.HelpLink));
            }
        }
    }

}
