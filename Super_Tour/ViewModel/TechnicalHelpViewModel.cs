using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Student_wpf_application.ViewModels.Command;
using Super_Tour.Ultis;

namespace Super_Tour.ViewModel
{
    internal class TechnicalHelpViewModel: ObservableObject
    {
        #region Declare variable      
        private string _title;  
        private string _description;
        private bool _isDataModified;
        #endregion

        #region Declare binding
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
                CheckDataModified();
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value; 
                OnPropertyChanged(nameof(Description));
                CheckDataModified();
            }
        }

        public bool IsDataModified
        {
            get { return _isDataModified; }
            set
            {
                _isDataModified = value;
                OnPropertyChanged(nameof(IsDataModified));
            }
        }
        #endregion

        #region Command 
        public ICommand SubmitCommand { get; }
        #endregion

        #region Constructor
        public TechnicalHelpViewModel()
        {
            SubmitCommand = new RelayCommand(ExecuteSubmitCommand);
        }
        #endregion

        #region Check data is modified 
        private void CheckDataModified()
        {
            if (string.IsNullOrEmpty(Title) || string.IsNullOrEmpty(Description))
                IsDataModified = false;
            else
                IsDataModified = true; 
        }
        #endregion

        #region Perform submit problem
        private void ExecuteSubmitCommand(object obj)
        {

        }
        #endregion
    }
}
