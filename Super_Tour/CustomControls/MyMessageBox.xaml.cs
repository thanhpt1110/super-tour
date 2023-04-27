using FontAwesome.Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Super_Tour.CustomControls
{
    /// <summary>
    /// Interaction logic for MyMessageBox.xaml
    /// </summary>
    public partial class MyMessageBox : Window
    {
        public enum MessageBoxButton
        {
            YesNoCancel,
            OK,
            YesNo,
            OkCancel
        }

        public enum ButtonResult
        {
            NULL,
            YES,
            NO,
            CANCEL,
            OK
        }

        public enum MessageBoxImage
        {
            Information,
            Question,
            Warning,
            Error
        }

        public static ButtonResult buttonResultClicked;
        public MyMessageBox(string message, string title, MessageBoxButton button, MessageBoxImage img)
        {
            InitializeComponent();
            txtBlockMessage.Text = message;
            txtBlockTitle.Text = title;
            buttonResultClicked = ButtonResult.NULL;
            setButton(button);
            setIcon(img);
        }

        private void setButton(MessageBoxButton button)
        {
            if (button == MessageBoxButton.YesNoCancel)
            {
                ContainButton.Children.Remove(btnOk);

            }
            else if (button == MessageBoxButton.OK)
            {
                ContainButton.Children.Remove(btnCancel);
                ContainButton.Children.Remove(btnNo);
                ContainButton.Children.Remove(btnYes);

            }
            else if (button == MessageBoxButton.YesNo)
            {
                ContainButton.Children.Remove(btnCancel);
                ContainButton.Children.Remove(btnOk);

            }
            else if (button == MessageBoxButton.OkCancel)
            {
                ContainButton.Children.Remove(btnYes);
                ContainButton.Children.Remove(btnNo);
            }
        }
        private void setIcon(MessageBoxImage img)
        {
            switch (img)
            {
                case MessageBoxImage.Information:
                    imgIcon.Icon = IconChar.Info;
                    break;
                case MessageBoxImage.Warning:
                    imgIcon.Icon = IconChar.Warning;
                    imgIcon.Foreground = new SolidColorBrush(Color.FromRgb(255, 74, 79));
                    break;
                case MessageBoxImage.Error:
                    imgIcon.Foreground = new SolidColorBrush(Color.FromRgb(255, 74, 79));
                    imgIcon.Icon = IconChar.CircleXmark;
                    break;
                case MessageBoxImage.Question:
                    imgIcon.Icon = IconChar.Question;
                    imgIcon.Foreground = new SolidColorBrush(Color.FromRgb(244, 198, 38));
                    break;
            }
        }

        public static void Show(string text, string title = "", MyMessageBox.MessageBoxButton button = MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage img = MyMessageBox.MessageBoxImage.Information)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate {
                new MyMessageBox(text, title, button, img).Show();
            });
        }
        public static void ShowDialog(string text, string title = "", MyMessageBox.MessageBoxButton button = MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage img = MyMessageBox.MessageBoxImage.Information)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate {
                new MyMessageBox(text, title, button, img).ShowDialog();
            });
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void pnlControlBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }
        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            buttonResultClicked = ButtonResult.YES;
            this.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            buttonResultClicked = ButtonResult.OK;
            this.Close();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            buttonResultClicked = ButtonResult.NO;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            buttonResultClicked = ButtonResult.CANCEL;
            this.Close();
        }
    }
}
