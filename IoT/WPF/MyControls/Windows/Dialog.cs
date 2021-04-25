using System;
using System.Collections.Generic;
using System.Linq;
using System.Mvc;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace System.Windows.Forms
{
    public interface IModal
    {
        bool? ShowDialog();
    }
    public class Dialog : Window, IModal
    {
        MyLabel _banner = new MyLabel { Css = "dlg-header", Text = "Template" };
        StyleElement _body = new StyleElement();

        PanelElement<StackPanel> _footer;

        MyButton _btnOK;
        public MyButton AcceptButton
        {
            get
            {
                if (_btnOK == null)
                {
                    _btnOK = new MyButton { };
                    _btnOK.Click += e =>
                    {
                        if (UpdateCompleted == null || UpdateCompleted.Invoke())
                        {
                            this.DialogResult = true;
                        }
                    };
                }
                return _btnOK;
            }
        }

        MyButton _btnCancel;
        public MyButton CancelButton
        {
            get
            {
                if (_btnCancel == null)
                {
                    _btnCancel = new MyButton
                    {
                        Css = "black"
                    };
                    _btnCancel.Click += e => this.DialogResult = false;
                }
                return _btnCancel;
            }
        }

        public MyLabel Banner => _banner;
        public UIElement Body
        {
            get => _body.Child;
            set => _body.Child = value;
        }

        public Dialog()
        {
            var div = new PanelElement<StackPanel>
            {
                //VerticalAlignment = VerticalAlignment.Bottom,
                Padding = new Thickness(10),
                //Height = 60,
                //Width = 400,
                Css = "dlg-footer",
            };

            div.Content.Orientation = Controls.Orientation.Horizontal;
            div.Content.HorizontalAlignment = Windows.HorizontalAlignment.Center;
            div.Add(AcceptButton);

            _footer = div;

            SizeToContent = SizeToContent.WidthAndHeight;
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            var border = new Border
            {
                BorderThickness = new Thickness(2),
                BorderBrush = new LinearGradientBrush(Colors.LightBlue, Colors.DarkBlue, 45),
            };

            var mainContent = new MyTableLayout();
            mainContent.Add(0, 0, _banner);
            mainContent.Add(1, 0, _body);
            mainContent.Add(2, 0, _footer);

            mainContent.SetHeights(40, 0, 60);

            this.Content = new Grid {
                Children = {
                    mainContent, border
                },
                MinWidth = 450,
            };
        }
        public Dialog(string text, string accept, string cancel)
            : this()
        {
            Body = new MyLabel {
                Text = text,
                Margin = new Thickness(50),
                Css = "dlg-body",
            };
            _btnOK.Text = accept;
            if (cancel != null)
            {
                CancelButton.Text = cancel;
                _footer.Add(_btnCancel);
            }
        }
        public Dialog(string text) : this(text, "OK", null)
        {
        }

        public Func<bool> UpdateCompleted;

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            this.Opacity = 0.5;
            MyBrowser.StartTimer(20, () => {
                return ((this.Opacity += 0.05) < 1);
            });
        }
    }
}
