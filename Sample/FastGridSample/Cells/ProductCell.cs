using Binwell.Controls.FastGrid.FastGrid;
using FastGridSample.DataObjects;
using FFImageLoading.Forms;
using Xamarin.Forms;

namespace FastGridSample.Cells
{
    public class ProductCell : FastGridCell
    {
        CachedImage _image;
        Label _name;
        Label _price;

        protected override void InitializeCell()
        {
            var screenWidth = Device.Info.ScaledScreenSize.Width;

            _image = new CachedImage
            {
                HorizontalOptions = LayoutOptions.Center,
                Aspect = Aspect.AspectFill,
                WidthRequest = screenWidth / 2 - 40,
                HeightRequest = screenWidth / 2 - 40
            };

            _name = new Label
            {
                HorizontalOptions = LayoutOptions.Center,
                FontSize = 20,
                TextColor = Color.Black
            };

            _price = new Label { HorizontalOptions = LayoutOptions.Center,
                FontSize = 14,
                TextColor = Color.Black
            };

            View = new StackLayout
            {
                BackgroundColor = Color.White,
                Padding = 20,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children =
                {
                    _image,
                    _name,
                    _price
                }
            };
        }

        protected override void SetupCell(bool isRecycled)
        {
            if (!(BindingContext is ProductObject bindingContext)) return;

            _image.Source = null;
            _image.Source = bindingContext.ImageUrl;
            _name.Text = bindingContext.Name;
            _price.Text = bindingContext.Price;
        }
    }
}
