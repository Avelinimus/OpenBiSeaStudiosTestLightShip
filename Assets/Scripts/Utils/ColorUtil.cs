using  UnityEngine;

namespace Utilities
{
    public static class ColorUtil
    {
        public static Color GetDisabledColor(Color c)
        {
            return new Color(c.r, c.g, c.b, .5f);
        }

        public static Color GetEnabledColor(Color c)
        {
            return new Color(c.r, c.g, c.b, 1f);
        }
    }
}