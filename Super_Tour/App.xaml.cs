using Super_Tour.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
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

            CultureInfo cultureInfo = new CultureInfo("vi-VN"); // Use "vi-VN" for Vietnamese format
            cultureInfo.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            cultureInfo.DateTimeFormat.LongTimePattern = "HH:mm:ss";
            System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;
            System.Threading.Thread.CurrentThread.CurrentUICulture = cultureInfo;
        }
        protected void ApplicationStart(object sender, StartupEventArgs e)
        {
            /*var loginView = new LoginView();
            loginView.Show();
            loginView.IsVisibleChanged += (s, ev) =>
            {
                if (loginView.IsVisible == false && loginView.IsLoaded)
                {
                    var mainView = new MainView();
                    mainView.Show();
                }
            };*/

            var loginView = new LoginView();
            loginView.Show();
        }
    }
}
