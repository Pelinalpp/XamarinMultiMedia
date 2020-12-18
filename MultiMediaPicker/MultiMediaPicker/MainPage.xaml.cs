using MultiMediaPicker.Models;
using MultiMediaPicker.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace MultiMediaPicker
{
    public partial class MainPage : ContentPage
    {
        MainPageViewModel MainPageVM { get; set; }
        public MainPage()
        {
            InitializeComponent();

            MainPageVM = new MainPageViewModel();
            BindingContext = MainPageVM;
        }

        public async void PickVideo(object sender, EventArgs e)
        {
            MultiMediaObject.Media = new ObservableCollection<MultiMediaPicker.Models.MediaFile>();
            var medias = await MultiMediaObject.MultiMediaPickerService.PickVideosAsync();
            string stringMedias = JsonConvert.SerializeObject(medias);
            MultiMediaObject.Media = JsonConvert.DeserializeObject<ObservableCollection<MultiMediaPicker.Models.MediaFile>>(stringMedias);
            MainPageVM.MediaFileList = MultiMediaObject.Media;
        }

        public async void PickImage(object sender, EventArgs e)
        {
            MultiMediaObject.Media = new ObservableCollection<MultiMediaPicker.Models.MediaFile>();
            var medias = await MultiMediaObject.MultiMediaPickerService.PickPhotosAsync();
            string stringMedias = JsonConvert.SerializeObject(medias);
            MultiMediaObject.Media = JsonConvert.DeserializeObject<ObservableCollection<MultiMediaPicker.Models.MediaFile>>(stringMedias);
            MainPageVM.MediaFileList = MultiMediaObject.Media;
        }
    }
}
