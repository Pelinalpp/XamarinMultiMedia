using MultiMediaPicker.Interfaces;
using MultiMediaPicker.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MultiMediaPicker.Droid.Services
{
    public class MultiMediaPickerService : IMultiMediaPickerService
    {
        public event EventHandler<MediaFile> OnMediaPicked;
        public event EventHandler<IList<MediaFile>> OnMediaPickedCompleted;

        public static MultiMediaPickerService SharedInstance = new MultiMediaPickerService();
        int MultiPickerResultCode = 9793;
        const string TemporalDirectoryName = "TmpMedia";

        MultiMediaPickerService()
        {
        }

        public void Clean()
        {
            throw new NotImplementedException();
        }

        public Task<IList<MediaFile>> PickPhotosAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IList<MediaFile>> PickVideosAsync()
        {
            throw new NotImplementedException();
        }
    }
}