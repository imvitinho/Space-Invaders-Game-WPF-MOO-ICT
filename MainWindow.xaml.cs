using Microsoft.VisualBasic;
using Space_Invaders_Game_WPF_MOO_ICT.Classes;
using Space_Invaders_Game_WPF_MOO_ICT.Engine;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace Space_Invaders_Game_WPF_MOO_ICT
{
    public partial class MainWindow : Window
    {
        private GameEngine engine;
        private DispatcherTimer uiTimer;
        private int playerCount;


        public MainWindow()
        {
            InitializeComponent();

            playerCount = AskPlayerCount();

            myCanvas.Focus();

            engine = new GameEngine(myCanvas, CreatePlayers(), enemiesLeft);
            engine.Initialize();

            uiTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(20) };
            uiTimer.Tick += UiTimer_Tick;
            uiTimer.Start();
        }

        private int AskPlayerCount()
        {
            string valor = Interaction.InputBox("Quantos jogadores? Digite um número entre 1 e 3:", "Escolha", "");

            if (int.TryParse(valor, out int numero) && numero >= 1 && numero <= 3)
            {
                return int.Parse(valor);
            }
            else
            {
                MessageBox.Show("Valor inválido.");
                return AskPlayerCount();
            }
        }

        public List<Player> CreatePlayers()
        {
            List<Player> playerList = new List<Player>();
            List<PlayerConfig> playerConfigs = PlayerConfigFactory.GetPlayerConfigs();

            for (int i = 0; i < playerCount; i++)
            {
                PlayerConfig config = playerConfigs[i];

                double x = myCanvas.Width * (i + 1) / (playerCount * 2);

                Rectangle rectangle = new Rectangle
                {
                    Width = 55,
                    Height = 65,
                    Fill = Brushes.White,
                };

                Canvas.SetTop(rectangle, 393);
                Canvas.SetLeft(rectangle, x);
                myCanvas.Children.Add(rectangle);

                Player player = new Player(rectangle, config.SkinPath, config.KeyBinding, config.BulletCollor);

                playerList.Add(player);
            }

            return playerList;
        }

        private void UiTimer_Tick(object sender, EventArgs e)
        {
            engine.Update();

            if (engine.GameOver)
            {
                uiTimer.Stop();
            }
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            engine?.OnKeyDown(e.Key);
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            engine?.OnKeyUp(e.Key);

            if (e.Key == Key.Enter && engine != null && engine.GameOver)
            {
                engine.Restart();
                uiTimer.Start();
            }
        }
    }
}