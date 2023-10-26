using Glacier.Common.Primitives;
using Glacier.Common.Provider;
using Glacier.Common.Util;
using GUI2.Extensions;
using GUI2.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GUI2.Controls
{
    public class GPieMenu : GChildrenComponent
    {
        private const double ANIM_SECONDS = .25;

        private GLabel ItemDescLabel;
        private GTexture[] Slices, ClockwiseSlices;
        private ValueRef<bool>[] ActivatedSlices, ClockwiseActivatedSlices;

        public delegate void OnSelectionHandler(GPieMenu sender, string name);
        public event OnSelectionHandler OnSelection;                

        public new Color Background { get; set; }
        public Color? High { get; set; }
        public Color? Click { get; set; }
        private int ContentSlices = 0, lastCell = 0;
        private bool Initialized = false;
        private Vector2 scale;
        private double animTimer;
        public bool Open
        {
            get; private set;
        }
        public bool HasSelection => SelectedIndex != -1;
        public int SelectedIndex
        {
            get; private set;
        }
        /// <summary>
        /// The distance from the center the mouse must be in order for a selection to be valid.
        /// This point is scaled based on the <see cref="DesiredPosition"/> attribute
        /// </summary>
        public float SelectionDistanceThreshold
        {
            get; set;
        } = 55;
        /// <summary>
        /// The max distance from the center the mouse can be to allow the user to select an option.
        /// This point is scaled based on the <see cref="DesiredPosition"/> attribute
        /// </summary>
        public float SelectionDistanceMax
        {
            get; set;
        } = 100;

        private Point Center => ActualPosition + ActualSize / new Point(2);

        private GTexture BackgroundTexture;

        /// <summary>
        /// Creates a PieMenu with the desired PieSlice textures, added in Clockwise order
        /// </summary>
        public GPieMenu(Color color, params string[] PieSliceTexKeys)
        {
            ContentMargin = new Thickness(0);
            int count = 0;
            Background = color;
            var textures = new GTexture[PieSliceTexKeys.Length];
            foreach (var str in PieSliceTexKeys) {
                textures[count] = GameResources.GetTexture(str).ToGTexture(color);
                textures[count].DesiredSize = new Point(50);
                count++;
            }
            Init(textures);
        }

        /// <summary>
        /// Creates a PieMenu with the desired PieSlice textures, added in Clockwise order
        /// </summary>
        /// <param name="PieSlices"></param>
        public GPieMenu(params GTexture[] PieSlices)
        {
            Init(PieSlices);
        }
        private void Init(params GTexture[] PieSlices)
        {
            if (Initialized)
                return;
            GameResources.InputProvider.PreviewInputEvent += InputProvider_InputEvent;
            Slices = new GTexture[8];
            ContentSlices = PieSlices.Length;
            if (ContentSlices < 8)
                return;
            ClockwiseSlices = PieSlices;
            ClockwiseActivatedSlices = new ValueRef<bool>[PieSlices.Length];
            ActivatedSlices = new ValueRef<bool>[Slices.Length];
            Slices[0] = PieSlices[0];
            ActivatedSlices[0] = ClockwiseActivatedSlices[0] = new ValueRef<bool>();
            Slices[1] = PieSlices[7];
            ActivatedSlices[1] = ClockwiseActivatedSlices[7] = new ValueRef<bool>();
            Slices[2] = PieSlices[1];
            ActivatedSlices[2] = ClockwiseActivatedSlices[1] = new ValueRef<bool>();
            Slices[3] = PieSlices[6];
            ActivatedSlices[3] = ClockwiseActivatedSlices[6] = new ValueRef<bool>();
            Slices[4] = PieSlices[2];
            ActivatedSlices[4] = ClockwiseActivatedSlices[2] = new ValueRef<bool>();
            Slices[5] = PieSlices[5];
            ActivatedSlices[5] = ClockwiseActivatedSlices[5] = new ValueRef<bool>();
            Slices[6] = PieSlices[3];
            ActivatedSlices[6] = ClockwiseActivatedSlices[3] = new ValueRef<bool>();
            Slices[7] = PieSlices[4];
            ActivatedSlices[7] = ClockwiseActivatedSlices[4] = new ValueRef<bool>();
            DesiredSize = new Point(325, 325);
            scale = DesiredSize.ToVector2() / new Point(200, 200).ToVector2();
            High = High ?? Color.Lerp(Background, Color.White, .5f);
            Click = Click ?? Color.Lerp(Background, Color.Black, .5f);            
            BackgroundTexture = new GTexture(GameResources.GetTexture("GUI/PieMenu/background"), Background * .55f, new Rectangle())
            {
                HorizontalAlignment = HorizontalAlignments.Stretch,
                VerticalAlignment = VerticalAlignments.Stretch,
                //Padding = new Primitives.Thickness((int)(30 * scale.X))
            };                        
            ContentMargin = new Primitives.Thickness(0);
            ItemDescLabel = new GLabel("Nothing Selected", Color.White);
            ItemDescLabel.HorizontalAlignment = HorizontalAlignments.Center;
            ItemDescLabel.VerticalAlignment = VerticalAlignments.Center;
            Initialized = true;
        }

        private void InputProvider_InputEvent(Glacier.Common.Provider.Input.InputProvider.InputEventArgs e)
        {
            if (e.MouseLeftClick && Open && SelectedIndex != -2)
            {
                Disappear();
                if (HasSelection)
                {
                    e.Handled = true;
                    OnSelection?.Invoke(this, ClockwiseSlices[SelectedIndex].Tag as string);
                }                
            }
        }

        public void Disappear()
        {
            Open = false;
            Availability = Availabilities.Invisible;
            OnDisappear();
        }

        public void Appear(IEnumerable<(string name, GTexture preview)> Data)
        {
            if (!Initialized) return;
            SelectedIndex = -2;
            foreach (var val in ActivatedSlices)
                val.Value = false;
            Children.Clear();
            //AddChild(BackgroundTexture);
            for(int index = 0; index < 8; index++) {                
                var slice = Slices[index];
                slice.IgnoreParentBoundary = true;                                        
                slice.HorizontalAlignment = HorizontalAlignments.Stretch;
                slice.VerticalAlignment = VerticalAlignments.Stretch;
                slice.Availability = Availabilities.Invisible;
                AddChild(slice);
                if (index < Data.Count())
                {
                    var tuple = Data.ElementAt(index);
                    Rectangle? safezone = null;
                    var content = ProviderManager.GetRoot().Get<ContentProvider>();
                    safezone = content.GetTextureSafezone(slice.RenderTexture);
                    tuple.preview.DesiredSize = new Point(50);
                    tuple.preview.DesiredPosition = ((safezone.Value.Center.ToVector2() * scale) -
                        (tuple.preview.DesiredSize.ToVector2() / new Vector2(2))).ToPoint();
                    AddChild(tuple.preview);
                    slice.Tag = tuple.name;
                    ActivatedSlices[index].Value = true;
                    slice.BlendingColor = Background;
                }
                else slice.BlendingColor = Background * .25f;
            }
            AddChild(ItemDescLabel);
            animTimer = 0;
        }
        protected internal override void Update(GameTime gt)
        {
            base.Update(gt);
            updateMouseSelection(gt);
            Anim_Open(gt);
            if (SelectedIndex == -2)
                SelectedIndex = -1;
        }

        private void Anim_Open(GameTime gt)
        {            
            if (animTimer < ANIM_SECONDS)
            {
                animTimer += gt.ElapsedGameTime.TotalSeconds;   
                var percent = (animTimer / ANIM_SECONDS) * 1;
                if (percent > 1)
                {
                    percent = 1;
                    Open = true;
                }
                var step = 1 / 7.0;
                var cell = (int)(percent / step);
                if (ClockwiseSlices[cell].Availability != Availabilities.Enabled)
                    ClockwiseSlices[cell].Availability = Availabilities.Enabled;
            }
        }

        protected virtual void OnDisappear()
        {
            ;
        }

        private void updateMouseSelection(GameTime gt)
        {
            void cell_select(int cellIndex)
            {
                GTexture slice = ClockwiseSlices[cellIndex];
                slice.BlendingColor = High.Value;
                slice.Padding = new Thickness(-10);
                SelectedIndex = cellIndex;
                ItemDescLabel.Text = slice.Tag as string ?? "ERROR";
            }
            void cell_deselect(int cellIndex)
            {
                ClockwiseSlices[cellIndex].BlendingColor = Background;
                ClockwiseSlices[cellIndex].Padding = new Thickness(0);
                SelectedIndex = -1;
                ItemDescLabel.Text = "Cancel";
            }
            if (!Initialized) return;
            if (Availability == Availabilities.Enabled)
            {
                var mousePos = Mouse.GetState().Position;
                var mouseDiff = mousePos - Center;
                var distance = mouseDiff.ToVector2().Length();
                if (distance < (SelectionDistanceThreshold * scale.Y)
                    || distance > (SelectionDistanceMax * scale.Y))
                {
                    if (ClockwiseActivatedSlices[lastCell].Value)
                        cell_deselect(lastCell);
                    return;
                }
                float angle = MathHelper.ToDegrees((float)Math.Atan2(mouseDiff.Y, mouseDiff.X));
                angle += (90 + 45) - (float)(360.0 / 8);
                if (angle > 360)
                    angle -= 360;
                else if (angle < 0)
                    angle += 360;
                var cell = (int)(angle / (360.0 / 8));
                if (cell != lastCell && ClockwiseActivatedSlices[lastCell].Value)
                    cell_deselect(lastCell);
                if (ClockwiseActivatedSlices[cell].Value)
                {
                    cell_select(cell);
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                        ClockwiseSlices[cell].BlendingColor = Click.Value;
                }
                lastCell = cell;
            }
        }
    }
}
