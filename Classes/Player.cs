using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Space_Invaders_Game_WPF_MOO_ICT.Classes
{
    public class Player : GameObject
    {
        public bool GoLeft { get; private set; }
        public bool GoRight { get; private set; }
        public Dictionary<Key, GameActionsEnum> KeyBinding { get; private set; }
        public SolidColorBrush BulletCollor { get; private set; }
        public Label AmmunitionLabel { get; private set; }
        public int Ammunition { get; private set; }

        public Player(Rectangle rectangle, string skinPath, Dictionary<Key, GameActionsEnum> keyBinding, SolidColorBrush bulletCollor, Label ammunitionLabel)
        {
            Rectangle = rectangle;
            SetGoLeft(false);
            SetGoRight(false);
            KeyBinding = keyBinding;
            SetSkin(skinPath);
            BulletCollor = bulletCollor;
            AmmunitionLabel = ammunitionLabel;
            Ammunition = 30;
        }

        public void AddAmmunition(int ammunition) => Ammunition += ammunition;

        public void DecreaseAmmunition(int ammunition) => Ammunition -= ammunition;

        public void SetGoLeft(bool goLeft) => GoLeft = goLeft;

        public void SetGoRight(bool goRight) => GoRight = goRight;

        public override Rect GetHitBox()
        {
            return new Rect(
                GetX() + 15,
                GetY() + 20,
                Rectangle.Width - 30,
                Rectangle.Height - 35
            );
        }
    }
}
