using System;
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
            setPos();
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
            }
        }

        public void setPos()
        {
            Rect workArea = SystemParameters.WorkArea;
            this.Left = workArea.Width - this.Width;
            this.Top = workArea.Height - this.Height;
        }
    }
}
