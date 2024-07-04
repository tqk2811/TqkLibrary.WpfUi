using System.Configuration;
using System.Data;
using System.Windows;
using TestWpfApp.TestUC.TestAsyncCollection;

namespace TestWpfApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Window window = GetWindow();
            window.Show();
        }

        Window GetWindow()
        {
            return new AsyncCollectionWindow();
        }
    }

}
