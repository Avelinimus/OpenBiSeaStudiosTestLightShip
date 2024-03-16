using DG.Tweening;

namespace Game.Utilities
{
    public static class DOTweenExtensions
    {
        public static Sequence Join(this Sequence sequence, Tween[] tweens)
        {
            foreach (var tween in tweens)
            {
                sequence.Join(tween);
            }
            return sequence;
        }
    }
}
