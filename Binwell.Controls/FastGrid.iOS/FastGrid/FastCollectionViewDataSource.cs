//  Based on https://github.com/twintechs/TwinTechsFormsLib
//  Special thanks to Twin Technologies from Binwell Ltd.

//  Distributed under Apache 2.0 License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using Foundation;
using UIKit;

namespace Binwell.Controls.FastGrid.iOS.FastGrid
{
    public class FastCollectionViewDataSource : UICollectionViewSource
    {
        public delegate UICollectionViewCell OnGetCell(UICollectionView collectionView, NSIndexPath indexPath);

        public delegate int OnRowsInSection(UICollectionView collectionView, nint section);

        public delegate int OnNumberOfSections(UICollectionView collectionView);

	    OnGetCell _onGetCell;
	    OnRowsInSection _onRowsInSection;
	    OnNumberOfSections _onNumberOfSections;

        public FastCollectionViewDataSource(OnGetCell onGetCell, OnRowsInSection onRowsInSection, OnNumberOfSections onNumberOfSections)
        {
            _onGetCell = onGetCell;
            _onRowsInSection = onRowsInSection;
            _onNumberOfSections = onNumberOfSections;
        }

        protected override void Dispose(bool disposing)
        {
            _onGetCell = null;
            _onRowsInSection = null;
            _onNumberOfSections = null;

            base.Dispose(disposing);
        }

        public override nint NumberOfSections(UICollectionView collectionView)
        {
            return _onNumberOfSections(collectionView);
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return _onRowsInSection(collectionView, section);
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            return _onGetCell(collectionView, indexPath);
        }
    }
}