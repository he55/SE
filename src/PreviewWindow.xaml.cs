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
            SetHide();
        }

        public void SetImage(string path)
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
                DelayHide();
            }
        }

        public void SetHide()
        {
            if (isShow)
            {
                this.Opacity = 0;
                this.Left = SystemParameters.PrimaryScreenWidth;
                this.Top = SystemParameters.PrimaryScreenHeight;

                isShow = false;
            }
        }

        async void DelayHide()
        {
            while (time > 0)
            {
                if (!isShow)
                    return;

                time--;
                await Task.Delay(1000);
            }

            SetHide();
        }

        private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            openAction?.Invoke();
        }
    }
}
