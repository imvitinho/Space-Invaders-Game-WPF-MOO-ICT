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
        private readonly Canvas Canvas;
        private readonly Player Player;
        private readonly Label enemiesLeftLabel;
        private Dictionary<Key, GameActionsEnum> KeyBinding;

        public bool GameOver { get; private set; }

        private readonly List<Rectangle> itemsToRemove = new List<Rectangle>();

        private int enemyImages = 0;
        private int bulletTimer = 0;
        private int bulletTimerLimit = 90;
        private int totalEnemies = 0;
        private int enemySpeed = 6;


        public GameEngine(Canvas canvas, Player player, Label enemiesLeftLabel, Dictionary<Key, GameActionsEnum> keyBinding)
        {
            Canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
            Player = player ?? throw new ArgumentNullException(nameof(player));
            this.enemiesLeftLabel = enemiesLeftLabel ?? throw new ArgumentNullException(nameof(enemiesLeftLabel));
            KeyBinding = keyBinding ?? throw new ArgumentNullException(nameof(keyBinding));
        }

        public void Initialize()
        {
            ResetPlayerPosition();
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
            HandlePlayerMovement();

            bulletTimer -= 3;

            if (bulletTimer < 0)
            {
                SpawnEnemyBullet(Player.GetX() + 20, 10);
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
            Dictionary<GameActionsEnum, Action> actions = new Dictionary<GameActionsEnum, Action>
            {
                { GameActionsEnum.MoveLeft, () => Player.SetGoLeft(true) },
                { GameActionsEnum.MoveRight, () => Player.SetGoRight(true) },
            };

            if(KeyBinding.TryGetValue(key, out GameActionsEnum actionToPerform))
            {
                actions.GetValueOrDefault(actionToPerform)?.Invoke();
            }
        }

        public void OnKeyUp(Key key)
        {
            Dictionary<GameActionsEnum, Action> actions = new Dictionary<GameActionsEnum, Action>
            {
                { GameActionsEnum.MoveLeft, () => Player.SetGoLeft(false) },
                { GameActionsEnum.MoveRight, () => Player.SetGoRight(false) },
                { GameActionsEnum.Shoot, FirePlayerBullet }
            };

            if(KeyBinding.TryGetValue(key, out GameActionsEnum actionToPerform))
            {
                actions.GetValueOrDefault(actionToPerform)?.Invoke();
            }
        }

        public void Restart()
        {
            Canvas.Children.Clear();
            Canvas.Children.Add(enemiesLeftLabel);
            Canvas.Children.Add(Player.Rectangle);

            itemsToRemove.Clear();
            enemyImages = 0;
            bulletTimer = 0;
            enemySpeed = 6;

            Initialize();
        }

        private void UpdateUi()
        {
            enemiesLeftLabel.Content = "Enemies Left: " + totalEnemies;
        }

        private void HandlePlayerMovement()
        {

            if (Player.GoLeft && Player.GetX() > 0)
            {
                Player.SetPosition(Player.GetX() - 10);
            }

            if (Player.GoRight && Player.GetX() + 80 < Application.Current.MainWindow.Width)
            {
                Player.SetPosition(Player.GetX() + 10);
            }
        }

        private void FirePlayerBullet()
        {
            var newBullet = new Rectangle
            {
                Tag = "bullet",
                Height = 20,
                Width = 5,
                Fill = Brushes.White,
                Stroke = Brushes.Red
            };

            Canvas.SetTop(newBullet, Player.GetY() - newBullet.Height);
            Canvas.SetLeft(newBullet, Player.GetX() + Player.Rectangle.Width / 2);
            Canvas.Children.Add(newBullet);
        }

        private void ProcessPlayerBullets()
        {
            foreach (var x in Canvas.Children.OfType<Rectangle>().ToList())
            {
                if ((string)x.Tag == "bullet")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) - 20);

                    if (Canvas.GetTop(x) < 10)
                    {
                        itemsToRemove.Add(x);
                    }

                    var bullet = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    foreach (var y in Canvas.Children.OfType<Rectangle>().ToList())
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
            var playerHitBox = Player.GetHitBox();

            foreach (var entity in Canvas.Children.OfType<Rectangle>().ToList())
            {
                if ((string)entity.Tag == "enemy")
                {
                    Canvas.SetLeft(entity, Canvas.GetLeft(entity) + enemySpeed);

                    if (Canvas.GetLeft(entity) > 820)
                    {
                        Canvas.SetLeft(entity, -80);
                    }

                    var enemyHitBox = new Rect(Canvas.GetLeft(entity), Canvas.GetTop(entity), entity.Width, entity.Height);

                    if (playerHitBox.IntersectsWith(enemyHitBox))
                    {
                        ShowGameOver("You were killed by the invaders!!");
                    }
                }
            }
        }

        private void ProcessEnemyBullets()
        {
            var playerHitBox = Player.GetHitBox();

            foreach (var x in Canvas.Children.OfType<Rectangle>().ToList())
            {
                if ((string)x.Tag == "enemyBullet")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + 10);

                    if (Canvas.GetTop(x) > 480)
                    {
                        itemsToRemove.Add(x);
                    }

                    var enemyBulletHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (playerHitBox.IntersectsWith(enemyBulletHitBox))
                    {
                        ShowGameOver("You were killed by the invader bullet!!");
                    }
                }
            }
        }

        private void CleanupRemovedItems()
        {
            foreach (var i in itemsToRemove.ToList())
            {
                Canvas.Children.Remove(i);
                itemsToRemove.Remove(i);
            }
        }

        private void SpawnEnemyBullet(double x, double y)
        {
            var enemyBullet = new Rectangle
            {
                Tag = "enemyBullet",
                Height = 30,
                Width = 5,
                Fill = Brushes.Red,
            };

            Canvas.SetTop(enemyBullet, y);
            Canvas.SetLeft(enemyBullet, x);
            Canvas.Children.Add(enemyBullet);
        }

        private void CreateEnemies(int limit)
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
                Canvas.Children.Add(newEnemy);

                left -= random.Next(100, 240);
                enemyImages++;

                if (enemyImages > 8)
                {
                    enemyImages = 1;
                }

                enemySkin.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/images/invader{enemyImages}.gif"));
            }
        }

        private void ResetPlayerPosition() => Player.SetPosition(381, 394);

        private void ShowGameOver(string msg)
        {
            GameOver = true;
            enemiesLeftLabel.Content += " " + msg + " Press Enter to play again";
        }
    }
}
