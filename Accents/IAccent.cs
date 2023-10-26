using GUI2.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI2.Accents
{
    public enum AccentDrawMode
    {
        /// <summary>
        /// This will be drawn before the component is drawn
        /// </summary>
        PreDraw,
        /// <summary>
        /// This will be drawn after the component is drawn
        /// </summary>
        PostDraw
    }
    public interface IAccent
    {
        GUIComponent Attachment
        {
            get;set;
        }
        Spritesheet Graphics
        {
            get; set;
        }
        AccentDrawMode DrawMode
        {
            get;set;
        }
        void DrawAccent(SpriteBatch batch);
    }
}
