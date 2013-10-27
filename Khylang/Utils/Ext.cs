using System;
using System.Collections;
using System.Linq;

namespace Khylang.Utils
{
    public static class Ext
    {
        public static Func<TIn, TOut> Fix<TIn, TOut>(Func<Func<TIn, TOut>, TIn, TOut> action)
        {
            return p => action(Fix(action), p);
        }

        public static string Format(this object toFormat)
        {
            var os = toFormat as IEnumerable;
            return os == null ? toFormat.ToString() : string.Format("{{{0}}}", string.Join(", ", os.Cast<object>().Select(o => o.Format())));
        }
    }
}