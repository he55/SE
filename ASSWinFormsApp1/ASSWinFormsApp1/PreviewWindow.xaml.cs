using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ASSWinFormsApp1
{
    public partial class PreviewWindow : Window
    {
        bool isShow = true;
        int time;
        public Action openAction;

        public PreviewWindow()
        {
            InitializeComponent();
            setHide();
        }

        public void SetImagePath(string path)
        {
            GC.Collect();

            int height = (int)((FrameworkElement)this.Content).ActualHeight;

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.UriSource = new Uri(path);
            bitmapImage.DecodePixelHeight = height;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            image1.Source = bitmapImage;

            time = 5;
            if (!isShow)
            {
                this.Opacity = 1;
                Rect workArea = SystemParameters.WorkArea;
                this.Left = workArea.Width - this.Width;
                this.Top = workArea.Height - this.Height;

                isShow = true;
                hideImage();
            }
        }

        public void setHide()
        {
            if (isShow)
            {
                this.Opacity = 0;
                this.Left = SystemParameters.PrimaryScreenWidth;
                this.Top = SystemParameters.PrimaryScreenHeight;

                isShow = false;
            }
        }

        async void hideImage()
        {
            while (time > 0)
            {
                if (!isShow)
                    return;

                time--;
                await Task.Delay(1000);
            }

            setHide();
        }

        private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            openAction?.Invoke();
        }
    }
}
