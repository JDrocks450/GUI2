using Glacier.Common.Provider;
using Glacier.Common.Provider.Input;
using Glacier.Common.Util;
using GUI2.Content;
using GUI2.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI2.Controls
{
    public class GRadioButton : GButton
    {
        public bool Selected
        {
            get; set;
        }

        public GRadioButton() : base()
        {
            OnClick += GRadioButton_OnClick;
        }        

        public GRadioButton(GTexture Texture) : this((GUIComponent)Texture)
        {

        }
        public GRadioButton(GUIComponent Child) : this()
        {
            if (Child is GTexture)
                (Child as GTexture).SizeMode = GTexture.SizeModes.UniformToFill;
            //Child.AutoSizeToParent = true;
            Child.HorizontalAlignment = HorizontalAlignments.Stretch;
            Child.VerticalAlignment = VerticalAlignments.Stretch;
            this.Child = Child;
        }
        public GRadioButton(string Text) : this()
        {
            TextLabel.Text = Text;
            Child = TextLabel;
            //AutosizeToChild = true;
        }
        public GRadioButton(string Text, Color BackgroundColor, Color ForeColor, Color Highlight, Color Click) : this()
        {
            //AutosizeToChild = true;
            SetupTextButton(Text, BackgroundColor, ForeColor, Highlight, Click, new Rectangle());
        }
        public GRadioButton(string Text, Color BackgroundColor, Color ForeColor, Color Highlight, Color Click, Rectangle Space) : this()
        {
            SetupTextButton(Text, BackgroundColor, ForeColor, Highlight, Click, Space);
        }

        protected internal override void Repaint(SpriteBatch sprite)
        {
            if (Selected) state = DrawState.Click;
            base.Repaint(sprite);
        }

        private void GRadioButton_OnClick(GButton sender)
        { 
            Selected = !Selected;
            if (Parent is GChildrenComponent)
                foreach (var child in (Parent as GChildrenComponent).Children.OfType<GRadioButton>())
                    if (child != this)
                        child.Selected = false;            
        }
    }
}
