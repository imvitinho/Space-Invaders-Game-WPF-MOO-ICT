using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Space_Invaders_Game_WPF_MOO_ICT.Classes
{
    public sealed class PlayerConfig
    {
        public string SkinPath { get; init; }
        public Dictionary<Key, GameActionsEnum> KeyBinding { get; init; }
        public SolidColorBrush BulletCollor { get; init; }
    }
}
