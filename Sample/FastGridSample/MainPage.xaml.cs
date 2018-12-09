using Binwell.Controls.FastGrid.FastGrid;
using FastGridSample.Cells;
using FastGridSample.DataObjects;
using Xamarin.Forms;

namespace FastGridSample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            var size = Device.Info.ScaledScreenSize;
            fastGridView.ItemTemplateSelector = new FastGridTemplateSelector(
                new FastGridDataTemplate(typeof(CategoryObject).Name, typeof(CategoryCell),new Size(size.Width, 70)),
                new FastGridDataTemplate(typeof(ProductObject).Name, typeof(ProductCell),new Size(size.Width / 2, 260))
            );

            BindingContext = new MainViewModel();
        }
    }
}