using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using DataFormats = System.Windows.DataFormats;
using DragEventArgs = System.Windows.DragEventArgs;
using Screen = System.Windows.Forms.Screen;
using Brushes = System.Windows.Media.Brushes;
namespace ImageFrame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool dragged = false;
        bool resize = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dragged = false;
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] file = (string[])e.Data.GetData(DataFormats.FileDrop);
                Uri uri = new Uri(file[0]);

                BitmapImage new_image = new BitmapImage(uri);

                double new_height = new_image.PixelHeight;
                double new_width = new_image.PixelWidth;
                double ratio = new_height / new_width;

                Trace.WriteLine(String.Format("Image is {0}x{1}", new_height, new_width));

                Screen screen = WindowHelpers.CurrentScreen(this);

                double max_height = screen.Bounds.Height * 0.50;
                double max_width = screen.Bounds.Width * 0.50;

                Trace.WriteLine(String.Format("Max size is {0}x{1}", max_height, max_width));

                if (new_height > max_height)
                {
                    new_height = max_height;
                    new_width = max_height / ratio;

                    Trace.WriteLine(String.Format("Too tall. Setting size to {0}x{1}", new_height, new_width));
                }

                if (new_width > max_width)
                {

                    new_width = max_width;
                    new_height = max_width * ratio;

                    Trace.WriteLine(String.Format("Too wide. Setting size to {0}x{1}", new_height, new_width));
                }

                resize = true;
                this.Height = new_height;
                this.Width = new_width;

                //viewbox.Height = new_height;
                //viewbox.Width = new_width;

                image.Source = new BitmapImage(uri);

                this.Background = Brushes.Gray;
                this.ResizeMode = ResizeMode.CanResizeWithGrip;
            }
        }

        private void viewbox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Viewbox vb = (Viewbox)sender;
            this.Height = vb.ActualHeight;
            this.Width = vb.ActualWidth;
        }

        public static class WindowHelpers
        {
            public static Screen CurrentScreen(Window window)
            {
                return Screen.FromPoint(new System.Drawing.Point((int)window.Left, (int)window.Top));
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (resize)
            {
                // new image loaded, height was already adjusted
            } else
            {
                this.Height = viewbox.ActualHeight;
                this.Width = viewbox.ActualWidth;
            }


            resize = false;
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Height = viewbox.ActualHeight;
            this.Width = viewbox.ActualWidth;

            if (!dragged)
            {
                if (this.Background == Brushes.Transparent)
                {
                    this.Background = Brushes.Gray;
                    this.ResizeMode = ResizeMode.CanResizeWithGrip;
                }
                else
                {
                    this.Background = Brushes.Transparent;
                    this.ResizeMode = ResizeMode.NoResize;
                }
            }

            dragged = false;

        }

        private void Window_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //this.Background = Brushes.Transparent;
            //this.ResizeMode = ResizeMode.NoResize;
        }

        private void Window_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //this.Background = System.Windows.Media.Brushes.Gray;
        }

        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed)
            {
                dragged = true;
                this.DragMove();
            }
        }

        private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}