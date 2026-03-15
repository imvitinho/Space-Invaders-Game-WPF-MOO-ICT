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
        public void UpdateUi(Label enemiesLeftLabel, int totalEnemies)
        {
            enemiesLeftLabel.Content = "Enemies Left: " + totalEnemies;
        }

        public void HandlePlayersMovement(List<Player> players, double canvasWidth)
        {
            foreach (var player in players)
            {
                if (player.GoLeft && player.GetX() > 0)
                {
                    player.SetPosition(player.GetX() - 10);
                }

                if (player.GoRight && player.GetX() + 80 < canvasWidth)
                {
                    player.SetPosition(player.GetX() + 10);
                }
            }
        }

        public void OnKeyDown(List<Player> players, Key key)
        {
            foreach (var player in players)
            {
                Dictionary<GameActionsEnum, Action> actions = new Dictionary<GameActionsEnum, Action>
                {
                    { GameActionsEnum.MoveLeft, () => player.SetGoLeft(true) },
                    { GameActionsEnum.MoveRight, () => player.SetGoRight(true) },
                };

                if (player.KeyBinding.TryGetValue(key, out GameActionsEnum actionToPerform))
                {
                    actions.GetValueOrDefault(actionToPerform)?.Invoke();
                }
            }
        }

        public void OnKeyUp(List<Player> players, Key key, Action<Player> onShoot)
        {
            foreach (var player in players)
            {
                Dictionary<GameActionsEnum, Action> actions = new Dictionary<GameActionsEnum, Action>
                {
                    { GameActionsEnum.MoveLeft, () => player.SetGoLeft(false) },
                    { GameActionsEnum.MoveRight, () => player.SetGoRight(false) },
                    { GameActionsEnum.Shoot, () => onShoot(player) },
                };

                if (player.KeyBinding.TryGetValue(key, out GameActionsEnum actionToPerform))
                {
                    actions.GetValueOrDefault(actionToPerform)?.Invoke();
                }
            }
        }

        public void FirePlayerBullet(Canvas canvas, Player player)
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
            canvas.Children.Add(newBullet);
        }

        public void ProcessPlayerBullets(Canvas canvas, List<Rectangle> itemsToRemove, ref int totalEnemies)
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

        public void ProcessEnemies(Canvas canvas, int enemySpeed)
        {
            Random random = new Random();
            var enemies = canvas.Children.OfType<Rectangle>()
                .Where(e => (string)e.Tag == "enemy")
                .ToList();

            foreach (var entity in enemies)
            {
                Canvas.SetLeft(entity, Canvas.GetLeft(entity) + enemySpeed);
            }

            foreach (var entity in enemies.Where(e => Canvas.GetLeft(e) > 820).OrderByDescending(Canvas.GetLeft).ToList())
            {
                double minLeft = enemies.Min(Canvas.GetLeft);
                Canvas.SetLeft(entity, Math.Min(0, minLeft) - entity.Width - random.NextInt64(35, 100));
            }
        }

        public bool ProcessEnemyBullets(Canvas canvas, List<Player> players, List<Rectangle> itemsToRemove)
        {
            var playersHitBoxes = players.Select(p => p.GetHitBox());
            bool playerHit = false;

            foreach (var bullet in canvas.Children.OfType<Rectangle>().ToList())
            {
                if ((string)bullet.Tag == "enemyBullet")
                {
                    Canvas.SetTop(bullet, Canvas.GetTop(bullet) + 16);

                    if (Canvas.GetTop(bullet) > 480)
                    {
                        itemsToRemove.Add(bullet);
                    }

                    var enemyBulletHitBox = new Rect(Canvas.GetLeft(bullet), Canvas.GetTop(bullet), bullet.Width, bullet.Height);

                    if (playersHitBoxes.Any(phb => phb.IntersectsWith(enemyBulletHitBox)))
                    {
                        playerHit = true;
                    }
                }
            }

            return playerHit;
        }

        public void CleanupRemovedItems(Canvas canvas, List<Rectangle> itemsToRemove)
        {
            foreach (var i in itemsToRemove.ToList())
            {
                canvas.Children.Remove(i);
                itemsToRemove.Remove(i);
            }
        }

        public void SpawnEnemyBullet(Canvas canvas, List<Player> players)
        {
            foreach (var player in players)
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
                canvas.Children.Add(enemyBullet);
            }
        }

        public void CreateEnemies(Canvas canvas, int limit)
        {
            int left = 0;
            int enemyImages = 0;
            int[] topPositions = { 30, 150, 270 };
            Random random = new Random();

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
                canvas.Children.Add(newEnemy);

                left -= random.Next(100, 240);
                enemyImages++;

                if (enemyImages > 8)
                {
                    enemyImages = 1;
                }

                enemySkin.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/images/invader{enemyImages}.gif"));
            }
        }

        public List<Player> CreatePlayers(Canvas canvas, int playerCount, List<PlayerConfig> playerConfigs)
        {
            List<Player> playerList = new List<Player>();

            for (int i = 0; i < playerCount; i++)
            {
                PlayerConfig config = playerConfigs[i];

                double x = canvas.Width * (i + 1) / (playerCount * 2);

                Rectangle rectangle = new Rectangle
                {
                    Width = 55,
                    Height = 65,
                    Fill = Brushes.White,
                };

                Canvas.SetTop(rectangle, 393);
                Canvas.SetLeft(rectangle, x);
                canvas.Children.Add(rectangle);

                playerList.Add(new Player(rectangle, config.SkinPath, config.KeyBinding, config.BulletCollor));
            }

            return playerList;
        }

        public void ResetPlayersPosition(Canvas canvas, List<Player> players)
        {
            for (int i = 0; i < players.Count; i++)
            {
                Player player = players[i];
                double x = canvas.Width * (i + 1) / (players.Count * 2);
                player.SetPosition(x - player.Rectangle.Width / 2, 394);
            }
        }
    }
}
