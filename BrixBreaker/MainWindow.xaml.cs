using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WPF.Sound;

namespace CrackOut
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GameManager gm = new GameManager();
            this.Cursor = Cursors.None;
            this.Loaded += new RoutedEventHandler(Page_Loaded);
        }

        void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
            this.KeyDown += new KeyEventHandler(Page_KeyDown);
            this.MouseMove += new MouseEventHandler(MainPage_MouseMove);

        }

        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                //case Key.P:
                //    if (this.isPaused)
                //        this.isPaused = false;
                //    else
                //        this.isPaused = true;
                //    break;
                //case Key.Right:
                //        paddlePosition += moveFactorPerSecond;
                //    break;

                //case Key.Left:
                //        paddlePosition -= moveFactorPerSecond;
                //        break;

                   
            }

            // Make sure paddle stay between 0 and 1 (offset 0.05f for paddle width)
            //if (paddlePosition < 0.14f)
            //    paddlePosition = 0.14f;
            //if (paddlePosition > 1 - 0.14f)
            //    paddlePosition = 1 - 0.14f;
		
            //UCRocket.position.X=paddlePosition *640;
            //UCRocket.position.Y=0.95f*480;
            //UCRocket.Draw();
        }

       
        void MainPage_MouseMove(object sender, MouseEventArgs e)
        {
            double MousePosX = e.GetPosition(Field).X;
            if (MousePosX != GameManager._rocket.position.X)
            {
                //txtScores.Text = MousePosX.ToString();
                //Check rocket collisions with walls

                if (MousePosX > LBorder.Width && (MousePosX + (float)GameManager._rocket.Width) < Canvas.GetLeft(RBorder))
                    GameManager._rocket.position.X = (float)MousePosX;

                if (MousePosX < 0)
                    GameManager._rocket.position.X = (float)LBorder.Width;
                if (MousePosX > Canvas.GetLeft(RBorder))
                    GameManager._rocket.position.X = (float)Canvas.GetLeft(RBorder) - (float)GameManager._rocket.Width;

                GameManager._rocket.Draw();
            }
            
        }
    }
}
