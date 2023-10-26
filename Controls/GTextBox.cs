using Glacier.Common.Provider;
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
    public class GTextBox : GLabel, IClickable
    {
        public delegate void OnTextAcceptedHandler(object sender);
        public event OnTextAcceptedHandler Accepted;

        public Color Background, Foreground, High, Active;
        private bool _active;

        public bool Enabled => Availability == Availabilities.Enabled;
        public int CursorPosition
        {
            get; internal set;
        }
        public int TextLength { get; internal set; }
        public bool IsActive
        {
            get => _active; set { _active = value; }
        }

        public Rectangle Safezone => new Rectangle();
        public Texture2D Texture { get; set; }
        public float Scale { get; set; }
        public Rectangle Hitbox { get => ActualSpace; set { } }
        public bool IsMouseOver { get; set; }
        public bool PerpixelHitDetection { get; set; }

        /// <summary>
        /// Creates a textbox with the default settings.
        /// </summary>
        public GTextBox()
        {
            Availability = Availabilities.Disabled;
            SetupTextBox("Untitled", Color.Black * .75f, Color.White, Color.Gray * .75f, Color.Gray,
            new Rectangle(0, 0, 500, 50));
        }
        public GTextBox(string Text, Color BackgroundColor, Color ForeColor, Color Highlight, Color Click, Rectangle Space)
        {
            SetupTextBox(Text, BackgroundColor, ForeColor, Highlight, Click, Space);
        }

        public GTextBox SetupTextBox(string Text, Color BackgroundColor, Color ForeColor, Color Highlight, Color Active, Rectangle Space)
        {
            base.Text = Text;
            Background = BackgroundColor;
            Foreground = ForeColor;
            High = Highlight;
            this.Active = Active;
            DesiredSpace = Space;
            if (Availability != Availabilities.Enabled)
            {
                var provider = ProviderManager.Root.Get<InputProvider>();
                provider.Subscribe(this);
                provider.InputEvent += GlobalInput_UserInput;
            }
            Availability = Availabilities.Enabled;
            return this;
        }

        bool UpperCase = false;
        private void GlobalInput_UserInput(Glacier.Common.Provider.Input.InputProvider.InputEventArgs e)
        {
            if (e.MouseLeftClick)
            {
                if (IsMouseOver)
                    IsActive = true;
                else
                    IsActive = false;
            }
            if (IsActive && e.PressedKeys.Any() && !FLAG_ClearKeyboard)
            {
                var old = Text.Length;
                var changes = "";
                foreach (var k in e.PressedKeys)
                {
                    var letter = Enum.GetName(typeof(Keys), k);
                    if (letter.Where(x => char.IsNumber(x)).Any())
                        letter = new string(letter.Where(x => char.IsNumber(x)).ToArray());
                    switch (k)
                    {
                        case Keys.Back:
                        case Keys.Delete:
                            continue;
                        case Keys.RightShift:
                        case Keys.LeftShift:
                            continue;
                        case Keys.Left:
                        case Keys.Right:
                            continue;
                        case Keys.Up:
                            CursorPosition = Text.Length;
                            continue;
                        case Keys.Down:
                            CursorPosition = 0;
                            continue;
                        case Keys.Space:
                            letter = " ";
                            break;
                        case Keys.OemPeriod:
                            letter = ".";
                            break;
                        case Keys.Enter:
                            Accepted?.Invoke(this);
                            return;
                    }
                    if (UpperCase)
                        letter = letter.ToUpper();
                    else
                        letter = letter.ToLower();
                    Text = Text.Insert(CursorPosition, letter);
                    changes += letter;
                }
                var _new = Text.Length;
                CursorPosition += _new - old;
            }
        }

        public bool CursorVisible = true;
        public const float BLINK_INTERVAL = .7f;
        bool FLAG_ClearKeyboard;

        TimeSpan _timeSinceLastHold;
        TimeSpan _timeSinceBlinkChange;
        bool canHold;
        protected internal override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Availability == Availabilities.Disabled)
                return;
            var mouse = Mouse.GetState();
            var MouseRect = new Rectangle(mouse.Position, new Point(1, 1));
            var keyboard = Keyboard.GetState();
            if (canHold && IsActive && !FLAG_ClearKeyboard)
            {
                UpperCase = keyboard.CapsLock;
                foreach (var k in keyboard.GetPressedKeys())
                    switch (k)
                    {
                        case Keys.Back:
                            if (CursorPosition > 0)
                            {
                                Text = Text.Remove(CursorPosition - 1, 1);
                                CursorPosition--;
                            }
                            continue;
                        case Keys.Delete:
                            if (CursorPosition < TextLength)
                                Text = Text.Remove(CursorPosition, 1);
                            continue;
                        case Keys.RightShift:
                        case Keys.LeftShift:
                            UpperCase = !UpperCase;
                            continue;
                        case Keys.Left:
                            if (CursorPosition > 0)
                                CursorPosition--;
                            continue;
                        case Keys.Right:
                            if (CursorPosition < Text.Length)
                                CursorPosition++;
                            continue;
                    }
                canHold = false;
            }
            if (IsActive && !FLAG_ClearKeyboard)
            {
                if ((keyboard.IsKeyDown(Keys.V) &&
                    (keyboard.IsKeyDown(Keys.LeftControl) ||
                    keyboard.IsKeyDown(Keys.RightControl))) &&
                    keyboard.GetPressedKeys().Count() == 2) //Check if only Ctrl and V are pressed (Left or Right Ctrl)
                {
                   
#if false
                    string str = "";
                    var t = new System.Threading.Thread(new System.Threading.ThreadStart(() => { str = GetClipboardText(); }));
                    t.SetApartmentState(System.Threading.ApartmentState.STA);
                    t.Start();
                    while (t.IsAlive) { }
                    Text = Text.Insert(CursorPosition, str);
                    CursorPosition = Text.Length;
                    FLAG_ClearKeyboard = true;
#endif
                }
                TextLength = Text.Length;
                if (CursorPosition < 0)
                    CursorPosition = 0;
                if (CursorPosition > TextLength)
                    CursorPosition = TextLength;
            }
            if (!canHold)
            {
                _timeSinceLastHold += gameTime.ElapsedGameTime;
                if (_timeSinceLastHold.TotalSeconds > .07f)
                {
                    canHold = true;
                    _timeSinceLastHold = TimeSpan.Zero;
                }
            }
            _timeSinceBlinkChange += gameTime.ElapsedGameTime;
            if (_timeSinceBlinkChange.TotalSeconds > BLINK_INTERVAL && IsActive)
            {
                CursorVisible = !CursorVisible;
                _timeSinceBlinkChange = TimeSpan.Zero;
            }
            else if (!IsActive)
                CursorVisible = IsActive;
        }

