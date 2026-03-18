using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Space_Invaders_Game_WPF_MOO_ICT.Classes
{
    public class Enemy
    {
        public Rectangle Rectangle { get; private set; }
        private ImageBrush Skin { get; set; }

        public Enemy(int imageIndex, int left, int top)
        {
            Skin = new ImageBrush();
            Rectangle = new Rectangle
            {
                Tag = "enemy",
                Height = 45,
                Width = 45,
                Fill = Skin
            };

            SetSkin(imageIndex);
            Canvas.SetLeft(Rectangle, left);
            Canvas.SetTop(Rectangle, top);
        }

        public double GetX() => Canvas.GetLeft(Rectangle);
        public double GetY() => Canvas.GetTop(Rectangle);

        public void SetPosition(double x, double y)
        {
            Canvas.SetLeft(Rectangle, x);
            Canvas.SetTop(Rectangle, y);
        }

        public Rect GetHitBox() => new Rect(GetX(), GetY(), Rectangle.Width, Rectangle.Height);

        private void SetSkin(int imageIndex)
        {
            try
            {
                Skin.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/images/invader{imageIndex}.gif"));
            }
            catch
            {
                Rectangle.Fill = Brushes.White;
            }
        }
    }
}
