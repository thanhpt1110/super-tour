using MySqlX.XDevAPI.Common;
using Super_Tour.Ultis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Drawing;
using BarcodeLib;
using System.Drawing.Imaging;
using System.IO;
using ZXing;
using Student_wpf_application.ViewModels.Command;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows;
using ZXing.QrCode.Internal;
using System.Security.RightsManagement;
using System.Windows.Controls;
using Super_Tour.CustomControls;
using Super_Tour.View.TicketView;
using System.Windows.Media;
using Super_Tour.Model;
using Super_Tour.View;

namespace Super_Tour.ViewModel
{
    internal class ExportedTicketViewModel : ObservableObject
    {
        #region Declare variable
        private string _barcodeText;
        private string _touristName;
        private string _travelName;
        private string _totalDay;
        private string _totalNight;
        private string _totalDayNight;
        private string _price;
        private string _startDate;
        private string _startTime;
        private string _startLocation;
        private TICKET _selectedItem = null; 
        private TicketPrintableContent _ticketPrintableContent;
        #endregion

        #region Declare binding
        public string StartLocation
        {
            get { return _startLocation; }
            set
            {
                _startLocation = value;
                OnPropertyChanged(nameof(StartLocation));
            }
        }

        public string StartTime
        {
            get { return _startTime; }
            set
            {
                _startTime = value;
                OnPropertyChanged(nameof(StartTime));
            }
        }

        public string StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                OnPropertyChanged(nameof(StartDate));
            }
        }

        public string Price
        {
            get { return _price; }
            set 
            {
                _price = value;
                OnPropertyChanged(nameof(Price));   
            }
        }
        public string TotalDayNight
        {
            get { return _totalDayNight; }
            set
            {
                _totalDayNight = value;
                OnPropertyChanged(nameof(TotalDayNight));   
            }
        }

        public string TravelName
        {
            get { return _travelName; }
            set
            {
                _travelName = value;
                OnPropertyChanged(nameof(TravelName));
            }
        }

        public string TouristName
        {
            get { return _touristName; }
            set
            {
                _touristName = value;
                OnPropertyChanged(nameof(TouristName));
            }
        }
        
        public string BarcodeText
        {
            get { return _barcodeText; }
            set
            {
                _barcodeText = value;
                OnPropertyChanged(nameof(BarcodeText));
                OnPropertyChanged(nameof(BarcodeImage));
            }
        }

        protected TicketPrintableContent TicketPrintableContent
        {
            get { return _ticketPrintableContent; }
        }

        public BitmapSource BarcodeImage
        {
            get { return GenerateBarcodeImage(); }
        }

        public BitmapSource BarcodeImage2
        {
            get { return GenerateBarcodeImage2(); }
        }

        #endregion

        #region Declare Command
        public ICommand PrintTicketCommand { get; private set; }
        #endregion

        #region Generate Bardcode
        private BitmapSource GenerateBarcodeImage()
        {
            // Tạo mã vạch bằng thư viện BarcodeLib
            BarcodeWriter barcodeWriter = new BarcodeWriter();
            barcodeWriter.Format = BarcodeFormat.CODE_128;
            Bitmap bitmap = barcodeWriter.Write(BarcodeText);

            // Xoay ảnh
            bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);

            // Cắt ảnh theo kích thước của mã vạch để loại bỏ chuỗi gốc
            int barcodeWidth = bitmap.Width - 30;
            int barcodeHeight = bitmap.Height; // 30 là chiều cao của chuỗi gốc
            Rectangle cropRect = new Rectangle(0, 0, barcodeWidth, barcodeHeight);
            Bitmap croppedBitmap = bitmap.Clone(cropRect, bitmap.PixelFormat);

            // Chuyển đổi thành BitmapSource
            BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                croppedBitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            return bitmapSource;
        }

        private BitmapSource GenerateBarcodeImage2()
        {
            // Tạo mã vạch bằng thư viện BarcodeLib
            BarcodeWriter barcodeWriter = new BarcodeWriter();
            barcodeWriter.Format = BarcodeFormat.CODE_128;
            Bitmap bitmap = barcodeWriter.Write(BarcodeText);

            // Xoay ảnh
            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);

            // Cắt ảnh theo kích thước của mã vạch để loại bỏ chuỗi gốc
            int barcodeWidth = bitmap.Width-30;
            int barcodeHeight = bitmap.Height; // 30 là chiều cao của chuỗi gốc
            Rectangle cropRect = new Rectangle(30, 0, barcodeWidth, barcodeHeight);
            Bitmap croppedBitmap = bitmap.Clone(cropRect, bitmap.PixelFormat);

            // Chuyển đổi thành BitmapSource
            BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                croppedBitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            return bitmapSource;
        }
        #endregion

        #region Constructor
        public ExportedTicketViewModel(TICKET ticket)
        {
            // Create objects 
            _selectedItem = ticket; 
            BarcodeText = "TK" + _selectedItem.Id_Ticket.ToString();
            TouristName = ticket.TOURIST.Name_Tourist;
            TravelName = ticket.TOURIST.BOOKING.TRAVEL.TOUR.Name_Tour;
            _totalDay = ticket.TOURIST.BOOKING.TRAVEL.TOUR.TotalDay.ToString();
            _totalNight = ticket.TOURIST.BOOKING.TRAVEL.TOUR.TotalNight.ToString();
            TotalDayNight = _totalDay + " DAY - " + _totalNight + " NIGHT";

            int totalTourists = ticket.TOURIST.BOOKING.TOURISTs.Count();
            decimal ticketPrice = 0;
            if (totalTourists != 0)     
                ticketPrice = ticket.TOURIST.BOOKING.TotalPrice / totalTourists;
            else
                ticketPrice = ticket.TOURIST.BOOKING.TotalPrice;
            _price = ticketPrice.ToString("#,#") + " VND";

            _startDate = ticket.TOURIST.BOOKING.TRAVEL.StartDateTimeTravel.ToString("dd/MM/yyyy");
            _startTime = ticket.TOURIST.BOOKING.TRAVEL.StartDateTimeTravel.ToString("HH/mm/ss");
            _startLocation = ticket.TOURIST.BOOKING.TRAVEL.StartLocation.ToString();

            // Create command
            PrintTicketCommand = new RelayCommand(ExecutePrintTicketCommand);
        }
        #endregion

        #region Print ticket
        private void ExecutePrintTicketCommand(object obj)
        {
            try
            {
                _ticketPrintableContent = new TicketPrintableContent();
                _ticketPrintableContent.DataContext = this;

                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    // Get the printable area of the page
                    double pageWidth = printDialog.PrintableAreaWidth;
                    double pageHeight = printDialog.PrintableAreaHeight;

                    // Adjust the size of the visual to fit within the printable area of the page
                    double scaleX = pageWidth / _ticketPrintableContent.Width;
                    double scaleY = pageHeight / _ticketPrintableContent.Height;
                    double scale = Math.Min(scaleX, scaleY);
                    _ticketPrintableContent.LayoutTransform = new ScaleTransform(scale, scale);

                    printDialog.PrintVisual(_ticketPrintableContent, "Print Ticket");
                    MyMessageBox.ShowDialog("Print ticket successfully!", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);

                    // Process UI event
                    ExportedTicketView exportedTicketView = null;
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window is ExportedTicketView)
                        {
                            exportedTicketView = window as ExportedTicketView;
                            break;
                        }
                    }
                    exportedTicketView.Close();
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
            finally { }
        }
        #endregion
    }
}
