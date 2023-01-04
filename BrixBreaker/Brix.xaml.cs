using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CrackOut
{
    public enum BrixType { none, Orange, Gray, Steel }
    public partial class BrixControl : UserControl
    {
        public Vector2 position = Vector2.Zero;
        public BoundingBox brixBox;
        public BrixType _BrixType;
        private Canvas _canvas;

        public BrixControl(BrixType brixType, Canvas canvas)
        {
            _canvas = canvas;
            // Required to initialize variables
            InitializeComponent();

            _BrixType = brixType;
            string sBrix = string.Empty;
            switch (_BrixType)
            {
                case BrixType.Orange:
                    sBrix = "Orange";
                    break;
                case BrixType.Gray:
                    sBrix = "Gray";
                    break;
                case BrixType.Steel:
                    sBrix = "sprites/Block1.png";
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

        public bool Hit()
        {
            bool Remove = false;
            GameManager._soundManager.Play("Pop3");
            switch (_BrixType)
            {
                case BrixType.Gray:
                    _BrixType = BrixType.Orange;
                    brixSprite.Fill = Resources["Orange"] as LinearGradientBrush;
                    break;

                case BrixType.Steel:
                    break;
                case BrixType.Orange:
                    Erase();
                    Remove = true;
                    break;
            }


            return Remove;
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