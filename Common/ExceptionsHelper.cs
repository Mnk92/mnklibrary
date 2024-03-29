﻿using System;
using Mnk.Library.Common.Log;

namespace Mnk.Library.Common
{
    public static class ExceptionsHelper
    {
        public static bool HandleException(Action action, Action<Exception> onException)
        {
            try
            {
                action();
                return false;
            }
            catch (Exception ex)
            {
                onException(ex);
            }
            return true;
        }

        public static bool HandleException(Action action, Func<string> messageGetter, ILog log)
        {
            return HandleException(action, ex => log.Write(ex, messageGetter()));
        }
    }
}
