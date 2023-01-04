using System.Windows.Controls;

namespace CrackOut
{
    public partial class BallControl : UserControl
    {
        public BoundingBox CollisionBox;
        public Vector2 Position = new Vector2(300, 200);
        public float MovementX = 1;
        public float MovementY = 1;
        public float Speed = 3.5f;
        public float Center = 8;

        private int _size = 16;
        private Canvas _canvas;
        public BallControl(Canvas canvas)
        {
            _canvas = canvas;
            // Required to initialize variables
            InitializeComponent();
            CalculateSize();
            _canvas.Children.Add(this);
        }

        private void CalculateSize()
        {
            Ball.Width = _size;
            Ball.Height = _size;
            Center = _size / 2;
            CollisionBox = new BoundingBox(Position,
               new Vector2((float)Ball.Width, (float)Ball.Width));
        }

        public void SetSize(int size)
        {
            _size = size;
            CalculateSize();
        }
        public void Draw()
        {
            Canvas.SetLeft(this, Position.X);
            Canvas.SetTop(this, Position.Y);
        }
    }
}