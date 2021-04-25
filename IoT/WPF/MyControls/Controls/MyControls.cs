using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;

namespace System.Windows.Controls
{
    //abstract class MyInput : MyLabel
    //{
    //    DivElement _container;
    //    public DivElement Container => _container;

    //    public MyInput()
    //    {
    //        _label.Content = "Caption";
    //        _label.VerticalAlignment = VerticalAlignment.Top;
    //        _label.HorizontalAlignment = HorizontalAlignment.Left;

    //        this.Margin = new Thickness(5);
    //        this.Children.Add(_container = new DivElement {
    //            Margin = new Thickness(0, 25, 0, 0),
    //        });

    //        var border = (Border)_container.Children[0];
    //        border.BorderThickness = new Thickness(1);
    //        border.CornerRadius = new CornerRadius(4);
    //        border.BorderBrush = Brushes.LightGray;
    //        border.Padding = new Thickness(10, 5, 10, 5);
    //    }
    //    public string Caption
    //    {
    //        get
    //        {
    //            return (string)_label.Content;
    //        }
    //        set
    //        {
    //            _label.Content = value;
    //        }
    //    }
    //    public abstract object Value { get; set; }
    //    public string BindingName { get; set; }
    //    public bool Required { get; set; }

    //    public abstract UIElement GetInputElement();

    //    public override string Text
    //    {
    //        get => Value?.ToString();
    //        set => Value = value;
    //    }

    //    string _error;
    //    protected virtual void OnErrorChanged() { }
    //    public string Error
    //    {
    //        get => _error;
    //        set
    //        {
    //            _error = value;
    //            OnErrorChanged();
    //        }
    //    }
    //}

    public interface IMyInput
    {
        object Value { get; set; }
        string BindingName { get; set; }
        string Error { get; set; }
        string Caption { get; set; }
        bool Required { get; set; }
    }
    public abstract class MyInput<T> : PanelElement<Grid>, IMyInput
        where T : Control, new()
    {
        public string BindingName { get; set; }
        public string Error { get; set; }
        public bool Required { get; set; }

        protected TextElement<T> _inputContainer;
        protected MyLabel _label;

        public string Caption
        {
            get => (string)_label.Text;
            set => _label.Text = value;
        }
        public MyInput()
        {
            this.Add(_inputContainer = new TextElement<T> {
                Margin = new Thickness(0, 35, 0, 0),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4),
                Css = "input",
                Padding = new Thickness(10),
            });
            this.Add(_label = new MyLabel {
                Css = "input-caption",
                VerticalAlignment = VerticalAlignment.Bottom,
            });
            _label.Content.HorizontalAlignment = HorizontalAlignment.Left;

            Input.BorderThickness = new Thickness(0);
            this.PreviewMouseLeftButtonDown += (s, e) =>
            {
                _inputContainer.Content.Focus();
            };
            _inputContainer.Content.GotFocus += (s, e) => {
                SetLabelPosition();
            };
            _inputContainer.Content.LostFocus += (s, e) => {
                SetLabelPosition();
            };
            SetLabelPosition();
        }

        public T Input => _inputContainer.Content;

        public abstract object Value { get; set; }

        protected virtual void SetLabelPosition()
        {
            if (Input.IsFocused || this.Value != null)
            {
                _label.Margin = new Thickness(_inputContainer.CornerRadius.TopLeft, 0, 0, _inputContainer.ActualHeight);
            }
            else
            {
                _label.Margin = new Thickness(_inputContainer.Padding.Left, 0, 0, _inputContainer.Padding.Bottom);
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            SetLabelPosition();
        }
    }

    public class MyTextBox : MyInput<TextBox>
    {
        public override object Value
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Input.Text))
                    return null;
                return Input.Text;
            }

            set
            {
                Input.Text = value?.ToString();
                SetLabelPosition();
            }
        }
    }

    public class MyDateBox : MyTextBox
    {
        public override object Value
        {
            get
            {
                var dv = new int[] { 0, DateTime.Today.Month, DateTime.Today.Year };
                int i = 0;
                foreach (var s in Input.Text.Split(' ', '/', '.', '-'))
                {
                    int d;
                    if (int.TryParse(s, out d)) { dv[i] = d; }

                    if (++i >= dv.Length) { break; }
                }

                if (dv[2] < 100) dv[2] += 2000;
                return new DateTime(dv[2], dv[1], dv[0]);
            }

            set
            {
                base.Value = string.Format("{0:dd/MM/yyyy}", value);
            }
        }
    }

    public class MyPasswordBox : MyInput<PasswordBox>
    {
        public override object Value
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Input.Password))
                    return null;
                return Input.Password;
            }

            set
            {
                Input.Password = (string)value;
                SetLabelPosition();
            }
        }
    }

    public class MyComboBox : MyInput<ComboBox>
    {
        public MyComboBox()
        {
        }

        public override object Value
        {
            get
            {
                return Input.SelectedValue;
            }

            set
            {
                Input.SelectedValue = value;
            }
        }
        protected override void SetLabelPosition()
        {
            //base.SetLabelPosition();
        }
    }

    public class MyIntegerBox : MyTextBox
    {
        public MyIntegerBox()
        {
            Input.Text = "0";
            Input.PreviewKeyDown += (s, e) =>
            {
                if (!IsKeyValid(e.Key))
                {
                    e.Handled = true;
                }
            };
        }
        protected bool IsKeyValid(Key key)
        {
            return key == Key.Enter 
                || key == Key.Delete 
                || key == Key.Back 
                || key == Key.Left
                || key == Key.Right
                || key == Key.Tab
                || (key >= Key.D0 && key <= Key.D9);
        }

        public override object Value
        {
            get
            {
                var s = Input.Text;
                if (string.IsNullOrWhiteSpace(s))
                    return 0;

                return int.Parse(Input.Text);
            }

            set => base.Value = value;
        }
    }
}
