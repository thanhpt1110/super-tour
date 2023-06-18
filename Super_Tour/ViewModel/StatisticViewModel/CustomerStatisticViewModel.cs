using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Student_wpf_application.ViewModels.Command;
using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Linq;
using Xceed.Document.NET;
using Xceed.Words.NET;
using System.Drawing.Imaging;
using System.Drawing;
using System.Windows.Documents;
using SautinSoft.Document;

namespace Super_Tour.ViewModel
{
    internal class CustomerStatisticViewModel: ObservableObject
    {
        #region Define CustomerStatisticClass

        public class CustomerStatistic: ObservableObject
        {
            private string _customerName;
            private int _totalBooking;
            private decimal _totalRevenue;
            public CustomerStatistic()
            {
                // Constructor mặc định
            }
            public CustomerStatistic(string customerName, int totalBooking, decimal totalRevenue)
            {
                CustomerName = customerName;
                TotalBooking = totalBooking;
                TotalRevenue = totalRevenue;
            }

            public string CustomerName 
            { 
                get => _customerName;
                set
                {
                    _customerName = value;
                    OnPropertyChanged(nameof(CustomerName));
                }
            }
            public int TotalBooking 
            { 
                get => _totalBooking;
                set
                {
                    _totalBooking = value;
                    OnPropertyChanged(nameof(TotalBooking));
                }
            }
            public decimal TotalRevenue
            {
                get => _totalRevenue;
                set
                {
                    _totalRevenue = value;
                    OnPropertyChanged(nameof(TotalRevenue));
                }
            }
        }

        #endregion

        #region Define CustomerChart Class
        public class CustomerChart: ObservableObject
        {
            private DateTime _bookingDate;
            private int _customerCount;

            public CustomerChart()
            {

            }
            public CustomerChart(DateTime bookingDate, int customerCount)
            {
                _bookingDate = bookingDate;
                _customerCount = customerCount;
            }

            public DateTime BookingDate
            { 
                get => _bookingDate;
                set
                {
                    _bookingDate = value;
                    OnPropertyChanged(nameof(BookingDate));
                }
            }
            public int CustomerCount
            {
                get => _customerCount;
                set
                {
                    _customerCount = value;
                    OnPropertyChanged(nameof(CustomerCount));
                }
            }
        }


        #endregion

        #region Declare variable

        private DispatcherTimer _timer = null;
        private UPDATE_CHECK _trackerCustomer = null;
        private UPDATE_CHECK _trackerTicket = null;
        private UPDATE_CHECK _trackerTravel = null;
        private UPDATE_CHECK _trackerBooking = null;
        private DateTime lastUpdate;
        private ObservableCollection<CustomerStatistic> _customerStatisticList;
        private SUPER_TOUR db = null;
        private int _totalCustomer;
        private int _totalReBookingCustomer;
        private int _totalTicket;
        private DateTime _startDate;

        #endregion

