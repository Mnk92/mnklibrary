using Grpc.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Mnk.Library.Interprocess
{
    public class InterprocessServer<TServer, TClient> : IDisposable
        where TServer : class
        where TClient : ClientBase<TClient>
    {
        private readonly WebApplication application;
        private readonly Thread thread;

        public string Handle { get; init; }
        public TServer Instance { get; init; }
        public Lazy<InterprocessClient<TClient>> Client { get; set; }
        public InterprocessServer(TServer instance)
        {
            Instance = instance;
            Handle = Path.Combine(Path.GetTempPath(), typeof(TServer).Name);
            var builder = WebApplication.CreateBuilder(Array.Empty<string>());
            builder.WebHost.ConfigureKestrel(options =>
            {
                if (File.Exists(Handle))
                {
                    File.Delete(Handle);
                }
                options.ListenUnixSocket(Handle, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http2;
                });
            });
            builder.Services.AddGrpc();
            builder.Services.AddSingleton<TServer>(instance);

            application = builder.Build();
            thread = new Thread(() => application.Run());
            Client = new Lazy<InterprocessClient<TClient>>(() => new InterprocessClient<TClient>(Handle));
        }

        public void Dispose()
        {
            try
            {
                if (Client.IsValueCreated)
                {
                    Client.Value.Dispose();
                }
            }
            finally
            {
                application.DisposeAsync().AsTask().Wait();
            }
            GC.SuppressFinalize(this);
        }
    }
}
