using Rimirin.Framework.Options.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Rimirin.Framework.Options
{
    public class HandlerOptions : IRimirinOptions
    {
        public List<TypeInfo> Handlers { get; set; } = new List<TypeInfo>();
    }
}
