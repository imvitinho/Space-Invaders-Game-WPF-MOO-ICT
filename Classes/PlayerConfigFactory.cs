

using System.Windows.Input;
using System.Windows.Media;

namespace Space_Invaders_Game_WPF_MOO_ICT.Classes
{
    public static class PlayerConfigFactory
    {
        public static List<PlayerConfig> GetPlayerConfigs()
        {
            return new List<PlayerConfig>()
            {
                new() {
                    SkinPath = "pack://application:,,,/images/player1.png",
                    KeyBinding = new Dictionary<Key, GameActionsEnum>
                    {
                        { Key.A, GameActionsEnum.MoveLeft },
                        { Key.D, GameActionsEnum.MoveRight },
                        { Key.LeftCtrl, GameActionsEnum.Shoot }
                    },
                    BulletCollor = Brushes.Blue
                },
                new() {
                    SkinPath = "pack://application:,,,/images/player2.png",
                    KeyBinding = new Dictionary<Key, GameActionsEnum>
                    {
                        { Key.OemComma, GameActionsEnum.MoveLeft },
                        { Key.OemPeriod, GameActionsEnum.MoveRight },
                        { Key.Space, GameActionsEnum.Shoot }
                    },
                    BulletCollor = Brushes.Red
                },
                new() {
                    SkinPath = "pack://application:,,,/images/player3.png",
                    KeyBinding = new Dictionary<Key, GameActionsEnum>
                    {
                        { Key.NumPad4, GameActionsEnum.MoveLeft },
                        { Key.NumPad6, GameActionsEnum.MoveRight },
                        { Key.RightCtrl, GameActionsEnum.Shoot }
                    },
                    BulletCollor= Brushes.Green
                },
            };
        }
    }
}
