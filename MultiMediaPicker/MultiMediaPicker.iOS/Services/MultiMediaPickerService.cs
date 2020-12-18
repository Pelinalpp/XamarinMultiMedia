using CoreFoundation;
using GMImagePicker;
using MultiMediaPicker.Helpers;
using MultiMediaPicker.Interfaces;
using MultiMediaPicker.Models;
using Photos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;

namespace MultiMediaPicker.iOS.Services
{
    public class MultiMediaPickerService : IMultiMediaPickerService
    {
        const string TemporalDirectoryName = "TmpMedia";

        //Events
        public event EventHandler<MediaFile> OnMediaPicked;
        public event EventHandler<IList<MediaFile>> OnMediaPickedCompleted;

        GMImagePickerController currentPicker;
        TaskCompletionSource<IList<MediaFile>> mediaPickTcs;

        public void Clean()
        {
            var documentsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), TemporalDirectoryName);

            if (Directory.Exists(documentsDirectory))
            {
                Directory.Delete(documentsDirectory);
            }
        }

        public async Task<IList<MediaFile>> PickPhotosAsync()
        {
            return await PickMediaAsync("Select Images", PHAssetMediaType.Image);
        }

        public async Task<IList<MediaFile>> PickVideosAsync()
        {
            return await PickMediaAsync("Select Videos", PHAssetMediaType.Video);
        }

        async Task<IList<MediaFile>> PickMediaAsync(string title, PHAssetMediaType type)
        {

            mediaPickTcs = new TaskCompletionSource<IList<MediaFile>>();
            currentPicker = new GMImagePickerController()
            {
                Title = title,
                MediaTypes = new[] { type }
            };

            currentPicker.FinishedPickingAssets += FinishedPickingAssets;
            currentPicker.Canceled += PickerCancelled;

            var window = UIApplication.SharedApplication.KeyWindow;
            //var window = UIApplication.SharedApplication.Delegate.GetWindow();
            var vc = window.RootViewController;

            while (vc.PresentedViewController != null)
            {
                vc = vc.PresentedViewController;
            }


            await vc.PresentViewControllerAsync(currentPicker, true);

            var results = await mediaPickTcs.Task;

            currentPicker.FinishedPickingAssets -= FinishedPickingAssets;
            currentPicker.Canceled -= PickerCancelled;
            OnMediaPickedCompleted?.Invoke(this, results);
            return results;
        }

        void PickerCancelled(object sender, EventArgs args)
        {
            MessagingCenter.Send("PickerCancelled", "PickerCancelled");
        }

        async void FinishedPickingAssets(object sender, MultiAssetEventArgs args)
        {
            IList<MediaFile> results = new List<MediaFile>();
            TaskCompletionSource<IList<MediaFile>> tcs = new TaskCompletionSource<IList<MediaFile>>();

            try
            {
                bool completed = false;
                for (var i = 0; i < args.Assets.Length; i++)
                {
                    var asset = args.Assets[i];

                    string fileName = string.Empty;
                    if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
                    {
                        fileName = PHAssetResource.GetAssetResources(asset).FirstOrDefault().OriginalFilename;
                        fileName = Path.GetFileNameWithoutExtension(fileName) + "(" + i.ToString() + ")" + Path.GetExtension(fileName);
                    }

                    switch (asset.MediaType)
                    {
                        case PHAssetMediaType.Video:
                            {
                                var vOptions = new PHVideoRequestOptions();
                                vOptions.NetworkAccessAllowed = true;
                                vOptions.Version = PHVideoRequestOptionsVersion.Original;
                                vOptions.DeliveryMode = PHVideoRequestOptionsDeliveryMode.Automatic;
                                string videoUrl = "";

                                PHImageManager.DefaultManager.RequestAvAsset(asset, vOptions, (avAsset, audioMix, vInfo) =>
                                {
                                    DispatchQueue.MainQueue.DispatchAsync(() => {

                                        var error = vInfo.ObjectForKey(PHImageKeys.Error);

                                        if (avAsset != null)
                                        {
                                            videoUrl = ((AVFoundation.AVUrlAsset)avAsset).Url.Path;
                                        }

                                        var meFile = new MediaFile()
                                        {
                                            FileName = fileName,
                                            FilePath = videoUrl
                                        };

                                        using (Stream source = File.OpenRead(videoUrl))
                                        {
                                            meFile.FileSize = source.Length;
                                        }
                                        results.Add(meFile);
                                        OnMediaPicked?.Invoke(this, meFile);

                                        if (args.Assets.Length == results.Count && !completed)
                                        {
                                            completed = true;
                                            tcs.TrySetResult(results);
                                        }

                                    });
                                });
                            }
                            break;
                        default:
                        {
                            var options = new PHImageRequestOptions()
                            {
                                NetworkAccessAllowed = true
                            };

                            options.Synchronous = false;
                            options.ResizeMode = PHImageRequestOptionsResizeMode.Fast;
                            options.DeliveryMode = PHImageRequestOptionsDeliveryMode.HighQualityFormat;
                
                            PHImageManager.DefaultManager.RequestImageData(asset, options, (data, dataUti, orientation, info) =>
                            {

                                string path = FileHelper.GetOutputPath(MediaFileType.Image, TemporalDirectoryName, fileName);

                                if (!File.Exists(path))
                                {
                                    Debug.WriteLine(dataUti);
                                    var imageData = data;
                                    imageData?.Save(path, true);
                                }

                                var meFile = new MediaFile()
                                {
                                    FileName = fileName,
                                    FilePath = path,
                                    FileSize = File.ReadAllBytes(path).Length
                                };

                                results.Add(meFile);
                                OnMediaPicked?.Invoke(this, meFile);
                                if (args.Assets.Length == results.Count && !completed)
                                {
                                    completed = true;
                                    tcs.TrySetResult(results);
                                }

                            });
                       }
                       break;
                    }
                }
            }
            catch
            {
                tcs.TrySetResult(results);

            }

            mediaPickTcs?.TrySetResult(await tcs.Task);
        }

        public async Task AsyncPHAssetRequestAvAsset()
        {

        }
    }
}
