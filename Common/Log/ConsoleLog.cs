using System;
using System.Globalization;
using System.Linq;

namespace Mnk.Library.Common.Log
{
    public class ConsoleLog : AbstractLog
    {
        public override void Write(string value)
        {
            var oldClr = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(value);
            Console.ForegroundColor = oldClr;
        }

        public override void Write(Exception ex, string value)
        {
            Write(string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", 
                value, 
                Environment.NewLine, 
                ExceptionPrinter.Print(ex, false)));
        }
    }
}
