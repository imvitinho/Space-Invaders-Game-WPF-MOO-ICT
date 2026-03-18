using Microsoft.VisualBasic;
using Space_Invaders_Game_WPF_MOO_ICT.Classes;
using Space_Invaders_Game_WPF_MOO_ICT.Engine;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace Space_Invaders_Game_WPF_MOO_ICT
{
    public partial class MainWindow : Window
    {
        private readonly GameEngine engine = new GameEngine();
        private DispatcherTimer uiTimer;

        // Game state
        private List<Player> players;
        private bool gameOver;
        private readonly List<Rectangle> itemsToRemove = new List<Rectangle>();
        private int bulletTimer = 0;
        private const int bulletTimerLimit = 76;
        private int totalEnemies = 0;
        private int enemySpeed = 6;
        private int ammoBoxTimer = 0;


        public MainWindow()
        {
            InitializeComponent();

            int playerCount = AskPlayerCount();
            players = engine.CreatePlayers(myCanvas, playerCount, PlayerConfigFactory.GetPlayerConfigs());

            myCanvas.Focus();

            Initialize();

            uiTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(20) };
            uiTimer.Tick += UiTimer_Tick;
            uiTimer.Start();
        }

        private int AskPlayerCount()
        {
            string valor = Interaction.InputBox("Quantos jogadores? Digite um número entre 1 e 3:", "Escolha", "");

            if (int.TryParse(valor, out int numero) && numero >= 1 && numero <= 3)
            {
                return numero;
            }
            else
            {
                MessageBox.Show("Valor inválido.");
                return AskPlayerCount();
            }
        }

        private void Initialize()
        {
            totalEnemies = 30;
            engine.CreateEnemies(myCanvas, totalEnemies);
            gameOver = false;
            ammoBoxTimer = Random.Shared.Next(50, 301);
        }

        private void Update()
        {
            if (gameOver) return;

            enemiesLeft.Content = "Enemies Left: " + totalEnemies;

            foreach (var player in players)
            {
                player.AmmunitionLabel.Content = player.Ammunition;
            }

            engine.HandlePlayersMovement(players, myCanvas.Width);

            bulletTimer -= 3;

            if (bulletTimer < 0)
            {
                engine.TryEnemyShot(myCanvas, players);
                bulletTimer = bulletTimerLimit;
            }

            engine.ProcessPlayerBullets(myCanvas, itemsToRemove, ref totalEnemies);
            engine.ProcessEnemies(myCanvas, enemySpeed);
            bool playerHit = engine.ProcessEnemyBullets(myCanvas, players, itemsToRemove);

            ammoBoxTimer--;
            if (ammoBoxTimer <= 0)
            {
                engine.SpawnAmmoBox(myCanvas, players);
                ammoBoxTimer = Random.Shared.Next(50, 301);
            }
            engine.ProcessAmmoBoxes(myCanvas, players, itemsToRemove);

            engine.CleanupRemovedItems(myCanvas, itemsToRemove);

            if (totalEnemies < 10) enemySpeed = 12;
            if (playerHit) ShowGameOver("You were killed by the invader bullet!!");
            if (totalEnemies < 1) ShowGameOver("You Win, you saved the world!");
        }

        private void Restart()
        {
            myCanvas.Children.Clear();
            myCanvas.Children.Add(enemiesLeft);

            foreach (var player in players)
            {
                myCanvas.Children.Add(player.Rectangle);
            }

            itemsToRemove.Clear();
            bulletTimer = 0;
            enemySpeed = 6;

            engine.ResetPlayersPosition(myCanvas, players);
            Initialize();
        }

        private void ShowGameOver(string msg)
        {
            gameOver = true;
            enemiesLeft.Content += " " + msg + " Press Enter to play again";
        }

        private void UiTimer_Tick(object sender, EventArgs e)
        {
            Update();

            if (gameOver)
            {
                uiTimer.Stop();
            }
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            engine.OnKeyDown(players, e.Key);
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            engine.OnKeyUp(players, e.Key, player => engine.FirePlayerBullet(myCanvas, player));

            if (e.Key == Key.Enter && gameOver)
            {
                Restart();
                uiTimer.Start();
            }
        }
    }
}