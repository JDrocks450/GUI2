using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glacier.Common.Util;
using GUI2.Content;
using GUI2.Controls;
using GUI2.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GUI2.Accents
{
    public class BorderAccent : IAccent
    {
        private int cornerIndex, borderIndex;
        public int CornerIndex
        {
            get => cornerIndex; set
            {
                cornerIndex = value;
                CornerTexture?.Graphics?.GetFrameByIndex(value);
            }
        }
        public int BorderIndex
        {
            get => borderIndex; set
            {
                borderIndex = value;
                BorderTexture?.Graphics?.GetFrameByIndex(value);
            }
        }
        private int cornerSize = -1;

        public GUIComponent Attachment { get; set; }
        public GTexture CornerTexture { get; set; }
        public GTexture BorderTexture { get; set; }
        public int CornerSize
        {
            get => cornerSize == -1 ? BorderThickness : cornerSize;
            internal set => cornerSize = value;
        }
        public int BorderThickness { get; internal set; } = 0;
        public Spritesheet Graphics { get; set; }
        public AccentDrawMode DrawMode { get; set; } = AccentDrawMode.PostDraw;

        /// <summary>
        /// The default untextured border
        /// </summary>
        /// <param name="BorderColor"></param>
        /// <param name="Thickness"></param>
        public BorderAccent(Color BorderColor, int Thickness = 1) : 
            this(new GTexture(GameResources.BaseTexture, BorderColor, new Rectangle()),
                new GTexture(GameResources.BaseTexture, BorderColor, new Rectangle()), Thickness)
        {
            
        }

        public BorderAccent(GTexture Corner, GTexture Border, int Thickness = 0)
        {
            CornerTexture = Corner;
            BorderTexture = Border;
            BorderThickness = Thickness;
        }

        public void DrawAccent(SpriteBatch batch)
        {
            if (Attachment.ContentMargin < new Thickness(BorderThickness))
                Attachment.ContentMargin = new Thickness(BorderThickness);
            if (CornerIndex != -1)
            {
                //topleft
                CornerTexture.DirectDraw(batch,
                    new Rectangle(Attachment.ActualSpace.Left, Attachment.ActualSpace.Top, CornerSize, CornerSize),
                    0f, SpriteEffects.FlipHorizontally);
                //topright
                CornerTexture.DirectDraw(batch,
                    new Rectangle(Attachment.ActualSpace.Right - CornerSize, Attachment.ActualSpace.Top, CornerSize, CornerSize),
                    0f, SpriteEffects.None);
                //botleft
                CornerTexture.DirectDraw(batch,
                    new Rectangle(Attachment.ActualSpace.Left, Attachment.ActualSpace.Bottom - CornerSize, CornerSize, CornerSize),
                    0f, SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically);
                //botright
                CornerTexture.DirectDraw(batch,
                    new Rectangle(Attachment.ActualSpace.Right - CornerSize, Attachment.ActualSpace.Bottom - CornerSize, CornerSize, CornerSize),
                    0f, SpriteEffects.FlipVertically);
            }
            if (BorderIndex != -1)
            {
                //up
                BorderTexture.DirectDraw(batch,
                    new Rectangle(Attachment.ActualSpace.Left + CornerSize, Attachment.ActualSpace.Top, Attachment.ActualSpace.Size.X - (CornerSize * 2), BorderThickness),
                    0f, SpriteEffects.None);
                //down
                BorderTexture.DirectDraw(batch,
                    new Rectangle(Attachment.ActualSpace.Left + CornerSize, Attachment.ActualSpace.Bottom - BorderThickness, Attachment.ActualSpace.Size.X - (CornerSize * 2), BorderThickness),
                    0f, SpriteEffects.FlipVertically);
                //left
                BorderTexture.DirectDraw(batch,
                    new Rectangle(Attachment.ActualSpace.Left, Attachment.ActualSpace.Bottom - CornerSize, Attachment.ActualSpace.Size.Y - (CornerSize * 2), BorderThickness),
                    MathHelper.ToRadians(-90), SpriteEffects.None);
                //right
                BorderTexture.DirectDraw(batch,
                    new Rectangle(Attachment.ActualSpace.Right, Attachment.ActualSpace.Top + CornerSize, Attachment.ActualSpace.Size.Y - (CornerSize * 2), BorderThickness),
                    MathHelper.ToRadians(90), SpriteEffects.None);                
            }
        }
    }
}
