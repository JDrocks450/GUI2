using Glacier.Common.Provider.Input;
using Glacier.Common.Util;
using GUI2.Content;
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
    /// <summary>
    /// A <see cref="GChildrenComponent"/> that clips content found outside of its boundary and scroll its content
    /// </summary>
    public class GScrollableContainer : GUI2.GChildrenComponent, IClickable
    {
        public enum ScrollMode
        {
            /// <summary>
            /// The scrollbar will not appear and the content will not be scrollable in this direction.
            /// </summary>
            Hidden,
            /// <summary>
            /// The scrollbar will always be available but content cannot scroll past the maximum scroll value.
            /// </summary>
            Enabled,
            /// <summary>
            /// The scrollbar will only appear when content can be scrolled.
            /// </summary>
            Automatic
        }

        private bool yScrolling = false, xScrolling = false;
        private Point scrollBegin;
        private const int SCROLLTARGET_SIZE = 25;
        private GButton scrollUpButton, scrollDownButton, yScrollbar,
            scrollLeftButton, scrollRightButton, xScrollbar;
        private SpriteBatch scrollBatch = new SpriteBatch(GameResources.Device);
        private float maxYScrollContentValue = 0, maxXScrollContentValue = 0;
        public bool Enabled => Availability == Availabilities.Enabled;

        /// <summary>
        /// The current amount of pixels the content is scrolled down.
        /// </summary>
        public float YScrollValue
        {
            get; set;
        }
        /// <summary>
        /// The current amount of pixels the content is scrolled right.
        /// </summary>
        public float XScrollValue
        {
            get; set;
        }

        /// <summary>
        /// The behavior of the Vertical Scrollbar
        /// </summary>
        public ScrollMode VerticalScrollbarMode
        {
            get; set;
        } = ScrollMode.Automatic;
        /// <summary>
        /// The behavior of the Horizontal Scrollbar
        /// </summary>
        public ScrollMode HorizontalScrollbarMode
        {
            get; set;
        } = ScrollMode.Automatic;

        /// <summary>
        /// The amount <see cref="YScrollValue"/> increments when the Scrollbuttons are pressed.
        /// </summary>
        public static float ScrollStep { get; set; } = 10;
        public Texture2D Texture { get; set; }
        public float Scale { get; set; }
        public Rectangle Safezone => new Rectangle();
        public Rectangle Hitbox { get => ActualSpace; set { } }
        public bool IsMouseOver { get; set; }
        public bool PerpixelHitDetection { get; set; }

        protected static GTexture StyleScrollButtonTexture, StyleScrollBarTexture;

        /// <summary>
        /// Creates a <see cref="GScrollableContainer"/> with the default style
        /// <seealso cref=""/>
        /// </summary>
        public GScrollableContainer() : this(StyleScrollButtonTexture, StyleScrollBarTexture)
        {

        }

        /// <summary>
        /// Creates a <see cref="GScrollableContainer"/> with <see cref="GTexture"/> objects for buttons and the scrollbar.
        /// </summary>
        public GScrollableContainer(GTexture ScrollButtonTexture, GTexture ScrollbarTexture)
        {            
            (scrollUpButton = new GButton("")
            {
                BackgroundTexture = new GTexture(ScrollButtonTexture)
            }).OnClick += ScrollUp;
            (scrollDownButton = new GButton("")
            {
                BackgroundTexture = new GTexture(ScrollButtonTexture)
                {
                    Effects = SpriteEffects.FlipVertically
                }
            }).OnClick += ScrollDown;
            (yScrollbar = new GButton("")
            {
                BackgroundTexture = new GTexture(ScrollbarTexture)
            }).OnClick += yScrollbar_Activate;
            (scrollLeftButton = new GButton("")
            {                
                BackgroundTexture = new GTexture(ScrollButtonTexture)
                {
                    Rotation = MathHelper.ToRadians(-90)
                }
            }).OnClick += ScrollLeft;
            (scrollRightButton = new GButton("")
            {
                BackgroundTexture = new GTexture(ScrollButtonTexture)
                {
                    Rotation = MathHelper.ToRadians(-90),
                    Effects = SpriteEffects.FlipVertically
                }
            }).OnClick += ScrollRight;
            (xScrollbar = new GButton("")
            {
                BackgroundTexture = new GTexture(ScrollbarTexture)
                {
                    Rotation = MathHelper.ToRadians(90)
                }
            }).OnClick += XScrollbar_Activate;
        }

        public static void SetStyle(GTexture ScrollButtonTexture, GTexture ScrollbarTexture)
        {
            StyleScrollBarTexture = ScrollbarTexture;
            StyleScrollButtonTexture = ScrollButtonTexture;
        }

        private void ScrollRight(GButton sender)
        {
            XScrollValue += ScrollStep;
        }

        private void ScrollLeft(GButton sender)
        {
            XScrollValue += ScrollStep;
        }

        private void XScrollbar_Activate(GButton sender)
        {
            xScrolling = true;
            scrollBegin = Mouse.GetState().Position;
        }

        private void yScrollbar_Activate(GButton sender)
        {
            yScrolling = true;
            scrollBegin = Mouse.GetState().Position;
        }

        private void ScrollUp(GButton sender)
        {
            YScrollValue -= ScrollStep;
        }

        private void ScrollDown(GButton sender)
        {
            YScrollValue += ScrollStep;
        }

        /// <summary>
        /// Defines whether the content can be scrolled vertically.
        /// </summary>
        /// <returns></returns>
        private bool WorkVScrollbar()
        {
            if (VerticalScrollbarMode != ScrollMode.Hidden)
            {
                if (VerticalScrollbarMode == ScrollMode.Automatic && ContentBoundary.Bottom <= ContentSpace.Bottom)
                {
                    return false;
                }
                maxYScrollContentValue = ContentBoundary.Bottom - ContentSpace.Bottom;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Defines whether the content can be scrolled horizontally.
        /// </summary>
        /// <returns></returns>
        private bool WorkHScrollbar()
        {
            if (HorizontalScrollbarMode != ScrollMode.Hidden)
            {
                if (HorizontalScrollbarMode == ScrollMode.Automatic && ContentBoundary.Right <= ContentSpace.Right)
                {
                    return false;
                }
                maxXScrollContentValue = ContentBoundary.Right - ContentSpace.Right;
                return true;
            }
            return false;
        }

        protected internal override void Update(GameTime gt)
        {
            var mousePos = Mouse.GetState().Position;
            var mouseDiff = mousePos - scrollBegin;
            if (WorkVScrollbar())
            {
                ContentMargin = new Primitives.Thickness(0, 0, SCROLLTARGET_SIZE, ContentMargin.Bottom);                
                if (yScrolling)
                {
                    yScrolling = GameResources.InputProvider.MouseLeftDown;
                    YScrollValue += ((mouseDiff.Y) / (float)ActualSpace.Height) * ContentBoundary.Height;                    
                }
                if (YScrollValue > maxYScrollContentValue) YScrollValue = maxYScrollContentValue;
                if (YScrollValue < 0) YScrollValue = 0;
                scrollUpButton.DesiredSpace = new Rectangle(ActualSpace.Location +
                    new Point(ActualSpace.Width - SCROLLTARGET_SIZE, 0),
                    new Point(SCROLLTARGET_SIZE));
                scrollUpButton.Update(gt);
                scrollDownButton.DesiredSpace = new Rectangle(ActualSpace.Location +
                    new Point(ActualSpace.Width - SCROLLTARGET_SIZE, ContentSpace.Height - SCROLLTARGET_SIZE),
                    new Point(SCROLLTARGET_SIZE));
                scrollDownButton.Update(gt);
                yScrollbar.DesiredSpace = new Rectangle(ActualSpace.Location +
                    new Point(ActualSpace.Width - SCROLLTARGET_SIZE,
                        SCROLLTARGET_SIZE +
                        (int)((YScrollValue / maxYScrollContentValue) * (ContentSpace.Height - (SCROLLTARGET_SIZE * 2) -
                        ((float)ContentSpace.Height / ContentBoundary.Height * (ContentSpace.Height - (SCROLLTARGET_SIZE * 2)))))),
                    new Point(SCROLLTARGET_SIZE, (int)
                        ((float)ContentSpace.Height / ContentBoundary.Height * (ContentSpace.Height - (SCROLLTARGET_SIZE * 2)))));
                if (yScrollbar.DesiredSpace.Height > ContentSpace.Height - (SCROLLTARGET_SIZE * 2))
                    yScrollbar.DesiredSpace = new Rectangle(
                        yScrollbar.DesiredSpace.Location, new Point(yScrollbar.DesiredSpace.Width, ContentSpace.Height - (SCROLLTARGET_SIZE * 2)));
                yScrollbar.Update(gt);
            }
            else
            {
                YScrollValue = 0;
                ContentMargin = new Primitives.Thickness(0, 0, 0, 0);
            }
            if (WorkHScrollbar())
            {
                ContentMargin = new Primitives.Thickness(0, 0, ContentMargin.Bottom, SCROLLTARGET_SIZE);                
                if (xScrolling)
                {
                    xScrolling = GameResources.InputProvider.MouseLeftDown;                                  
                    XScrollValue += ((mouseDiff.X) / (float)ContentSpace.Width) * ContentBoundary.Width;
                }
                if (XScrollValue > maxXScrollContentValue) XScrollValue = maxXScrollContentValue;
                if (XScrollValue < 0) XScrollValue = 0;
                scrollLeftButton.DesiredSpace = new Rectangle(ActualSpace.Location +
                    new Point(0, ActualSpace.Height - SCROLLTARGET_SIZE),
                    new Point(SCROLLTARGET_SIZE));
                scrollLeftButton.Update(gt);
                scrollRightButton.DesiredSpace = new Rectangle(ActualSpace.Location +
                    new Point(ContentSpace.Width - SCROLLTARGET_SIZE, ActualSpace.Height - SCROLLTARGET_SIZE),
                    new Point(SCROLLTARGET_SIZE));
                scrollRightButton.Update(gt);
                xScrollbar.DesiredSpace = new Rectangle(ActualSpace.Location +
                    new Point(SCROLLTARGET_SIZE +
                        (int)((XScrollValue / maxXScrollContentValue) * (ContentSpace.Width - (SCROLLTARGET_SIZE * 2) -
                        ((float)ContentSpace.Width / ContentBoundary.Width * (ContentSpace.Width - (SCROLLTARGET_SIZE * 2))))),
                        ActualSpace.Height - SCROLLTARGET_SIZE),
                    new Point((int)((float)ContentSpace.Width / ContentBoundary.Width * (ContentSpace.Width - (SCROLLTARGET_SIZE * 2))),
                        SCROLLTARGET_SIZE));
                if (xScrollbar.DesiredSpace.Width > ContentSpace.Width - (SCROLLTARGET_SIZE * 2))
                    xScrollbar.DesiredSpace = new Rectangle(
                        xScrollbar.DesiredSpace.Location, new Point(ContentSpace.Width - (SCROLLTARGET_SIZE * 2), xScrollbar.DesiredSpace.Height));
                xScrollbar.Update(gt);
            }
            else
            {
                XScrollValue = 0;
                ContentMargin = new Primitives.Thickness(0, 0, ContentMargin.Bottom, 0);
            }
            scrollBegin = mousePos;
            base.Update(gt);
        }        

        protected internal override void Repaint(SpriteBatch batch)
        {
            //batch.Draw(Resources.BaseTexture, Space, Color.Black * .5f);            
            batch.End();    
            batch.GraphicsDevice.ScissorRectangle = ContentSpace;
            var scrollMatrix = Matrix.CreateTranslation(new Vector3(-XScrollValue, -YScrollValue, 0)) *
                                         Matrix.CreateRotationZ(0) *
                                         Matrix.CreateScale(new Vector3(1, 1, 0));
            batch.Begin(SpriteSortMode.Deferred, null, ((GlacierSpriteBatch)batch).SampleState, null, new RasterizerState() { ScissorTestEnable = true }, null, scrollMatrix);
#if DEBUG && false
            batch.Draw(GameResources.BaseTexture, ContentBoundary, (IsMouseOver ? Color.Green : Color.DarkGreen) * .5f);
#endif
            foreach (var child in Children)
            {
                child._Draw(batch);
            }
            batch.End();
            ((GlacierSpriteBatch)batch).Begin();
            if (WorkVScrollbar()) {
                scrollUpButton.Repaint(batch);
                scrollDownButton.Repaint(batch);
                yScrollbar.Repaint(batch);
            }
            if (WorkHScrollbar())
            {
                scrollLeftButton.Repaint(batch);
                scrollRightButton.Repaint(batch);
                xScrollbar.Repaint(batch);
            }
        }

        public void MouseEnter(GameTime time, Glacier.Common.Provider.Input.InputProvider.InputEventArgs args)
        {
            throw new NotImplementedException();
        }

        public void Clicked(GameTime time, InputProvider.InputEventArgs args)
        {
            throw new NotImplementedException();
        }

        public void MouseDown(GameTime time, Glacier.Common.Provider.Input.InputProvider.InputEventArgs args)
        {
            throw new NotImplementedException();
        }

        public void MouseLeave(GameTime time, InputProvider.InputEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
