using UnityEngine;

namespace TrueBRChaos.UI
{
    internal class UIColors
    {
        public static Color timer_color_background  = new Color(0.96f, 0.96f, 0.96f, 0.85f);
        public static Color timer_color_foreground  = new Color(0.30f, 0.75f, 0.80f, 0.85f);
        public static Color dtimer_color_foreground = new Color(timer_color_foreground.r * 0.7f, timer_color_foreground.g * 0.7f, timer_color_foreground.b * 0.7f, 0.6f);
        public static Color dtimer_color_background = new Color(timer_color_background.r * 0.7f, timer_color_background.g * 0.7f, timer_color_background.b * 0.7f, 0.6f);
        public static Color timer_event_trigger     = new Color(timer_color_foreground.r * 0.5f, timer_color_foreground.g * 0.5f, timer_color_foreground.b * 0.5f, timer_color_foreground.a * 0.7f);
        public static Color timer_color_timerending = new Color(1.00f, 0.15f, 0.10f, 0.85f);
    }
}
