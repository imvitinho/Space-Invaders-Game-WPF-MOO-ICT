using System.Collections.Generic;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Space_Invaders_Game_WPF_MOO_ICT.Classes
{
    public class Player
    {
        public Rectangle Rectangle { get; private set; }
        public bool GoLeft { get; private set; }
        public bool GoRight { get; private set; }
        public Dictionary<Key, GameActionsEnum> KeyBinding { get; private set; }
        private ImageBrush Skin { get; set; }
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

        public double GetX() => Canvas.GetLeft(Rectangle);
        public double GetY() => Canvas.GetTop(Rectangle);

        public void AddAmmunition(int ammunition) => Ammunition += ammunition;

        public void DecreaseAmmunition(int ammunition) => Ammunition -= ammunition;

        public void SetGoLeft(bool goLeft)
        {
            GoLeft = goLeft;
        }

        public void SetGoRight(bool goRight)
        {
            GoRight = goRight;
        }

        public void SetPosition(double? x = null, double? y = null)
        {
            Canvas.SetLeft(Rectangle, x ?? GetX());
            Canvas.SetTop(Rectangle, y ?? GetY());
        }

        public void SetSkin(string skinPath)
        {
            try
            {
                Skin = new ImageBrush(new BitmapImage(new Uri(skinPath, UriKind.RelativeOrAbsolute)));
                Rectangle.Fill = Skin;
            }
            catch
            {
                Rectangle.Fill = Brushes.White;
            }
        }

        public Rect GetHitBox()
        {
            return new Rect(
                Canvas.GetLeft(Rectangle) + 15, 
                Canvas.GetTop(Rectangle) + 20, 
                Rectangle.Width - 30, 
                Rectangle.Height - 35
            );
        }
    }
}
