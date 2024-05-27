using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace RemoveBG
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //ExtractBG(@"E:\WPF\shoe.jpg");
            RemoveBG(@"e:\wpf\shoe.jpg");
           // MessageBox.Show("hoan thanh");

        }
        void RemoveBG2(string path)
        {
            string imagePath = path;

            OpenCvSharp.Mat src = OpenCvSharp.Cv2.ImRead(imagePath);

            // Kiểm tra nếu ảnh được đọc thành công
            if (src.Empty())
            {
                MessageBox.Show("Không thể đọc hình ảnh.");
                return;
            }

            // Chuyển đổi sang không gian màu xám
            OpenCvSharp.Mat gray = new OpenCvSharp.Mat();
            OpenCvSharp.Cv2.CvtColor(src, gray, OpenCvSharp.ColorConversionCodes.BGR2GRAY);

            // Làm mờ ảnh để giảm nhiễu
            OpenCvSharp.Mat blurred = new OpenCvSharp.Mat();
            OpenCvSharp.Cv2.GaussianBlur(gray, blurred, new OpenCvSharp.Size(5, 5), 0);

            // Phát hiện cạnh sử dụng thuật toán Canny
            OpenCvSharp.Mat edges = new OpenCvSharp.Mat();
            OpenCvSharp.Cv2.Canny(blurred, edges, 100, 200);

            // Tìm các đường viền (contours)
            OpenCvSharp.Point[][] contours;
            OpenCvSharp.HierarchyIndex[] hierarchy;
            OpenCvSharp.Cv2.FindContours(edges, out contours, out hierarchy, OpenCvSharp.RetrievalModes.External, OpenCvSharp.ContourApproximationModes.ApproxSimple);

            // Tạo một mặt nạ (mask) từ các đường viền
            OpenCvSharp.Mat mask = OpenCvSharp.Mat.Zeros(src.Size(), OpenCvSharp.MatType.CV_8UC1);

            // Tìm đường viền lớn nhất (được cho là đường viền của vật thể chính - chiếc giày)
            int largestContourIndex = -1;
            double largestArea = 0;
            for (int i = 0; i < contours.Length; i++)
            {
                double area = OpenCvSharp.Cv2.ContourArea(contours[i]);
                if (area > largestArea)
                {
                    largestArea = area;
                    largestContourIndex = i;
                }
            }

            // Vẽ đường viền lớn nhất vào mặt nạ
            if (largestContourIndex != -1)
            {
                OpenCvSharp.Cv2.DrawContours(mask, contours, largestContourIndex, OpenCvSharp.Scalar.White, -1);
            }

            // Tạo một ảnh mới với nền trong suốt
            OpenCvSharp.Mat result = new OpenCvSharp.Mat(src.Size(), OpenCvSharp.MatType.CV_8UC4);
            for (int y = 0; y < src.Rows; y++)
            {
                for (int x = 0; x < src.Cols; x++)
                {
                    if (mask.At<byte>(y, x) == 255)
                    {
                        OpenCvSharp.Vec3b color = src.At<OpenCvSharp.Vec3b>(y, x);
                        result.Set(y, x, new OpenCvSharp.Vec4b(color.Item0, color.Item1, color.Item2, 255));
                    }
                    else
                    {
                        result.Set(y, x, new OpenCvSharp.Vec4b(0, 0, 0, 0));
                    }
                }
            }

            // Lưu ảnh kết quả dưới dạng PNG với nền trong suốt
            OpenCvSharp.Cv2.ImWrite(@"E:\output111111.png", result);

            MessageBox.Show("Đã lưu ảnh với nền trong suốt thành công.");
        }
        void RemoveBG(string path)
        {
            
            string imagePath = path;

            // read image
            OpenCvSharp.Mat src = OpenCvSharp.Cv2.ImRead(imagePath);

            // Check image is not empty
            if (src.Empty())
            {
                MessageBox.Show("Không thể đọc hình ảnh.");
                return;
            }

            // covert to Gray color
            OpenCvSharp.Mat gray = new OpenCvSharp.Mat();
            OpenCvSharp.Cv2.CvtColor(src, gray, OpenCvSharp.ColorConversionCodes.BGR2GRAY);

            // Làm mờ ảnh để giảm nhiễu
            OpenCvSharp.Mat blurred = new OpenCvSharp.Mat();
            OpenCvSharp.Cv2.GaussianBlur(gray, blurred, new OpenCvSharp.Size(5, 5), 0);

            // Phát hiện cạnh sử dụng thuật toán Canny
            OpenCvSharp.Mat edges = new OpenCvSharp.Mat();
            OpenCvSharp.Cv2.Canny(blurred, edges, 100, 200);

            // Tìm các đường viền (contours)
            OpenCvSharp.Point[][] contours;
            OpenCvSharp.HierarchyIndex[] hierarchy;
            OpenCvSharp.Cv2.FindContours(edges, out contours, out hierarchy, OpenCvSharp.RetrievalModes.External, OpenCvSharp.ContourApproximationModes.ApproxSimple);

            // Tạo một mặt nạ (mask) từ các đường viền
            OpenCvSharp.Mat mask = OpenCvSharp.Mat.Zeros(src.Size(), OpenCvSharp.MatType.CV_8UC1);
            foreach (var contour in contours)
            {
                OpenCvSharp.Cv2.DrawContours(mask, new[] { contour }, -1, OpenCvSharp.Scalar.White, -1);
            }

            // Tạo một ảnh mới với nền trong suốt
            OpenCvSharp.Mat result = new OpenCvSharp.Mat(src.Size(), OpenCvSharp.MatType.CV_8UC4);
            for (int y = 0; y < src.Rows; y++)
            {
                for (int x = 0; x < src.Cols; x++)
                {
                    if (mask.At<byte>(y, x) == 255)
                    {
                        OpenCvSharp.Vec3b color = src.At<OpenCvSharp.Vec3b>(y, x);
                        result.Set(y, x, new OpenCvSharp.Vec4b(color.Item0, color.Item1, color.Item2, 255));
                    }
                    else
                    {
                        result.Set(y, x, new OpenCvSharp.Vec4b(0, 0, 0, 0));
                    }
                }
            }

            // Lưu ảnh kết quả dưới dạng PNG với nền trong suốt
            OpenCvSharp.Cv2.ImWrite(@"E:\output.png", result);

            MessageBox.Show("Đã lưu ảnh với nền trong suốt thành công.");
        }

        void BorderObj(string path)
        {
            // Đường dẫn đến hình ảnh của bạn
            string imagePath = path;

            // Đọc hình ảnh
            OpenCvSharp.Mat src = OpenCvSharp.Cv2.ImRead(imagePath);

            // Kiểm tra nếu ảnh được đọc thành công
            if (src.Empty())
            {
                Console.WriteLine("Không thể đọc hình ảnh.");
                return;
            }

            // Chuyển đổi sang không gian màu xám
            OpenCvSharp.Mat gray = new OpenCvSharp.Mat();
            OpenCvSharp.Cv2.CvtColor(src, gray, OpenCvSharp.ColorConversionCodes.BGR2GRAY);

            // Làm mờ ảnh để giảm nhiễu
            OpenCvSharp.Mat blurred = new OpenCvSharp.Mat();
            OpenCvSharp.Cv2.GaussianBlur(gray, blurred, new OpenCvSharp.Size(5, 5), 0);

            // Phát hiện cạnh sử dụng thuật toán Canny
            OpenCvSharp.Mat edges = new OpenCvSharp.Mat();
            OpenCvSharp.Cv2.Canny(blurred, edges, 100, 200);

            // Tìm các đường viền (contours)
            OpenCvSharp.Point[][] contours;
            OpenCvSharp.HierarchyIndex[] hierarchy;
            OpenCvSharp.Cv2.FindContours(edges, out contours, out hierarchy, OpenCvSharp.RetrievalModes.External, OpenCvSharp.ContourApproximationModes.ApproxSimple);

            // Tạo một mặt nạ (mask) từ đường viền lớn nhất
            OpenCvSharp.Mat mask = OpenCvSharp.Mat.Zeros(src.Size(), OpenCvSharp.MatType.CV_8UC1);
            int largestContourIndex = -1;
            double largestArea = 0;

            for (int i = 0; i < contours.Length; i++)
            {
                double area = OpenCvSharp.Cv2.ContourArea(contours[i]);
                if (area > largestArea)
                {
                    largestArea = area;
                    largestContourIndex = i;
                }
            }

            if (largestContourIndex != -1)
            {
                OpenCvSharp.Cv2.DrawContours(mask, contours, largestContourIndex, OpenCvSharp.Scalar.White, -1);
            }

            // Tạo một ảnh mới với nền trong suốt
            OpenCvSharp.Mat result = new OpenCvSharp.Mat(src.Size(), OpenCvSharp.MatType.CV_8UC4);
            for (int y = 0; y < src.Rows; y++)
            {
                for (int x = 0; x < src.Cols; x++)
                {
                    if (mask.At<byte>(y, x) == 255)
                    {
                        OpenCvSharp.Vec3b color = src.At<OpenCvSharp.Vec3b>(y, x);
                        result.Set(y, x, new OpenCvSharp.Vec4b(color.Item0, color.Item1, color.Item2, 255));
                    }
                    else
                    {
                        result.Set(y, x, new OpenCvSharp.Vec4b(0, 0, 0, 0));
                    }
                }
            }

            // Lưu ảnh kết quả
            OpenCvSharp.Cv2.ImWrite(@"E:\WPF\Output12.png", result);
        }
        (int W,int H) Resolution(string path)
        {
            {
                // Đường dẫn đến hình ảnh của bạn
                string imagePath = path;

                // Đọc hình ảnh
                OpenCvSharp.Mat src = OpenCvSharp.Cv2.ImRead(imagePath);

                // Kiểm tra nếu ảnh được đọc thành công
                if (src.Empty())
                {
                    Console.WriteLine("Không thể đọc hình ảnh.");
                    return (0,0);
                }

                // Xác định kích thước của bức ảnh
                return (src.Width, src.Height);
                ((int, string), int) a;
                a = ((3, "tin"), 5);

                
            }
        }
        (byte H, byte S, byte V) HSV_Position(string path)
        {
            // Đọc hình ảnh
            OpenCvSharp.Mat src = OpenCvSharp.Cv2.ImRead(path);

            // Kiểm tra nếu ảnh được đọc thành công
            if (src.Empty())
            {
                MessageBox.Show("Không thể đọc hình ảnh.");
                ;
            }

            // Chuyển đổi hình ảnh sang không gian màu HSV
            OpenCvSharp.Mat hsv = new OpenCvSharp.Mat();
            OpenCvSharp.Cv2.CvtColor(src, hsv, OpenCvSharp.ColorConversionCodes.BGR2HSV);

            // Xác định vị trí cần lấy màu (x, y)
            int x = 10; // Vị trí x (cột)
            int y = 10;  // Vị trí y (dòng)

            // Kiểm tra nếu vị trí nằm trong giới hạn của hình ảnh
            if (x >= hsv.Cols || y >= hsv.Rows || x < 0 || y < 0)
            {
                MessageBox.Show("Vị trí nằm ngoài giới hạn của hình ảnh.");
                return (0,0,0);
            }

            // Lấy giá trị HSV tại vị trí (x, y)
            OpenCvSharp.Vec3b hsvValue = hsv.At<OpenCvSharp.Vec3b>(y, x);
            byte hue = hsvValue.Item0;        // Hue
            byte saturation = hsvValue.Item1; // Saturation
            byte value = hsvValue.Item2;      // Value

            
          return( hue, saturation, value);
        }
    
        void ExtractBG(string path)
        {
            var gethsv = HSV_Position(path);

            OpenCvSharp.Mat src = OpenCvSharp.Cv2.ImRead(path);

            // Chuyển đổi hình ảnh sang không gian màu HSV
            OpenCvSharp.Mat hsv = new OpenCvSharp.Mat();
            OpenCvSharp.Cv2.CvtColor(src, hsv, OpenCvSharp.ColorConversionCodes.BGR2HSV);

            // Xác định màu nền (ví dụ: màu trắng)
            OpenCvSharp.Scalar lowerBound = new OpenCvSharp.Scalar(gethsv.H, gethsv.S, gethsv.V);  // Giá trị HSV thấp hơn cho màu trắng
            OpenCvSharp.Scalar upperBound = new OpenCvSharp.Scalar(180, 20, 255); // Giá trị HSV cao hơn cho màu trắng

            // Tạo mask dựa trên màu nền
            OpenCvSharp.Mat mask = new OpenCvSharp.Mat();
            OpenCvSharp.Cv2.InRange(hsv, lowerBound, upperBound, mask);

            // Invert mask để giữ lại phần không phải background
            OpenCvSharp.Mat invertedMask = new OpenCvSharp.Mat();
            OpenCvSharp.Cv2.BitwiseNot(mask, invertedMask);

            // Tạo một hình ảnh với nền trong suốt
            OpenCvSharp.Mat transparentBg = new OpenCvSharp.Mat(src.Size(), OpenCvSharp.MatType.CV_8UC4, OpenCvSharp.Scalar.All(0));

            // Copy các pixel không phải background vào hình ảnh mới
            src.CopyTo(transparentBg, invertedMask);

            // Lưu hình ảnh dưới dạng PNG với nền trong suốt
            OpenCvSharp.Cv2.ImWrite(@"E:\WPF\Output.png", transparentBg);
        }
        void ExtractBG()
        {

            Bitmap bmp = new Bitmap(@"E:\WPF\shoe.jpg");

            // Lock the bitmap's bits.
            var rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            // Define white and black in ARGB format.
            byte whiteA = 255, whiteR = 255, whiteG = 255, whiteB = 255;
            byte blackA = 255, blackR = 0, blackG = 0, blackB = 0;

            // Replace non-black pixels with white.
            for (int i = 0; i < rgbValues.Length; i += 4)
            {
                byte b = rgbValues[i];
                byte g = rgbValues[i + 1];
                byte r = rgbValues[i + 2];
                byte a = rgbValues[i + 3];

                if (r != blackR || g != blackG || b != blackB)
                {
                    rgbValues[i] = whiteB;
                    rgbValues[i + 1] = whiteG;
                    rgbValues[i + 2] = whiteR;
                    rgbValues[i + 3] = whiteA;
                }
            }

            // Copy the RGB values back to the bitmap.
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);

            // Save the modified image.
            bmp.Save(@"E:\WPF\Output.Bmp", ImageFormat.Bmp);
        }
    }
}
