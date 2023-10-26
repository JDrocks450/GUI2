using Glacier.Common.Util;
using GUI2.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI2.Controls
{
    public class GTexture : GUIComponent
    {
        public enum RotationModes
        {
            /// <summary>
            /// The <see cref="OriginRatio"/> will automatically be adjusted to allow the <see cref="GTexture"/> to rotate about the center
            /// </summary>
            AutoCenter,
            /// <summary>
            /// The <see cref="OriginRatio"/> will not be changed
            /// </summary>
            ManualAdjust
        }
        public RotationModes RotationMode
        {
            get;set;
        }
        public enum SizeModes
        {
            Stretch, 
            UniformToFill
        }
        public SizeModes SizeMode
        {
            get; set;
        }
        public Texture2D RenderTexture
        {
            get; set;
        }
        public Rectangle? Source
        {
            get; set;
        } = null;
        public SpriteEffects Effects
        {
            get;set;
        }
        public float Rotation
        {
            get; set;        
        }
        public Vector2 OriginRatio
        {
            get
            {
                if (RotationMode == RotationModes.AutoCenter && Rotation != 0f)
                    return new Vector2(.5f, .5f);
                else
                    return _originRatio;
            }
            set => _originRatio = value;
        }
        public Vector2 Origin => new Vector2(RenderTexture.Width, RenderTexture.Height) * OriginRatio;
        public Spritesheet Graphics;
        private Vector2 _originRatio;

        public Color BlendingColor { get; set; } = Color.White;
        public GTexture(GTexture copyFrom)
        {
            RenderTexture = copyFrom.RenderTexture;
            BlendingColor = copyFrom.BlendingColor;
        }
        public GTexture(Color solidColor) : this(GameResources.BaseTexture, solidColor, new Rectangle())
        {

        }
        public GTexture(Texture2D texture) : this(texture, new Point())
        {
            
        }
        public GTexture(Texture2D Texture, Color color, Rectangle DestinationRect)
        {            
            RenderTexture = Texture;
            BlendingColor = color;
            DesiredSpace = DestinationRect;
        }
        public GTexture(Texture2D Texture, Point Position)
        {
            RenderTexture = Texture;
            DesiredSpace = new Rectangle(Position, new Point(Texture.Width, Texture.Height));
        }
        public static GTexture GetSolidColor(Color color) => new GTexture(color); 
        protected internal override void Repaint(SpriteBatch sprite)
        {
            DirectDraw(sprite, new Rectangle(ActualSpace.Location, ActualSpace.Size), Rotation, Effects);
        }
        public void DirectDraw(SpriteBatch batch)
        {
            DirectDraw(batch, new Rectangle(ActualSpace.Location,ActualSpace.Size), Rotation, Effects);                    
        }
        public void DirectDraw(SpriteBatch Batch, Rectangle Destination, float Rotation, SpriteEffects Effects)
        {
            if (RenderTexture != null)
            {
                if (SizeMode == SizeModes.UniformToFill)
                {
                    var source = Source ?? new Rectangle(0, 0, RenderTexture.Width, RenderTexture.Height);
                    var newDestination = Destination;
                    var aspectRatio = (double)RenderTexture.Width / RenderTexture.Height;
                    if (Destination.Width <= Destination.Height) // favor width                    
                        newDestination.Height = (int)(source.Height * ((double)Destination.Width / source.Width));
                    else // favor height
                        newDestination.Width = (int)(source.Width * ((double)Destination.Height / source.Height));
                    Destination = newDestination;
                    DesiredSize = Destination.Size;
                }
                Batch.Draw(RenderTexture, new Rectangle(Destination.Location + (Origin/new Vector2(2)).ToPoint(),Destination.Size),
                    Source, BlendingColor, Rotation, Origin, Effects, 1);
            }
        }
    }
}