        #region Declare binding
        public DispatcherTimer Timer { get => _timer; set => _timer = value; }

        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                OnPropertyChanged(nameof(StartDate));
                LoadChart();
            }
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                OnPropertyChanged(nameof(EndDate));
                LoadChart();
            }
        }
        private SeriesCollection _customerSeries;
        private List<string> _labels;

        public SeriesCollection CustomerSeries
        {
            get { return _customerSeries; }
            set
            {
                _customerSeries = value;
                OnPropertyChanged(nameof(CustomerSeries));
            }
        }

        public List<string> Labels
        {
            get { return _labels; }
            set
            {
                _labels = value;
                OnPropertyChanged(nameof(Labels));
            }
        }
        public int TotalCustomer 
        { 
            get => _totalCustomer;
            set
            {
                _totalCustomer = value;
                OnPropertyChanged(nameof(TotalCustomer));
            }
        }
        public int TotalReBookingCustomer 
        { 
            get => _totalReBookingCustomer;
            set
            {
                _totalReBookingCustomer = value;
                OnPropertyChanged(nameof(TotalReBookingCustomer));
            }
        }
        public int TotalTicket 
        { 
            get => _totalTicket;
            set
            {
                _totalTicket = value;
               OnPropertyChanged(nameof(TotalTicket));
            }
        }

        public ObservableCollection<CustomerStatistic> CustomerStatisticList 
        { 
            get => _customerStatisticList;
            set
            {
                _customerStatisticList = value;
                OnPropertyChanged(nameof(CustomerStatisticList));
            }
        }

        #endregion
        #region Command
        public RelayCommand PrintToImageCommand { get; set; }
        #endregion

        #region Constructor
        public CustomerStatisticViewModel()
        {
            db = SUPER_TOUR.db;
            CustomerStatisticList = new ObservableCollection<CustomerStatistic>();
            PrintToImageCommand = new RelayCommand(ExecutePrint);
            StartDate = DateTime.Parse("01/06/2023");
            EndDate = DateTime.Today;
            lastUpdate = DateTime.Now;
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(0.5);
            Timer.Tick += Timer_Tick;
            

            LoadDataAsync();

        }
        #endregion
        #region Print chart
        
        private void ExecutePrint(object obj)
        {
            try
            {
                CartesianChart chart = obj as CartesianChart;
                var streamBitmap = new MemoryStream();
                Bitmap chartBitmap = ConvertChartToBitmap(chart);
                BitmapImage chartBitmapImage = ConvertFromBitmapToBitmapImage(chartBitmap, streamBitmap);
                var saveFileDialog = new System.Windows.Forms.SaveFileDialog();
                saveFileDialog.Filter = "PDF Files|*.pdf";
                saveFileDialog.Title = "Save PDF Document";
                saveFileDialog.ShowDialog();
                if (saveFileDialog.FileName != "")
                {
                    // Tạo tài liệu Word
                    var doc = DocX.Create("myDocx.docx");

                    // Tạo đầu đề "CUSTOMER REPORT 16/06/2023"
                    var headingFormat = new Formatting
                    {
                        FontFamily = new Xceed.Document.NET.Font("Times New Roman"),
                        Size = 16,
                        Bold = true
                    };
                    var heading = doc.InsertParagraph("CUSTOMER REPORT", false, headingFormat);
                    heading.Alignment = Alignment.center;
                    var dateTimeReportFormat = new Formatting
                    {
                        FontFamily = new Xceed.Document.NET.Font("Times New Roman"),
                        Size = 13,
                        Italic = true
                    };
                    doc.InsertParagraph("");
                    var dateTimeReport = doc.InsertParagraph("At " + DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy"), false, dateTimeReportFormat);
                    dateTimeReport.Alignment = Alignment.center;
                    #region Add Table Report
                    // Thêm bảng vào
                    var tableTitleFormat = new Formatting
                    {
                        FontFamily = new Xceed.Document.NET.Font("Times New Roman"),
                        Size = 13,
                        Bold = true
                    };
                    var tableBodyFormat = new Formatting
                    {
                        FontFamily = new Xceed.Document.NET.Font("Times New Roman"),
                        Size = 13
                    };
                    doc.InsertParagraph("");
                    Xceed.Document.NET.Table table = doc.AddTable(_customerStatisticList.Count + 1, 3);
                    table.Alignment = Alignment.center;
                    table.Rows[0].Cells[0].Paragraphs.First().Append("Customer", tableTitleFormat).Alignment = Alignment.left;
                    table.Rows[0].Cells[1].Paragraphs.First().Append("Total bookings", tableTitleFormat).Alignment = Alignment.center;
                    table.Rows[0].Cells[2].Paragraphs.First().Append("Total revenue", tableTitleFormat).Alignment = Alignment.right;
                    int i = 1;
                    foreach (var customer in _customerStatisticList)
                    {
                        table.Rows[i].Cells[0].Paragraphs.First().Append(customer.CustomerName.ToString(), tableBodyFormat).Alignment = Alignment.left;
                        table.Rows[i].Cells[1].Paragraphs.First().Append(customer.TotalBooking.ToString(), tableBodyFormat).Alignment = Alignment.center;
                        table.Rows[i].Cells[2].Paragraphs.First().Append(customer.TotalRevenue.ToString("#,#"), tableBodyFormat).Alignment = Alignment.right;
                        i++;
                    }
                    doc.InsertTable(table);
                    #endregion
                    #region Add chart into docx file
                    // Thêm hình ảnh chart vào 
                    doc.InsertParagraph("");
                    doc.InsertParagraph("");
                    var chartPicture = doc.AddImage(chartBitmapImage.StreamSource);
                    var chartPictureParagraph = doc.InsertParagraph("", false);
                    chartPictureParagraph.AppendPicture(chartPicture.CreatePicture());
                    chartPictureParagraph.Alignment = Alignment.center;
                    var NameChartFormat = new Formatting
                    {
                        FontFamily = new Xceed.Document.NET.Font("Times New Roman"),
                        Size = 13,
                        Italic = true,
                        Bold = true
                    };
                    var NameChart = doc.InsertParagraph("Customer chart from " + _startDate.ToString("dd/MM/yyyy") + " to " + _endDate.ToString("dd/MM/yyyy"), false, NameChartFormat);
                    NameChart.InsertParagraphBeforeSelf("");
                    NameChart.Alignment = Alignment.center;
                    #endregion
                    #region Add 3 total
                    doc.InsertParagraph("");
                    // Thêm các đoạn văn bản "Service: Admin", "Total customer: 7", "Total re-booking customers: 0" và "Total ticket: 8"
                    string[] texts = new string[] { "Total customer: " , _totalCustomer.ToString() + ".", "Total re-booking customers: " , _totalReBookingCustomer.ToString() + "."
                    , "Total ticket: "  , _totalTicket.ToString() + "." };
                    var paragraphFormat = new Formatting
                    {
                        FontFamily = new Xceed.Document.NET.Font("Times New Roman"),
                        Size = 14,
                        Bold = false,
                        Italic = false
                    };
                    var boldFormat = new Formatting
                    {
                        FontFamily = new Xceed.Document.NET.Font("Times New Roman"),
                        Size = 14,
                        Bold = true
                    };
                    for (i = 0; i < texts.Length; i++)
                    {
                        var paragraph1 = doc.InsertParagraph(texts[i], false, paragraphFormat);
                        paragraph1.InsertText(texts[++i], false, boldFormat);
                    }
                    #endregion
                    #region Add reporter
                    doc.InsertParagraph("");
                    doc.InsertParagraph("");
                    var paragraphReporterTitle = doc.InsertParagraph(); 
                    paragraphReporterTitle.Append("Reporter ", boldFormat);
                    paragraphReporterTitle.IndentationFirstLine = 230.0f; // Thụt đầu dòng 230 điểm ảnh
                    paragraphReporterTitle.Alignment = Alignment.center;
                    doc.InsertParagraph("");
                    string name = MyApp.CurrentUser.Account_Name;
                    var paragraphReporterName = doc.InsertParagraph();
                    paragraphReporterName.Append(name, paragraphFormat);
                    paragraphReporterName.IndentationFirstLine = paragraphReporterTitle.IndentationFirstLine - (name.Length - "Reporter".Length) / 2;

                    paragraphReporterName.Alignment = Alignment.center;
                    #endregion
                    #region Save word and pdf
                    // Lưu tài liệu Word
                    doc.Save();
                    DocumentCore documentCore = DocumentCore.Load("myDocx.docx");
                    documentCore.Save(saveFileDialog.FileName);
                    File.Delete("myDocx.docx");
                    #endregion
                    MyMessageBox.ShowDialog("Export file successfully!", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
                }
            }   
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        public Bitmap ConvertChartToBitmap(CartesianChart chart)
        {
            int maxWidth = 600;
            // Tạo đối tượng RenderTargetBitmap với kích thước bằng với kích thước của CartesianChart
            var renderBitmap = new RenderTargetBitmap(
                (int)chart.ActualWidth, (int)chart.ActualHeight,
                96, 96, PixelFormats.Pbgra32);

            // Vẽ CartesianChart vào đối tượng RenderTargetBitmap
            renderBitmap.Render(chart);

            // Tạo đối tượng Bitmap từ đối tượng RenderTargetBitmap
            var chartBitmap = new Bitmap(
                renderBitmap.PixelWidth, renderBitmap.PixelHeight,
                System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            // Sao chép dữ liệu pixel từ đối tượng RenderTargetBitmap sang đối tượng Bitmap
            var bitmapData = chartBitmap.LockBits(
                new Rectangle(0, 0, chartBitmap.Width, chartBitmap.Height),
                ImageLockMode.WriteOnly, chartBitmap.PixelFormat);

            renderBitmap.CopyPixels(
                Int32Rect.Empty, bitmapData.Scan0,
                bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            chartBitmap.UnlockBits(bitmapData);
            if (chartBitmap.Width > maxWidth)
            {
                var scaleFactor = (float)maxWidth / (float)chartBitmap.Width;
                var newWidth = (int)(chartBitmap.Width * scaleFactor);
                var newHeight = (int)(chartBitmap.Height * scaleFactor* 1.2);
                var newBitmap = new Bitmap(chartBitmap, new System.Drawing.Size(newWidth, newHeight));
                chartBitmap.Dispose();
                chartBitmap = newBitmap;
            }
            // Trả về đối tượng Bitmap
            return chartBitmap;
        }
        private BitmapImage ConvertFromBitmapToBitmapImage(Bitmap chartBitmap, MemoryStream chartMemoryStream)
        {
            // Lưu hình ảnh vào đối tượng MemoryStream
            chartBitmap.Save(chartMemoryStream, ImageFormat.Png);
            // Tạo đối tượng BitmapImage từ đối tượng MemoryStream
            var chartBitmapImage = new BitmapImage();
            chartBitmapImage.BeginInit();
            chartBitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            chartBitmapImage.StreamSource = chartMemoryStream;
            chartBitmapImage.EndInit();
            // Trả về đối tượng BitmapImage
            return chartBitmapImage;
        }
        #endregion

        #region LoadData
        private async Task LoadDataAsync()
        {
            try
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            LoadChart();
                        });
                    }
                    catch (Exception ex)
                    {
                        MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                    }
                });
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        #endregion

        #region LoadChart
        private void LoadChart()
        {
            TotalCustomer = db.CUSTOMERs.Count();
            TotalReBookingCustomer = db.BOOKINGs.GroupBy(bd => bd.Id_Customer_Booking).Count(g => g.Count() > 1);
            TotalTicket = db.TICKETs.Count();
            var startDateParam = new SqlParameter("@StartDate", StartDate);
            var endDateParam = new SqlParameter("@EndDate", EndDate);
            
            var sqlQuery = @"
                            SELECT CUSTOMER.Name_Customer AS CustomerName,
                                COUNT(BOOKING.Id_Booking) AS TotalBooking,
                                SUM(BOOKING.TotalPrice * (1 - TRAVEL.Discount / 100))  AS TotalRevenue
                                FROM CUSTOMER
                                JOIN BOOKING ON CUSTOMER.ID_Customer = BOOKING.Id_Customer_Booking
                                JOIN TRAVEL ON BOOKING.Id_Travel = TRAVEL.Id_Travel
                                JOIN TOUR ON TRAVEL.Id_Tour = TOUR.Id_Tour
                                WHERE BOOKING.Status = 'Paid'
                                GROUP BY CUSTOMER.Name_Customer";


            var result = db.Database.SqlQuery<CustomerStatistic>(sqlQuery);
            Console.WriteLine(result);

            CustomerStatisticList.Clear();  
            foreach (var item in result)
            {
                // Tạo một đối tượng CustomerStatistic từ kết quả truy vấn
                CustomerStatistic customerStatistic = new CustomerStatistic(item.CustomerName, item.TotalBooking, item.TotalRevenue);

                // Thêm đối tượng CustomerStatistic vào ObservableCollection
                CustomerStatisticList.Add(customerStatistic);
            }

            //Load Chart Data
            // Khởi tạo SeriesCollection và Labels
            CustomerSeries = new SeriesCollection();
            Labels = new List<string>();


            var customerCounts = db.BOOKINGs
                 .Where(b => b.Booking_Date >= StartDate && b.Booking_Date <= EndDate)
                 .GroupBy(b => b.Booking_Date)
                 .Select(g => new { Date = g.Key, Count = g.Select(b => b.Id_Customer_Booking).Distinct().Count() })
                 .OrderBy(item => item.Date)
                 .ToList();

            // Xử lý dữ liệu để hiển thị trên biểu đồ
            var values = new ChartValues<int>();

            foreach (var item in customerCounts)
            {
                values.Add(item.Count);
                Labels.Add(item.Date.ToString("dd/MM/yyyy")); // Hàm GetFormattedDate để định dạng ngày theo yêu cầu của bạn
            }

            // Tạo Series và thêm vào SeriesCollection
            var series = new ColumnSeries
            {
                Title = "Number of Customers",
                Values = values
            };


            CustomerSeries.Add(series);
        }
        #endregion

        #region Check data persecond
        private async void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                await Task.Run(async () =>
                {
                    // Compare customer
                    _trackerCustomer = UPDATE_CHECK.getTracker("UPDATE_CUSTOMER");
                    if (DateTime.Parse(_trackerCustomer.DateTimeUpdate) > lastUpdate)
                    {
                        lastUpdate = (DateTime.Parse(_trackerCustomer.DateTimeUpdate));
                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            LoadDataAsync();
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        #endregion

    }
}
