using System.Windows.Controls;

namespace CrackOut
{
    public partial class Rocket : UserControl
    {
        public Vector2 position;
        private Canvas _canvas;

        public Rocket(Canvas canvas)
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