using System.Windows.Shapes;

namespace Space_Invaders_Game_WPF_MOO_ICT.Classes
{
    public class GameState
    {
        public List<Player> Players { get; set; } = new();
        public List<Enemy> Enemies { get; set; } = new();
        public List<AmmoBox> AmmoBoxes { get; set; } = new();
        public List<Rectangle> ItemsToRemove { get; } = new();

        public bool GameOver { get; set; }
        public int BulletTimer { get; set; }
        public int EnemySpeed { get; set; } = 6;
        public int AmmoBoxTimer { get; set; }

        public const int BulletTimerLimit = 76;
    }
}
