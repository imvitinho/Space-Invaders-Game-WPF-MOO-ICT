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
        private readonly GameState state = new();


        public MainWindow()
        {
            InitializeComponent();

            int playerCount = AskPlayerCount();
            state.Players = engine.CreatePlayers(myCanvas, playerCount, PlayerConfigFactory.GetPlayerConfigs());

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
            state.Enemies = engine.CreateEnemies(myCanvas, 30);
            state.AmmoBoxes.Clear();
            state.GameOver = false;
            state.AmmoBoxTimer = Random.Shared.Next(50, 301);
        }

        private void Update()
        {
            if (state.GameOver) return;

            enemiesLeft.Content = "Enemies Left: " + state.Enemies.Count;

            foreach (var player in state.Players)
            {
                player.AmmunitionLabel.Content = player.Ammunition;
            }

            engine.HandlePlayersMovement(state.Players, myCanvas.Width);

            state.BulletTimer -= 3;

            if (state.BulletTimer < 0)
            {
                engine.TryEnemyShot(myCanvas, state.Players, state.Enemies, state.AmmoBoxes);
                state.BulletTimer = GameState.BulletTimerLimit;
            }

            engine.ProcessPlayerBullets(myCanvas, state.ItemsToRemove, state.Enemies);
            engine.ProcessEnemies(myCanvas, state.EnemySpeed, state.Enemies);
            bool playerHit = engine.ProcessEnemyBullets(myCanvas, state.Players, state.ItemsToRemove);

            state.AmmoBoxTimer--;
            if (state.AmmoBoxTimer <= 0)
            {
                engine.SpawnAmmoBox(myCanvas, state.Players, state.AmmoBoxes);
                state.AmmoBoxTimer = Random.Shared.Next(50, 301);
            }
            engine.ProcessAmmoBoxes(myCanvas, state.Players, state.AmmoBoxes, state.ItemsToRemove);

            engine.CleanupRemovedItems(myCanvas, state.ItemsToRemove, state.Enemies, state.AmmoBoxes);

            if (state.Enemies.Count < 10) state.EnemySpeed = 12;
            if (playerHit) ShowGameOver("You were killed by the invader bullet!!");
            if (state.Enemies.Count < 1) ShowGameOver("You Win, you saved the world!");
        }

        private void Restart()
        {
            myCanvas.Children.Clear();
            myCanvas.Children.Add(enemiesLeft);

            foreach (var player in state.Players)
            {
                myCanvas.Children.Add(player.Rectangle);
            }

            state.ItemsToRemove.Clear();
            state.BulletTimer = 0;
            state.EnemySpeed = 6;

            engine.ResetPlayersPosition(myCanvas, state.Players);
            Initialize();
        }

        private void ShowGameOver(string msg)
        {
            state.GameOver = true;
            enemiesLeft.Content += " " + msg + " Press Enter to play again";
        }

        private void UiTimer_Tick(object sender, EventArgs e)
        {
            Update();

            if (state.GameOver)
            {
                uiTimer.Stop();
            }
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            engine.OnKeyDown(state.Players, e.Key);
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            engine.OnKeyUp(state.Players, e.Key, player => engine.FirePlayerBullet(myCanvas, player));

            if (e.Key == Key.Enter && state.GameOver)
            {
                Restart();
                uiTimer.Start();
            }
        }
    }
}