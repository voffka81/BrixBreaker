using System;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace CrackOut
{
    public partial class PrizeBox : UserControl
    {
        public Vector2 position = Vector2.Zero;
        public BoundingBox boundingBox;
        private Canvas _canvas;

        public PrizeBox(Canvas canvas, Vector2 initialPosition)
        {
            _canvas = canvas;
            position = initialPosition;

            // Required to initialize variables
            InitializeComponent();

            boundingBox = new BoundingBox(position, new Vector2((float)32, (float)32));

            Canvas.SetLeft(this, position.X);
            Canvas.SetTop(this, position.Y);
            _canvas.Children.Add(this);
        }

        public void Draw()
        {
            Canvas.SetLeft(this, position.X);
            Canvas.SetTop(this, position.Y);

            position.Y++;
            boundingBox.Update(position);
        }

        public bool Hit()
        {
            return true;
        }
        public void Erase()
        {
            Storyboard BrixFadeOut = this.TryFindResource("BoxHide") as Storyboard;
            BrixFadeOut.Begin();
            BrixFadeOut.Completed += new EventHandler(FadeOut_Completed);

        }

        void FadeOut_Completed(object sender, EventArgs e)
        {
            _canvas.Children.Remove(this);
        }
    }
}