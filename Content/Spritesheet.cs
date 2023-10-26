//#define SPRITESHEET_COMPAT

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI2.Content
{
    public class Spritesheet
    {
        public const int CELLSIZE = 50;
        public const int COLUMNS = 6;
        public static int CellSize
        {
            get; set;
        } = CELLSIZE;
        public int Width
        {
            get; private set;
        }
        public int Height
        {
            get; private set;
        }
        public Texture2D ImageData
        {
            get; private set;
        }
        public int Index
        {
            get => (Rows * CellSize) + Column;
            set => GetFrameByIndex(value);
        }
        public int Rows
        {
            get => Height / CellSize;
        }
        public int Columns
        {
            get => Width / CellSize;
        }
        public int Row
        {
            get; private set;
        }
        public int Column
        {
            get; private set;
        }

#if SPRITESHEET_COMPAT
        public static Spritesheet ConvertTo(ShortCircuit.Content.Spritesheet subject)
        {
            return new Spritesheet(subject.ImageData);
        }
#endif

        public Spritesheet(Texture2D Image)
        {
            try
            {
                ImageData = Image;
                Width = ImageData.Width;
                Height = ImageData.Height;
            }
            catch (Exception)
            {

            }
        }
        [Obsolete]
        public Spritesheet(GraphicsDevice d, ContentManager m, string path)
        {
            try
            {
                ImageData = m.Load<Texture2D>(path);
                Width = ImageData.Width;
                Height = ImageData.Height;
            }
            catch (Exception)
            {

            }
        }
        public Spritesheet(int IMSizeWidth, int IMSizeHeight)
        {
            Width = IMSizeWidth;
            Height = IMSizeHeight;
        }
        public Rectangle GetFrame()
        {
            if (Row > Rows)
                Row = 0;
            if (Column > Columns)
                Column = 0;
            return new Rectangle(Column * CellSize, Row * CellSize, CellSize, CellSize);
        }
        public Rectangle GetFrame(int row, int column)
        {
            Row = row;
            Column = column;
            return GetFrame();
        }
        public Rectangle Advance(int rows, int columns)
        {
            Row += rows;
            Column += columns;
            return GetFrame();
        }

        public Rectangle GetFrameByIndex(int index, int columnAmount = COLUMNS)
        {
            if (index < 0)
                return Rectangle.Empty;
            Row = index / columnAmount;
            Column = index - (columnAmount * Row);
            return GetFrame();
        }
    }
}
