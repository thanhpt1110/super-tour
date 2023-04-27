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

        private BitmapImage MyImage = null;
        private void setIcon(MessageBoxImage img)
        {
            switch (img)
            {
                case MessageBoxImage.Information:
                    MyImage = new BitmapImage(new Uri("/images/message.png", UriKind.Relative));
                    imgIcon.Source = MyImage;
                    imgIcon.Stretch = Stretch.Fill;
                    break;
                case MessageBoxImage.Warning:

                    break;
                case MessageBoxImage.Error:

                    break;
                case MessageBoxImage.Question:

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
