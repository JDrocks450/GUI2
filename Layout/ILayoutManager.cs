using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI2.Layout
{
    public interface ILayoutManager
    {
        Rectangle Safezone
        {
            get;
            set;
        }
        void OnLayoutRequested(IEnumerable<ILayoutItem> Source);
    }
}
