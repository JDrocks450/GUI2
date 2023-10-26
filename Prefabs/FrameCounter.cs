using GUI2.Content;
using GUI2.Controls;
using GUI2.Layout;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GUI2.Prefabs
{
    /// <summary>
    /// Creates a GContainer that displays the current framerate of the game, along with other timing info.
    /// </summary>
    public class FrameRateDisplay : GContainer
    {
        public long TotalFrames { get; private set; }
        public double TotalSeconds { get; private set; }
        public double AverageFramesPerSecond { get; private set; }
        public double CurrentFramesPerSecond { get; private set; }
        private TimeSpan TotalGameTime;

        private const int MAXIMUM_SAMPLES = 100;

        private Queue<double> _sampleBuffer = new Queue<double>();

        /// <summary>
        /// Populates the <see cref="GContainer"/> with framerate labels on first call of <see cref="Update(GameTime)"/> and also <see cref="GUIComponent.Invalidate(GameTime)"/>
        /// </summary>
        /// <param name="gt"></param>
        protected internal override void Update(GameTime gameTime)
        {
            if (formatBuffer == null)
            {
                formatBuffer = new GLabel[]
                {
                        new GLabel($"FPS: {CurrentFramesPerSecond}", CurrentFramesPerSecond > 30 ? Color.Green : Color.Red)
                            { Font = new Text.InterfaceFont(12, Text.InterfaceFont.FontWeight.Bold) },
                        new GLabel($"Total Frames: {TotalFrames}", Color.White),
                        new GLabel($"Average FPS: {AverageFramesPerSecond}", Color.White),
                        new GLabel(string.Format("Game Time: {0:hh\\:mm\\:ss}", TotalGameTime), Color.White)
                };
                AddChild(formatBuffer);
            }
            base.Update(gameTime);
            if (gameTime == null)
                return;
            CurrentFramesPerSecond = 1.0d / gameTime.ElapsedGameTime.TotalSeconds;
            CurrentFramesPerSecond = Math.Truncate(CurrentFramesPerSecond);
            _sampleBuffer.Enqueue(CurrentFramesPerSecond);

            if (_sampleBuffer.Count > MAXIMUM_SAMPLES)
            {
                _sampleBuffer.Dequeue();
                AverageFramesPerSecond = _sampleBuffer.Average(i => i);
            }
            else
            {
                AverageFramesPerSecond = CurrentFramesPerSecond;
            }

            TotalFrames++;
            TotalGameTime = gameTime.TotalGameTime;
            TotalSeconds += gameTime.ElapsedGameTime.TotalSeconds;
        }

        GLabel[] formatBuffer;
        public FrameRateDisplay(Rectangle Space) : base(Space)
        {
            Background.Color= Color.Black * .5f;
            LayoutManager = new StackLayout();
        }

        public void Reformat()
        {            
            formatBuffer[0].SetText($"FPS: {CurrentFramesPerSecond}", CurrentFramesPerSecond > 30 ? Color.Green : Color.Red);
            formatBuffer[1].SetText($"Total Frames: {TotalFrames}", Color.White);
            formatBuffer[2].SetText($"Average FPS: {AverageFramesPerSecond}", Color.White);
            formatBuffer[3].SetText(string.Format("Game Time: {0:hh\\:mm\\:ss}", TotalGameTime), Color.White);
        }

        protected internal override void Repaint(SpriteBatch batch)
        {
            Reformat();
            base.Repaint(batch);
        }
    }
}

