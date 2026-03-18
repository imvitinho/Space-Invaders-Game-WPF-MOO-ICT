using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Space_Invaders_Game_WPF_MOO_ICT.Classes
{
    public class Enemy : GameObject
    {
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

            LoadSkin(imageIndex);
            SetPosition(left, top);
        }

        private void LoadSkin(int imageIndex)
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
