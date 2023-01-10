using System.Windows.Controls;

namespace CrackOut
{
    public partial class Rocket : UserControl
    {
        public Vector2 position;
        public BoundingBox Boundings;
        private Canvas _canvas;

        public Rocket(Canvas canvas)
        {
            _canvas = canvas;
            // Required to initialize variables
            InitializeComponent();
            _canvas.Children.Add(this);
            Boundings = new BoundingBox(Vector2.Zero, new Vector2((float)Width, (float)Height));
        }

        public void Draw()
        {
            Canvas.SetLeft(this, position.X);
            Canvas.SetTop(this, position.Y);
            Boundings.Update(position);
        }
    }
}