#if false
        [STAThread]
        static string GetClipboardText()
        {
            if (System.Windows.Forms.Clipboard.ContainsText())
                return System.Windows.Forms.Clipboard.GetText();
            return "";
        }
#endif

        protected internal override void Repaint(SpriteBatch sprite)
        {
            Color DrawColor = Background;
            if (IsMouseOver && Mouse.GetState().LeftButton != ButtonState.Pressed) //Hover
                DrawColor = High;
            if (IsMouseOver && Mouse.GetState().LeftButton == ButtonState.Pressed) //Hover
                DrawColor = High;
            if (IsActive) //Active
                DrawColor = Active;
            sprite.Draw(GameResources.BaseTexture, ActualSpace, DrawColor);
            var cursorloc = Font.Measure(Text.Substring(0, CursorPosition)).ToPoint();
            var textSize = Font.Measure(Text);
            if (textSize.Y == 0)
                textSize.Y = Font.Measure("REMY").Y;
            Font.DrawString(sprite, Text,
                new Vector2(ActualSpace.X + (int)(ActualSpace.Height - textSize.Y),
                ActualSpace.Y + (ActualSpace.Height / 2) - (int)(textSize.Y / 2)),
                Foreground);
            if (CursorVisible)
                sprite.Draw(GameResources.BaseTexture, new Rectangle(ActualSpace.X + (ActualSpace.Height - (int)textSize.Y) + cursorloc.X,
                    ActualSpace.Y + (ActualSpace.Height / 2) - (int)(textSize.Y / 2), 2, (int)textSize.Y), Foreground); //Draw cursor
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
