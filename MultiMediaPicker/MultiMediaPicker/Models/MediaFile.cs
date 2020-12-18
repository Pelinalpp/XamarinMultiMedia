using MultiMediaPicker.Interfaces;
using System.Collections.ObjectModel;

namespace MultiMediaPicker.Models
{
    public enum MediaFileType
    {
        Image,
        Video
    }

    public class MediaFile
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
    }


    public static class MultiMediaObject
    {
        public static IMultiMediaPickerService MultiMediaPickerService;
        public static ObservableCollection<MediaFile> Media;
    }
}
