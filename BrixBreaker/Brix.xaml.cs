using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CrackOut
{
    public enum BrixType { none, Orange, Gray, Green }
    public partial class BrixControl : UserControl
    {
        public Vector2 position = Vector2.Zero;
        public BoundingBox brixBox;
        public BrixType Type;
        private Canvas _canvas;

        public BrixControl(BrixType brixType, Canvas canvas, float width, float heigth)
        {
            _canvas = canvas;
            // Required to initialize variables
            InitializeComponent();

            this.Width = width;
            this.Height = heigth;

            SetType(brixType);
        }

        public void SetType(BrixType brixType)
        {
            Type = brixType;
            string sBrix = string.Empty;
            switch (Type)
            {
                case BrixType.Orange:
                    sBrix = "Orange";
                    break;
                case BrixType.Gray:
                    sBrix = "Gray";
                    break;
                case BrixType.Green:
                    sBrix = "Green";
                    break;
            }
            brixSprite.Fill = Application.Current.MainWindow.Resources[sBrix] as LinearGradientBrush;
        }

        public void Draw()
        {
            Canvas.SetLeft(this, position.X);
            Canvas.SetTop(this, position.Y);
            _canvas.Children.Add(this);
        }

        public void Erase()
        {
            Storyboard BrixFadeOut = this.TryFindResource("Hide") as Storyboard;
            BrixFadeOut.Begin();
            BrixFadeOut.Completed += new EventHandler(FadeOut_Completed);

        }

        void FadeOut_Completed(object sender, EventArgs e)
        {
            _canvas.Children.Remove(this);
        }
    }
}