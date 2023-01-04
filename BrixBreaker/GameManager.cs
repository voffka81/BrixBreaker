using PropertyChanged;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Threading;
using WPF.Sound;

namespace CrackOut
{
    [AddINotifyPropertyChangedInterface]
    public class GameManager
    {
        public enum State { Menu, StartGame, InGame, GameOver };

        private State _gameState;
        public static SoundManager _soundManager;

        List<BrixControl> lstLevel = new List<BrixControl>();

        int NumOfRows = 10; //12
        int NumOfColumns = 15;//14

        DateTime lastTick;

        bool bGameOver = false;


        public int Lives { get; set; }
        public int Scores { get; set; } = 0;
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
                    _ball.Position = new Vector2((float)(_rocket.position.X + _rocket.Width / 2), (float)(GameField.ActualHeight - 140));
                    _rocket.Draw();
                    StartLevel();
                    _gameState = State.InGame;
                    Lives = 3;
                    break;
                case State.InGame:
                    DateTime now = DateTime.Now;
                    TimeSpan elapsed = now - lastTick;
                    lastTick = now;
                    CheckCollisions();

                    _ball.Draw();
                    break;

                default:
                    if (bGameOver)
                    {
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
            float fNextPositionX = (float)(_ball.Position.X) + _ball.MovementX * _ball.Speed;
            float fNextPositionY = (float)(_ball.Position.Y) + _ball.MovementY * _ball.Speed;

            //with left and right wall
            if (fNextPositionX < 0 ||
                fNextPositionX + _ball.ActualWidth > GameField.ActualWidth)
            {
                _soundManager.Play("Pop2");
                _ball.MovementX *= -1;
                isHit = true;
            }
            // Top
            if (fNextPositionY < 0)
            {
                _soundManager.Play("Pop2");
                _ball.MovementY *= -1;
                isHit = true;
            }

            // Bottom 
            if (fNextPositionY + _ball.ActualHeight > GameField.ActualHeight)
            {
                _soundManager.PlaySync("Miss");

                Lives--;
                fNextPositionX = 300;
                fNextPositionY = (float)(GameField.ActualHeight / 2);
            }
            // with paddle
            _ball.CollisionBox.Update(new Vector2(fNextPositionX, fNextPositionY));

            BoundingBox paddleBox = new BoundingBox(new Vector2(_rocket.position.X, _rocket.position.Y),
                new Vector2((float)_rocket.ActualWidth, (float)_rocket.ActualHeight));

            if (_ball.CollisionBox.Intersects(paddleBox))
            {
                isHit = true;
                _soundManager.Play("Pop1");
                float x = (paddleBox.Max.X - (_ball.CollisionBox.Min.X + (float)_ball.ActualWidth / 2)) / (float)_rocket.Width;
                double theta = Lerp((float)Math.PI / 4, (float)(Math.PI - Math.PI / 4), x);

                _ball.MovementX = (float)Math.Cos(theta);
                _ball.MovementY = -(float)Math.Sin(theta);
            }

            #region Collision with brix
            for (int icount = 0; icount < lstLevel.Count; icount++)
            {
                BrixControl brix = lstLevel[icount];

                if (brix.brixBox.Intersects(_ball.CollisionBox))
                {
                    isHit = true;
                    //Right
                    if (Math.Abs((brix.brixBox.Max.X - _ball.CollisionBox.Min.X)) < _ball.Center)
                        _ball.MovementX *= -1;
                    else if (Math.Abs(brix.brixBox.Min.X - _ball.CollisionBox.Max.X) <
                                _ball.Center)
                        _ball.MovementX *= -1;
                    //Bottom
                    else if (Math.Abs(brix.brixBox.Max.Y - _ball.CollisionBox.Min.Y) <
                            _ball.Center)
                        _ball.MovementY *= -1;
                    //Top
                    else if (Math.Abs(brix.brixBox.Min.Y - _ball.CollisionBox.Max.Y) <
                                _ball.Center)
                    {
                        _ball.MovementY *= -1;
                    }
                    Scores += 1;
                    switch (brix.Hit())
                    {
                        case BrixType.Green:
                            fNextPositionY += 16;
                            _ball.SetSize(32);
                            break;
                        case BrixType.Orange:
                            lstLevel.Remove(brix);
                            break;
                    }

                }
            }
            #endregion
            if (lstLevel.Count == 0)
            {
                bGameOver = true;
            }
            if (!isHit)
                _ball.Position = new Vector2(fNextPositionX, fNextPositionY);

            if (Lives < 0)
                _gameState = State.GameOver;
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
