using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Util;
using Binwell.Controls.FastGrid.Android.FastGrid;
using Binwell.Controls.FastGrid.FastGrid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Application = Android.App.Application;
using Point = Xamarin.Forms.Point;
using Size = Xamarin.Forms.Size;
using View = Android.Views.View;

[assembly: ExportRenderer(typeof(FastGridView), typeof(FastGridViewRenderer))]

namespace Binwell.Controls.FastGrid.Android.FastGrid
{
    public class FastGridViewRenderer :
        ViewRenderer<FastGridView, SwipeRefreshLayout>,
        SwipeRefreshLayout.IOnRefreshListener, IGridViewProvider
    {
        readonly Orientation _orientation = Orientation.Undefined;

        ScrollRecyclerView _recyclerView;

        FastGridAdapter _adapter;
        int _columns = 1;

        GridLayoutManager _gridLayoutManager;
        float _density;
        SwipeRefreshLayoutWithDisabling _refresh;
        RecyclerView.ItemDecoration _paddingDecoration;

        GridSpanSizeLookup _sizeLookup;
        int _originalRefreshOffset;
        EndlessRecyclerViewScrollListener _scrollListener;

        public FastGridViewRenderer(Context context) : base(context)
        {
        }

        protected override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            if (newConfig.Orientation != _orientation)
                OnElementChanged(
                    new ElementChangedEventArgs<FastGridView>(Element,
                        Element));
        }

        protected override void OnElementChanged(
            ElementChangedEventArgs<FastGridView> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement == null) return;

            _density = Context.Resources.DisplayMetrics.Density;
            CreateRecyclerView();

            e.NewElement.GridViewProvider = this;

            var refresh = new SwipeRefreshLayoutWithDisabling(Context);
            refresh.SetOnRefreshListener(this);
            refresh.Refreshing = e.NewElement.IsRefreshing;
            refresh.IsPullToRefreshEnabled = e.NewElement.IsPullToRefreshEnabled;
            _originalRefreshOffset = refresh.ProgressViewStartOffset;

            if (Element.RefreshTopOffset != -1)
            {
                refresh.SetProgressViewOffset(true, _originalRefreshOffset,
                    (int) ((e.NewElement.RefreshTopOffset) * _density));
            }

            refresh.AddView(_recyclerView, LayoutParams.MatchParent);
            _refresh = refresh;
            SetNativeControl(_refresh);

