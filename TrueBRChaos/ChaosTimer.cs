using UnityEngine;
using TrueBRChaos.UI;
using static TrueBRChaos.Commons;
using static TrueBRChaos.UI.UIColors;
using static TrueBRChaos.ChaosConfig.UI;

namespace TrueBRChaos
{
    internal class ChaosTimer : MonoBehaviour
    {
        public static GameObject Create(float timeMax, TimerEnd onEndMethod, Vector2 position, Vector2 size, Vector2 anchor, string text = null, string name = null, Color? textColor = null)
        {
            GameObject go_chaosTimer    = new GameObject(name, typeof(ChaosTimer));
            ChaosTimer chaosTimer       = go_chaosTimer.GetComponent<ChaosTimer>();
            DontDestroyOnLoad(chaosTimer);

            chaosTimer.canvas = ChaosManager.canvas.Canvas;

            chaosTimer.TimeMax      = timeMax;
            chaosTimer.Position     = position;
            chaosTimer.Size         = size;
            chaosTimer.Anchor       = anchor;
            chaosTimer.Text         = text ?? chaosTimer._text;
            chaosTimer.Name         = name ?? chaosTimer._name;
            chaosTimer.Time         = chaosTimer.TimeMax;
            chaosTimer.TextColor    = textColor ?? Color.white;

            chaosTimer.TimerBackground  = new UIBox(chaosTimer.Position, chaosTimer._size, chaosTimer.Anchor, timer_color_background, chaosTimer.canvas.transform, $"TimerBack ({chaosTimer.Name})");
            chaosTimer.TimerForeground  = new UIBox(chaosTimer.Position, chaosTimer._size, chaosTimer.Anchor, timer_color_foreground, chaosTimer.canvas.transform, $"TimerFore ({chaosTimer.Name})");

            chaosTimer.OnTimerEnd += onEndMethod;

            return go_chaosTimer;
        }

        public void OnDestroy()
        {
            if (TimerBackground?.Box != null)
                Destroy(TimerBackground.Box.gameObject);

            if (TimerForeground?.Box != null)
                Destroy(TimerForeground.Box.gameObject);

            if (TimerText?.TextPro != null)
                Destroy(TimerText.TextPro.gameObject);
        }

        public delegate void TimerEnd();
        public event TimerEnd OnTimerEnd;

        public bool timerActive = false;

        public  float   TimeMax = 30f;
        public  float   Time    = 30f;

        private Vector2 _position = Vector2.zero;
        public  Vector2 Position
        {
            get
            {
                return _position;
            }

            set
            {
                _position = value;

                if (TimerBackground != null && TimerForeground != null)
                {
                    TimerBackground.Position = _position;
                    TimerForeground.Position = _position;
                }

                if (TimerText != null)
                {
                    TimerText.Position = new Vector2(TimerTextPositionX, _position.y);
                }
            }
        }

        private Vector2 _size = Vector2.zero;
        public  Vector2 Size
        {
            get
            {
                return _size;
            }

            set
            {
                _size = value;

                if (TimerBackground != null && TimerForeground != null)
                {
                    TimerBackground.Size = _size;
                    TimerForeground.Size = new Vector2((Time / TimeMax) * _size.x, _size.y);
                }

                if (TimerText != null)
                {
                    TimerText.Position = new Vector2(TimerTextPositionX, TimerText.Position.y);
                }
            }
        }

        private Vector2 _anchor = Vector2.zero;
        public  Vector2 Anchor
        {
            get
            {
                return _anchor;
            }

            set
            {
                _anchor = value;

                if (TimerBackground != null && TimerForeground != null)
                {
                    TimerBackground.Anchor = _anchor;
                    TimerForeground.Anchor = _anchor;
                }

                if (TimerText != null)
                {
                    TimerText.Anchor = _anchor;
                }
            }
        }

        private string  _name = "ChaosTimer";
        public  string  Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;

                if (TimerBackground != null && TimerForeground != null)
                {
                    TimerBackground.Box.gameObject.name = _name;
                    TimerForeground.Box.gameObject.name = _name;
                }

                if (TimerText != null)
                {
                    TimerText.TextPro.gameObject.name = _name;
                }
            }
        }

        public string _text = string.Empty;
        public string Text
        {
            get
            {
                return _text;
            }

            set
            {
                _text = value;

                if (TimerText == null && _text != string.Empty)
                {
                    CreateText();
                }
                else if (TimerText != null)
                {
                    TimerText.Text = _text;
                }
            }
        }

        public Color _textColor = Color.white;
        public Color TextColor
        {
            get
            {
                return _textColor;
            }

            set
            {
                _textColor = value;

                if (TimerText != null)
                {
                    TimerText.Color = _textColor;
                }
            }
        }

        internal    UIBox   TimerBackground;
        internal    UIBox   TimerForeground;
        private     UIText  TimerText;

        private float TimerTextPositionX    => Position.x + Size.x + Text_Offset;

        private Canvas canvas;

        private void CreateText()
        {
            TimerText = new UIText(new Vector2(TimerTextPositionX, Position.y), new Vector2(Screen.width, Size.y), Anchor, TextColor, TMPro.TextAlignmentOptions.MidlineLeft, (int)Size.y, Text, canvas.transform, 0.35f, Color.black, $"Text ({Name})");
        }

        public void Update()
        {
            if (timerActive && ChaosShouldRun)
            {
                TimerForeground.Width = (Time / TimeMax) * Size.x;
                UpdateColor();

                if (Time == 0f)
                    OnTimerEnd.Invoke();

                Time = Time > 0f ? Mathf.Max(Time - Delta, 0f) : TimeMax;
            }
        }

        private void UpdateColor()
        {
            float timemax_warn = TimeMax * Timer_Warn_Percent;

            if (Time <= timemax_warn)
            {
                TimerForeground.Color = new Color
                (
                    Mathf.Lerp(timer_color_timerending.r, timer_color_foreground.r, Time / timemax_warn),
                    Mathf.Lerp(timer_color_timerending.g, timer_color_foreground.g, Time / timemax_warn),
                    Mathf.Lerp(timer_color_timerending.b, timer_color_foreground.b, Time / timemax_warn),
                    Mathf.Lerp(timer_color_timerending.a, timer_color_foreground.a, Time / timemax_warn)
                );
            }
            else
            {
                TimerForeground.Color = timer_color_foreground;
            }
        }
    }
}
