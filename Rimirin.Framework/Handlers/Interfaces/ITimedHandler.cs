using System;
using System.Threading.Tasks;

namespace Rimirin.Framework.Handlers.Interfaces
{
    public interface ITimedHandler : IHandler
    {
        Task DoHandle(TimeSpan interval, DateTime accessTime);
    }
}