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
    public enum BrixType { none,Orange, Gray,Steel }
	public partial class BrixControl : UserControl
	{
        public Vector2 position=Vector2.Zero;
        public BoundingBox brixBox;
        public BrixType _BrixType;

        public BrixControl(BrixType brixType)
		{
			// Required to initialize variables
			InitializeComponent();
            _BrixType=brixType;
            string sBrix = string.Empty;
            switch(_BrixType)
            {
                case BrixType.Orange:
                    sBrix = "sprites/Block1.png";
                    break;
                case BrixType.Gray:
                    sBrix = "sprites/Block2.png";
                    break;
                case BrixType.Steel:
                    sBrix = "sprites/Block1.png";
                    break;
            }
            imgBrix.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("pack://application:,,,/CrackOut;component/"+sBrix);
		}

        public void Draw()
        {
           // (CrackOut.App.Current.RootVisual as MainPage).Field.Children.Remove(this);
            Canvas.SetLeft(this, position.X);
            Canvas.SetTop(this, position.Y);
            (App.Current.MainWindow as MainWindow).Field.Children.Add(this);
        }

        public bool Hit()
        {
            bool Remove = false;
            GameManager._soundManager.Play("Pop3");
             string sBrix = string.Empty;
             switch (_BrixType)
             {
                 case BrixType.Gray:
                     _BrixType = BrixType.Orange;
                     sBrix = "sprites/Block1.png";
                     imgBrix.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("pack://application:,,,/CrackOut;component/" + sBrix);
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
            
            (App.Current.MainWindow as MainWindow).Field.Children.Remove(this);
        }
	}
}