            _recyclerView.Enabled = e.NewElement.IsPullToRefreshEnabled;
            e.NewElement.GetScrollPositionCommand = new Command(GetScrollPosition);
            _scrollListener.EnableLoadMore = Element?.LoadMoreCommand != null;
        }

        void GetScrollPosition(object obj)
        {
            var func = obj as Func<Point, object>;
            if (func == null) return;
            var point = new Point(_recyclerView.GetHorizontalScrollOffset() / _density,
                _recyclerView.GetVerticalScrollOffset() / _density);
            func.Invoke(point);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == FastGridView.ItemsSourceProperty.PropertyName)
            {
                CalculateLayoutRects();
                if (_adapter != null && Element != null) _adapter.Items = Element.ItemsSource;
                _recyclerView?.GetLayoutManager()?.ScrollToPosition(0);
            }
            else if (e.PropertyName == VisualElement.WidthProperty.PropertyName || e.PropertyName ==
                     FastGridView.ItemTemplateSelectorProperty.PropertyName)
            {
                _recyclerView?.GetRecycledViewPool()?.Clear();
                _recyclerView?.SetAdapter(null);
                _recyclerView?.SetAdapter(_adapter);
                CalculateLayoutRects();
                _adapter?.NotifyDataSetChanged();
            }
            else if (e.PropertyName == FastGridView.IsScrollEnabledProperty
                         .PropertyName)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (_recyclerView != null && Element != null) _recyclerView.Enabled = Element.IsScrollEnabled;
                });
            }
            else if (e.PropertyName ==
                     FastGridView.IsRefreshingProperty.PropertyName)
            {
                if (_refresh != null && Element != null)
                    _refresh.Refreshing = Element.IsRefreshing;
            }
            else if (e.PropertyName == FastGridView
                         .IsPullToRefreshEnabledProperty.PropertyName)
            {
                if (_refresh != null && Element != null)
                {
                    _refresh.IsPullToRefreshEnabled = Element.IsPullToRefreshEnabled;
                }
            }
            else if (e.PropertyName == FastGridView.RefreshTopOffsetProperty
                         .PropertyName)
            {
                if (_refresh != null && Element != null)
                {
                    if (Element.RefreshTopOffset != -1)
                    {
                        _refresh.Refreshing = false;
                        var size = (int) (Element.RefreshTopOffset * _density);
                        _refresh.SetProgressViewOffset(true, 0, size);
                        _refresh.Refreshing = Element.IsRefreshing;
                    }
                }
            }
            else if (e.PropertyName == FastGridView.LoadMoreCommandProperty
                         .PropertyName)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (_scrollListener != null) _scrollListener.EnableLoadMore = Element?.LoadMoreCommand != null;
                });
            }
        }


        LinearLayoutManager _layoutManager;
        void CreateRecyclerView()
        {
            _recyclerView = new ScrollRecyclerView(Application.Context);
            _recyclerView.SetClipToPadding(false);
            _adapter = new FastGridAdapter(Element.ItemsSource, _recyclerView, Element, Resources.DisplayMetrics,
                this);
            if (Element.IsHorizontal)
            {
                _layoutManager =
                    new LinearLayoutManager(Context, OrientationHelper.Horizontal,
                        false); /*{AutoMeasureEnabled = true}*/
                _recyclerView.HasFixedSize = true;
                CalculateLayoutRects();
            }
            else
            {
                _gridLayoutManager = new SmoothGridLayoutManager(Context, _columns > 0 ? _columns : 1,
                    OrientationHelper.Vertical, false)
                {
                    RecyclerView = _recyclerView
                };
                _recyclerView.HasFixedSize = true;
                _layoutManager = _gridLayoutManager;
                CalculateLayoutRects();
            }

            _recyclerView.SetLayoutManager(_layoutManager);

            var scrollListener = new EndlessRecyclerViewScrollListener(_layoutManager, Element, _recyclerView)
            {
                EnableLoadMore = Element.LoadMoreCommand != null
            };
            scrollListener.LoadMore += LoadMore;
            _recyclerView.AddOnScrollListener(scrollListener);
            _scrollListener = scrollListener;

            _recyclerView.HorizontalScrollBarEnabled = Element.IsHorizontal;
            _recyclerView.VerticalScrollBarEnabled = !Element.IsHorizontal;

            _recyclerView.SetAdapter(_adapter);
        }

        void LoadMore()
        {
            Element?.RaiseLoadMoreEvent();
        }

        protected internal void CalculateLayoutRects()
        {
            if (Element == null || Element.Width < 10 || _layoutManager == null) return;

            var itemTemplate = Element.ItemTemplateSelector;
            if (!(itemTemplate is FastGridTemplateSelector templateSelector)) return;
            templateSelector.Prepare();

            if (!Element.IsHorizontal)
            {
                var width = Element.Width;
                var widths = templateSelector.DataTemplates.Select(t => t.CellSize.Width);
                _columns = Math.Max(1, widths.Max(w => (int)(width / w)));

                _gridLayoutManager.SpanCount = _columns;
                if (_sizeLookup == null)
                {
                    _sizeLookup = new GridSpanSizeLookup();
                    _gridLayoutManager.SetSpanSizeLookup(_sizeLookup);
                }

                _sizeLookup.MaxColumns = _columns;
                _sizeLookup.Width = width;
            }
           

            var source = Element.ItemsSource as ICollection;
            var numberOfItems = source?.Count ?? 0;

            var layoutInfo = new Size[numberOfItems];
            if (numberOfItems == 0 || source == null) return;
            var density = _density;

            var items = source.Cast<object>().ToArray();

            for (var i = 0; i < numberOfItems; i++)
            {
                var item = items[i];
                var size = GetSizeByItem(templateSelector, item);
                size.Height *= density;
                size.Width *= density;
                layoutInfo[i] = size;
            }

            var widthByPos = layoutInfo.Select(t => t.Width / density).ToList();

            if (!Element.IsHorizontal)
            {
                _sizeLookup.WidthByPos = widthByPos;
            }
        }

        public class GridSpanSizeLookup : GridLayoutManager.SpanSizeLookup
        {
            public int MaxColumns { get; set; }
            public double Width { get; set; }
            public List<double> WidthByPos { get; set; }

            public override int GetSpanSize(int position)
            {
                if (MaxColumns < 2 || Width < 1 || WidthByPos == null || WidthByPos.Count == 0 ||
                    WidthByPos.Count <= position)
                    return 1;
                var span = MaxColumns + 1 - (int) (Width / WidthByPos[position]);
                span = span < 1 ? 1 : span;
                return Math.Min(span, MaxColumns);
            }
        }

        Size GetSizeByItem(FastGridTemplateSelector templateSelector, object item)
        {
            var cellSize = Size.Zero;
            if (templateSelector?.OnSelectTemplate(item) is FastGridDataTemplate template)
                cellSize = template.CellSize;

            return cellSize;
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);
            UpdatePadding();
        }

        void UpdatePadding()
        {
            _recyclerView.SetPadding((int) (Element.ContentPaddingLeft * _density),
                (int) (Element.ContentPaddingTop * _density),
                (int) (Element.ContentPaddingRight * _density),
                (int) (Element.ContentPaddingBottom * _density));
            if (Element.IsHorizontal)
            {
                if (_paddingDecoration != null)
                {
                    _recyclerView.RemoveItemDecoration(_paddingDecoration);
                }

                var cs = Element.ColumnSpacing;
                var rs = Element.RowSpacing;
                if (cs > 0 || rs > 0)
                {
                    _paddingDecoration =
                        new HorizontalSpacesItemDecoration(ConvertDpToPixels((float) cs), ConvertDpToPixels((int) rs));
                    _recyclerView.AddItemDecoration(_paddingDecoration);
                }
            }
            else
                UpdateGridLayout();
        }

        void UpdateGridLayout()
        {
            if (_paddingDecoration != null)
                _recyclerView.RemoveItemDecoration(_paddingDecoration);

            _recyclerView.InvalidateItemDecorations();
            var cs = Element.ColumnSpacing;
            var rs = Element.RowSpacing;
            if (cs > 0 || rs > 0)
            {
                _paddingDecoration = new HorizontalSpacesItemDecoration(ConvertDpToPixels((int) Element.ColumnSpacing),
                    ConvertDpToPixels((int) Element.RowSpacing));
                _recyclerView.AddItemDecoration(_paddingDecoration);
            }
        }

        int ConvertDpToPixels(float dpValue)
        {
            var pixels = (int) ((dpValue) * Resources.DisplayMetrics.Density);
            return pixels;
        }

        public void OnRefresh()
        {
            Element?.Refresh();
        }

        public void ReloadData()
        {
            CalculateLayoutRects();
            _adapter.Items = Element.ItemsSource;
        }

        public void ScrollToItem(int index, bool animated)
        {
            if (!animated)
                _recyclerView.ScrollToPosition(index);
            else
                _recyclerView.SmoothScrollToPosition(index);
        }

        public void ScrollTo(float x, float y)
        {
            _recyclerView?.GetLayoutManager()?.ScrollToPosition((int) (x));
        }

        public int GetVisibleItemsCount()
        {
            var layoutManager = (LinearLayoutManager) _recyclerView.GetLayoutManager();
            var firstVisiblePosition = layoutManager.FindFirstCompletelyVisibleItemPosition();
            var lastVisiblePosition = layoutManager.FindLastCompletelyVisibleItemPosition();
            return lastVisiblePosition - firstVisiblePosition;
        }
    }


    public class HorizontalSpacesItemDecoration : RecyclerView.ItemDecoration
    {
        readonly int _columnSpacing;
        readonly int _rowSpacing;

        public HorizontalSpacesItemDecoration(int columnSpacing, int rowSpacing)
        {
            _rowSpacing = rowSpacing;
            _columnSpacing = columnSpacing;
        }

        public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
        {
            outRect.Left = _columnSpacing / 2;
            outRect.Right = _columnSpacing / 2;
            outRect.Bottom = _rowSpacing / 2;
            outRect.Top = _rowSpacing / 2;
        }
    }

    public class ScrollRecyclerView : RecyclerView
    {
        public ScrollRecyclerView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }


        public ScrollRecyclerView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
        }


        public ScrollRecyclerView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }


        public ScrollRecyclerView(Context context) : base(context)
        {
        }

        public int GetVerticalScrollOffset()
        {
            return ComputeVerticalScrollOffset();
        }

        public int GetHorizontalScrollOffset()
        {
            return ComputeHorizontalScrollOffset();
        }
    }
}