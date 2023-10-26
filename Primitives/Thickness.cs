using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI2.Primitives
{
    public struct Thickness
    {         
        public int Left, Right, Top, Bottom;
        /// <summary>
        /// Creates a new <see cref="Thickness"/> with the specified values
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        public Thickness(int left, int top, int right, int bottom)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }
        /// <summary>
        /// Creates a new <see cref="Thickness"/> with Left and Top as specified values, but right and bottom as zero.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        public Thickness(int left, int top) : this(left, top, 0, 0)
        {

        }
        /// <summary>
        /// All dimensions in this <see cref="Thickness"/> will be the specified value.
        /// </summary>
        /// <param name="Uniform"></param>
        public Thickness(int Uniform) : this(Uniform, Uniform, Uniform, Uniform)
        {

        }
        public static Thickness operator +(Thickness left, Thickness right)
        {
            return new Thickness(left.Left + right.Left,
                left.Top + right.Top,
                left.Right + right.Right,
                left.Bottom + right.Bottom);
        }
        public static Thickness operator -(Thickness left, Thickness right)
        {
            return new Thickness(left.Left - right.Left,
                left.Top - right.Top,
                left.Right - right.Right,
                left.Bottom - right.Bottom);
        }
        public static bool operator >(Thickness left, Thickness right)
        {
            if (left.Left > right.Left &&
                left.Right > right.Right &&
                left.Top > right.Top &&
                left.Bottom > right.Bottom)
                return true;
            return false;
        }
        public static bool operator <(Thickness left, Thickness right)
        {
            if (left.Left < right.Left &&
                left.Right < right.Right &&
                left.Top < right.Top &&
                left.Bottom < right.Bottom)
                return true;
            return false;
        }
        /// <summary>
        /// Converts this <see cref="Thickness"/> to a <see cref="Point"/> with Left and Top as X and Y
        /// </summary>
        /// <returns></returns>
        public Point ToPoint()
        {
            return new Point(Left, Top);
        }
    }
}
