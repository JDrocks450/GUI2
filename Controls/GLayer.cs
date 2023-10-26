using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glacier.Common.Util;
using GUI2.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GUI2.Controls
{
    /// <summary>
    /// A space to place and display interactive components.
    /// </summary>
    public class GLayer : GUI2.GChildrenComponent
    {        
        public enum FilteringModes
        {
            BilinearClamp,
            NearestNeighborClamp,
            BilinearRepeating,
            NearestNeighborRepeating
        }
        public FilteringModes FilteringMode
        {
            get; set;
        } = FilteringModes.BilinearClamp;
        public float LayerIndex
        {
            get;set;
        }
        public GLayer(float layer = 0)
        {
            DesiredSpace = GameResources.Screen;
            LayerIndex = layer;
        }

        /// <summary>
        /// Updates the GLayer instance and every child component it contains.
        /// </summary>
        /// <param name="gt"></param>
        public new void Update(GameTime gt)
        {
            DesiredSpace = GameResources.Screen;            
            base.Update(gt);
        }
        /// <summary>
        /// Draws the GLayer and every child component it contains.
        /// </summary>
        /// <param name="gt"></param>
        public void Draw(GlacierSpriteBatch batch)
        {
            _Draw(batch);
        }
    }
}
