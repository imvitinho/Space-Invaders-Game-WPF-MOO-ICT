using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Space_Invaders_Game_WPF_MOO_ICT.Engine;

namespace Space_Invaders_Game_WPF_MOO_ICT
{
    public partial class MainWindow : Window
    {
        private GameEngine engine;
        private DispatcherTimer uiTimer;

        public MainWindow()
        {
            InitializeComponent();

            myCanvas.Focus();

            engine = new GameEngine(myCanvas, player, enemiesLeft);
            engine.Initialize();

            uiTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(20) };
            uiTimer.Tick += UiTimer_Tick;
            uiTimer.Start();
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