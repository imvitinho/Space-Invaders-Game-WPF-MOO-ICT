using System.Collections.Generic;
using System.Windows.Input;

namespace Space_Invaders_Game_WPF_MOO_ICT.Classes
{
    public sealed class PlayerKeyBinding
    {
        public Dictionary<Key, GameActionsEnum> KeyBindings { get; init; }

        public PlayerKeyBinding()
        {
            KeyBindings = new Dictionary<Key, GameActionsEnum>();
        }
    }
}
