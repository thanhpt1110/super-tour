using LiveCharts;
using LiveCharts.Wpf;
using SautinSoft.Document;
using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Threading;
using Xceed.Document.NET;
using Xceed.Words.NET;
using static Super_Tour.ViewModel.CustomerStatisticViewModel;
using Student_wpf_application.ViewModels.Command;

namespace Super_Tour.ViewModel
{
    internal class TravelStatisticViewModel: ObservableObject
    {
        #region Declare TravelBookingDate Class

        public class TravelBookingDate: ObservableObject
        {
            private string _date;
            private int _count;
            public TravelBookingDate()
            {

            }
            public TravelBookingDate(string date, int count)
            {
                Date = date;
                Count = count;
            }

            public string Date 
            { 
                get => _date;
                set
                {
                    _date = value;
                    OnPropertyChanged(nameof(Date));
                }
            }
            public int Count 
            { 
                get => _count; 
                set 
                {
                    _count = value;
                    OnPropertyChanged(nameof(Count));
                }
            }
        }

        #endregion

        #region Declare variable

        private DispatcherTimer _timer = null;
        private UPDATE_CHECK _trackerBooking = null;
        private DateTime lastUpdateBooking;
        private SUPER_TOUR db = null;
        private ObservableCollection<TravelStatistic> _travelStatisticList;
        private int _totalTravel;
        private int _totalBooking;
        private int _totalCancelBooking;
        private TravelBookingDate _top1TravelBookingDate;
        private TravelBookingDate _top2TravelBookingDate;
        private TravelBookingDate _top3TravelBookingDate;
        private DateTime _startDate;
        private DateTime _endDate;
        private SeriesCollection _bookingSeries;
        private List<string> _labels;

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
                LoadChartAsync();
            }
        }

        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                OnPropertyChanged(nameof(EndDate));
                LoadChartAsync();
            }
        }
        public SeriesCollection BookingSeries
        {
            get { return _bookingSeries; }
            set
            {
                _bookingSeries = value;
                OnPropertyChanged(nameof(BookingSeries));
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
        public ObservableCollection<TravelStatistic> TravelStatisticList
        {
            get => _travelStatisticList;
            set {
                _travelStatisticList = value;
                OnPropertyChanged(nameof(TravelStatisticList));
            }
        }
        public int TotalTravel
        {
            get => _totalTravel;
            set
            {
                _totalTravel = value;
                OnPropertyChanged(nameof(TotalTravel));
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
        public int TotalCancelBooking 
        { 
            get => _totalCancelBooking;
            set
            {
                _totalCancelBooking = value;
                OnPropertyChanged(nameof(TotalCancelBooking));
            }
        }

        public TravelBookingDate Top1TravelBookingDate
        {
            get => _top1TravelBookingDate;
            set
            {
                _top1TravelBookingDate = value;
                OnPropertyChanged(nameof(Top1TravelBookingDate));
            }
        }
        public TravelBookingDate Top2TravelBookingDate
        {
            get => _top2TravelBookingDate;
            set
            {
                _top2TravelBookingDate = value;
                OnPropertyChanged(nameof(Top2TravelBookingDate));
            }
        }
        public TravelBookingDate Top3TravelBookingDate 
        { 
            get => _top3TravelBookingDate; 
            set 
            {
                _top3TravelBookingDate = value;
                OnPropertyChanged(nameof(Top3TravelBookingDate));
            }
        }

        #endregion

        #region Command
        public RelayCommand PrintToImageCommand { get; set; }
        #endregion

        #region Constructor
        public TravelStatisticViewModel()
        {
            db = SUPER_TOUR.db;
            _top1TravelBookingDate = new TravelBookingDate();
            _top2TravelBookingDate = new TravelBookingDate();
            _top3TravelBookingDate = new TravelBookingDate();
            PrintToImageCommand = new RelayCommand(ExecutePrint);
            TravelStatisticList = new ObservableCollection<TravelStatistic>();
            StartDate = DateTime.Parse("01/06/2023");
            EndDate = DateTime.Today;


            lastUpdateBooking = DateTime.Now;
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
                    var heading = doc.InsertParagraph("TRAVEL REPORT", false, headingFormat);
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
                    Xceed.Document.NET.Table table = doc.AddTable(_travelStatisticList.Count + 1, 3);
                    table.Alignment = Alignment.center;
                    table.Rows[0].Cells[0].Paragraphs.First().Append("Travel name", tableTitleFormat).Alignment = Alignment.left;
                    table.Rows[0].Cells[1].Paragraphs.First().Append("Total bookings", tableTitleFormat).Alignment = Alignment.center;
                    table.Rows[0].Cells[2].Paragraphs.First().Append("Total revenue", tableTitleFormat).Alignment = Alignment.right;
                    int i = 1;
                    foreach (var travel in _travelStatisticList)
                    {
                        table.Rows[i].Cells[0].Paragraphs.First().Append(travel.TravelName.ToString(), tableBodyFormat).Alignment = Alignment.left;
                        table.Rows[i].Cells[1].Paragraphs.First().Append(travel.TotalBooking.ToString(), tableBodyFormat).Alignment = Alignment.center;
                        table.Rows[i].Cells[2].Paragraphs.First().Append(travel.TotalRevenue.ToString("#,#"), tableBodyFormat).Alignment = Alignment.right;
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
                    var NameChart = doc.InsertParagraph("Travel chart from " + _startDate.ToString("dd/MM/yyyy") + " to " + _endDate.ToString("dd/MM/yyyy"), false, NameChartFormat);
                    NameChart.InsertParagraphBeforeSelf("");
                    NameChart.Alignment = Alignment.center;
                    #endregion
                    #region Add top 3 revenue
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
                    doc.InsertParagraph("");

                    Xceed.Document.NET.Paragraph top3RevenueTitle = doc.InsertParagraph();
                    int top = 1;
                    string[] top3Booking = new string[] {_top1TravelBookingDate.Date,_top1TravelBookingDate.Count.ToString()
                        ,_top2TravelBookingDate.Date,_top2TravelBookingDate.Count.ToString()
                        ,_top3TravelBookingDate.Date,_top3TravelBookingDate.Count.ToString()};
                    for (i = 0; i < top3Booking.Length; i++)
                    {
                        var paragraghTop3Body = doc.InsertParagraph();
                        paragraghTop3Body.InsertText("Top " + (top++).ToString() + ": ", false, boldFormat);
                        paragraghTop3Body.InsertText(top3Booking[i], false, paragraphFormat);
                        paragraghTop3Body.InsertText(" - " + top3Booking[++i] + " bookings.", false, paragraphFormat);
                    }
                    #endregion
                    #region Add 3 total 
                    doc.InsertParagraph("");
                    // Thêm các đoạn văn bản "Service: Admin", "Total customer: 7", "Total re-booking customers: 0" và "Total ticket: 8"
                    string[] texts = new string[] { "Total travel: " , _totalTravel.ToString() + ".", "Total booking: " , _totalBooking.ToString() + "."
                    , "Total cancle booking: "  , _totalCancelBooking.ToString() + "." };

                    for (i = 0; i < texts.Length; i++)
                    {
                        var paragraph1 = doc.InsertParagraph(texts[i], false, paragraphFormat);
                        paragraph1.InsertText(texts[++i], false, boldFormat);
                    }
                    #endregion
                    #region Add Reporter
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
                    // Tạo đoạn văn bản chứa tên
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
                var newHeight = (int)(chartBitmap.Height * scaleFactor * 1.2);
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

        #region Load Data
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
                            LoadChartAsync();
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
        private async Task LoadChartAsync()
        {

            using (var db = new SUPER_TOUR())
            {
                var top3Dates = await Task.Run(() =>
            {
                return (from booking in db.BOOKINGs
                        where booking.Booking_Date >= StartDate && booking.Booking_Date <= EndDate
                        group booking by booking.Booking_Date into g
                        orderby g.Count() descending
                        select new
                        {
                            Count = g.Count(),
                            Date = g.Key,
                        }).Take(3).ToList()
                             .Select(x => new TravelBookingDate
                             {
                                 Count = x.Count,
                                 Date = x.Date.ToString("dd-MM-yyyy"),
                             }).ToList();
            });

                var sqlQuery = @"SELECT TOUR.Name_Tour AS TravelName, COUNT(BOOKING.Id_Booking) AS TotalBooking, SUM(BOOKING.TotalPrice) AS TotalRevenue
                            FROM TRAVEL
                            JOIN TOUR ON TRAVEL.Id_Tour = TOUR.Id_Tour
                            JOIN BOOKING ON BOOKING.Id_Travel = TRAVEL.Id_Travel
                            GROUP BY TOUR.Name_Tour";

                var result = db.Database.SqlQuery<TravelStatistic>(sqlQuery);
                BookingSeries = new SeriesCollection();
                Labels = new List<string>();

                var bookingCounts = await db.BOOKINGs
                     .Where(b => b.Status == "Paid" && b.Booking_Date >= StartDate && b.Booking_Date <= EndDate) // Lọc theo trạng thái xác nhận (tuỳ vào yêu cầu của bạn)
                     .GroupBy(b => b.Booking_Date) // Nhóm booking theo ngày đặt tour
                     .Select(g => new { Date = g.Key, Count = g.Count() }) // Chọn ngày và số lượng khách hàng
                     .OrderBy(item => item.Date) // Sắp xếp theo ngày tăng dần
                     .ToListAsync();


                // Xử lý dữ liệu để hiển thị trên biểu đồ
                var values = new ChartValues<int>();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    TotalBooking = db.BOOKINGs.Count();

                    TotalTravel = (
                        from booking in db.BOOKINGs
                        join tourists in db.TOURISTs on booking.Id_Booking equals tourists.Id_Booking
                        join ticket in db.TICKETs on tourists.Id_Tourist equals ticket.Id_Tourist
                        where ticket.Status != "Cancel" && booking.Booking_Date >= StartDate && booking.Booking_Date <= EndDate
                        select booking.Id_Travel
                    ).Distinct().Count();

                    TotalCancelBooking = (
                        from booking in db.BOOKINGs
                        join tourists in db.TOURISTs on booking.Id_Booking equals tourists.Id_Booking
                        join ticket in db.TICKETs on tourists.Id_Tourist equals ticket.Id_Tourist
                        where ticket.Status == "Cancel" && booking.Booking_Date >= StartDate && booking.Booking_Date <= EndDate
                        select booking.Id_Travel
                    ).Distinct().Count();
                    TravelStatisticList.Clear();
                    foreach (var item in result)
                    {
                        // Tạo một đối tượng TravelStatistic từ kết quả truy vấn
                        TravelStatistic travelStatistic = new TravelStatistic(item.TravelName, item.TotalBooking, item.TotalRevenue);

                        // Thêm đối tượng TravelStatistic vào ObservableCollection
                        TravelStatisticList.Add(travelStatistic);
                    }
                    Top1TravelBookingDate = top3Dates.ElementAtOrDefault(0);
                    Top2TravelBookingDate = top3Dates.ElementAtOrDefault(1);
                    Top3TravelBookingDate = top3Dates.ElementAtOrDefault(2);
                    foreach (var item in bookingCounts)
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
                    BookingSeries.Add(series);

                });
            }
        }
        #endregion

        #region Check data persecond
        private async void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                await Task.Run(async () =>
                {
                    // Compare Booking & Ticket
                    _trackerBooking = UPDATE_CHECK.getTracker("UPDATE_BOOKING");
                    if (DateTime.Parse(_trackerBooking.DateTimeUpdate) > lastUpdateBooking)
                    {
                        lastUpdateBooking = (DateTime.Parse(_trackerBooking.DateTimeUpdate));
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
