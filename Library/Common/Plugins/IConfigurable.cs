using System;

namespace Mnk.Library.Common.Plugins
{
    public interface IConfigurable
    {
        Type ConfigType { get; }
        object ConfigObject { get; set; }
    }
}
