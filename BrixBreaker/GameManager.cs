using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using WPF.Sound;

namespace CrackOut
{
    class GameManager
    {
        public static SoundManager _soundManager;

        List<BrixControl> lstLevel = new List<BrixControl>();

        int NumOfRows = 10; //12
        int NumOfColumns = 15;//14

        DateTime lastTick;
        bool isPaused = false;
        int iScores = 0;
        bool bGameOver = false;
        int iLives = 3;

        public static BallControl _ball;
        public static Rocket _rocket;

        public GameManager()
        {
            _soundManager = new SoundManager();
            _soundManager.LoadSounds();

            _ball = new BallControl();
            _rocket = new Rocket();

            _rocket.position = new Vector2(100, (float)(App.Current.MainWindow as MainWindow).Height - 100);
            _ball.position = new Vector2((float)(_rocket.position.X + _rocket.Width / 2), (float)(App.Current.MainWindow as MainWindow).Height - 140);
            _rocket.Draw();
            
            StartLevel();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private void StartLevel()
        {
            String level;
            using (StreamReader sr = new StreamReader("levels/level1.lvl"))
            {
                level = sr.ReadToEnd();
                level = level.Replace("\r\n", string.Empty);
            }
            // Init all blocks, set positions and bounding boxes
            for (int y = 0; y < NumOfRows; y++)
                for (int x = 0; x < NumOfColumns; x++)
                {
                    BrixType bt = (BrixType)Convert.ToInt32(level[x + (y) * (NumOfColumns)].ToString());
                    
                    if (bt != BrixType.none)
                    {
                        BrixControl brix = new BrixControl(bt);
                        brix.position = new Vector2(
                            //LeftOffset + Widtg*x
                        25f     + 48 * x,
                            //TopOffset + heigth*y
                        85f + 16 * y);

                        brix.brixBox = new BoundingBox(
                           brix.position,
                            new Vector2(48f, 16f));
                        lstLevel.Add(brix);
                        brix.Draw();
                    }
                } // for for (int)

            //_ball.Draw();
          
            //CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            TimeSpan elapsed = now - lastTick;
            lastTick = now;
            CheckCollisions();

            _ball.Height = 32;
            _ball.Width = 32;

            _ball.Draw();

            if (bGameOver)
            {
                iLives = 3;
                bGameOver = false;
                iScores = 0;
                //UCBall.fballSpeed = 1.5f;
                StartLevel();
            }
        }

        private void CheckCollisions()
        {
            //check ball collisions
            bool isHit=false;
            float fNextPositionX = (float)(_ball.position.X) + _ball.fBallMovementX * _ball.fballSpeed;
            float fNextPositionY = (float)(_ball.position.Y) + _ball.fBallMovementY * _ball.fballSpeed;

            //with left and right wall
            if (fNextPositionX < (App.Current.MainWindow as MainWindow).LBorder.Width ||
                fNextPositionX + _ball.Width >Canvas.GetLeft((App.Current.MainWindow as MainWindow).RBorder))
            {
                _soundManager.Play("Pop2");
                _ball.fBallMovementX *= -1;
                isHit = true;
            }
            // Top
            if (fNextPositionY < Canvas.GetTop((App.Current.MainWindow as MainWindow).UBorder) + (App.Current.MainWindow as MainWindow).UBorder.Height)
            {
                _soundManager.Play("Pop2");
                _ball.fBallMovementY *= -1;
                isHit = true;
            }

            // Bottom 
            if (fNextPositionY + _ball.Height > (App.Current.MainWindow as MainWindow).Height)
            {
                _soundManager.PlaySync("Miss");

                iLives--;
                fNextPositionX = 300;
                fNextPositionY = (float)(App.Current.MainWindow as MainWindow).Height / 2;
            }
            // with paddle
            BoundingBox ballBox = new BoundingBox(new Vector2(fNextPositionX, fNextPositionY),
                new Vector2((float)_ball.Width, (float)_ball.Height));

            BoundingBox paddleBox = new BoundingBox(new Vector2(_rocket.position.X, _rocket.position.Y),
                new Vector2((float)_rocket.Width, (float)_rocket.Height));
            //txtScores.Text =paddleBox.min.ToString() + " " + paddleBox.max.ToString(); ;
            if (ballBox.Intersects(paddleBox))
            {
                isHit = true;
                _soundManager.Play("Pop1");
                float x = (paddleBox.max.X - (ballBox.min.X + (float)_ball.Width / 2)) / (float)_rocket.Width;
                double theta = Lerp((float)Math.PI / 4, (float)(Math.PI - Math.PI / 4), x);

                _ball.fBallMovementX = (float)Math.Cos(theta);
                _ball.fBallMovementY = -(float)Math.Sin(theta);

                //UCBall.fBallMovementX += (fNextPositionX - (UCRocket.position.X-45)) / (45 * 3);
                //UCBall.fBallMovementY = -1;// -ballSpeedVector.Y;
                //// Move away from the paddle
                //fNextPositionY -= UCBall.fBallMovementY*UCBall.fballSpeed;
                //// Normalize vector

            }

            #region Collision with brix
            for (int icount = 0; icount < lstLevel.Count; icount++)
            {
                BrixControl brix = lstLevel[icount];

                if (brix.brixBox.Intersects(ballBox))
                {
                    isHit = true;
                    //Right
                    if (Math.Abs((brix.brixBox.max.X - ballBox.min.X)) < _ball.fBallCenter)
                        _ball.fBallMovementX *= -1;
                    else if (Math.Abs(brix.brixBox.min.X - ballBox.max.X) <
                                _ball.fBallCenter)
                        _ball.fBallMovementX *= -1;
                    //Bottom
                    else if (Math.Abs(brix.brixBox.max.Y - ballBox.min.Y) <
                            _ball.fBallCenter)
                        _ball.fBallMovementY *= -1;
                    //Top
                    else if (Math.Abs(brix.brixBox.min.Y - ballBox.max.Y) <
                                _ball.fBallCenter)
                    {
                        _ball.fBallMovementY *= -1;
                    }
                    iScores += 1;
                    if(brix.Hit())
                        lstLevel.Remove(brix);
                }
            }
            #endregion
            if (lstLevel.Count == 0)
            {
                bGameOver = true;
            }
            if(!isHit)
            _ball.position = new Vector2(fNextPositionX, fNextPositionY);

            if (iLives < 0)
                bGameOver = true;
        }

        private float Lerp(float value1, float value2, float amount)
        {
            if (value1 == value2)
            {
                return value1;
            }
            return ((1 - amount) * value1) + (amount * value2);
        }

    }


}
