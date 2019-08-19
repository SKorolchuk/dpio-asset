using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Deeproxio.Infrastructure.Runtime
{
    public class SelfHostServerBuilder<T> : IDisposable where T : class, IServerStartup
    {
        private readonly string[] _args;
        private IWebHost _serverConfiguration;

        public SelfHostServerBuilder(string[] args)
        {
            _args = args;
        }

        public IWebHost Build()
        {
            var initialSetup = WebHost.CreateDefaultBuilder(_args)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<T>()
                .Build();

            return _serverConfiguration;
        }

        public void Dispose()
        {
            if (_serverConfiguration == null)
                return;

            _serverConfiguration.Dispose();
            _serverConfiguration = null;
        }
    }
}
