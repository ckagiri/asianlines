using System;
using System.Diagnostics;

namespace Ligi.Core.Utils
{
    public static class Clock
    {
        private static Func<DateTime> _now = () => DateTime.UtcNow;

        public static Func<DateTime> Now
        {
            [DebuggerStepThrough]
            get { return _now; }

            [DebuggerStepThrough]
            set
            {
                _now = value;
            }
        }

        public static void Reset()
        {
            _now = () => DateTime.UtcNow;
        }
    }
}
