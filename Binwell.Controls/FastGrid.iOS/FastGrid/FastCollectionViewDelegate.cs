//  Based on https://github.com/twintechs/TwinTechsFormsLib
//  Special thanks to Twin Technologies from Binwell Ltd.

//  Distributed under Apache 2.0 License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using Binwell.Controls.FastGrid.FastGrid;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Binwell.Controls.FastGrid.iOS.FastGrid
{
    public class FastCollectionViewDelegate : UICollectionViewDelegateFlowLayout
    {
        public delegate void OnScrolled(CGPoint contentOffset, ScrollActionType type);

        public delegate CGSize OnGetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath);

        public delegate nfloat OnMinimumInterItemSpacing(UICollectionView collectionView, int section);

        public delegate UIEdgeInsets OnSectionInsetForItem(UICollectionView collectionView, int index);

	    OnScrolled _onScrolled;
	    OnGetSizeForItem _onGetSizeForItem;
	    readonly Action<CGPoint, ScrollActionType, bool> _onScrollEnded;
	    readonly Action<CGPoint, ScrollActionType> _onScrollStarted;

	    public FastCollectionViewDelegate(OnScrolled onScrolled, OnGetSizeForItem onGetSizeForItem, Action<CGPoint, ScrollActionType> onScrollStarted, Action<CGPoint, ScrollActionType, bool> onScrollEnded) {
		    
		    _onScrolled = onScrolled;
		    _onGetSizeForItem = onGetSizeForItem;
		    _onScrollStarted = onScrollStarted;
		    _onScrollEnded = onScrollEnded;
	    }

	    protected override void Dispose(bool disposing)
        {
            _onScrolled = null;
            _onGetSizeForItem = null;
            base.Dispose(disposing);
        }

        public override void Scrolled(UIScrollView scrollView)
        {
            _onScrolled(scrollView.ContentOffset, ScrollActionType.Finger);
        }

	    public override void DecelerationEnded(UIScrollView scrollView) {
		    _onScrollEnded?.Invoke(scrollView.ContentOffset, ScrollActionType.Fling, true);
	    }
		

	    public override void DraggingStarted(UIScrollView scrollView) {
		    _onScrollStarted?.Invoke(scrollView.ContentOffset, ScrollActionType.Finger);
	    }

	    public override void DraggingEnded(UIScrollView scrollView, bool willDecelerate) {
		    _onScrollEnded?.Invoke(scrollView.ContentOffset, ScrollActionType.Finger, !willDecelerate);
	    }

	    public override CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout,
            NSIndexPath indexPath)
        {
            return _onGetSizeForItem(collectionView, layout, indexPath);
        }

		public override CGSize GetReferenceSizeForHeader(UICollectionView collectionView, UICollectionViewLayout layout, nint section)
		{
			return new CGSize(0, 0);
		}

		public override CGSize GetReferenceSizeForFooter(UICollectionView collectionView, UICollectionViewLayout layout, nint section)
		{
			return new CGSize(0, 0);
		}
    }
}