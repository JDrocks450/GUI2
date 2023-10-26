using GUI2.Primitives;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI2.Layout
{
    public interface ILayoutItem
    {
        /// <summary>
        /// Specifies whether this <see cref="ILayoutItem"/> is ignored in the Layout System
        /// </summary>
        bool LayoutIgnore { get; set; }
        Thickness Padding
        {
            get;
            set;
        }
        Rectangle DesiredSpace
        {
            get; set;
        }
        Rectangle ActualSpace
        {
            get;
        }
        /// <summary>
        /// When this component has been included in a layout
        /// </summary>
        void OnLayout(LayoutEventArgs args);
    }
}
