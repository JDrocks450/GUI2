using GUI2.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI2.Extensions
{
    public static class GTextureExtensions
    {
        public static GTexture ToGTexture(this Texture2D texture)
        {
            return new GTexture(texture, new Point(0, 0));
        }
        public static GTexture ToGTexture(this Texture2D texture, Color color)
        {
            return ToGTexture(texture, new Point(), color);
        }
        public static GTexture ToGTexture(this Texture2D texture, Point point)
        {
            return new GTexture(texture, point);
        }
        public static GTexture ToGTexture(this Texture2D texture, Point point, Color color)
        {
            return new GTexture(texture, color, new Rectangle(point, new Point(texture.Width, texture.Height)));
        }
    }
}
