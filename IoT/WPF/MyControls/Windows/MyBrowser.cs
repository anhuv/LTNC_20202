using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DTimer = System.Windows.Threading.DispatcherTimer;

namespace System.Windows.Forms
{
    public class MyBrowser : Window
    {
        public static DTimer StartTimer(int ms, Func<bool> callback)
        {
            var timer = new DTimer {
                Interval = TimeSpan.FromMilliseconds(ms)
            };
            timer.Tick += (s, e) =>
            {
                if (!callback.Invoke())
                {
                    timer.Stop();
                    return;
                }
            };
            timer.Start();

            return timer;
        }
        public static DTimer StartTimer(int ms, int steps, Func<int, bool> callback)
        {
            var timer = new DTimer {
                Interval = TimeSpan.FromMilliseconds(ms)
            };
            var step = 0;
            timer.Tick += (s, e) => {
                if (!callback.Invoke(step))
                {
                    timer.Stop();
                    return;
                }

                if (++step == steps)
                {
                    timer.Stop();
                    return;
                }
            };
            timer.Start();

            return timer;
        }
    }
}
