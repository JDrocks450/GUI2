using GUI2.Content;
using GUI2.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI2.Accents
{
    public class BackgroundAccent : IAccent
    {
        private Color _color;

        public GTexture Texture
        {
            get; set;
        }

        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                Texture = GTexture.GetSolidColor(value);
            }
        }

        public BackgroundAccent(Color color)
        {
            Color = color;
        }
        public BackgroundAccent(GTexture gTexture)
        {
            this.Texture = gTexture;
        }

        public GUIComponent Attachment { get; set; }
        public Spritesheet Graphics { get; set; }
        public AccentDrawMode DrawMode { get; set; } = AccentDrawMode.PreDraw;

        public void DrawAccent(SpriteBatch batch)
        {
            Texture.DirectDraw(batch, Attachment.ActualSpace, 0f, SpriteEffects.None);
        }
    }
}
