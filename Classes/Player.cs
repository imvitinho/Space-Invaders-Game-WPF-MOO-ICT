using System.Numerics;
using System.Windows;
using System.Windows.Controls;
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
        private ImageBrush Skin { get; set; }

        public Player(Rectangle rectangle, string skinPath)
        {
            Rectangle = rectangle;
            SetGoLeft(false);
            SetGoRight(false);
            SetSkin(skinPath);
        }

        public double GetX() => Canvas.GetLeft(Rectangle);
        public double GetY() => Canvas.GetTop(Rectangle);


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
