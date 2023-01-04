using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CrackOut
{
	public partial class BallControl : UserControl
	{
        public Vector2 position=new Vector2(300, 200);
        public float fBallMovementX = 1;
        public float fBallMovementY = 1;
        public float fballSpeed=3.5f;
        public float fBallCenter = 16;
		public BallControl()
		{
			// Required to initialize variables
			InitializeComponent();
		}

        public void Draw()
        {
            (App.Current.MainWindow as MainWindow).Field.Children.Remove(this);
            Canvas.SetLeft(this, position.X);
            Canvas.SetTop(this, position.Y);
            (App.Current.MainWindow as MainWindow).Field.Children.Add(this);
        }
	}
}