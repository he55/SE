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
        }

        public string ImagePath
        {
            set
            {
                GC.Collect();

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(value);
                bitmapImage.DecodePixelHeight = (int)this.Height;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                image1.Source = bitmapImage;
            }
        }
    }
}
