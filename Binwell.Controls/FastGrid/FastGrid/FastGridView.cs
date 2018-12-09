//  Based on https://github.com/twintechs/TwinTechsFormsLib
//  Special thanks to Twin Technologies from Binwell Ltd.

//  Distributed under Apache 2.0 License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections;
using System.Windows.Input;
using Xamarin.Forms;

namespace Binwell.Controls.FastGrid.FastGrid
{

    public interface IGridViewProvider
    {
        void ReloadData();
        void ScrollToItem(int index, bool animated);
        void ScrollTo(float x, float y);
        int GetVisibleItemsCount();
    }

    public class FastGridView : ContentView, IScrollAwareElement, IDisposable
    {
        int _initialIndex;
        IGridViewProvider _gridViewProvider;

        public bool SelectionEnabled { get; set; } = true;
        public bool CollectionChangedWithoutAnimation { get; set; }

        public event EventHandler<FastGridEventArgs<object>> ItemSelected;

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource),
            typeof(IEnumerable), typeof(FastGridView));

        public static readonly BindableProperty ItemTemplateSelectorProperty = BindableProperty.Create(
            nameof(ItemTemplateSelector),
            typeof(FastGridTemplateSelector), typeof(FastGridView));

        public static readonly BindableProperty RefreshCommandProperty = BindableProperty.Create(
            nameof(RefreshCommand), typeof(ICommand), typeof(FastGridView));

        public static readonly BindableProperty LoadMoreCommandProperty =
            BindableProperty.Create(nameof(LoadMoreCommand), typeof(ICommand), typeof(FastGridView));

        public static readonly BindableProperty ItemSelectedCommandProperty =
            BindableProperty.Create(nameof(ItemSelectedCommand), typeof(ICommand), typeof(FastGridView));

        public static readonly BindableProperty RowSpacingProperty = BindableProperty.Create(nameof(RowSpacing),
            typeof(double), typeof(FastGridView), 0.0);

        public static readonly BindableProperty IsScrollEnabledProperty =
            BindableProperty.Create(nameof(IsScrollEnabled), typeof(bool), typeof(FastGridView), true);

        public static readonly BindableProperty ColumnSpacingProperty = BindableProperty.Create(nameof(ColumnSpacing),
            typeof(double), typeof(FastGridView), 0.0);

        public static readonly BindableProperty ContentPaddingLeftProperty =
            BindableProperty.Create(nameof(ContentPaddingLeft), typeof(double), typeof(FastGridView), 0.0);

        public static readonly BindableProperty ContentPaddingRightProperty =
            BindableProperty.Create(nameof(ContentPaddingRight), typeof(double), typeof(FastGridView), 0.0);

        public static readonly BindableProperty ContentPaddingBottomProperty =
            BindableProperty.Create(nameof(ContentPaddingBottom), typeof(double), typeof(FastGridView), 0.0);

        public static readonly BindableProperty SectionPaddingLeftProperty =
            BindableProperty.Create(nameof(SectionPaddingLeft), typeof(double), typeof(FastGridView), default(double),
                BindingMode.Default);

        public static readonly BindableProperty SectionPaddingBottomProperty =
            BindableProperty.Create(nameof(SectionPaddingBottom), typeof(double), typeof(FastGridView), 0.0);

        public static readonly BindableProperty SectionPaddingTopProperty =
            BindableProperty.Create(nameof(SectionPaddingTop), typeof(double), typeof(FastGridView), 0.0);

        public static readonly BindableProperty ContentPaddingTopProperty =
            BindableProperty.Create(nameof(ContentPaddingTop), typeof(double), typeof(FastGridView), 0.0);

        public static readonly BindableProperty IsHorizontalProperty = BindableProperty.Create(nameof(IsHorizontal),
            typeof(bool), typeof(FastGridView), false);

        public static readonly BindableProperty IsPullToRefreshEnabledProperty =
            BindableProperty.Create(nameof(IsPullToRefreshEnabled), typeof(bool), typeof(FastGridView), false);

        public static readonly BindableProperty IsRefreshingProperty = BindableProperty.Create(nameof(IsRefreshing),
            typeof(bool), typeof(FastGridView), false);

        public static readonly BindableProperty RefreshTopOffsetProperty =
            BindableProperty.Create(nameof(RefreshTopOffset), typeof(double), typeof(FastGridView), -1d,
                BindingMode.Default);

        public static readonly BindableProperty GetScrollPositionCommandProperty =
            BindableProperty.Create(nameof(GetScrollPositionCommand), typeof(ICommand), typeof(FastGridView),
                default(ICommand), BindingMode.Default);

        public ICommand GetScrollPositionCommand
        {
            get => (ICommand) GetValue(GetScrollPositionCommandProperty);
            set => SetValue(GetScrollPositionCommandProperty, value);
        }

        public double RefreshTopOffset
        {
            get => (double) GetValue(RefreshTopOffsetProperty);
            set => SetValue(RefreshTopOffsetProperty, value);
        }

        public IGridViewProvider GridViewProvider
        {
            get => _gridViewProvider;
            set
            {
                _gridViewProvider = value;
                _initialIndex = 0;
                _gridViewProvider?.ScrollToItem(_initialIndex, false);
            }
        }

        public IEnumerable ItemsSource
        {
            get => (IEnumerable) GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public ICommand RefreshCommand
        {
            get => (ICommand) GetValue(RefreshCommandProperty);
            set => SetValue(RefreshCommandProperty, value);
        }

        public ICommand ItemSelectedCommand
        {
            get => (ICommand) GetValue(ItemSelectedCommandProperty);
            set => SetValue(ItemSelectedCommandProperty, value);
        }

        public ICommand LoadMoreCommand
        {
            get => (ICommand) GetValue(LoadMoreCommandProperty);
            set => SetValue(LoadMoreCommandProperty, value);
        }

        public FastGridTemplateSelector ItemTemplateSelector
        {
            get => (FastGridTemplateSelector) GetValue(ItemTemplateSelectorProperty);
            set => SetValue(ItemTemplateSelectorProperty, value);
        }

        public double RowSpacing
        {
            get => (double) GetValue(RowSpacingProperty);
            set => SetValue(RowSpacingProperty, value);
        }

        public double ColumnSpacing
        {
            get => (double) GetValue(ColumnSpacingProperty);
            set => SetValue(ColumnSpacingProperty, value);
        }

        public bool IsPullToRefreshEnabled
        {
            get => (bool) GetValue(IsPullToRefreshEnabledProperty);
            set => SetValue(IsPullToRefreshEnabledProperty, value);
        }

        public bool IsScrollEnabled
        {
            get => (bool) GetValue(IsScrollEnabledProperty);
            set => SetValue(IsScrollEnabledProperty, value);
        }

        public double ContentPaddingLeft
        {
            get => (double) GetValue(ContentPaddingLeftProperty);
            set => SetValue(ContentPaddingLeftProperty, value);
        }

        public double ContentPaddingRight
        {
            get => (double) GetValue(ContentPaddingRightProperty);
            set => SetValue(ContentPaddingRightProperty, value);
        }
        public double ContentPaddingTop
        {
            get => (double) GetValue(ContentPaddingTopProperty);
            set => SetValue(ContentPaddingTopProperty, value);
        }
        public double ContentPaddingBottom
        {
            get => (double) GetValue(ContentPaddingBottomProperty);
            set => SetValue(ContentPaddingBottomProperty, value);
        }

        public double SectionPaddingTop
        {
            get => (double) GetValue(SectionPaddingTopProperty);
            set => SetValue(SectionPaddingTopProperty, value);
        }

        public double SectionPaddingLeft
        {
            get => (double) GetValue(SectionPaddingLeftProperty);
            set => SetValue(SectionPaddingLeftProperty, value);
        }

        public double SectionPaddingBottom
        {
            get => (double) GetValue(SectionPaddingBottomProperty);
            set => SetValue(SectionPaddingBottomProperty, value);
        }

        public bool IsHorizontal
        {
            get => (bool) GetValue(IsHorizontalProperty);
            set => SetValue(IsHorizontalProperty, value);
        }

        public bool IsRefreshing
        {
            get => (bool) GetValue(IsRefreshingProperty);
            set => SetValue(IsRefreshingProperty, value);
        }

        public void RaiseLoadMoreEvent()
        {
            LoadMoreCommand?.Execute(null);
        }

        public void InvokeItemSelectedEvent(object sender, object item)
        {
            ItemSelectedCommand?.Execute(item);
            ItemSelected?.Invoke(sender, new FastGridEventArgs<object>(item));
        }

        public void Dispose()
        {
            _gridViewProvider = null;
        }

        public void ReloadData()
        {
            GridViewProvider?.ReloadData();
        }

        public void ScrollToItem(int section, int index, bool animated)
        {
            if (GridViewProvider != null)
                GridViewProvider.ScrollToItem(index, animated);
            else
                _initialIndex = index;
        }

        public void ScrollTo(float x, float y)
        {
            GridViewProvider?.ScrollTo(x, y);
        }

        public void Refresh()
        {
            if (IsPullToRefreshEnabled == false) return;
            RefreshCommand?.Execute(null);
        }

        #region IScrollAwareElement

        public event EventHandler<ControlScrollEventArgs> OnScrollEvent;
        public event EventHandler<ControlScrollEventArgs> OnStartScrollEvent;
        public event EventHandler<ControlScrollEventArgs> OnStopScrollEvent;

        public void RaiseOnScroll(double delta, double currentX, double currentY, ScrollActionType type)
        {
            var args = new ControlScrollEventArgs(delta, currentX, currentY, type);
            OnScrollEvent?.Invoke(this, args);
        }

        public void RaiseOnStartScroll(double currentX, double currentY, ScrollActionType type)
        {
            var args = new ControlScrollEventArgs(0, currentX, currentY, type);
            OnStartScrollEvent?.Invoke(this, args);
        }

        public void RaiseOnStopScroll(double currentX, double currentY, ScrollActionType type, bool fullStop)
        {
            var args = new ControlScrollEventArgs(0, currentX, currentY, type, fullStop);
            OnStopScrollEvent?.Invoke(this, args);
        }

        #endregion
    }
}