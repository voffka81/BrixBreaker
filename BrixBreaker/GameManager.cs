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
        public enum State { Menu, InitGame, StartCountdown, InGame, LostLive, GameOver };
        private TimeSpan _gameElapsed = TimeSpan.Zero;
        private State _gameState;
        public static SoundManager _soundManager;

        public int CountDown { get; set; }
        public bool Overlay { get; set; } = true;
        private DateTime _countDownreferenceTime;
        private List<BrixControl> lstLevel = new List<BrixControl>();
        private List<PrizeBox> _prizeBoxes = new List<PrizeBox>();

        private int _numOfRows = 10;
        private int _numOfColumns = 15;

        private float _brixXSize;
        private float _brixYSize;
        private float _ballSize;

        private DateTime _lastTick = DateTime.Now;

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
            _gameState = State.InitGame;
        }

        public void UpdateScreenSize(double screenWidth, double screenHeight)
        {
            _brixXSize = (float)(screenWidth / (_numOfColumns + 2));
            _brixYSize = _ballSize = _brixXSize / 3;
        }

        private void RenderLevel()
        {
            string level;
            using (StreamReader sr = new StreamReader("levels/level1.lvl"))
            {
                level = sr.ReadToEnd();
                level = level.Replace("\r\n", string.Empty);
                level = level.Replace("\n", string.Empty);
            }
            // Init all blocks, set positions and bounding boxes
            for (int y = 0; y < _numOfRows; y++)
                for (int x = 0; x < _numOfColumns; x++)
                {
                    BrixType bt = (BrixType)Convert.ToInt32(level[x + (y) * (_numOfColumns)].ToString());

                    if (bt != BrixType.none)
                    {
                        BrixControl brix = new BrixControl(bt, GameField, _brixXSize, _brixYSize);
                        brix.position = new Vector2(
                        //LeftOffset + Widtg*x
                         _brixXSize * x,
                        //TopOffset + heigth*y
                         _brixYSize * y);

                        brix.brixBox = new BoundingBox(
                           brix.position,
                            new Vector2(_brixXSize, _brixYSize));
                        lstLevel.Add(brix);
                        brix.Draw();
                    }
                } // for for (int)

        }

        void UpdateGame(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            TimeSpan elapsed = now - _lastTick;
            _gameElapsed += elapsed;
            _lastTick = now;

            switch (_gameState)
            {
                case State.InitGame:
                    Overlay = true;
                    _rocket.position = new Vector2(100, (float)GameField.ActualHeight - 50);
                    _ball.Position = new Vector2((float)(_rocket.position.X + _rocket.Width / 2), (float)(_rocket.position.Y - _rocket.Height));
                    _rocket.Draw();
                    _ball.SetSize(_ballSize);
                    _ball.Draw();
                    RenderLevel();
                    _countDownreferenceTime = DateTime.Now.AddSeconds(4);
                    _gameState = State.StartCountdown;
                    break;

                case State.StartCountdown:
                    CountDown = (_countDownreferenceTime - DateTime.Now).Seconds;
                    if (DateTime.Now > _countDownreferenceTime)
                    {
                        Overlay = false;
                        _gameState = State.InGame;
                        Lives = 3;
                    }
                    break;
                case State.InGame:
                    if (lstLevel.Count == 0)
                        _gameState = State.GameOver;
                    CheckCollisions();

                    foreach (var item in _prizeBoxes.ToArray())
                    {
                        item.Draw();
                        if (item.boundingBox.Intersects(_rocket.Boundings))
                        {
                            item.Erase();
                            _prizeBoxes.Remove(item);
                            _ball.SetSize(_ballSize * 2);
                        }
                        if (item.position.Y - item.boundingBox.Max.Y > GameField.ActualHeight)
                        {
                            item.Erase();
                            _prizeBoxes.Remove(item);
                        }
                    }

                    _ball.Draw();
                    break;

                case State.LostLive:
                    if (Lives == 0)
                        _gameState = State.GameOver;
                    Lives -= 1;
                    _prizeBoxes.ForEach(x => x.Erase());
                    _prizeBoxes.Clear();
                    _ball.SetSize(_ballSize);
                    _ball.Position = new Vector2((float)(_rocket.position.X + _rocket.Width / 2), (float)(_rocket.position.Y - _rocket.Height));
                    _gameState = State.InGame;
                    break;

                case State.GameOver:
                    _gameState = State.InitGame;
                    break;
                default: break;
            }
        }

        private void CheckCollisions()
        {
            //check ball collisions
            bool isHit = false;
            float fNextPositionX = (_ball.Position.X) + _ball.MovementX * _ball.Speed;
            float fNextPositionY = (_ball.Position.Y) + _ball.MovementY * _ball.Speed;

            //with left and right wall
            if (fNextPositionX < 0 ||
                fNextPositionX + _ball.ActualWidth > GameField.ActualWidth)
            {
                _soundManager.Play("Pop2");
                _ball.MovementX *= -1;
                isHit = true;
            }
            // Top
            else if (fNextPositionY < 0)
            {
                _soundManager.Play("Pop2");
                _ball.MovementY *= -1;
                isHit = true;
            }

            // Bottom 
            else if (fNextPositionY + _ball.ActualHeight > GameField.ActualHeight)
            {
                _soundManager.PlaySync("Miss");

                _gameState = State.LostLive;
                return;
            }
            // with paddle
            _ball.CollisionBox.Update(new Vector2(fNextPositionX, fNextPositionY));

            if (_ball.CollisionBox.Intersects(_rocket.Boundings))
            {
                isHit = true;
                _soundManager.Play("Pop1");
                float x = (_rocket.Boundings.Max.X - (_ball.CollisionBox.Min.X + (float)_ball.ActualWidth / 2)) / (float)_rocket.Width;
                double theta = Lerp((float)Math.PI / 4, (float)(Math.PI - Math.PI / 4), x);

                _ball.MovementX = (float)Math.Cos(theta);
                _ball.MovementY = -(float)Math.Sin(theta);
            }

            #region Collision with brix
            for (int icount = 0; icount < lstLevel.Count; icount++)
            {
                BrixControl brix = lstLevel[icount];

                if (brix.brixBox.Intersects(_ball.CollisionBox))
                    isHit = HitBrix(ref fNextPositionY, brix);
            }
            #endregion
            if (!isHit)
                _ball.Position = new Vector2(fNextPositionX, fNextPositionY);

            if (Lives < 0)
                _gameState = State.GameOver;
        }

        private bool HitBrix(ref float fNextPositionY, BrixControl brix)
        {
            bool isHit;
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


                switch (brix.Type)
                {
                    case BrixType.Gray:
                        brix.SetType(BrixType.Orange);
                        break;
                    case BrixType.Green:
                        _prizeBoxes.Add(new PrizeBox(GameField, brix.position));
                        brix.Erase();
                        lstLevel.Remove(brix);
                        break;
                    case BrixType.Orange:
                        brix.Erase();
                        lstLevel.Remove(brix);
                        break;
                }

            }

            return isHit;
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
