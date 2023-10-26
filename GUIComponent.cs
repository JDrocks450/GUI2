using Glacier.Common.Util;
using GUI2.Accents;
using GUI2.Content;
using GUI2.Layout;
using GUI2.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI2
{
    public abstract class GChildComponent : GUIComponent
    {
        private GUIComponent _child;
        private bool _autoSize;

        public bool AutosizeToChild
        {
            get
            {
                if (!_autoSize)
                    return HorizontalAlignment != HorizontalAlignments.Stretch && VerticalAlignment != VerticalAlignments.Stretch;
                return _autoSize;
            }
            set
            {
                _autoSize = value;
            }
        }

        public GUIComponent Child
        {
            get => _child;
            set { _child = value; _child.Parent = this; }
        }

        public HorizontalAlignments HorizontalContentAlignment
        {
            get; set;
        } = HorizontalAlignments.Stretch;
        public VerticalAlignments VerticalContentAlignment
        {
            get; set;
        } = VerticalAlignments.Stretch;

        protected internal override void Update(GameTime gt)
        {
            if (Child != null)
            {
                Child.HorizontalAlignment = HorizontalContentAlignment;
                Child.VerticalAlignment = VerticalContentAlignment;
                if (AutosizeToChild)
                {
                    if (!Child.AutoSizeToParent)
                    {
                        //throw new Exception("AutoSizeToParent is set for the child, so this control cannot be set to AutoSizeToChild.");
                        if (DesiredPosition.X < Child.ActualSize.X && DesiredPosition.Y < Child.ActualSize.Y)
                            DesiredSpace = new Rectangle(DesiredSpace.Location, Child.DesiredSpace.Size + new Point(ContentMargin.Left + ContentMargin.Right, ContentMargin.Top + ContentMargin.Bottom));
                    }
                }
            }
            base.Update(gt);
            Child?.Refresh(gt);
        }
        protected internal override void Repaint(SpriteBatch batch)
        {
            Child?._Draw(batch);                                            
        }
    }
    public abstract class GChildrenComponent : GUIComponent
    {
        /// <summary>
        /// The children in this <see cref="GChildrenComponent"/>
        /// </summary>
        public GObservableCollection<GUIComponent> Children
        {
            get; private set;
        } = new GObservableCollection<GUIComponent>();

        /// <summary>
        /// A rectangle that represents the area occupied by all of this <see cref="GChildrenComponent"/> children.
        /// </summary>
        public Rectangle ContentBoundary
        {
            get;
            private set;
        }

        public Layout.ILayoutManager LayoutManager
        {
            get;
            set;
        }
        public bool HasLayout
        {
            get => LayoutManager != null;
        }
        public bool AutoSizeToChildren
        {
            get;set;
        }
        
        public BackgroundAccent Background
        {
            get => Accents.OfType<BackgroundAccent>().FirstOrDefault();
            set => AddAccent(value);
        }

        public void SetLayout(ILayoutManager manager)
        {
            LayoutManager = manager;
        }

        public GChildrenComponent()
        {
            Children.CollectionUpdated += Children_CollectionUpdated;
            Children.CollectionCleared += Children_CollectionCleared;
        }

        private void Children_CollectionCleared(GUIComponent[] Objects)
        {
            foreach (var obj in Objects)
                obj.Parent = null;
        }

        private void Children_CollectionUpdated(GUIComponent Object, GObservableCollection<GUIComponent>.EventType type)
        {
            switch (type)
            {
                case GObservableCollection<GUIComponent>.EventType.Add:
                case GObservableCollection<GUIComponent>.EventType.Insert:
                    Object.Parent = this;
                    Object.OnDestroy += ChildDestroyed;
                    break;
                case GObservableCollection<GUIComponent>.EventType.Remove:
                    Object.Parent = null;
                    Object.OnDestroy -= ChildDestroyed;
                    break;
            }
        }

        private void ChildDestroyed(object sender, EventArgs e)
        {
            var send = (GUIComponent)sender;
            if (send.Parent == this)
                Children.Remove(send);
        }

        public void AddChild(params GUIComponent[] Children)
        {
            foreach (var child in Children)
            {
                this.Children.Add(child);
            }  
        }
        public void RemoveChild(GUIComponent Child)
        {
            Children.Remove(Child);
        }

        public override void Destroy()
        {
            var childCache = Children.ToList();
            foreach (var child in childCache)
                child.Destroy();
            base.Destroy();
        }

        protected internal override void Repaint(SpriteBatch batch)
        {
            foreach (var child in Children)
                if (child.Availability != Availabilities.Invisible)
                    child._Draw(batch);
            if (Debug_DrawDebugInfo == this)            
                batch.Draw(GameResources.BaseTexture, ContentBoundary, Color.Blue * .25f);            
        }

        protected internal override void Update(GameTime gt)
        {
            if (Children.Any())
            {
                if (HasLayout)
                {
                    LayoutManager.Safezone = ContentSpace;
                    LayoutManager.OnLayoutRequested(Children.OfType<Layout.ILayoutItem>());
                }
                var contentBoundary = Children.First().ActualSpace;
                foreach (var child in Children)
                {
                    child.Update(gt);
                    if (child.ActualSpace.X < contentBoundary.X)
                        contentBoundary.X = child.ActualSpace.X;
                    if (child.ActualSpace.Right > contentBoundary.Right)
                        contentBoundary.Width = child.ActualSpace.Right - ActualSpace.X;
                    if (child.ActualSpace.Y < contentBoundary.Y)
                        contentBoundary.Y = (child.ActualSpace.Y);
                    if (child.ActualSpace.Bottom > contentBoundary.Bottom)
                        contentBoundary.Height = (child.ActualSpace.Bottom - ActualSpace.Y) + child.Padding.Bottom;
                }
                ContentBoundary = new Rectangle(contentBoundary.Location, contentBoundary.Size);
            }
            base.Update(gt);
            if (AutoSizeToChildren)
                ActualSpace = new Rectangle(ActualSpace.Location
                    - new Point(ContentMargin.Left, ContentMargin.Top), ContentBoundary.Size
                    + new Point(ContentMargin.Right, ContentMargin.Bottom));
            foreach (var child in Children)
            {
                child.Update(new GameTime(gt.TotalGameTime, TimeSpan.FromSeconds(0)));
            }
        }        
    }

    public enum HorizontalAlignments
    {
        /// <summary>
        /// The <see cref="GUIComponent"/> will be aligned to the left side of the parent ContentSpace.
        /// </summary>
        Left,
        /// <summary>
        /// The <see cref="GUIComponent"/> will be aligned to the center of the parent ContentSpace.
        /// </summary>
        Center,
        /// <summary>
        /// The <see cref="GUIComponent"/> will be aligned to the right side of the parent ContentSpace.
        /// </summary>
        Right,
        Stretch
    }
    public enum VerticalAlignments
    {
        /// <summary>
        /// The <see cref="GUIComponent"/> will be aligned to the top of the parent ContentSpace.
        /// </summary>
        Top,
        /// <summary>
        /// The <see cref="GUIComponent"/> will be aligned to the center of the parent ContentSpace.
        /// </summary>
        Center,
        /// <summary>
        /// The <see cref="GUIComponent"/> will be aligned to the bottom of the parent ContentSpace.
        /// </summary>
        Bottom,
        Stretch
    }
    public abstract class GUIComponent : ILayoutItem
    {
        public event EventHandler OnDestroy;

        public bool Destroyed
        {
            get; protected set;
        }

        public string Name
        {
            get; set;
        }

        public Accents.IAccent[] Accents
        {
            get => _accents.ToArray();
        }

        public void AddAccent(IAccent accent)
        {
            accent.Attachment = this;
            _accents.Add(accent);
        }
        public void RemoveAccent(IAccent accent)
        {
            _accents.Remove(accent);
        }
        public bool IgnoreParentBoundary
        {
            get; set;
        } = false;

        /// <summary>
        /// An object associated with this component
        /// </summary>
        public object Tag
        {
            get;set;
        }

        /// <summary>
        /// This component's parent if it has one
        /// </summary>
        public GUIComponent Parent
        {
            get;
            protected internal set;
        }
        /// <summary>
        /// The position you want this <see cref="GUIComponent"/> to be, within it's Parent component
        /// </summary>
        public Point DesiredPosition
        {
            get;set;
        }
        /// <summary>
        /// The size you want this <see cref="GUIComponent"/> to fill
        /// </summary>
        public Point DesiredSize
        {
            get;set;
        }
        /// <summary>
        /// Dictates whether the <see cref="DesiredSpace"/> is different than the <see cref="ActualSpace"/>
        /// </summary>
        public bool DesiredSpaceDirty
        {
            get => DesiredSpace != ActualSpace;
        }
        /// <summary>
        /// This component's boundary
        /// </summary>
        public Rectangle DesiredSpace
        {
            get => new Rectangle(DesiredPosition, DesiredSize);
            set
            {
                DesiredPosition = value.Location;
                DesiredSize = value.Size;
            }
        }
        /// <summary>
        /// Defines this component's actual space on the screen. This is how the framework decides where to draw components.
        /// </summary>
        public Rectangle ActualSpace        
        {
            get => new Rectangle(ActualPosition, ActualSize);
            set
            {
                ActualPosition = value.Location;
                ActualSize = value.Size;
            }
        }
        /// <summary>
        /// The position you want this <see cref="GUIComponent"/> to be, within it's Parent component
        /// </summary>
        public Point ActualPosition
        {
            get => _actualPosition;
            protected set
            {
                _actualPosition = value;
            }
        }
        /// <summary>
        /// The size you want this <see cref="GUIComponent"/> to fill
        /// </summary>
        public Point ActualSize
        {
            get => _actualSize;// + new Point(Padding.Left + Padding.Right, Padding.Top + Padding.Bottom);
            protected set
            {
                _actualSize = value;
            }
        }
        /// <summary>
        /// The space between the outer edge of the component and its ContentSpace
        /// </summary>
        public virtual Primitives.Thickness ContentMargin
        {
            get;
            set;
        } = new Primitives.Thickness(0);        

        /// <summary>
        /// This component's content boundary - where content is allowed to be.
        /// </summary>
        public Rectangle ContentSpace
        {
            get => new Rectangle(ActualSpace.Location + ContentMargin.ToPoint(),
                ActualSpace.Size - new Point(ContentMargin.Left + ContentMargin.Right, ContentMargin.Top + ContentMargin.Bottom));
        }

        public enum Availabilities
        {
            /// <summary>
            /// This component can be interacted with
            /// </summary>
            Enabled,
            /// <summary>
            /// This component cannot be interacted with
            /// </summary>
            Disabled,
            /// <summary>
            /// This component will call <see cref="Update(GameTime)"/> but it will not be drawn
            /// </summary>
            Invisible
        }

        public HorizontalAlignments HorizontalAlignment
        {
            get;set;
        }
        public VerticalAlignments VerticalAlignment
        {
            get;set;
        }

        /// <summary>
        /// The current <see cref="Availabilities"/> of this component.
        /// </summary>
        public Availabilities Availability
        {
            get; set;
        }
        /// <summary>
        /// This component will actively try to fill the parent's content space - even if there are other components there.
        /// </summary>
        public bool AutoSizeToParent
        {
            get => HorizontalAlignment == HorizontalAlignments.Stretch && VerticalAlignment == VerticalAlignments.Stretch;
            set
            {
                if (value)
                {
                    HorizontalAlignment = HorizontalAlignments.Stretch;
                    VerticalAlignment = VerticalAlignments.Stretch;
                }
            }
        }

        /// <summary>
        /// Padding for <see cref="ILayoutManager"/> to use if there is one set for the parent
        /// </summary>
        public Thickness Padding
        {
            get;
            set;
        }
        public bool LayoutIgnore { get; set; }
        protected static GUIComponent Debug_DrawDebugInfo
        {
            get; private set;
        }

        private List<IAccent> _accents = new List<IAccent>();
        private Point _actualPosition;
        private Point _actualSize;

        /// <summary>
        /// Calls a forced update to this component.
        /// </summary>
        /// <param name="gt"></param>
        public virtual void Invalidate(GameTime gt = default)
        {
            Update(gt);
        }

        /// <summary>
        /// Forces this component to refresh itself, calling it's Update void immediately
        /// </summary>
        /// <param name="time"></param>
        public void Refresh(GameTime time)
        {
            Update(time);
        }

        /// <summary>
        /// THIS IS WHERE COMPONENTS HAVE THEIR <see cref="DesiredSpace"/> APPLIED TO THEIR <see cref="ActualSpace"/> ATTRIBUTE -
        /// setting <see cref="DesiredSpace"/> after this is called will have the changes applied on the next frame.
        /// </summary>
        /// <param name="gt"></param>
        protected internal virtual void Update(GameTime gt)
        {
            ActualSpace = DesiredSpace;
            if (Parent != null)
            {
                var contentSpace = Parent.ContentSpace;
                ActualSpace = new Rectangle(contentSpace.Location + DesiredSpace.Location, ActualSpace.Size);
                if (ActualSpace.Size.X > contentSpace.Width)
                    ActualSpace = new Rectangle(ActualSpace.Location, new Point(contentSpace.Width, ActualSpace.Height));
                if (ActualSpace.Size.Y > contentSpace.Height)
                    ActualSpace = new Rectangle(ActualSpace.Location, new Point(ActualSpace.Width, contentSpace.Height));
                switch (HorizontalAlignment)
                {
                    case HorizontalAlignments.Left:
                        ActualSpace = new Rectangle(
                            new Point(DesiredSpace.X + contentSpace.X, ActualSpace.Y), ActualSpace.Size);
                        break;
                    case HorizontalAlignments.Center:
                        ActualSpace = new Rectangle(
                            new Point(DesiredSpace.X + contentSpace.X + (contentSpace.Width / 2) - (ActualSpace.Width / 2),
                                      ActualSpace.Y),
                            ActualSpace.Size);
                        break;
                    case HorizontalAlignments.Right:
                        ActualSpace = new Rectangle(
                            new Point(DesiredSpace.X + contentSpace.X + (contentSpace.Width - ActualSize.X), ActualSpace.Y), ActualSpace.Size);
                        break;
                    case HorizontalAlignments.Stretch:
                        ActualSpace = new Rectangle(contentSpace.Location.X + Padding.Left + DesiredSpace.X, ActualSpace.Y, contentSpace.Width - (Padding.Left + Padding.Right + DesiredSpace.X), ActualSize.Y);
                        break;
                }
                switch (VerticalAlignment)
                {
                    case VerticalAlignments.Center:
                        ActualSpace = new Rectangle(
                            new Point(ActualSpace.X,
                            DesiredSpace.Y + contentSpace.Y + (contentSpace.Height / 2) - (ActualSpace.Height / 2)),
                            ActualSpace.Size);
                        break;
                    case VerticalAlignments.Bottom:
                        ActualSpace = new Rectangle(
                            new Point(ActualSpace.X,
                            DesiredSpace.Y + contentSpace.Y + (contentSpace.Height) - (ActualSpace.Height)),
                            ActualSpace.Size);
                        break;
                    case VerticalAlignments.Stretch:
                        ActualSpace = new Rectangle(ActualSpace.X, contentSpace.Y + Padding.Top + DesiredSpace.Y, ActualSize.X, contentSpace.Height - (Padding.Top + Padding.Bottom + DesiredSpace.Y));
                        break;
                }
                if (!IgnoreParentBoundary)
                {
                    if (ActualSpace.X < contentSpace.X)
                        ActualSpace = new Rectangle(new Point(contentSpace.X, ActualSpace.Y), ActualSpace.Size);
                    if (ActualSpace.Y < contentSpace.Y)
                        ActualSpace = new Rectangle(new Point(ActualSpace.X, contentSpace.Y), ActualSpace.Size);
                }
            }
            if (GameResources.Debug_GUIDebuggingActivated)
            {
                var mouseRect = new Rectangle(Mouse.GetState().Position, new Point(1));
                if (mouseRect.Intersects(ActualSpace))                
                    Debug_DrawDebugInfo = this;                
            }
        }

        internal void _Draw(SpriteBatch batch)
        {
            foreach (var accent in _accents.Where(x => x.DrawMode == AccentDrawMode.PreDraw))
                accent.DrawAccent(batch);
            Repaint(batch);
            foreach (var accent in _accents.Where(x => x.DrawMode == AccentDrawMode.PostDraw))
                accent.DrawAccent(batch);
            if (Debug_DrawDebugInfo == this)
            {
                batch.Draw(GameResources.BaseTexture, ActualSpace, Color.Black * .15f);
                batch.Draw(GameResources.BaseTexture, ContentSpace, Color.Orange * .15f);
            }
        }

        protected internal abstract void Repaint(SpriteBatch batch);

        public virtual void Destroy()
        {
            OnDestroy?.Invoke(this, new EventArgs());
            Destroyed = true;
        }

        public void OnLayout(LayoutEventArgs args)
        {
            DesiredSpace = args.NewSpace;
        }
    }
}
