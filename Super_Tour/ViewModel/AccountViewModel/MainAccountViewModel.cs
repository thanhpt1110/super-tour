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
        #region Declare variable
        private List<ACCOUNT> _listAccountOriginal;
        private ObservableCollection<ACCOUNT> _listObservableAccount;
        private string _searchUserNameAccount;
        private SUPER_TOUR db = null;
        private DispatcherTimer _timer = null;
        #endregion

        #region Declare binding
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
        #endregion

        #region Command
        public ICommand OpenCreateAccountViewCommand { get; }
        public DispatcherTimer Timer { get => _timer; set => _timer = value; }
        #endregion

        public MainAccountViewModel()
        {
            db = MainViewModel.db;
            _listObservableAccount = new ObservableCollection<ACCOUNT>();
            OpenCreateAccountViewCommand = new RelayCommand(ExecuteOpenCreateAccountViewCommand);
            LoadAccountDataAsync();
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(3);
            Timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {

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
                        db.Entry(_listAccountOriginal).Reload();
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
                _timer.Start();
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);

            }
        }
    }
}