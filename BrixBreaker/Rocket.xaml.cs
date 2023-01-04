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
	public partial class Rocket : UserControl
	{
        public Vector2 position;
        //public int Width = 90;
        //public int Height = 10;
		public Rocket()
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