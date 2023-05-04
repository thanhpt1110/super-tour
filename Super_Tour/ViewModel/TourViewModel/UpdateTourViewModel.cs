using Student_wpf_application.ViewModels.Command;
using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using Super_Tour.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static Super_Tour.ViewModel.CreateTourViewModel;

namespace Super_Tour.ViewModel
{
    internal class UpdateTourViewModel: ObservableObject
    {
        private TOUR _tour;
        private ObservableCollection<DateActivity> _listDateActitvities;
        private SUPER_TOUR db = new SUPER_TOUR();
        private string _nameTour;
        private int _totalDay;
        private int _totalNight;
        private bool _executeSave = true;
        public int TotalDay
        {
            get
            {
                return _totalDay;
            }
            set
            {
                _totalDay = value;
                OnPropertyChanged(nameof(TotalDay));
            }
        }
        public int TotalNight
        {
            get
            {
               return _totalNight;
            }
            set
            {
                _totalNight = value;
                OnPropertyChanged(nameof(TotalNight));
            }
        }
        public string NameTour
        {
            get { return _nameTour; }
            set
            {
                _nameTour = value;
                OnPropertyChanged(nameof(NameTour));
            }
        }
        
        public ObservableCollection<DateActivity> ListDateAcitivities
        {
            get
            {
                return _listDateActitvities;
            }
            set
            {
                _listDateActitvities = value;
                OnPropertyChanged(nameof(ListDateAcitivities));
            }
        }
        public ICommand SaveUpdateTourCommand { get; }
        public ICommand AddADayCommand { get; }

        public UpdateTourViewModel() 
        {

        }
        public UpdateTourViewModel(TOUR tour)
        {
            _tour = tour;
            NameTour = tour.Name_Tour;
            _listDateActitvities = new ObservableCollection<DateActivity>();
            AddADayCommand = new RelayCommand(ExecuteAddADayCommand);
            SaveUpdateTourCommand= new RelayCommand(ExecuteCreateTourCommand);
            LoadPage();
        }
        
