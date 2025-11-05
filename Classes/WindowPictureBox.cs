namespace DesktopDance.Utility
{
    public class WindowPictureBox : PictureBox
    {
        private CharacterEntity? _controledPicture;
        private Point _pictureStartLocation;
        private Point _startLocation;
        private System.Windows.Forms.Timer _animationTimer;
        private const int TargetFPS = 60; // 60 кадров в секунду

        public WindowPictureBox(Rectangle screenBounds)
        {
            MouseDown += PictureMouseDown!;
            MouseUp += PictureMouseUp!;
            MouseMove += PictureMouseMove!;
            Bounds = screenBounds;
            
            // Настраиваем таймер для контролируемой анимации
            _animationTimer = new System.Windows.Forms.Timer();
            _animationTimer.Interval = 1000 / TargetFPS; // ~33ms для 30 FPS
            _animationTimer.Tick += (s, e) => Invalidate();
            _animationTimer.Start();
        }

        private void PictureMouseDown(object sender, MouseEventArgs args)
        {
            if (CharacterManager.IsLocked)
                return;

            foreach (var character in CharacterManager.Characters)
                if (character.Bounds.Contains(args.Location))
                {
                    _controledPicture = character;
                    _pictureStartLocation = character.Location;
                    _startLocation = args.Location;
                    return;
                }
        }

        private void PictureMouseUp(object sender, MouseEventArgs args)
        {
            if (CharacterManager.IsLocked)
                return;

            _controledPicture = null;
        }

        private void PictureMouseMove(object sender, MouseEventArgs args)
        {
            if (CharacterManager.IsLocked)
                return;

            if (_controledPicture is not null)
            {
                _controledPicture.Left = args.X + _pictureStartLocation.X - _startLocation.X;
                _controledPicture.Top = args.Y + _pictureStartLocation!.Y - _startLocation.Y;
            }
        }

        protected override void OnPaint(PaintEventArgs args)
        {
            // Включаем сглаживание для лучшего качества
            args.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
            args.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            
            ImageAnimator.UpdateFrames();
            
            foreach (var character in CharacterManager.Characters)
            {
                if (character.IsFlipped)
                {
                    // Отзеркаливание по горизонтали
                    var state = args.Graphics.Save();
                    args.Graphics.TranslateTransform(character.Location.X + character.Size.Width, character.Location.Y);
                    args.Graphics.ScaleTransform(-1, 1);
                    args.Graphics.DrawImage(
                        character.AnimatedImage,
                        0, 0,
                        character.Size.Width,
                        character.Size.Height
                    );
                    args.Graphics.Restore(state);
                }
                else
                {
                    args.Graphics.DrawImage(
                        character.AnimatedImage, 
                        character.Location.X, 
                        character.Location.Y,
                        character.Size.Width,
                        character.Size.Height
                    );
                }
            }
            // УБРАЛИ Invalidate() отсюда! Теперь используется таймер
        }

    }
}
