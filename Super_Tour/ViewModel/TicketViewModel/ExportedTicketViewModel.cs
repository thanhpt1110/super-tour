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

namespace Super_Tour.ViewModel
{
    internal class ExportedTicketViewModel : ObservableObject
    {
        private string _barcodeText;
        private TicketPrintableContent _ticketPrintableContent;


        #region Declare Binding Command
        public ICommand PrintTicketCommand { get; private set; }
        #endregion

        protected TicketPrintableContent TicketPrintableContent
        {
            get { return _ticketPrintableContent; }
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

        public BitmapSource BarcodeImage
        {
            get { return GenerateBarcodeImage(); }
        }
        public BitmapSource BarcodeImage2
        {
            get { return GenerateBarcodeImage2(); }
        }
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
        public ExportedTicketViewModel()
        {
            BarcodeText = "BK00123468";
            PrintTicketCommand = new RelayCommand(ExecutePrintTicketCommand);
        }

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
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
            finally { }
        }
    }
}
