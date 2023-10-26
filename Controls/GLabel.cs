using GUI2.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI2.Controls
{
    public class GLabel : GUIComponent
    {
        public Color TextColor
        {
            get;
            set;
        } = Color.White;
        public string Text
        {
            get; set;
        } = "";
        public float TextSize
        {
            get => Font.RenderSize;
            set => Font.RenderSize = value;
        }
        public InterfaceFont Font { get; set; } = new InterfaceFont();
        public bool Underline
        {
            get => Font.Underline;
            set => Font.Underline = value;
        }

        /// <summary>
        /// Measures Text with the current Font
        /// </summary>
        /// <returns></returns>
        public Vector2 Measure()
        {
            return Font.Measure(Text);
        }

        public void SetText(string NewText, Color NewTextColor)
        {
            Text = NewText;
            TextColor = NewTextColor;
        }

        protected internal override void Update(GameTime gameTime)
        {
            DesiredSpace = new Rectangle(DesiredSpace.Location, Measure().ToPoint());
            base.Update(gameTime);
        }

        protected internal override void Repaint(SpriteBatch sprite)
        {
            Font.DrawString(sprite, Text, ActualSpace.Location.ToVector2(), TextColor);
        }

        public GLabel()
        {

        }
        public GLabel(string Text, InterfaceFont font, Color TextColor) : this()
        {
            this.Text = Text;
            this.TextColor = TextColor;
            Font = font;
        }        
        public GLabel(string Text, Color TextColor) : this(Text, new InterfaceFont(), TextColor)
        {
            
        }
        public GLabel(string Text, float PtSize, Color TextColor) : this(Text, new InterfaceFont(PtSize, InterfaceFont.FontWeight.Regular), TextColor)
        {

        }
    }
}
