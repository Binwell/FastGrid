using System;
using Xamarin.Forms;

namespace Binwell.Controls.FastGrid.FastGrid
{
    public class FastGridDataTemplate : DataTemplate
    {
        public string Key { get; }
        public Size CellSize { get; }

        public FastGridDataTemplate(string key, Type cellType, Size cellSize): base(cellType)
        {
            Key = key;
            CellSize = cellSize;
        }
    }
}
