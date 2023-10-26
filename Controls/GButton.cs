using Glacier.Common.Provider;
using Glacier.Common.Provider.Input;
using Glacier.Common.Util;
using GUI2.Accents;
using GUI2.Content;
using GUI2.Primitives;
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
    public class GButton : GChildComponent, IClickable
    {
        //EVENTS
        public delegate void OnClickEventHandler(GButton sender);
        public event OnClickEventHandler OnClick;
        public event EventHandler<InputProvider.InputEventArgs> OnMouseEnter, OnMouseLeave;

        protected enum DrawState
        {
            Regular,
            Hover,
            Click,
            Disable
        }
        protected DrawState state;

        private Rectangle lastDesiredSpace;        

        protected GLabel TextLabel = new GLabel();

        public GTexture BackgroundTexture
        {
            get; set;
        }

        public string Text
        {
            get => TextLabel.Text;
            set => TextLabel.Text = value;
        }
        public Texture2D Texture { get => BackgroundTexture?.RenderTexture; set { } }
        public float Scale { get; set; } = 1f;
        public Rectangle Hitbox { get => ActualSpace; set { } }
        public bool IsMouseOver { get; set; }
        public bool PerpixelHitDetection { get; set; }
        public bool Enabled => Availability == Availabilities.Enabled;
        private Color _background = Color.White;
        public BorderAccent Border
        {
            set => AddAccent(value);
        }
        public override Thickness ContentMargin { get; set; } = new Thickness(0);
        public Color Background
        {
            get => _background;
            set
            {
                _background = value;                
                High = Color.Lerp(Background, Color.White, .5f);
                Click = Color.Lerp(Background, Color.Black, .5f);
            }
        }
        public Color Foreground
        {
            get; set;
        } = Color.Black;
        public Color High
        {
            get; set;
        } = default;
        public Color Click
        {
            get;
            set;
        } = default;
        public Rectangle Safezone => new Rectangle();

        public GButton()
        {
            var provider = ProviderManager.Root.Get<InputProvider>();
            provider.Subscribe(this, InputProvider.TransformGroup.Untransformed);
            HorizontalContentAlignment = HorizontalAlignments.Center;            
        }
        public GButton(GTexture Texture) : this((GUIComponent)Texture)
        {

        }
        public GButton(GUIComponent Child) : this()
        {
            if (Child is GTexture)
                (Child as GTexture).SizeMode = GTexture.SizeModes.UniformToFill;
            //Child.AutoSizeToParent = true;
            this.Child = Child;
        }
        public GButton(string Text) : this()
        {
            TextLabel.Text = Text;
            Child = TextLabel;
            //AutosizeToChild = true;
        }
        public GButton(string Text, Color BackgroundColor, Color ForeColor, Color Highlight, Color Click) : this()
        {
            //AutosizeToChild = true;
            SetupTextButton(Text, BackgroundColor, ForeColor, Highlight, Click, new Rectangle());
        }
        public GButton(string Text, Color BackgroundColor, Color ForeColor, Color Highlight, Color Click, Rectangle Space) : this()
        {
            SetupTextButton(Text, BackgroundColor, ForeColor, Highlight, Click, Space);
        }
        public GButton SetupTextButton(string Text, Color BackgroundColor, Color ForeColor, Color Highlight, Color Click, Rectangle Space)
        {
            //AutosizeToChild = false;
            TextLabel.Text = Text;
            Background = BackgroundColor;
            Foreground = ForeColor;
            High = Highlight;
            this.Click = Click;
            DesiredSpace = Space;
            return this;
        }

        protected internal override void Update(GameTime gameTime)
        {
            if (Availability == Availabilities.Disabled)
                return;
            var mouse = Mouse.GetState();
            var MouseRect = new Rectangle(mouse.Position, new Point(1, 1));
            BackgroundTexture?.Refresh(gameTime);
            base.Update(gameTime);
            state = DrawState.Regular;
            if (IsMouseOver && Mouse.GetState().LeftButton != ButtonState.Pressed) //Hover            
                state = DrawState.Hover;            
            if (IsMouseOver && Mouse.GetState().LeftButton == ButtonState.Pressed) //MouseDown            
                state = DrawState.Click;            
        }

        protected internal override void Repaint(SpriteBatch sprite)
        {
            if (Availability != Availabilities.Invisible)
            {
                Color DrawColor = Background;
                switch (state)
                {
                    case DrawState.Hover: DrawColor = High; break;
                    case DrawState.Click: DrawColor = Click; break;
                }
                if (BackgroundTexture == null)
                    sprite.Draw(GameResources.BaseTexture, ActualSpace, DrawColor);
                else
                {
                    BackgroundTexture.DesiredSpace = ActualSpace;
                    BackgroundTexture.Repaint(sprite);
                    sprite.Draw(GameResources.BaseTexture, ActualSpace, DrawColor * .25f);
                }
            }
            base.Repaint(sprite);
        }

        public void MouseEnter(GameTime time, InputProvider.InputEventArgs args)
        {
            OnMouseEnter?.Invoke(this, args);
        }

        public void Clicked(GameTime time, InputProvider.InputEventArgs e)
        {
            if (Availability != Availabilities.Disabled)
                if (e.MouseLeftClick)
                {
                    if (IsMouseOver)
                        OnClick?.Invoke(this);
                }
        }

        public void MouseDown(GameTime time, Glacier.Common.Provider.Input.InputProvider.InputEventArgs args)
        {
            
        }

        public void MouseLeave(GameTime time, InputProvider.InputEventArgs args)
        {
            OnMouseLeave?.Invoke(this, args);
        }
    }
}
