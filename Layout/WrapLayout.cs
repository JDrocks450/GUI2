using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GUI2.Layout
{
    public class WrapLayout : ILayoutManager
    {
        public Rectangle Safezone { get; set; }

        void ILayoutManager.OnLayoutRequested(IEnumerable<ILayoutItem> Source)
        {
            int row = 0, column = 0;
            int LastXPosition = 0;
            Dictionary<int, int> ColumnDefinitions = new Dictionary<int, int>();
            foreach(var item in Source)
            {
                if (item.LayoutIgnore) continue;
                var args = new LayoutEventArgs();
                bool setColumn = false;
                int LastYPosition = 0;                 
                int calculatedXPosition = item.Padding.Left + item.Padding.Right + item.DesiredSpace.Size.X;
                if (LastXPosition + calculatedXPosition > Safezone.Width)
                {
                    column = 0;
                    LastXPosition = 0;
                }
                if (ColumnDefinitions.Keys.Contains(column))
                    LastYPosition = ColumnDefinitions[column];
                else
                    setColumn = true;
                args.NewSpace = new Rectangle(
                    new Point(LastXPosition, LastYPosition) +
                    new Point(item.Padding.Left, item.Padding.Top),
                    item.DesiredSpace.Size);
                item.OnLayout(args);
                if (setColumn)
                    ColumnDefinitions.Add(column, item.Padding.Top + item.Padding.Bottom + item.DesiredSpace.Size.Y);
                else
                    ColumnDefinitions[column] = LastYPosition + item.Padding.Top + item.Padding.Bottom + item.DesiredSpace.Size.Y;
                LastXPosition += calculatedXPosition;                                
                column++;
            }
        }
    }
}
