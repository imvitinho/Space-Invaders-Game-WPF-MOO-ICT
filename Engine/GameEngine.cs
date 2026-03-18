using Space_Invaders_Game_WPF_MOO_ICT.Classes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Space_Invaders_Game_WPF_MOO_ICT.Engine
{
    public class GameEngine
    {
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
            if(player.Ammunition > 0)
            {
                var newBullet = new Rectangle
                {
                    Tag = "bullet",
                    Height = 20,
                    Width = 5,
                    Fill = Brushes.White,
                    Stroke = player.BulletCollor,
                };

                player.DecreaseAmmunition(1);
                Canvas.SetTop(newBullet, player.GetY() - newBullet.Height);
                Canvas.SetLeft(newBullet, player.GetX() + player.Rectangle.Width / 2);
                canvas.Children.Add(newBullet);
            }
        }

        public void ProcessPlayerBullets(Canvas canvas, List<Rectangle> itemsToRemove, List<Enemy> enemies)
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

                    foreach (var enemy in enemies.ToList())
                    {
                        if (bullet.IntersectsWith(enemy.GetHitBox()))
                        {
                            itemsToRemove.Add(x);
                            itemsToRemove.Add(enemy.Rectangle);
                        }
                    }
                }
            }
        }

        public void ProcessEnemies(Canvas canvas, int enemySpeed, List<Enemy> enemies)
        {
            Random random = new Random();

            foreach (var enemy in enemies)
            {
                enemy.SetPosition(enemy.GetX() + enemySpeed, enemy.GetY());
            }

            foreach (var enemy in enemies.Where(e => e.GetX() > 820).OrderByDescending(e => e.GetX()).ToList())
            {
                double minLeft = enemies.Min(e => e.GetX());
                enemy.SetPosition(Math.Min(0, minLeft) - enemy.Rectangle.Width - random.NextInt64(35, 100), enemy.GetY());
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

        public void CleanupRemovedItems(Canvas canvas, List<Rectangle> itemsToRemove, List<Enemy> enemies, List<AmmoBox> ammoBoxes)
        {
            foreach (var i in itemsToRemove.ToList())
            {
                canvas.Children.Remove(i);
                enemies.RemoveAll(e => e.Rectangle == i);
                ammoBoxes.RemoveAll(a => a.Rectangle == i);
                itemsToRemove.Remove(i);
            }
        }

        public void TryEnemyShot(Canvas canvas, List<Player> players, List<Enemy> enemies, List<AmmoBox> ammoBoxes)
        {
            foreach (var enemy in enemies)
            {
                bool playerShot = TryShootPlayer(canvas, players, enemy);

                if (!playerShot)
                {
                    TryShootAmmoBox(canvas, ammoBoxes, enemy);
                }
            }
        }

        private static void TryShootAmmoBox(Canvas canvas, List<AmmoBox> ammoBoxes, Enemy enemy)
        {
            foreach (var box in ammoBoxes)
            {
                bool isAboveBox =
                    enemy.GetX() + enemy.Rectangle.Width >= box.GetX() &&
                    enemy.GetX() <= box.GetX() + box.Rectangle.Width;

                if (isAboveBox)
                {
                    SpawnEnemyBullet(canvas, enemy);
                    break;
                }
            }
        }

        private static bool TryShootPlayer(Canvas canvas, List<Player> players, Enemy enemy)
        {
            foreach (var player in players)
            {
                double playerX = player.GetX();
                double playerWidth = player.Rectangle.Width;

                bool isAbovePlayer =
                    enemy.GetX() + enemy.Rectangle.Width >= playerX &&
                    enemy.GetX() <= playerX + playerWidth;

                if (isAbovePlayer)
                {
                    SpawnEnemyBullet(canvas, enemy);
                    return true;
                }
            }
            return false;
        }

        private static void SpawnEnemyBullet(Canvas canvas, Enemy enemy)
        {
            Rectangle enemyBullet = new Rectangle
            {
                Tag = "enemyBullet",
                Width = 5,
                Height = 30,
                Fill = Brushes.Red
            };

            Canvas.SetLeft(enemyBullet, enemy.GetX() + enemy.Rectangle.Width / 2);
            Canvas.SetTop(enemyBullet, enemy.GetY() + enemy.Rectangle.Height);

            canvas.Children.Add(enemyBullet);
        }

        public List<Enemy> CreateEnemies(Canvas canvas, int limit)
        {
            var enemies = new List<Enemy>();
            int left = 0;
            int enemyImages = 0;
            int[] topPositions = { 30, 150, 270 };
            Random random = new Random();

            for (int i = 0; i < limit; i++)
            {
                enemyImages++;
                if (enemyImages > 8)
                {
                    enemyImages = 1;
                }

                int top = topPositions[random.Next(topPositions.Length)];
                var enemy = new Enemy(enemyImages, left, top);
                canvas.Children.Add(enemy.Rectangle);
                enemies.Add(enemy);

                left -= random.Next(100, 240);
            }

            return enemies;
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
                Label ammunitionLabel = CreateAmmunitionLabel(canvas, i, config);

                playerList.Add(new Player(rectangle, config.SkinPath, config.KeyBinding, config.BulletCollor, ammunitionLabel));
            }

            return playerList;
        }

        private static Label CreateAmmunitionLabel(Canvas canvas, int i, PlayerConfig config)
        {
            Label ammunitionLabel = new Label();
            ammunitionLabel.Width = 30;
            ammunitionLabel.Height = 30;
            ammunitionLabel.Background = config.BulletCollor;
            ammunitionLabel.Foreground = Brushes.White;
            ammunitionLabel.FontWeight = FontWeights.Bold;
            ammunitionLabel.FontSize = 16;
            Canvas.SetRight(ammunitionLabel, 0);
            Canvas.SetTop(ammunitionLabel, 30 * i);
            canvas.Children.Add(ammunitionLabel);
            return ammunitionLabel;
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

        public void SpawnAmmoBox(Canvas canvas, List<Player> players, List<AmmoBox> ammoBoxes)
        {
            if (ammoBoxes.Count >= players.Count) return;

            Random random = new Random();
            var box = new AmmoBox(
                random.NextDouble() * (canvas.Width - 65),
                393
            );
            canvas.Children.Add(box.Rectangle);
            ammoBoxes.Add(box);
        }

        public void ProcessAmmoBoxes(Canvas canvas, List<Player> players, List<AmmoBox> ammoBoxes, List<Rectangle> itemsToRemove)
        {
            foreach (var box in ammoBoxes.ToList())
            {
                var boxRect = box.GetHitBox();

                foreach (var bullet in canvas.Children.OfType<Rectangle>().Where(b => (string)b.Tag == "bullet").ToList())
                {
                    var bulletRect = new Rect(Canvas.GetLeft(bullet), Canvas.GetTop(bullet), bullet.Width, bullet.Height);
                    if (bulletRect.IntersectsWith(boxRect))
                    {
                        itemsToRemove.Add(box.Rectangle);
                        itemsToRemove.Add(bullet);
                        break;
                    }
                }

                if (itemsToRemove.Contains(box.Rectangle)) continue;

                foreach (var enemyBullet in canvas.Children.OfType<Rectangle>().Where(b => (string)b.Tag == "enemyBullet").ToList())
                {
                    var enemyBulletRect = new Rect(Canvas.GetLeft(enemyBullet), Canvas.GetTop(enemyBullet), enemyBullet.Width, enemyBullet.Height);
                    if (enemyBulletRect.IntersectsWith(boxRect))
                    {
                        itemsToRemove.Add(box.Rectangle);
                        itemsToRemove.Add(enemyBullet);
                        break;
                    }
                }

                if (itemsToRemove.Contains(box.Rectangle)) continue;

                foreach (var player in players)
                {
                    var playerRect = new Rect(player.GetX(), player.GetY(), player.Rectangle.Width, player.Rectangle.Height);
                    if (playerRect.IntersectsWith(boxRect))
                    {
                        player.AddAmmunition(AmmoBox.AmmunitionAmount);
                        itemsToRemove.Add(box.Rectangle);
                        break;
                    }
                }
            }
        }
    }
}
