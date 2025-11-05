namespace DesktopKonata.Utility
{
    public class CharacterEntity : IDisposable
    {
        private bool disposed = false;
        private Rectangle _bounds = new();
        private float _scale = 1.0f;
        private Size _originalSize;
        private bool _isFlipped = false;

        public Rectangle Bounds
        { 
            get => _bounds; 
            set => _bounds = value; 
        }
        public Size Size
        {
            get => _bounds.Size;
            set => _bounds.Size = value;
        }
        public Point Location
        {
            get => _bounds.Location;
            set => _bounds.Location = value;
        }
        public int Left
        {
            get => _bounds.Left;
            set => _bounds.X = value;
        }
        public int Top
        {
            get => _bounds.Top;
            set => _bounds.Y = value;
        }

        public float Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                UpdateSize();
            }
        }

        public bool IsFlipped
        {
            get => _isFlipped;
            set => _isFlipped = value;
        }

        public string Name { get; set; } = "Character";

        public Bitmap AnimatedImage { get; init; }

        public CharacterEntity(Bitmap characterBitmap, Rectangle screenBounds, string name = "Character")
        {
            Name = name;
            _originalSize = characterBitmap.Size;
            Size = characterBitmap.Size;
            AnimatedImage = characterBitmap;
            Left = Random.Shared.Next(0, screenBounds.Width - Size.Width);
            Top = Random.Shared.Next(0, screenBounds.Height - Size.Height);
            ImageAnimator.Animate(AnimatedImage, new EventHandler(OnFrameChanged!));
        }

        private void UpdateSize()
        {
            int newWidth = (int)(_originalSize.Width * _scale);
            int newHeight = (int)(_originalSize.Height * _scale);
            Size = new Size(newWidth, newHeight);
        }

        private void OnFrameChanged(object sender, EventArgs args)
        {

        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    ImageAnimator.StopAnimate(AnimatedImage, new EventHandler(OnFrameChanged!));
                    AnimatedImage.Dispose();
                }
                disposed = true;
            }
        }

        ~CharacterEntity()
        {
            Dispose(disposing: false);
        }
    }
}
