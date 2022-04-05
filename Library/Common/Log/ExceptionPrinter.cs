using System;
using System.Linq;

namespace Mnk.Library.Common.Log
{
    public static class ExceptionPrinter
    {
        public static string Print(Exception ex, bool full = true)
        {
            if (ex == null) return String.Empty;
            var result = full ? ex.ToString() : ex.Message;
            if (ex.InnerException != null)
            {
                result += Environment.NewLine + Print(ex.InnerException, full);
            }

            if (ex is AggregateException aex)
            {
                return result + Environment.NewLine +
                       string.Join(Environment.NewLine,
                       aex.InnerExceptions.Select(x => Print(x, full)));
            }
            return result;
        }
    }
}
