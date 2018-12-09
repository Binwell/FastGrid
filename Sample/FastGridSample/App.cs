using Xamarin.Forms;

namespace FastGridSample
{
    public class App : Application
    {
        public App()
        {
            MainPage = new NavigationPage(new MainPage());
        }
    }
}
