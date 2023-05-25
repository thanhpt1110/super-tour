using Student_wpf_application.ViewModels.Command;
using Super_Tour.Model;
using System.Windows;

namespace Super_Tour.ViewModel
{
    internal class CreateCustomerViewModel
    {
        public string customer_name;
        public string customer_IdNum;
        public string customer_PhoneNum;
        public string customer_Address;
        public RelayCommand CommandBttAdd;
        private SUPER_TOUR db = new SUPER_TOUR();
        public CreateCustomerViewModel()
        {
            CommandBttAdd = new RelayCommand(AddNewCustomer);
        }
        public void AddNewCustomer(object a)
        {
            if(string.IsNullOrEmpty(customer_name) || string.IsNullOrEmpty(customer_Address))
            {
                MessageBox.Show("Please fill all information", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }    
            CUSTOMER customer = new CUSTOMER();
            customer.Id_Customer = 1;
            customer.IdNumber = customer_IdNum;
            customer.PhoneNumber = customer_PhoneNum;
            customer.Address = customer_Address;
            customer.Name_Customer= customer_name;
            db.CUSTOMERs.Add(customer);
            db.SaveChanges();
        }
    }
}
