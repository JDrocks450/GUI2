using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GUI2.Layout
{
    public class StackLayout : ILayoutManager
    {
        public enum Orientation
        {
            Vertical,
            Horizontal
        }
        public Orientation ContentOrientation
        {
            get; set;
        } = Orientation.Horizontal;
        public Rectangle Safezone { get; set; }

        public StackLayout(Orientation ContentOrientation = Orientation.Vertical)
        {
            this.ContentOrientation = ContentOrientation;
        }

        public void OnLayoutRequested(IEnumerable<ILayoutItem> Source)
        {
            int lastLocation = 0;
            LayoutEventArgs e = new LayoutEventArgs();
            foreach (var child in Source)
            {
                if (child.LayoutIgnore)
                    continue;
                switch (ContentOrientation)
                {
                    case Orientation.Vertical:
                        e.NewSpace = new Rectangle(
                            new Point(child.Padding.Left,
                                lastLocation + child.Padding.Top),
                            child.DesiredSpace.Size);
                        lastLocation += child.Padding.Top + child.DesiredSpace.Height;/* - child.Padding.Bottom; */                    
                        break;
                    case Orientation.Horizontal:
                        e.NewSpace = new Rectangle(
                            new Point(lastLocation + child.Padding.Left,
                                child.Padding.Top),
                            child.DesiredSpace.Size);
                        lastLocation += child.Padding.Left + child.ActualSpace.Width + child.Padding.Right;                                          
                        break;
                }
                child.OnLayout(e);    
            }
        }
    }
}

