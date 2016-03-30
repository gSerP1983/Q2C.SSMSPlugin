using System;
using System.Linq;
using System.Threading;
using Q2C.Core.Commands;

namespace Q2C.Core
{
    public static class Factory
    {
        private static readonly Lazy<ICommand[]> Commands = new Lazy<ICommand[]>(GetCommandsInner, LazyThreadSafetyMode.None);

        public static ICommand[] GetCommands()
        {
            return Commands.Value;
        }

        private static ICommand[] GetCommandsInner()
        {
            var interfaceType = typeof (ICommand);
            return interfaceType.Assembly.GetTypes()
                .Where(x => x.IsImplementInterface(interfaceType))
                .Select(x => Activator.CreateInstance(x) as ICommand)
                .OrderBy(x => x.Name)
                .ToArray()
                ;
        }
    }
}