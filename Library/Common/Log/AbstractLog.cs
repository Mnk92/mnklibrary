using System;
using System.Globalization;

namespace Mnk.Library.Common.Log
{
    public abstract class AbstractLog : ILog
    {
        public abstract void Write(string value);
        public virtual void Write(Exception ex, string value)
        {
            Write(string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}",
                value,
                Environment.NewLine,
                ExceptionPrinter.Print(ex)));
        }

        public void Write(string format, params object[] args)
        {
            Write(string.Format(CultureInfo.InvariantCulture, format, args));
        }

        public void Write(Exception ex, string format, params object[] args)
        {
            Write(ex, string.Format(CultureInfo.InvariantCulture, format, args));
        }
    }
}
