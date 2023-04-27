using Super_Tour.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Super_Tour
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Lấy hình ảnh biểu tượng và tạo một đối tượng BitmapSource.
            BitmapSource icon = new BitmapImage(new Uri("pack://application:,,,/Super_Tour;component/Images/Logo.ico"));

            // Đặt hình ảnh biểu tượng cho ứng dụng.
            Current.MainWindow.Icon = icon;
        }
        protected void ApplicationStart(object sender, StartupEventArgs e)
        {
            var loginView = new LoginView();
            loginView.Show();
            loginView.IsVisibleChanged += (s, ev) =>
            {
                if (loginView.IsVisible == false && loginView.IsLoaded)
                {
                    var mainView = new MainView();
                    mainView.Show();
                }
            };
        }
    }
}
