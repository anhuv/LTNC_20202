using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Reflection;
using System.ComponentModel;

namespace System.Windows
{
    public class Rgb
    {
        int _value;
        public byte B
        {
            get => (byte)(_value);
            set => _value = (_value & 0xFFFF00) | value;
        }
        public byte G
        {
            get => (byte)(_value >> 8);
            set => _value = (_value & 0xFF00FF) | (value << 8);
        }
        public byte R
        {
            get => (byte)(_value >> 16);
            set => _value = (_value & 0xFFFF) | (value << 16);
        }

        public Rgb() { }
        public Rgb(byte r, byte g, byte b)
        {
            _value = (r << 16) | (g << 8) | b;
        }
        public Rgb(long color)
        {
            _value = (int)color;
        }
        public Rgb(string hex)
        {
            byte[] v = new byte[6];
            int i;
            for (i = 0; i < hex.Length && i < v.Length; i++)
            {
                char c = hex[i];
                if (c >= 'a') { c -= ' '; }

                byte b = (byte)(c >= 'A' ? c - 55 : (c & 15));
                v[i] = b;
            }

            if (i < 4)
            {
                for (--i; i >= 0; i--)
                {
                    int k = i << 1;
                    v[k + 1] = v[k] = v[i];
                }
            }

            _value = 0;
            foreach (var n in v)
            {
                _value = (_value << 4) | n;
            }
        }

        public static explicit operator Color(Rgb rgb)
        {
            if (rgb._value < 0) { return Colors.Transparent; }
            return Color.FromRgb(rgb.R, rgb.G, rgb.B);
        }
        public static explicit operator Brush(Rgb rgb)
        {
            return new SolidColorBrush((Color)rgb);
        }

        public static implicit operator Rgb(long color)
        {
            return new Rgb(color);
        }
        public static implicit operator Rgb(string color)
        {
            return new Rgb(color);
        }
    }

    public class Bound
    {
        public int Left;
        public int Top;
        public int Bottom;
        public int Right;

        public Bound() { }
        public Bound(int all)
        {
            Left = Top = Bottom = Right = all;
        }
        public Bound(string value)
        {
            int[] v = new int[4];
            int a = 0;
            int i = 0;
            bool minus = false;
            char last = ' ';
            foreach (var c in value)
            {
                switch (c)
                {
                    case '-': minus = true; continue;
                    case ' ': case ',':
                        if (last == ' ') continue;
                        v[i++] = (minus ? -a : a);

                        a = 0; minus = false;
                        last = ' ';
                        break;

                    default:
                        if (c >= '0' && '9' >= c)
                        {
                            a = (a << 1) + (a << 3) + (c & 15);
                        }
                        last = c;
                        break;
                }
                if (i >= v.Length) { break; }
            }

            if (i < v.Length)
            {
                v[i] = (minus ? -a : a);
            }
            if (i == 1)
            {
                v[2] = v[0];
                v[3] = v[1];
            }
            Left = v[0]; Right = v[2];
            Top = v[1]; Bottom = v[3];
        }

        public static implicit operator Bound(long all)
        {
            return new Bound((int)all);
        }
        public static implicit operator Bound(string value)
        {
            return new Bound(value);
        }
        public static explicit operator Thickness(Bound b)
        {
            return new Thickness(b.Left, b.Top, b.Right, b.Bottom);
        }
        public static explicit operator CornerRadius(Bound b)
        {
            return new CornerRadius(b.Left, b.Top, b.Right, b.Bottom);
        }
    }
    public class Theme
    {
        public Rgb Foreground { get; set; }
        public Rgb Background { get; set; }
        public Rgb BorderColor { get; set; }
        public Bound BorderThickness { get; set; }
        public Bound Padding { get; set; }
        public Bound Margin { get; set; }
        public Bound CornerRadius { get; set; }
        public double? Opacity { get; set; } = 1;
        public double? FontSize { get; set; }
        public int? FontWeight { get; set; }
        public LayoutOptions? HAlign { get; set; }
        public LayoutOptions? VAlign { get; set; }
        public static Theme Default
        {
            get
            {
                return new Theme
                {
                    Opacity = 1,
                    Foreground = 0,
                };
            }
        }
        public void SetStyle(UIElement elem)
        {
            var type = elem.GetType();
            foreach (var p in typeof(Theme).GetProperties())
            {
                var v = p.GetValue(this);
                if (v != null)
                {
                    var q = type.GetProperty(p.Name);
                    if (q != null)
                    {
                        if (v is Bound)
                        {
                            if (q.PropertyType == typeof(CornerRadius))
                            {
                                v = (CornerRadius)(Bound)v;
                            }
                            else
                            {
                                v = (Thickness)(Bound)v;
                            }
                        }
                        else if (v is Rgb)
                        {
                            v = (Brush)(Rgb)v;
                        }
                        else
                        {
                            v = Convert.ChangeType(v, q.PropertyType);
                        }
                        q.SetValue(elem, v);
                    }
                }
            }
        }

        public Theme Clone()
        {
            return (Theme)MemberwiseClone();
        }
    }

    public class StyleSheetCollection
    {
        static Dictionary<string, StyleSheet> _map;

        public static StyleSheet SetClass(string name, StyleSheet item)
        {
            name = name.ToLower();
            _map.Add(name, item);

            return item;
        }
        public static StyleSheet GetClass(string name)
        {
            StyleSheet t;
            if (_map == null || _map.TryGetValue(name, out t) == false)
            {
                t = new StyleSheet { Default = Theme.Default };
            }
            return t;
        }

        public static void Load(string filename)
        {
            try
            {
                var items = Vst.Json.Read<Dictionary<string, StyleSheet>>(filename);
                if (_map == null)
                {
                    _map = new Dictionary<string, StyleSheet>();
                    foreach (var p in items)
                    {
                        SetClass(p.Key, p.Value);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class StyleSheet
    {
        public Theme Default { get; set; }
        public Theme Hover { get; set; }
        public Theme Hold { get; set; }
        public Theme Focus { get; set; }

        public StyleSheet Clone()
        {
            return (StyleSheet)MemberwiseClone();
        }

        public static implicit operator StyleSheet(string text)
        {
            return StyleSheetCollection.GetClass(text).Clone();
        }

        public void Apply(UIElement e)
        {
            Default?.SetStyle(e);

            e.MouseMove += delegate { Hover?.SetStyle(e); };
            e.MouseLeave += delegate {
                if (Focus == null && !e.IsFocused)
                {
                    Default?.SetStyle(e);
                }
            };
            e.PreviewMouseLeftButtonDown += delegate { Hold?.SetStyle(e); };

            e.GotFocus += delegate { Focus?.SetStyle(e); };
            e.LostFocus += delegate { Default?.SetStyle(e); };
        }
    }

    public class StyleSheetList : List<StyleSheet>
    {
        public void Apply(UIElement e)
        {
            foreach (var css in this)
            {
                css.Apply(e);
            }
        }
        public static implicit operator StyleSheetList(string text)
        {
            var lst = new StyleSheetList();
            foreach (var name in text.Trim().Split(' '))
            {
                if (name != string.Empty)
                {
                    lst.Add(StyleSheetCollection.GetClass(name));
                }
            }
            return lst;
        }
    }
}
