using Assets.Scripts.Buffs;

namespace Assets.Scripts.UI.Frames.Buffs
{
    public class BuffUI
    {
        public Buff Buff { get; }
        
        public BuffVisualIcon BuffVisualIcon { get; set; }

        public int Row { get; set; }
        public int Column { get; set; }

        public BuffUI(Buff buff)
        {
            Buff = buff;
        }
    }
}