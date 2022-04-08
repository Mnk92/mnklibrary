using System.Collections.Generic;
using Mnk.Library.Common.Log;

namespace Mnk.Library.WpfControls.Code.Log
{
    public class BulkLog : AbstractLog
    {
        public IList<string> Values { get; private set; }

        public BulkLog()
        {
            Values = new List<string>();
        }

        public override void Write(string value)
        {
            Values.Add(value);
        }
    }
}
