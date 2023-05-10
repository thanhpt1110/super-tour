using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Firebase.Storage;
using Student_wpf_application.ViewModels.Command;
using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using Super_Tour.View;

namespace Super_Tour.ViewModel
{
    internal class MainBookingViewModel: ObservableObject
    {
        private SUPER_TOUR db;
        private List<BOOKING> _listOriginalBooking;
        private DispatcherTimer timer;
        private List<BOOKING> _listSearchBooking;
        private ObservableCollection<BOOKING> _listObservableBooking;
        private string _searchItem;
        private string _selectedItem;
        public ObservableCollection<BOOKING> ListOriginalBooking
        {
            get
            {
                return _listObservableBooking;
            }
            set
            {
                _listObservableBooking = value;
                OnPropertyChanged(nameof(ListOriginalBooking));
            }
        }

        public string SearchItem { 
            get { 
                return _searchItem; 
            } 
            set
            {
                _searchItem = value;
                OnPropertyChanged(nameof(SearchItem));
            } 
        }
        public string SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }
        public ICommand OpenCreateBookingViewCommand { get; }
        public ICommand UpdateBookingViewCommand { get; }
        public ICommand DeleteBookingViewCommand { get; }
        public MainBookingViewModel() 
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += Timer_Tick;
            OpenCreateBookingViewCommand = new RelayCommand(ExecuteOpenCreateBookingViewCommand);
            _listObservableBooking = new ObservableCollection<BOOKING>();
            UpdateBookingViewCommand = new RelayCommand(ExecuteUpdateBooking);
            DeleteBookingViewCommand = new RelayCommand(ExecuteDeleteBooking);
        }

        private async void Timer_Tick(object sender, EventArgs e)
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
                        List<BOOKING> UpdateBooking = db.BOOKINGs.ToList();
                        if (!UpdateBooking.SequenceEqual(_listOriginalBooking))
                        {
                            _listOriginalBooking = UpdateBooking;
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                LoadGrid(_listOriginalBooking);

                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                    }
                });
         }
        private async void ExecuteUpdateBooking(object obj)
        {
            UpdateBookingView view = new UpdateBookingView();
            view.DataContext = new UpdateBookingViewModel();
            timer.Stop();
            view.ShowDialog();
            LoadBookingDataAsync();
        }
        private async void ExecuteDeleteBooking(object obj)
        {

            try
            {
                MyMessageBox.ShowDialog("Are you sure you want to delete this item?", "Question", MyMessageBox.MessageBoxButton.YesNo, MyMessageBox.MessageBoxImage.Warning);
                if (MyMessageBox.buttonResultClicked == MyMessageBox.ButtonResult.YES)
                {
                    await db.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                //Console.WriteLine("Lỗi: " + ex.InnerException.Message);
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
            finally
            {
                timer.Start();
            }
        }
        private void LoadGrid(List<BOOKING> listBooking)
        {
            _listObservableBooking.Clear();
            foreach (BOOKING booking in listBooking)
            {
                _listObservableBooking.Add(booking);
            }
        }
        public async Task LoadBookingDataAsync()
        {
            await Task.Run(async () =>
            {
                try
                {
                    if (db != null)
                    {
                        db.Dispose();
                    }
                    db = new SUPER_TOUR();
                    _listOriginalBooking = await db.BOOKINGs.ToListAsync();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        LoadGrid(_listOriginalBooking);
                    });
                    timer.Start();
                }
                catch (Exception ex)
                {
                    MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);

                }
            });
        }

        private void ExecuteOpenCreateBookingViewCommand(object obj)
        {
            {
                CreateBookingView createBookingView = new CreateBookingView();
                
                createBookingView.ShowDialog();

            }
        }
    }   
}
