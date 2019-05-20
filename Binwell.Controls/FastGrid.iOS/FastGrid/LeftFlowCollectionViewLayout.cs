using System;
using CoreGraphics;
using UIKit;

namespace Binwell.Controls.FastGrid.iOS.FastGrid
{
	public class LeftFlowCollectionViewLayout : UICollectionViewFlowLayout
	{
	    public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect(CGRect rect)
	    {
	        var attributes = base.LayoutAttributesForElementsInRect(rect);

	        if (ScrollDirection == UICollectionViewScrollDirection.Horizontal) 
	            return attributes;

            for (int i = 1; i < attributes.Length; ++i)
            {
                var currentLayoutAttributes = attributes[i];
                var prevLayoutAttributes = attributes[i - 1];
                var origin = prevLayoutAttributes.Frame.GetMaxX();

                CGRect frame = currentLayoutAttributes.Frame;
                if (origin + MinimumInteritemSpacing + currentLayoutAttributes.Frame.Size.Width < CollectionViewContentSize.Width)
                {
                    frame.X = (nfloat)(origin + MinimumInteritemSpacing);
                    currentLayoutAttributes.Frame = frame;
                }
                else
                {
                    frame.X = SectionInset.Left;
                    currentLayoutAttributes.Frame = frame;
                }

                if (i == 1)
                {
                    var prevFrame = prevLayoutAttributes.Frame;
                    prevFrame.X = SectionInset.Left;
                    prevLayoutAttributes.Frame = prevFrame;
                }
            }

            return attributes;
        }
    }
}