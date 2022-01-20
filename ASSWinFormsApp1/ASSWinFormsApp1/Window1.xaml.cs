using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ASSWinFormsApp1
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            setHide();
        }

        public string ImagePath
        {
            set
            {
                GC.Collect();

                int hh = (int)((FrameworkElement)this.Content).ActualHeight;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(value);
                bitmapImage.DecodePixelHeight = hh;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                image1.Source = bitmapImage;

                time = 6;
                if (!isShow)
                {
                    showImage();
                }
            }
        }

        bool isShow;
        int time;

        async void showImage()
        {
            this.Opacity = 1;
            setPos();
            isShow = true;

            while (time > 0)
            {
                time--;
                await Task.Delay(1000);
            }

            setHide();
            isShow = false;
        }

        void setPos()
        {
            Rect workArea = SystemParameters.WorkArea;
            this.Left = workArea.Width - this.Width;
            this.Top = workArea.Height - this.Height;
        }

        void setHide()
        {
            this.Opacity = 0;
            this.Left = SystemParameters.PrimaryScreenWidth;
            this.Top = SystemParameters.PrimaryScreenHeight;
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
