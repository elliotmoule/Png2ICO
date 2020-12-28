using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace PngToICO
{
    public partial class MainWindow : Window
    {
        private PngToIcoViewModel _viewModel;

        public PngToIcoViewModel ViewModel
        {
            get { return _viewModel; }
            private set { _viewModel = value; }
        }

        public MainWindow()
        {
            ViewModel = new PngToIcoViewModel();
            DataContext = ViewModel;
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ViewModel.SelectImageToConvert();
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            ViewModel.Drop(e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Convert();
        }

        private void RestoreWindowPosition()
        {
            if (Properties.Settings.Default.HasSetDefaults)
            {
                System.Drawing.Point location = Properties.Settings.Default.Location;
                SetLocation(location);
                System.Drawing.Size size = Properties.Settings.Default.Size;
                SetSize(size);
            }
        }

        private void SaveWindowPosition()
        {
            Properties.Settings.Default.Location = GetLocation();
            Properties.Settings.Default.Size = GetSize();
            Properties.Settings.Default.HasSetDefaults = true;

            Properties.Settings.Default.Save();
        }

        private void ResetWindowPosition()
        {
            var x = (SystemParameters.PrimaryScreenWidth / 2) - (this.Width / 2);
            var y = (SystemParameters.PrimaryScreenHeight / 2) - (this.Height / 2);
            var loc = new System.Drawing.Point((int)Math.Round(x), (int)Math.Round(y));
            Properties.Settings.Default.Location = loc;
            Properties.Settings.Default.Size = GetSize();
            Properties.Settings.Default.HasSetDefaults = true;

            Properties.Settings.Default.Save();

            SetLocation(loc);
        }

        private void SetLocation(System.Drawing.Point point)
        {
            Application.Current.MainWindow.Left = point.X;
            Application.Current.MainWindow.Top = point.Y;
        }

        private void SetSize(System.Drawing.Size size)
        {
            Application.Current.MainWindow.Width = size.Width;
            Application.Current.MainWindow.Height = size.Height;
        }

        private System.Drawing.Point GetLocation()
        {
            return new System.Drawing.Point((int)Application.Current.MainWindow.Left, (int)Application.Current.MainWindow.Top);
        }

        private System.Drawing.Size GetSize()
        {
            return new System.Drawing.Size((int)Application.Current.MainWindow.Width, (int)Application.Current.MainWindow.Height);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RestoreWindowPosition();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            SaveWindowPosition();
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if ((Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
               && Utility.GetKey(e) == Key.Escape)
            {
                ResetWindowPosition();
            }
        }
    }
}
