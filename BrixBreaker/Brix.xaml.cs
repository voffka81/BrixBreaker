using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CrackOut
{
    public enum BrixType { none, Orange, Gray, Green }
    public partial class BrixControl : UserControl
    {
        public Vector2 position = Vector2.Zero;
        public BoundingBox brixBox;
        private BrixType _brixType;
        private Canvas _canvas;

        public BrixControl(BrixType brixType, Canvas canvas, float width, float heigth)
        {
            _canvas = canvas;
            // Required to initialize variables
            InitializeComponent();

            this.Width = width;
            this.Height = heigth;

            _brixType = brixType;
            string sBrix = string.Empty;
            switch (_brixType)
            {
                case BrixType.Orange:
                    sBrix = "Orange";
                    break;
                case BrixType.Gray:
                    sBrix = "Gray";
                    break;
                case BrixType.Green:
                    sBrix = "Green";
                    break;
            }
            brixSprite.Fill = Resources[sBrix] as LinearGradientBrush;
        }

        public void Draw()
        {
            Canvas.SetLeft(this, position.X);
            Canvas.SetTop(this, position.Y);
            _canvas.Children.Add(this);
        }

        public BrixType Hit()
        {
            GameManager._soundManager.Play("Pop3");
            var actualType = _brixType;
            switch (_brixType)
            {
                case BrixType.Gray:
                    _brixType = BrixType.Orange;
                    brixSprite.Fill = Resources["Orange"] as LinearGradientBrush;
                    break;

                case BrixType.Green:
                    break;
                case BrixType.Orange:
                    Erase();
                    break;
            }

            return actualType;
        }

        public void Erase()
        {
            Storyboard BrixFadeOut = this.TryFindResource("Hide") as Storyboard;
            BrixFadeOut.Begin();
            BrixFadeOut.Completed += new EventHandler(BrixFadeOut_Completed);

        }

        void BrixFadeOut_Completed(object sender, EventArgs e)
        {
            _canvas.Children.Remove(this);
        }
    }
}