using Glacier.Common.Util;
using GUI2.Accents;
using GUI2.Content;
using GUI2.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI2.Controls
{
    /// <summary>
    /// A menu with a border that contains elements
    /// </summary>
    public class GContainer : GUI2.GChildrenComponent
    {
        private Accents.BorderAccent Border;

        public int CornerSize
        {
            get => Border.CornerSize; set => Border.CornerSize = value;
        }
        public override Thickness ContentMargin { get => new Thickness(BorderThickness); }
        public int BorderThickness
        {
            get => Border?.BorderThickness ?? 0; set { if (Border != null) Border.BorderThickness = value; }
        }        
        public GContainer(Rectangle Space)
        {
            DesiredSpace = Space;            
        }
        public GContainer(Rectangle Space, BorderAccent accent) : this(Space)
        {
            AddAccent(Border = accent);
        }
        public GContainer(Rectangle Space, GTexture Corner, GTexture Border, int BorderThickness = 0) : this(Space, new Accents.BorderAccent(Corner, Border, BorderThickness))
        {
            AddAccent(this.Border = new Accents.BorderAccent(Corner, Border, BorderThickness));
        }
        public GContainer(Rectangle Space, GTexture Corner, GTexture Border, GTexture Background) : this(Space, Corner, Border)
        {
            this.Background = new BackgroundAccent(Background);
        }

        protected internal override void Update(GameTime gt)
        {         
            base.Update(gt);                       
        }

        protected internal override void Repaint(SpriteBatch batch)
        {                                 
            base.Repaint(batch);
        }
    }
}
