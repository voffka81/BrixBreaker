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
            this.MouseMove += new MouseEventHandler(MainPage_MouseMove);

            (DataContext as GameManager).UpdateScreenSize(Width, Height);

        }

        void MainPage_MouseMove(object sender, MouseEventArgs e)
        {
            double MousePosX = e.GetPosition(Field).X;

            (DataContext as GameManager).UpdateMousePosition(MousePosX);

        }
    }
}
