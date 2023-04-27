using c971.Services;
using c971.Views;
using Xamarin.Forms;

namespace c971
{
    public partial class App : Application
    {
        public static string FilePath;
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new Login());          
        }

        protected override async void OnStart()
        {
            await DBservice.Init();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
