using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using FastGridSample.DataObjects;
using MvvmHelpers;
using Xamarin.Forms;

namespace FastGridSample
{
    public class MainViewModel : BaseViewModel
    {
        ObservableRangeCollection<object> _itemsSource = new ObservableRangeCollection<object>();
        ICommand _loadMoreCommand;
        ICommand _refreshCommand;
        ICommand _itemSelectedCommand;

        public ICommand LoadMoreCommand => _loadMoreCommand ??
                                           (_loadMoreCommand = new Command(async () => await LoadMoreCommandAsync()));
        public ICommand RefreshCommand =>
            _refreshCommand ?? (_refreshCommand = new Command(async () => await RefreshCommandAsync()));
        public ICommand ItemSelectedCommand => _itemSelectedCommand ??
                                               (_itemSelectedCommand = new Command(async (o) =>
                                                   await ItemSelectedCommandAsync(o)));

        public ObservableRangeCollection<object> ItemsSource
        {
            get => _itemsSource;
            set
            {
                _itemsSource = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            GenerateSource();
        }
  
        async Task LoadMoreCommandAsync()
        {
            IsBusy = true;
            await Task.Delay(3000);
            GenerateSource();
            IsBusy = false;
        }

        async Task RefreshCommandAsync()
        {
            IsBusy = true;
            await Task.Delay(3000);
            ItemsSource.Clear();
            await Task.Delay(150);
            GenerateSource();
            IsBusy = false;
        }

        async Task ItemSelectedCommandAsync(object obj)
        {
            if (obj is ProductObject product) 
                await Application.Current.MainPage.DisplayAlert("Selected item", product.Name, "Ok");
        }

        void GenerateSource()
        {
            var size = Device.Info.ScaledScreenSize;
            var imageSize = (int) ((size.Width / 2 - 40) * 2);
            var imageUrl = $"https://loremflickr.com/{imageSize}/{imageSize}/";
            var r = new Random(DateTime.Now.Millisecond);

            string GetImage(string name)
            {
                return $"{imageUrl}{name}?random={r.Next()}";
            }

            var items = new List<object>
            {
                new CategoryObject {Name = "Fruits"},
                new ProductObject()
                {
                    ImageUrl = GetImage("Pears"),
                    Name = "Pears",
                    Price = "120 rub"
                },
                new ProductObject()
                {
                    ImageUrl = GetImage("Apples"),
                    Name = "Apples",
                    Price = "50 rub"
                },
                new ProductObject()
                {
                    ImageUrl = GetImage("Bananas"),
                    Name = "Bananas",
                    Price = "55 rub"
                },
                new ProductObject()
                {
                    ImageUrl = GetImage("Oranges"),
                    Name = "Oranges",
                    Price = "89 rub"
                },
                new CategoryObject {Name = "Vegetables"},
                new ProductObject()
                {
                    ImageUrl = GetImage("Tomatos"),
                    Name = "Tomatos",
                    Price = "110 rub."
                },
                new ProductObject()
                {
                    ImageUrl = GetImage("Cucumbers"),
                    Name = "Cucumbers",
                    Price = "100 rub."
                },
                new ProductObject()
                {
                    ImageUrl = GetImage("Eggplants"),
                    Name = "Eggplants",
                    Price = "280 rub."
                },
                new ProductObject()
                {
                    ImageUrl = GetImage("Pumpkins"),
                    Name = "Pumpkins",
                    Price = "40 rub."
                },
            };
            ItemsSource.AddRange(items);
        }
    }
}
