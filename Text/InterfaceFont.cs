using Glacier.Common.Provider;
using Glacier.Common.Util;
using GUI2.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GUI2.Text
{
    /// <summary>
    /// Defines an Interface-wide statically-available font that can be accessed by style.
    /// </summary>
    public class InterfaceFont
    {
        /// <summary>
        /// The default size for text without a determined size.
        /// </summary>
        public float TextPtSize => 18;

        static SpriteFont RegFont;
        static SpriteFont BoldFont;

        /// <summary>
        /// The style to draw text with.
        /// </summary>
        public SpriteFont RenderFont
        {
            get
            {
                switch (Style)
                {
                    case FontWeight.Regular:
                        return RegFont;
                    case FontWeight.Bold:
                        return BoldFont;
                    default:
                        return RegFont;
                }
            }
        }
        /// <summary>
        /// The pt-size to render the text at.
        /// </summary>
        public float RenderSize { get; internal set; }
        float Scale
        {
            get => RenderSize / TextPtSize;
        }
        /// <summary>
        /// Renders the selected text with the current font settings.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public Vector2 Measure(string text)
        {
            if (text == cachedSizeText)
                return cachedSize;
            var size = (RenderFont.MeasureString(text ?? "") * Scale);
            cachedSize = size;
            cachedSizeText = text;
            return cachedSize;
        }
        private Vector2 cachedSize;
        private string cachedSizeText;
        public bool Underline
        {
            get;set;
        }

        /// <summary>
        /// Font-weight
        /// </summary>
        public enum FontWeight
        {
            Regular,
            Bold
        }
        /// <summary>
        /// The current fontweight to display the text with
        /// </summary>
        public FontWeight Style { get; internal set; }
        /// <summary>
        /// Loads fonts -- needs to be called before using fonts.
        /// </summary>
        public static void LoadFonts()
        {
            var provider = ProviderManager.Root.Get<ContentProvider>();
            RegFont = provider.GetContent<SpriteFont>("Text/Font_Regular");
            BoldFont = provider.GetContent<SpriteFont>("Text/Font_Bold");
        }

        /// <summary>
        /// Creates a font instance with the selected settings.
        /// </summary>
        /// <param name="Size"></param>
        /// <param name="Style"></param>
        public InterfaceFont(float Size = 12f, FontWeight Style = FontWeight.Regular)
        {
            this.Style = Style;
            RenderSize = Size;
        }

        /// <summary>
        /// Draws the text with the current settings.
        /// </summary>
        /// <param name="batch">SpriteBatch instance</param>
        /// <param name="text">The text to draw</param>
        /// <param name="Location">The location on screen of the text</param>
        /// <param name="color">The color of the text to draw</param>
        public void DrawString(SpriteBatch batch, string text, Vector2 Location, Color color)
        {
            if (text == null)
                text = "NULL";
            batch.DrawString(RenderFont, text, Location, color, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
            if (Underline)
            {
                batch.Draw(GameResources.BaseTexture, new Rectangle(Location.ToPoint() + new Point(0, (int)Measure(text).Y), new Point((int)Measure(text).X, 2)), color);
            }           
        }
    }
}
