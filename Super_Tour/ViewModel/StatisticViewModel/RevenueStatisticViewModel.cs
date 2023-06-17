using LiveCharts;
using LiveCharts.Wpf;
using SautinSoft.Document;
using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using System.Drawing;
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
using Xceed.Document.NET;
using Xceed.Words.NET;
using static Super_Tour.ViewModel.TravelStatisticViewModel;
using Student_wpf_application.ViewModels.Command;

namespace Super_Tour.ViewModel
{
    internal class RevenueStatisticViewModel : ObservableObject
    {
        #region Declare RevenueDate class
        public class RevenueDate: ObservableObject
        {
            private string _date;
            private decimal _revenue;
            public RevenueDate()
            {

            }
            public RevenueDate(string date, decimal revenue)
            {
                Date = date;
                Revenue = revenue;
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
            public decimal Revenue 
            { 
                get => _revenue;
                set
                {
                    _revenue = value;
                    OnPropertyChanged(nameof(Revenue));
                }
            }
        }
        #endregion

        #region Declare variable

        private DispatcherTimer _timer = null;
        private UPDATE_CHECK _trackerTicket = null;
        private UPDATE_CHECK _trackerBooking = null;
        private DateTime lastUpdateBooking;
        private DateTime lastUpdateTicket;
        private SUPER_TOUR db = null;
        private ObservableCollection<TravelStatistic> _travelStatisticList;
        private decimal _totalRevenue;
        private decimal _totalCancelMoney;
        private decimal _totalTourist;
        private RevenueDate _top1RevenueDate;
        private RevenueDate _top2RevenueDate;
        private RevenueDate _top3RevenueDate;
        private DateTime _startDate;
        private DateTime _endDate;
        private SeriesCollection _revenueSeries;
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
                LoadChart();
            }
        }

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
        public SeriesCollection RevenueSeries
        {
            get { return _revenueSeries; }
            set
            {
                _revenueSeries = value;
                OnPropertyChanged(nameof(RevenueSeries));
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
            set
            {
                _travelStatisticList = value;
                OnPropertyChanged(nameof(TravelStatisticList));
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
        public decimal TotalCancelMoney 
        { 
            get => _totalCancelMoney;
            set
            {
                _totalCancelMoney = value;
                OnPropertyChanged(nameof(TotalCancelMoney));
            }
        }
        public decimal TotalTourist
        {
            get => _totalTourist;
            set
            {
                _totalTourist = value;
                OnPropertyChanged(nameof(TotalTourist));
            }
        }

        public RevenueDate Top1RevenueDate 
        { 
            get => _top1RevenueDate;
            set
            {
                _top1RevenueDate = value;
                OnPropertyChanged(nameof(Top1RevenueDate));
            }
        }
        public RevenueDate Top2RevenueDate 
        { 
            get => _top2RevenueDate;
            set
            {
                _top2RevenueDate = value;
                OnPropertyChanged(nameof(Top2RevenueDate));
            }
        }
        public RevenueDate Top3RevenueDate
        {
            get => _top3RevenueDate;
            set
            {
                _top3RevenueDate = value;
                OnPropertyChanged(nameof(Top3RevenueDate));
            }
        }

        #endregion

        #region Command
        public RelayCommand PrintToImageCommand { get; set; }
        #endregion

        #region Constructor
        public RevenueStatisticViewModel()
        {
            db = SUPER_TOUR.db;
            TravelStatisticList = new ObservableCollection<TravelStatistic>();
            PrintToImageCommand = new RelayCommand(ExecutePrint);
            StartDate = DateTime.Parse("01/06/2023");
            EndDate = DateTime.Today;

            lastUpdateBooking = DateTime.Now;
            lastUpdateTicket = DateTime.Now;

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
                    var heading = doc.InsertParagraph("REVENUE REPORT", false, headingFormat);
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
                    var NameChart = doc.InsertParagraph("Revenue chart from " + _startDate.ToString("dd/MM/yyyy") + " to " + _endDate.ToString("dd/MM/yyyy"), false, NameChartFormat);
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
                    int top = 1;
                    Xceed.Document.NET.Paragraph top3RevenueTitle = doc.InsertParagraph();
                    string[] top3Revenue = new string[] {_top1RevenueDate.Date,_top1RevenueDate.Revenue.ToString("#,#")
                        ,_top2RevenueDate.Date,_top2RevenueDate.Revenue.ToString("#,#")
                        ,_top3RevenueDate.Date,_top3RevenueDate.Revenue.ToString("#,#")};
                    for (i = 0; i < top3Revenue.Length; i++)
                    {
                        var paragraghTop3Body = doc.InsertParagraph();
                        paragraghTop3Body.InsertText("Top " + (top++).ToString() + ": ", false, boldFormat);
                        paragraghTop3Body.InsertText(top3Revenue[i], false, paragraphFormat);
                        paragraghTop3Body.InsertText(" - " + top3Revenue[++i] + " VNĐ", false, paragraphFormat);
                    }
                    #endregion
                    #region Add 3 total 
                    doc.InsertParagraph("");
                    // Thêm các đoạn văn bản "Service: Admin", "Total customer: 7", "Total re-booking customers: 0" và "Total ticket: 8"
                    string[] texts = new string[] { "Total revenue: " , _totalRevenue.ToString("#,#") + " VNĐ", "Total cancle money: " , _totalCancelMoney.ToString("#,#") + " VNĐ"
                    , "Total tourist: "  , _totalTourist.ToString() + " tourists" };

                    for (i = 0; i < texts.Length; i++)
                    {
                        var paragraph1 = doc.InsertParagraph(texts[i], false, paragraphFormat);
                        paragraph1.InsertText(texts[++i], false, boldFormat);
                    }
                    #endregion
                    #region Add Reporter
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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
        private void LoadChart()
        {
            var sqlQueryTotalRevenue = @"
                                SELECT SUM(BOOKING.TotalPrice) 
                                FROM BOOKING
                                WHERE BOOKING.Status='Paid'";
            var totalRevenue = db.Database.SqlQuery<Decimal>(sqlQueryTotalRevenue);
            TotalRevenue = totalRevenue.ElementAtOrDefault(0);

            var sqlQueryTotalCancelMoney = @"SELECT SUM(TOUR.PriceTour) * COUNT(DISTINCT TOURIST.Id_Tourist)
                                FROM BOOKING
                                JOIN TOURIST ON TOURIST.Id_Booking = BOOKING.Id_Booking
                                JOIN TICKET ON TICKET.Id_Tourist = TOURIST.Id_Tourist
                                JOIN TRAVEL ON TRAVEL.Id_Travel = BOOKING.Id_Travel
                                JOIN TOUR ON TOUR.Id_Tour = TRAVEL.Id_Tour
                                WHERE TICKET.Status = 'Canceled'
                                GROUP BY BOOKING.Id_Booking";

            TotalCancelMoney = db.Database.SqlQuery<Decimal>(sqlQueryTotalCancelMoney).ElementAtOrDefault(0);


            TotalTourist = db.TOURISTs.Count();

            var sqlTravelStatisticQuery = @"
                                SELECT TOUR.Name_Tour AS TravelName, COUNT(BOOKING.Id_Booking) AS TotalBooking, SUM(BOOKING.TotalPrice) AS TotalRevenue
                                FROM TRAVEL
                                JOIN TOUR ON TRAVEL.Id_Tour = TOUR.Id_Tour
                                JOIN BOOKING ON BOOKING.Id_Travel = TRAVEL.Id_Travel
                                GROUP BY TOUR.Name_Tour
                                ";

            var result = db.Database.SqlQuery<TravelStatistic>(sqlTravelStatisticQuery);

            TravelStatisticList.Clear();
            foreach (var item in result)
            {
                // Tạo một đối tượng CustomerStatistic từ kết quả truy vấn
                TravelStatistic travelStatistic = new TravelStatistic(item.TravelName, item.TotalBooking, item.TotalRevenue);

                // Thêm đối tượng CustomerStatistic vào ObservableCollection
                TravelStatisticList.Add(travelStatistic);
            }

            var top3Dates = (from booking in db.BOOKINGs
                             group booking by booking.Booking_Date into g
                             orderby g.Sum(x => x.TotalPrice) descending
                             select new
                             {
                                 Revenue = g.Sum(x => x.TotalPrice),
                                 Date = g.Key,
                             }).Take(3).ToList()
                             .Select(x => new RevenueDate
                             {
                                 Revenue = x.Revenue,
                                 Date = x.Date.ToString("dd-MM-yyyy"),
                             }).ToList();

            Top1RevenueDate = top3Dates.ElementAtOrDefault(0);
            Top2RevenueDate = top3Dates.ElementAtOrDefault(1);
            Top3RevenueDate = top3Dates.ElementAtOrDefault(2);

            RevenueSeries = new SeriesCollection();
            Labels = new List<string>();

            var revenueByDate = db.BOOKINGs
                 .Where(b => b.Booking_Date >= StartDate && b.Booking_Date <= EndDate)
                 .GroupBy(b => b.Booking_Date)
                 .Select(g => new { Date = g.Key, TotalMoney = g.Sum(b => b.TotalPrice) })
                 .ToList();


            var values = new ChartValues<decimal>();

            foreach (var item in revenueByDate)
            {
                values.Add(item.TotalMoney);
                Labels.Add(item.Date.ToString("dd/MM/yyyy"));

                
            }
            var series = new ColumnSeries
            {
                Title = "Revenue by day",
                Values = values
            };
            RevenueSeries.Add(series);

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
                    _trackerTicket = UPDATE_CHECK.getTracker("UPDATE_TICKET");
                    if (DateTime.Parse(_trackerBooking.DateTimeUpdate) > lastUpdateBooking)
                    {
                        lastUpdateBooking = (DateTime.Parse(_trackerBooking.DateTimeUpdate));
                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            LoadDataAsync();
                        });
                    }
                    else if(DateTime.Parse(_trackerTicket.DateTimeUpdate) > lastUpdateTicket)
                    {
                        lastUpdateTicket = (DateTime.Parse(_trackerTicket.DateTimeUpdate));
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
