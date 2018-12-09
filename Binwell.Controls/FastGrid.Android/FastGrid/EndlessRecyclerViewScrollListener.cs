//  Based on https://github.com/ardok/codepath/blob/master/TwitterClient/app/src/main/java/com/codepath/twitterclient/listeners/EndlessRecyclerViewScrollListener.java

using System;
using Android.Support.V7.Widget;
using Android.Widget;
using Binwell.Controls.FastGrid.FastGrid;

namespace Binwell.Controls.FastGrid.Android.FastGrid
{
    public class EndlessRecyclerViewScrollListener : RecyclerView.OnScrollListener
    {
        readonly int _visibleThreshold = 5;
        bool _loading = true;
        int _previousTotalItemCount;
        int _startScrollPosition;
        ScrollState _lastState = ScrollState.Idle;

        readonly RecyclerView.LayoutManager _layoutManager;
        readonly FastGridView _fastGridView;
        readonly ScrollRecyclerView _recyclerView;
        readonly float _density;

        public bool EnableLoadMore { get; set; }

        public event Action LoadMore;

        public EndlessRecyclerViewScrollListener(LinearLayoutManager layoutManager,
            FastGridView fastGridView, ScrollRecyclerView recyclerView)
        {
            _layoutManager = layoutManager;
            _fastGridView = fastGridView;
            _recyclerView = recyclerView;
            _density = recyclerView.Resources.DisplayMetrics.Density;
        }


        public static int GetLastVisibleItem(int[] lastVisibleItemPositions)
        {
            var maxSize = 0;
            for (var i = 0; i < lastVisibleItemPositions.Length; i++)
                if (i == 0 || lastVisibleItemPositions[i] > maxSize)
                    maxSize = lastVisibleItemPositions[i];
            return maxSize;
        }

        public override void OnScrollStateChanged(RecyclerView recyclerView, int newState)
        {
            base.OnScrollStateChanged(recyclerView, newState);
            var state = (ScrollState) newState;
            var x = _recyclerView.GetHorizontalScrollOffset() / _density;
            var y = _startScrollPosition / _density;

            if (_lastState == ScrollState.Idle && (state == ScrollState.TouchScroll || state == ScrollState.Fling))
                _startScrollPosition = _recyclerView.GetVerticalScrollOffset();

            if (state == ScrollState.TouchScroll || state == ScrollState.Fling)
                _fastGridView.RaiseOnStartScroll(x, y,
                    state == ScrollState.TouchScroll ? ScrollActionType.Finger : ScrollActionType.Fling);

            if (_lastState == ScrollState.TouchScroll && (state == ScrollState.Fling || state == ScrollState.Idle))
                _fastGridView.RaiseOnStopScroll(x, y, ScrollActionType.Finger, state == ScrollState.Idle);

            if (_lastState == ScrollState.Fling && state == ScrollState.Idle)
                _fastGridView.RaiseOnStopScroll(x, y, ScrollActionType.Fling, true);

            _lastState = state;
        }

        public override void OnScrolled(RecyclerView view, int dx, int dy)
        {
            if (dy == 0) return;
            _startScrollPosition += dy;
            _fastGridView.RaiseOnScroll(dy / _density, _recyclerView.GetHorizontalScrollOffset() / _density,
                _startScrollPosition / _density, ScrollActionType.Finger);


            if (!EnableLoadMore) return;
            var lastVisibleItemPosition = 0;
            var totalItemCount = _layoutManager.ItemCount;

            switch (_layoutManager)
            {
                case StaggeredGridLayoutManager manager:
                    var lastVisibleItemPositions = manager.FindLastVisibleItemPositions(null);
                    lastVisibleItemPosition = GetLastVisibleItem(lastVisibleItemPositions);
                    break;
                case LinearLayoutManager _:
                    lastVisibleItemPosition = ((LinearLayoutManager) _layoutManager).FindLastVisibleItemPosition();
                    break;
            }

            if (totalItemCount < _previousTotalItemCount)
            {
                _previousTotalItemCount = totalItemCount;
                if (totalItemCount == 0)
                    _loading = true;
            }

            if (_loading && (totalItemCount > _previousTotalItemCount))
            {
                _loading = false;
                _previousTotalItemCount = totalItemCount;
            }

            if (_loading || (lastVisibleItemPosition + _visibleThreshold) <= totalItemCount) return;

            LoadMore?.Invoke();
            _loading = true;
        }
        
        public void ResetState()
        {
            _loading = true;
            _previousTotalItemCount = 0;
        }
    }
}