using MultiMediaPicker.Interfaces;
using MultiMediaPicker.Models;
using Xamarin.Forms;

namespace MultiMediaPicker
{
    public partial class App : Application
    {
        public App(IMultiMediaPickerService multiMediaPickerService)
        {
            InitializeComponent();

            MultiMediaObject.MultiMediaPickerService = multiMediaPickerService;

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
