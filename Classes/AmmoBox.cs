using System.Windows.Shapes;

namespace Space_Invaders_Game_WPF_MOO_ICT.Classes
{
    public class AmmoBox : GameObject
    {
        public const int AmmunitionAmount = 30;

        public AmmoBox(double left, double top)
        {
            Rectangle = new Rectangle
            {
                Tag = "ammoBox",
                Width = 65,
                Height = 65,
            };
            SetPosition(left, top);
            SetSkin("pack://application:,,,/images/ammunitionBox.png");
        }
    }
}
