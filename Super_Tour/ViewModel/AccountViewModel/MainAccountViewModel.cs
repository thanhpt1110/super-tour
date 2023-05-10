using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Student_wpf_application.ViewModels.Command;
using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using Super_Tour.View;
using System.Windows;
using System.Windows.Threading;
using System.Collections.ObjectModel;

namespace Super_Tour.ViewModel
{
    internal class MainAccountViewModel : ObservableObject
    {
        private List<ACCOUNT> _listAccountOriginal;
        private ObservableCollection<ACCOUNT> _listObservableAccount;
        private string _searchUserNameAccount;
        private SUPER_TOUR db = new SUPER_TOUR();
        private DispatcherTimer timer;
        public string SearchUserNameAccount
        {
            get { return _searchUserNameAccount; }
            set
            {
                _searchUserNameAccount = value;
                OnPropertyChanged(nameof(SearchUserNameAccount));
            }
        }
        public ObservableCollection<ACCOUNT> ListObservableAccount
        {
            get
            {
                return _listObservableAccount;
            }
            set
            {
                _listObservableAccount = value;
                OnPropertyChanged(nameof(ListObservableAccount));
            }
        }
        public ICommand OpenCreateAccountViewCommand { get; }

        public MainAccountViewModel()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += Timer_Tick;
            _listObservableAccount = new ObservableCollection<ACCOUNT>();
            OpenCreateAccountViewCommand = new RelayCommand(ExecuteOpenCreateAccountViewCommand);
            LoadAccountDataAsync();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
           // throw new NotImplementedException();
        }

        private void ExecuteOpenCreateAccountViewCommand(object obj)
        {
            CreateAccountView createAccountView = new CreateAccountView();
            createAccountView.ShowDialog();
            LoadAccountDataAsync();
        }
        private async Task LoadAccountDataAsync()
        {
            try
            {
                await Task.Run(() =>
                {
                    try
                    {
                        if (db != null)
                        {
                            db.Dispose();
                        }
                        db = new SUPER_TOUR();
                        _listAccountOriginal = db.ACCOUNTs.ToList();
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _listObservableAccount.Clear();
                            foreach (var account in _listAccountOriginal)
                            {
                                _listObservableAccount.Add(account);
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);

                    }
                });
                timer.Start();
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);

            }
        }
    }
}