using System.Windows.Controls;

namespace CrackOut
{
    public partial class BallControl : UserControl
    {
        public Vector2 position = new Vector2(300, 200);
        public float fBallMovementX = 1;
        public float fBallMovementY = 1;
        public float fballSpeed = 3.5f;
        public float fBallCenter = 16;
        private Canvas _canvas;
        public BallControl(Canvas canvas)
        {
            _canvas = canvas;
            // Required to initialize variables
            InitializeComponent();
            _canvas.Children.Add(this);
        }

        public void Draw()
        {
            Canvas.SetLeft(this, position.X);
            Canvas.SetTop(this, position.Y);
        }
    }
}