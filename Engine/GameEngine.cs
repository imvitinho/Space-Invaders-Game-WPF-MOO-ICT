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
        private readonly Canvas canvas;
        private readonly Rectangle player;
        private readonly Label enemiesLeftLabel;

        public bool GameOver { get; private set; }

        private readonly ImageBrush playerSkin = new ImageBrush();
        private readonly List<Rectangle> itemsToRemove = new List<Rectangle>();

        private bool goLeft, goRight;
        private int enemyImages = 0;
        private int bulletTimer = 0;
        private int bulletTimerLimit = 90;
        private int totalEnemies = 0;
        private int enemySpeed = 6;

        private Rect GetPlayerHitBox()
        {
            return new Rect(
                Canvas.GetLeft(player) + 15, 
                Canvas.GetTop(player) + 20, 
                player.Width - 30, 
                player.Height - 35
                );
        }


        public GameEngine(Canvas canvas, Rectangle player, Label enemiesLeftLabel)
        {
            this.canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
            this.player = player ?? throw new ArgumentNullException(nameof(player));
            this.enemiesLeftLabel = enemiesLeftLabel ?? throw new ArgumentNullException(nameof(enemiesLeftLabel));
        }

        public void Initialize()
        {
            try
            {
                playerSkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/player.png"));
                player.Fill = playerSkin;
            }
            catch
            {
                player.Fill = Brushes.White;
            }

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
                SpawnEnemyBullet(Canvas.GetLeft(player) + 20, 10);
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
            if (key == Key.Left)
            {
                goLeft = true;
            }

            if (key == Key.Right)
            {
                goRight = true;
            }
        }

        public void OnKeyUp(Key key)
        {
            if (key == Key.Left)
            {
                goLeft = false;
            }

            if (key == Key.Right)
            {
                goRight = false;
            }

            if (key == Key.Space)
            {
                FirePlayerBullet();
            }
        }

        public void Restart()
        {
            canvas.Children.Clear();
            canvas.Children.Add(enemiesLeftLabel);
            canvas.Children.Add(player);

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
            if (goLeft && Canvas.GetLeft(player) > 0)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) - 10);
            }

            if (goRight && Canvas.GetLeft(player) + 80 < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) + 10);
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

            Canvas.SetTop(newBullet, Canvas.GetTop(player) - newBullet.Height);
            Canvas.SetLeft(newBullet, Canvas.GetLeft(player) + player.Width / 2);
            canvas.Children.Add(newBullet);
        }

        private void ProcessPlayerBullets()
        {
            foreach (var x in canvas.Children.OfType<Rectangle>().ToList())
            {
                if ((string)x.Tag == "bullet")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) - 20);

                    if (Canvas.GetTop(x) < 10)
                    {
                        itemsToRemove.Add(x);
                    }

                    var bullet = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    foreach (var y in canvas.Children.OfType<Rectangle>().ToList())
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
            var playerHitBox = GetPlayerHitBox();

            foreach (var x in canvas.Children.OfType<Rectangle>().ToList())
            {
                if ((string)x.Tag == "enemy")
                {
                    Canvas.SetLeft(x, Canvas.GetLeft(x) + enemySpeed);

                    if (Canvas.GetLeft(x) > 820)
                    {
                        Canvas.SetLeft(x, -80);
                        Canvas.SetTop(x, Canvas.GetTop(x) + (x.Height + 10));
                    }

                    var enemyHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (playerHitBox.IntersectsWith(enemyHitBox))
                    {
                        ShowGameOver("You were killed by the invaders!!");
                    }
                }
            }
        }

        private void ProcessEnemyBullets()
        {
            var playerHitBox = GetPlayerHitBox();

            foreach (var x in canvas.Children.OfType<Rectangle>().ToList())
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
                canvas.Children.Remove(i);
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
            canvas.Children.Add(enemyBullet);
        }

        private void CreateEnemies(int limit)
        {
            int left = 0;
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

                Canvas.SetTop(newEnemy, 30);
                Canvas.SetLeft(newEnemy, left);
                canvas.Children.Add(newEnemy);

                left -= 60;
                enemyImages++;

                if (enemyImages > 8)
                {
                    enemyImages = 1;
                }

                switch (enemyImages)
                {
                    case 1:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/invader1.gif"));
                        break;
                    case 2:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/invader2.gif"));
                        break;
                    case 3:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/invader3.gif"));
                        break;
                    case 4:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/invader4.gif"));
                        break;
                    case 5:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/invader5.gif"));
                        break;
                    case 6:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/invader6.gif"));
                        break;
                    case 7:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/invader7.gif"));
                        break;
                    case 8:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/invader8.gif"));
                        break;
                }
            }
        }

        private void ResetPlayerPosition()
        {
            Canvas.SetLeft(player, 381);
            Canvas.SetTop(player, 394);
        }

        private void ShowGameOver(string msg)
        {
            GameOver = true;
            enemiesLeftLabel.Content += " " + msg + " Press Enter to play again";
        }
    }
}
