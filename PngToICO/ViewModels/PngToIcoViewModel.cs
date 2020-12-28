using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PngToICO
{
    public class PngToIcoViewModel : INotifyPropertyChanged
    {
        public PngToIcoViewModel()
        {
            PopulateIconSizes();
            LoadIntoMemory(@"pack://application:,,,/PngToICO;component/Images/empty-image.png", true);
        }

        #region Props
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private BitmapSource _userImage;

        public BitmapSource UserImage
        {
            get { return _userImage; }
            set
            {
                _userImage = value;
                OnPropertyChanged();
            }
        }

        private bool _buttonEnabled = false;

        public bool ButtonEnabled
        {
            get { return _buttonEnabled; }
            set
            {
                _buttonEnabled = value;
                OnPropertyChanged();
            }
        }

        private string _currentImageLocation;

        public string CurrentImageLocation
        {
            get { return _currentImageLocation; }
            set
            {
                _currentImageLocation = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<SelectListItem> _iconSizes;

        public ObservableCollection<SelectListItem> IconSizes
        {
            get { return _iconSizes; }
            set
            {
                _iconSizes = value;
                OnPropertyChanged();
            }
        }

        private SelectListItem _selectedIconSize;

        public SelectListItem SelectedIconSize
        {
            get { return _selectedIconSize; }
            set
            {
                _selectedIconSize = value;
                OnPropertyChanged();
            }
        }

        public int ImageWidth
        {
            get;
            set;
        }
        #endregion

        internal bool Convert()
        {
            var filter = FileFilterX.FileFilterBuilder(false, new Filter { FileName = "Icon file", Extension = new string[] { "ico", "ICO" } });
            SaveFileDialog fileDialog = new SaveFileDialog
            {
                CheckPathExists = true,
                DefaultExt = "ico",
                Filter = filter,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                Title = "Select where to save your new ICO file"
            };

            var saveResult = fileDialog.ShowDialog();

            if (saveResult.HasValue && saveResult.Value)
            {
                // User selected a location.
                if (SavePNGToICO(CurrentImageLocation, fileDialog.FileName))
                {
                    MessageBox.Show($"Successfully saved icon to: {fileDialog.FileName}", "File saved");
                    return true;
                }
            }

            return false;
        }

        internal bool SelectImageToConvert()
        {
            var filter = FileFilterX.FileFilterBuilder(false, new Filter { FileName = "Image files", Extension = new string[] { "png", "PNG" } });
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                CheckFileExists = true,
                Filter = filter,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                Title = "Select a PNG to convert to ICO"
            };

            var returnVal = fileDialog.ShowDialog();

            if (returnVal.HasValue && returnVal.Value)
            {
                // Have a file.
                LoadIntoMemory(fileDialog.FileName);

                return true;
            }

            return false;
        }

        internal bool Drop(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Length > 0 && files[0] != null && Path.GetExtension(files[0]).ToLower() == ".png")
                {
                    LoadIntoMemory(files[0]);
                    return true;
                }
            }

            return false;
        }

        private void PopulateIconSizes()
        {
            IconSizes = new ObservableCollection<SelectListItem>();
            foreach (IconSize icon in Enum.GetValues(typeof(IconSize)))
            {
                IconSizes.Add(new SelectListItem
                {
                    Value = ((int)icon).ToString(),
                    Text = Utility.GetString(icon)
                });
            }

            SelectedIconSize = IconSizes[0];
        }

        private void UpdateUserImage(string location)
        {
            if (!string.IsNullOrWhiteSpace(location))
            {
                try
                {
                    byte[] bytes = null;
                    using (MemoryStream ms = new MemoryStream())
                    using (FileStream file = new FileStream(location, FileMode.Open, FileAccess.Read))
                    {
                        bytes = new byte[file.Length];
                        file.Read(bytes, 0, (int)file.Length);
                        ms.Write(bytes, 0, (int)file.Length);
                    }

                    using (MemoryStream newMs = new MemoryStream(bytes))
                    {
                        var img = new Bitmap(newMs);
                        ImageWidth = img.Width;
                        UserImage = BitmapFrame.Create(newMs, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to retrieve the image.\nError: {ex}", "Import Error");
                }
            }
        }

        private void LoadIntoMemory(string input, bool isDefault = false)
        {
            if (!string.IsNullOrWhiteSpace(input) && Path.GetExtension(input).ToLower() == ".png")
            {
                if (!isDefault)
                {
                    CurrentImageLocation = input;
                    ButtonEnabled = true;
                }
                else
                {
                    var file = File.ReadAllBytes("./Images/empty-image.png");
                    input = Path.Combine(Path.GetTempPath(), "empty-image.png");
                    File.WriteAllBytes(input, file);
                    CurrentImageLocation = string.Empty;
                    ButtonEnabled = false;
                }

                UpdateUserImage(input);
            }
        }

        internal int GetIconSize()
        {
            IconSize icon = (IconSize)Enum.Parse(typeof(IconSize), SelectedIconSize.Value);

            return (int)icon;
        }

        internal bool SavePNGToICO(string source, string destination)
        {
            using (FileStream inputStream = new FileStream(source, FileMode.Open))
            using (FileStream outputStream = new FileStream(destination, FileMode.OpenOrCreate))
            {
                try
                {
                    return ImagingHelper.ConvertToIcon(inputStream, outputStream, GetIconSize(), false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to convert PNG to ICO.\nError: {ex}", "Conversion Error");
                }
            }

            return false;
        }
    }
}
