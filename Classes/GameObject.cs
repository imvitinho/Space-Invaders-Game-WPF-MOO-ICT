using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Space_Invaders_Game_WPF_MOO_ICT.Classes
{
    public abstract class GameObject
    {
        public Rectangle Rectangle { get; protected set; }
        protected ImageBrush Skin { get; set; }

        public double GetX() => Canvas.GetLeft(Rectangle);
        public double GetY() => Canvas.GetTop(Rectangle);

        public void SetPosition(double? x = null, double? y = null)
        {
            Canvas.SetLeft(Rectangle, x ?? GetX());
            Canvas.SetTop(Rectangle, y ?? GetY());
        }

        public virtual Rect GetHitBox() => new Rect(GetX(), GetY(), Rectangle.Width, Rectangle.Height);

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
    }
}
