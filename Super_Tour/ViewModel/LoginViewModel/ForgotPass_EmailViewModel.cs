using Student_wpf_application.ViewModels.Command;
using Super_Tour.Model;
using Super_Tour.View.LoginView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Super_Tour.ViewModel.LoginViewModel
{
    internal class ForgotPass_EmailViewModel
    {
        public string email_text;
        public RelayCommand command_SendEmail;
        public RelayCommand command_BackToLoginPage;
        private SUPER_TOUR db = new SUPER_TOUR();
        public ForgotPass_EmailViewModel()
        {
            command_SendEmail = new RelayCommand(sendEmail);
            command_BackToLoginPage = new RelayCommand(backToLoginPage);
        } 
        public void backToLoginPage(object a)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is ForgotPass_EmailView)
                {
                   Application.Current.MainWindow.Show();
                    window.Close();
                    break;
                }
            }
        }
        public void sendEmail(object a)
        {
            if(checkEmail())
            {
                // Sang trang tiếp theo.
                CheckOTPView view = new CheckOTPView();
                foreach(Window window in Application.Current.Windows)
                {
                    if(window is ForgotPass_EmailView)
                    {
                        view.Show();
                        window.Close();
                        break;
                    }
                }    
            }    
        }
        public bool checkEmail()
        {
            return db.ACCOUNTs.Where(p=>p.Email==email_text).SingleOrDefault()!=null;
        }    
    }
}
