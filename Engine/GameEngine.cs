using Space_Invaders_Game_WPF_MOO_ICT.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Space_Invaders_Game_WPF_MOO_ICT.Engine
{
    public class GameEngine
    {
        private readonly Canvas MyCanvas;
        private readonly List<Player> Players;
        private readonly Label enemiesLeftLabel;

        public bool GameOver { get; private set; }

        private readonly List<Rectangle> itemsToRemove = new List<Rectangle>();

        private int enemyImages = 0;
        private int bulletTimer = 0;
        private int bulletTimerLimit = 90;
        private int totalEnemies = 0;
        private int enemySpeed = 6;


        public GameEngine(Canvas canvas, List<Player> players, Label enemiesLeftLabel)
        {
            MyCanvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
            Players = players ?? throw new ArgumentNullException(nameof(players));
            this.enemiesLeftLabel = enemiesLeftLabel ?? throw new ArgumentNullException(nameof(enemiesLeftLabel));
        }

        public void Initialize()
        {
            CreateEnemies(30);
            GameOver = false;
        }

        public void Update()
        {
            if (GameOver)
            {
                return;
            }

            UpdateUi();
            HandlePlayersMovement();

            bulletTimer -= 3;

            if (bulletTimer < 0)
            {
                SpawnEnemyBullet();
                bulletTimer = bulletTimerLimit;
            }

            ProcessPlayerBullets();
            ProcessEnemies();
            ProcessEnemyBullets();
            CleanupRemovedItems();

            if (totalEnemies < 10)
            {
                enemySpeed = 12;
            }

            if (totalEnemies < 1)
            {
                ShowGameOver("You Win, you saved the world!");
            }
        }

        public void OnKeyDown(Key key)
        {
            foreach (var player in Players)
            {
                Dictionary<GameActionsEnum, Action> actions = new Dictionary<GameActionsEnum, Action>
                {
                    { GameActionsEnum.MoveLeft, () => player.SetGoLeft(true) },
                    { GameActionsEnum.MoveRight, () => player.SetGoRight(true) },
                };

                if(player.KeyBinding.TryGetValue(key, out GameActionsEnum actionToPerform))
                {
                    actions.GetValueOrDefault(actionToPerform)?.Invoke();
                }
            }
        }

        public void OnKeyUp(Key key)
        {
            foreach (var player in Players)
            {
                Dictionary<GameActionsEnum, Action> actions = new Dictionary<GameActionsEnum, Action>
                {
                    { GameActionsEnum.MoveLeft, () => player.SetGoLeft(false) },
                    { GameActionsEnum.MoveRight, () => player.SetGoRight(false) },
                    { GameActionsEnum.Shoot, () => FirePlayerBullet(player) },
                };

                if(player.KeyBinding.TryGetValue(key, out GameActionsEnum actionToPerform))
                {
                    actions.GetValueOrDefault(actionToPerform)?.Invoke();
                }
            }
        }

        public void Restart()
        {
            MyCanvas.Children.Clear();
            MyCanvas.Children.Add(enemiesLeftLabel);

            foreach (var player in Players) { MyCanvas.Children.Add(player.Rectangle); }


            itemsToRemove.Clear();
            enemyImages = 0;
            bulletTimer = 0;
            enemySpeed = 6;

            ResetPlayersPosition();
            Initialize();
        }

        private void UpdateUi()
        {
            enemiesLeftLabel.Content = "Enemies Left: " + totalEnemies;
        }

        private void HandlePlayersMovement()
        {

            foreach (var player in Players)
            {
                if (player.GoLeft && player.GetX() > 0)
                {
                    player.SetPosition(player.GetX() - 10);
                }

                if (player.GoRight && player.GetX() + 80 < Application.Current.MainWindow.Width)
                {
                    player.SetPosition(player.GetX() + 10);
                }   
            }
        }

        private void FirePlayerBullet(Player player)
        {
            var newBullet = new Rectangle
            {
                Tag = "bullet",
                Height = 20,
                Width = 5,
                Fill = Brushes.White,
                Stroke = player.BulletCollor,
            };

            Canvas.SetTop(newBullet, player.GetY() - newBullet.Height);
            Canvas.SetLeft(newBullet, player.GetX() + player.Rectangle.Width / 2);
            MyCanvas.Children.Add(newBullet);
        }

        private void ProcessPlayerBullets()
        {
            foreach (var x in MyCanvas.Children.OfType<Rectangle>().ToList())
            {
                if ((string)x.Tag == "bullet")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) - 20);

                    if (Canvas.GetTop(x) < 10)
                    {
                        itemsToRemove.Add(x);
                    }

                    var bullet = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    foreach (var y in MyCanvas.Children.OfType<Rectangle>().ToList())
                    {
                        if ((string)y.Tag == "enemy")
                        {
                            var enemyHit = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);

                            if (bullet.IntersectsWith(enemyHit))
                            {
                                itemsToRemove.Add(x);
                                itemsToRemove.Add(y);
                                totalEnemies -= 1;
                            }
                        }
                    }
                }
            }
        }

        private void ProcessEnemies()
        {
            foreach (var entity in MyCanvas.Children.OfType<Rectangle>().ToList())
            {
                if ((string)entity.Tag == "enemy")
                {
                    Canvas.SetLeft(entity, Canvas.GetLeft(entity) + enemySpeed);

                    if (Canvas.GetLeft(entity) > 820)
                    {
                        Canvas.SetLeft(entity, -80);
                    }
                }
            }
        }

        private void ProcessEnemyBullets()
        {
            var playersHitBoxes = Players.Select(p => p.GetHitBox());

            foreach (var bullet in MyCanvas.Children.OfType<Rectangle>().ToList())
            {
                if ((string)bullet.Tag == "enemyBullet")
                {
                    Canvas.SetTop(bullet, Canvas.GetTop(bullet) + 10);

                    if (Canvas.GetTop(bullet) > 480)
                    {
                        itemsToRemove.Add(bullet);
                    }

                    var enemyBulletHitBox = new Rect(Canvas.GetLeft(bullet), Canvas.GetTop(bullet), bullet.Width, bullet.Height);

                    if (playersHitBoxes.Any(phb => phb.IntersectsWith(enemyBulletHitBox)))
                    {
                        //ShowGameOver("You were killed by the invader bullet!!");
                    }
                }
            }
        }

        private void CleanupRemovedItems()
        {
            foreach (var i in itemsToRemove.ToList())
            {
                MyCanvas.Children.Remove(i);
                itemsToRemove.Remove(i);
            }
        }

        private void SpawnEnemyBullet()
        {
            foreach (var player in Players)
            {
                double x = player.GetX() + 20;
                double y = 10;

                var enemyBullet = new Rectangle
                {
                    Tag = "enemyBullet",
                    Height = 30,
                    Width = 5,
                    Fill = Brushes.Red,
                };

                Canvas.SetTop(enemyBullet, y);
                Canvas.SetLeft(enemyBullet, x);
                MyCanvas.Children.Add(enemyBullet);
            }
        }

        public void CreateEnemies(int limit)
        {
            int left = 0;
            int[] topPositions = { 30, 150, 270 };
            Random random = new Random();
            totalEnemies = limit;

            for (int i = 0; i < limit; i++)
            {
                var enemySkin = new ImageBrush();
                var newEnemy = new Rectangle
                {
                    Tag = "enemy",
                    Height = 45,
                    Width = 45,
                    Fill = enemySkin
                };

                Canvas.SetTop(newEnemy, topPositions[random.Next(topPositions.Length)]);
                Canvas.SetLeft(newEnemy, left);
                MyCanvas.Children.Add(newEnemy);

                left -= random.Next(100, 240);
                enemyImages++;

                if (enemyImages > 8)
                {
                    enemyImages = 1;
                }

                enemySkin.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/images/invader{enemyImages}.gif"));
            }
        }

        private void ResetPlayersPosition()
        {
            for (int i = 0; i < Players.Count; i++)
            {
                Player player = Players[i];

                double x = MyCanvas.Width * (i + 1) / (Players.Count * 2);

                player.SetPosition(x - player.Rectangle.Width / 2, 394);
            }
        }

        private void ShowGameOver(string msg)
        {
            GameOver = true;
            enemiesLeftLabel.Content += " " + msg + " Press Enter to play again";
        }
    }
}