        private async Task LoadPage()
        {
            await Task.Run(() => {
                List<TOUR_DETAILS> listTourDetail = db.TOUR_DETAILs.Where(p => p.Id_Tour == _tour.Id_Tour).ToList();
                Application.Current.Dispatcher.Invoke(() => {
                    int i = 1;
                    while(listTourDetail.Where(p=>p.Date_Order_Package==i).ToList().Count>0)
                    {
                        List<TOUR_DETAILS>
                        dateNum = listTourDetail.Where(p => p.Date_Order_Package == i).ToList();
                        DateActivity dateActivity = new DateActivity(i,true,_tour);
                        foreach (TOUR_DETAILS date in dateNum)
                        {
                            GridActivity gridActivity = new GridActivity() { Tour_detail = date, TimeOfPackage = DateTime.Now.Date.Add(date.Start_Time_Package), PackageName = db.PACKAGEs.Find(date.Id_Package).Name_Package };

                            if (date.Session == Constant.MORNING)
                            {
                                dateActivity.MorningActivities.Add(gridActivity);
                            }
                            else if (date.Session == Constant.AFTERNOON)
                            {
                                dateActivity.AfternoonActivities.Add(gridActivity);
                            }
                            else
                            {
                                dateActivity.EveningActivities.Add(gridActivity);
                            }
                        }    
                            _listDateActitvities.Add(dateActivity);
                        i++;
                    }
                    TotalDay = _listDateActitvities.Count;
                    TotalNight = _listDateActitvities.Count - 1;
                });
                });
        }
        private async void ExecuteCreateTourCommand(object obj)
        {
            if (string.IsNullOrEmpty(_nameTour) || _listDateActitvities.Count == 0)
            {
                MyMessageBox.ShowDialog("Please fill all information.", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                return;
            }
            try
            {
                List<TOUR_DETAILS> listTourDetail = db.TOUR_DETAILs.Where(p => p.Id_Tour == _tour.Id_Tour).ToList();
                _executeSave = false;
                int i = 1;
                int IdTour = _tour.Id_Tour;
                foreach (DateActivity dateActivity in _listDateActitvities)
                {
                    if (listTourDetail.Where(p => p.Date_Order_Package == i).ToList().Count == 0)
                    {
                            foreach (GridActivity activity in dateActivity.MorningActivities)
                            {
                                TOUR_DETAILS tourDetail = activity.Tour_detail;
                                tourDetail.Id_Tour = IdTour;
                                tourDetail.Date_Order_Package = i;
                                tourDetail.Start_Time_Package = activity.TimeOfPackage.TimeOfDay;
                                tourDetail.Id_TourDetails = 1;
                                tourDetail.Session = Constant.MORNING;
                                db.TOUR_DETAILs.Add(tourDetail);
                            }
                            foreach (GridActivity activity in dateActivity.AfternoonActivities)
                            {
                                TOUR_DETAILS tourDetail = activity.Tour_detail;
                                tourDetail.Date_Order_Package = i;
                                tourDetail.Start_Time_Package = activity.TimeOfPackage.TimeOfDay;
                                tourDetail.Session = Constant.AFTERNOON;
                                tourDetail.Id_TourDetails = 1;
                                tourDetail.Id_Tour = IdTour;
                                db.TOUR_DETAILs.Add(tourDetail);
                            }
                            foreach (GridActivity activity in dateActivity.EveningActivities)
                            {
                                TOUR_DETAILS tourDetail = activity.Tour_detail;
                                tourDetail.Id_Tour = IdTour;
                                tourDetail.Id_TourDetails = 1;
                                tourDetail.Date_Order_Package = i;
                                tourDetail.Start_Time_Package = activity.TimeOfPackage.TimeOfDay;
                                tourDetail.Session = Constant.EVENING;
                                db.TOUR_DETAILs.Add(tourDetail);
                            }
                        
                    }
                    else
                    {
                        foreach (GridActivity activity in dateActivity.MorningActivities)
                        {
                            TOUR_DETAILS tourDetail = activity.Tour_detail;
                            tourDetail.Session = Constant.MORNING;
                            tourDetail.Start_Time_Package = activity.TimeOfPackage.TimeOfDay;
                            tourDetail.Date_Order_Package = i;
                            db.TOUR_DETAILs.AddOrUpdate(tourDetail);
                        }
                        foreach (GridActivity activity in dateActivity.AfternoonActivities)
                        {
                            TOUR_DETAILS tourDetail = activity.Tour_detail;
                            tourDetail.Date_Order_Package = i;
                            tourDetail.Session = Constant.AFTERNOON;
                            tourDetail.Start_Time_Package = activity.TimeOfPackage.TimeOfDay;
                            db.TOUR_DETAILs.AddOrUpdate(tourDetail);
                        }
                        foreach (GridActivity activity in dateActivity.EveningActivities)
                        {
                            TOUR_DETAILS tourDetail = activity.Tour_detail;
                            tourDetail.Date_Order_Package = i;
                            tourDetail.Session = Constant.EVENING;
                            tourDetail.Start_Time_Package = activity.TimeOfPackage.TimeOfDay;
                            db.TOUR_DETAILs.AddOrUpdate(tourDetail);
                        }
                        List<TOUR_DETAILS> tOUR_DETAILs= db.TOUR_DETAILs.Where(p => p.Id_Tour == _tour.Id_Tour && p.Date_Order_Package==i).ToList();
                        while (dateActivity.MorningActivities.Count< tOUR_DETAILs.Where(p=>p.Session==Constant.MORNING).ToList().Count)
                        {
                            TOUR_DETAILS tour_detail = tOUR_DETAILs.Where(p => p.Session == Constant.EVENING).ToList().Last();
                            tOUR_DETAILs.Remove(tour_detail);
                            db.TOUR_DETAILs.Remove(tour_detail);
                        }
                        while (dateActivity.AfternoonActivities.Count < tOUR_DETAILs.Where(p=> p.Session == Constant.AFTERNOON).ToList().Count)
                        {
                            TOUR_DETAILS tour_detail = tOUR_DETAILs.Where(p => p.Session == Constant.AFTERNOON).ToList().Last();
                            tOUR_DETAILs.Remove(tour_detail);
                            db.TOUR_DETAILs.Remove(tour_detail);
                        }
                        while (dateActivity.EveningActivities.Count < tOUR_DETAILs.Where(p=>p.Session == Constant.EVENING).ToList().Count)
                        {
                            TOUR_DETAILS tour_detail = tOUR_DETAILs.Where(p => p.Session == Constant.EVENING).ToList().Last();
                            tOUR_DETAILs.Remove(tour_detail);
                            db.TOUR_DETAILs.Remove(tour_detail);
                        }
                    }
                    i++;
                }
                await db.SaveChangesAsync();
                MyMessageBox.ShowDialog("Update tour successful!", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
                UpdateTourView updateTourView = null;
                foreach (Window window in Application.Current.Windows)
                {
                    Console.WriteLine(window.ToString());
                    if (window is UpdateTourView)
                    {
                        updateTourView = window as UpdateTourView;
                        break;
                    }
                }
                updateTourView.Close();
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
            finally
            {
                _executeSave = true;
            }

        }
        private void ExecuteAddADayCommand(object obj)
        {
            DateActivity dateActivity = new DateActivity(ListDateAcitivities.Count + 1);
            ListDateAcitivities.Add(dateActivity);
            TotalNight = (ListDateAcitivities.Count - 1);
            TotalDay = ListDateAcitivities.Count;
        }
    }
}
