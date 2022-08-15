using System.Windows;

namespace Task_Scheduler
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow wnd = new MainWindow();
            wnd.Title = "Task Scheduler";
            wnd.Show();
        }
    }
}
