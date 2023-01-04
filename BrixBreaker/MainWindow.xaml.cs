using System.Windows;
using System.Windows.Input;

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

            (DataContext as GameManager).UpdateMousePosition(MousePosX);

        }
    }
}
