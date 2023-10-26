using Glacier.Common.Util;
using GUI2.Accents;
using GUI2.Layout;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI2.Controls
{
    /// <summary>
    /// A dialog box that displays content
    /// </summary>
    public class GWindow : GContainer
    {
        public BorderAccent WindowBorder
        {
            get => Accents.OfType<BorderAccent>().FirstOrDefault();
            set => AddAccent(value);
        }
        public GContainer ToolStrip { get; private set; }
        public GContainer ContentPane { get; private set; }
        public BackgroundAccent ToolStripBackground
        {
            get => ToolStrip.Background;
            set => ToolStrip.Background = value;
        }
        public GButton ExitButton
        {
            get; protected set;
        }
        private Point _oldMousePosition;
        public bool IsDragging
        {
            get; private set;
        }
        public GLabel TitleLabel { get; set; }
        public string Title { get; set; }
        public Color BackgroundColor { get; set; } = Color.DarkCyan;

        public GWindow(Rectangle Space) : base(Space) => Initialize();
        public GWindow(Rectangle Space, BorderAccent Border) : base(Space, Border) => Initialize();

        private void Initialize()
        {
            GameResources.InputProvider.PreviewInputEvent += InputProvider_PreviewInputEvent;
            Background = new BackgroundAccent(BackgroundColor);
            HorizontalAlignment = GUI2.HorizontalAlignments.Center;
            VerticalAlignment = GUI2.VerticalAlignments.Center;
            SetLayout(new StackLayout());            
            ToolStrip = new GContainer(new Rectangle(0, 0, 0, 30))
            {
                Background = new BackgroundAccent(Color.Black * .25f),
                HorizontalAlignment = GUI2.HorizontalAlignments.Stretch,
            };
            TitleLabel = new GLabel("Untitled Window", 14f, Color.White)
            {
                DesiredPosition = new Point(10, 5)
            };
            ToolStrip.Children.Add(TitleLabel);
            ExitButton = new GButton("X")
            {
                HorizontalAlignment = GUI2.HorizontalAlignments.Right,
                VerticalAlignment = GUI2.VerticalAlignments.Stretch,
                DesiredSpace = new Rectangle(0, 0, 30, 0),
                VerticalContentAlignment = GUI2.VerticalAlignments.Center,
                Background = ToolStrip.Background.Color,
                High = Color.Red,
                Click = Color.DarkRed,
            };
            ExitButton.OnClick += ExitButton_OnClick;
            ToolStrip.Children.Add(ExitButton);
            Children.Add(ToolStrip);
            Children.Add(ContentPane = new GContainer(new Rectangle(0, 0, 0, 0))
            {
                HorizontalAlignment = HorizontalAlignments.Stretch,
                VerticalAlignment = VerticalAlignments.Stretch
            });
        }

        private void InputProvider_PreviewInputEvent(Glacier.Common.Provider.Input.InputProvider.InputEventArgs e)
        {
            var state = Mouse.GetState();
            e.Handled = new Rectangle(state.Position, new Point(1)).Intersects(ActualSpace);
        }

        protected internal override void Update(GameTime gt)
        {
            TitleLabel.Text = Title;
            Background.Color = BackgroundColor;
            var state = Mouse.GetState();
            if (state.LeftButton == ButtonState.Pressed)
            {
                if (new Rectangle(state.Position, new Point(1)).Intersects(ToolStrip.ActualSpace) && !IsDragging)
                {
                    IsDragging = true;
                    HorizontalAlignment = HorizontalAlignments.Left;
                    VerticalAlignment = VerticalAlignments.Top;
                    DesiredPosition = ActualPosition;
                }
                else if (IsDragging)
                {
                    var change = state.Position - _oldMousePosition;
                    DesiredPosition += change;
                }
            }
            else if (IsDragging)
            {
                IsDragging = false;
            }
            _oldMousePosition = state.Position;
            base.Update(gt);
        }

        /// <summary>
        /// When the user attempts to close the dialog, the return value of this can be used to cancel the closure of the dialog
        /// </summary>
        /// <returns></returns>
        protected virtual bool OnClose() => true;

        public override void Destroy()
        {
            GameResources.InputProvider.PreviewInputEvent -= InputProvider_PreviewInputEvent;
            base.Destroy();
        }

        private void ExitButton_OnClick(GButton sender)
        {
            if (OnClose())
                Destroy();
        }
    }
}
