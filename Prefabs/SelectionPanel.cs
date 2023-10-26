using Glacier.Common.Provider.Input;
using Glacier.Common.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI2.Prefabs
{
    /// <summary>
    /// Lets the user click and drag out a rectangular selection
    /// </summary>
    public class SelectionPanel : GUIComponent
    {
        public bool Enabled => Availability == Availabilities.Enabled;
        public Rectangle? Selection
        {
            get; private set;
        } = null;
        public bool HasSelection => Selection.HasValue && Selection.Value.Width > 5 && Selection.Value.Height > 5;

        public delegate void SelectionEventArgs(SelectionPanel sender, Rectangle screenSelectionArea);
        public event SelectionEventArgs SelectionFinished;

        public Texture2D Texture { get; set; }
        public float Scale { get; set; }
        public Rectangle Hitbox { get => ActualSpace; set { } }
        public bool PerpixelHitDetection { get; set; }

        bool mouseLeftDown = false;
        Point initialMousePos;
        public SelectionPanel()
        {
            //GameResources.InputProvider.Subscribe(this, InputProvider.TransformGroup.Untransformed);
            GameResources.InputProvider.PreviewInputEvent += InputProvider_InputEvent;
        }        

        protected internal override void Update(GameTime gt)
        {
            base.Update(gt);
            var state = Mouse.GetState();
            if (new Rectangle(state.Position, new Point(1)).Intersects(ActualSpace) && state.LeftButton == ButtonState.Pressed)
            {
                if (!mouseLeftDown) // initial click                
                    initialMousePos = state.Position;                
                mouseLeftDown = true;
                var position = state.Position;
                Selection = new Rectangle(
                    Math.Min(initialMousePos.X, position.X),
                    Math.Min(initialMousePos.Y, position.Y),
                    Math.Max(initialMousePos.X, position.X) - Math.Min(initialMousePos.X, position.X),
                    Math.Max(initialMousePos.Y, position.Y) - Math.Min(initialMousePos.Y, position.Y));
            }
            else if (mouseLeftDown)
            {
                mouseLeftDown = false;
                SelectionFinished?.Invoke(this, Selection.Value);
                Selection = null;
            }            
        }

        protected internal override void Repaint(SpriteBatch batch)
        {
            if (HasSelection)            
                batch.Draw(GameResources.BaseTexture, Selection.Value, Color.Blue * .25f);            
        }

        private void InputProvider_InputEvent(InputProvider.InputEventArgs e)
        {
            e.Handled = HasSelection ? true : e.Handled;
        }
    }
}
