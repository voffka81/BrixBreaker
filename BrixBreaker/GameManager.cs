using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Threading;
using WPF.Sound;

namespace CrackOut
{
    public class GameManager
    {
        public enum State { Menu, StartGame, InGame, GameOver };

        private State _gameState;
        public static SoundManager _soundManager;

        List<BrixControl> lstLevel = new List<BrixControl>();

        int NumOfRows = 10; //12
        int NumOfColumns = 15;//14

        DateTime lastTick;
        bool isPaused = false;
        int iScores = 0;
        bool bGameOver = false;
        int _lives = 3;

        public int Lives => _lives;

        public Canvas GameField { get; set; }

        public BallControl _ball;
        public Rocket _rocket;

        public GameManager()
        {
            GameField = new Canvas();

            _soundManager = new SoundManager();
            _soundManager.LoadSounds();

            _ball = new BallControl(GameField);
            _rocket = new Rocket(GameField);




            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += UpdateGame;
            timer.Start();
            _gameState = State.StartGame;
        }

        private void StartLevel()
        {
            String level;
            using (StreamReader sr = new StreamReader("levels/level1.lvl"))
            {
                level = sr.ReadToEnd();
                level = level.Replace("\r\n", string.Empty);
                level = level.Replace("\n", string.Empty);
            }
            // Init all blocks, set positions and bounding boxes
            for (int y = 0; y < NumOfRows; y++)
                for (int x = 0; x < NumOfColumns; x++)
                {
                    BrixType bt = (BrixType)Convert.ToInt32(level[x + (y) * (NumOfColumns)].ToString());

                    if (bt != BrixType.none)
                    {
                        BrixControl brix = new BrixControl(bt, GameField);
                        brix.position = new Vector2(
                        //LeftOffset + Widtg*x
                        25f + 48 * x,
                        //TopOffset + heigth*y
                        85f + 16 * y);

                        brix.brixBox = new BoundingBox(
                           brix.position,
                            new Vector2(48f, 16f));
                        lstLevel.Add(brix);
                        brix.Draw();
                    }
                } // for for (int)

        }

        void UpdateGame(object sender, EventArgs e)
        {
            switch (_gameState)
            {
                case State.StartGame:
                    _rocket.position = new Vector2(100, (float)GameField.ActualHeight - 50);
                    _ball.position = new Vector2((float)(_rocket.position.X + _rocket.Width / 2), (float)(GameField.ActualHeight - 140));
                    _rocket.Draw();
                    StartLevel();
                    _gameState = State.InGame;
                    break;
                case State.InGame:
                    DateTime now = DateTime.Now;
                    TimeSpan elapsed = now - lastTick;
                    lastTick = now;
                    CheckCollisions();

                    _ball.Height = 32;
                    _ball.Width = 32;

                    _ball.Draw();
                    break;

                default:
                    if (bGameOver)
                    {
                        _lives = 3;
                        bGameOver = false;
                        iScores = 0;
                        //UCBall.fballSpeed = 1.5f;
                        StartLevel();
                    }
                    break;
            }
        }

        private void CheckCollisions()
        {
            //check ball collisions
            bool isHit = false;
            float fNextPositionX = (float)(_ball.position.X) + _ball.fBallMovementX * _ball.fballSpeed;
            float fNextPositionY = (float)(_ball.position.Y) + _ball.fBallMovementY * _ball.fballSpeed;

            //with left and right wall
            if (fNextPositionX < 0 ||
                fNextPositionX + _ball.Width > GameField.ActualWidth)
            {
                _soundManager.Play("Pop2");
                _ball.fBallMovementX *= -1;
                isHit = true;
            }
            // Top
            if (fNextPositionY < 0)
            {
                _soundManager.Play("Pop2");
                _ball.fBallMovementY *= -1;
                isHit = true;
            }

            // Bottom 
            if (fNextPositionY + _ball.Height > GameField.ActualHeight)
            {
                _soundManager.PlaySync("Miss");

                _lives--;
                fNextPositionX = 300;
                fNextPositionY = (float)(GameField.ActualHeight / 2);
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
                    if (brix.Hit())
                        lstLevel.Remove(brix);
                }
            }
            #endregion
            if (lstLevel.Count == 0)
            {
                bGameOver = true;
            }
            if (!isHit)
                _ball.position = new Vector2(fNextPositionX, fNextPositionY);

            if (_lives < 0)
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

        public void UpdateMousePosition(double mousePosX)
        {
            if (_gameState == State.InGame)
            {
                if (mousePosX != _rocket.position.X)
                {
                    if (mousePosX > 0 && (mousePosX + (float)_rocket.Width) < GameField.ActualWidth)
                        _rocket.position.X = (float)mousePosX;

                    if (mousePosX < 0)
                        _rocket.position.X = 0;
                    if (mousePosX > GameField.ActualWidth)
                        _rocket.position.X = (float)(GameField.ActualWidth - _rocket.Width);

                    _rocket.Draw();
                }
            }
        }
    }


}
