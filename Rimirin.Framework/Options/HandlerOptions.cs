using Rimirin.Framework.Options.Interface;
using System.Collections.Generic;
using System.Reflection;

namespace Rimirin.Framework.Options
{
    public class HandlerOptions : IRimirinOptions
    {
        public List<TypeInfo> Handlers { get; set; } = new List<TypeInfo>();
    }
}