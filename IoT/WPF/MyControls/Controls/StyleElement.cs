using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace System.Windows
{
    public enum LayoutOptions { Near, Center, Far, Stretch };
}
namespace System.Windows.Controls
{
    public class StyleElement : Border
    {
        public Brush BorderColor
        {
            get => (Brush)GetValue(BorderBrushProperty);
            set => SetValue(BorderBrushProperty, (Brush)value);
        }
        public LayoutOptions HAlign
        {
            get => (LayoutOptions)GetValue(HorizontalAlignmentProperty);
            set => SetValue(HorizontalAlignmentProperty, (HorizontalAlignment)value);
        }
        public LayoutOptions VAlign
        {
            get => (LayoutOptions)GetValue(VerticalAlignmentProperty);
            set => SetValue(VerticalAlignmentProperty, (VerticalAlignment)value);
        }

        public StyleElement()
        {
        }

        StyleSheetList _css;
        public StyleSheetList Css
        {
            get => _css;
            set
            {
                (_css = value).Apply(this);
            }
        }

        public virtual void Refresh() { }
    }
    public class StyleElement<T> : StyleElement
        where T: UIElement, new()
    {
        public StyleElement()
        {
            Child = new T();
        }
        public T Content
        {
            get => (T)Child;
            set => Child = value;
        }
    }
    public class PanelElement<T> : StyleElement<T>
        where T: Panel, new()
    {
        public PanelElement()
        {
            Child = new T();
        }

        public void AddRange(params UIElement[] items)
        {
            var children = ((Panel)Child).Children;
            foreach (var e in items) { children.Add(e); }
        }

        public void Add(UIElement item)
        {
            var children = ((Panel)Child).Children;
            children.Add(item);
        }
        public void Clear()
        {
            Content.Children.Clear();
        }
    }

    public interface ITextElement
    {
        Brush Foreground { set; }
    }
    public class TextElement<T> : StyleElement<T>, ITextElement
        where T: Control, new()
    {
        public Brush Foreground
        {
            get => (Brush)Content.GetValue(Control.ForegroundProperty);
            set => Content.SetValue(Control.ForegroundProperty, value);
        }
        public double FontSize
        {
            get => (double)Content.GetValue(Control.FontSizeProperty);
            set => Content.SetValue(Control.FontSizeProperty, value);
        }
        public int FontWeight
        {
            get => ((FontWeight)Content.GetValue(Control.FontWeightProperty)).ToOpenTypeWeight();
            set => Content.SetValue(Control.FontWeightProperty, 
                System.Windows.FontWeight.FromOpenTypeWeight(value));
        }

        new public bool IsFocused => Content.IsFocused;
        public virtual string Text { get; set; }
    }

    public class MyLabel : TextElement<Label>
    {
        public MyLabel()
        {
            Content.HorizontalAlignment = HorizontalAlignment.Center;
            Content.VerticalAlignment = VerticalAlignment.Center;
        }
        public override string Text
        {
            get => (string)Content.GetValue(Label.ContentProperty);
            set => Content.SetValue(Label.ContentProperty, value);
        }
    }
}
