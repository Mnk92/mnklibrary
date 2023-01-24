using System.Net;
using System.Net.Sockets;
using Grpc.Core;
using Grpc.Net.Client;

namespace Mnk.Library.Interprocess
{
    public class InterprocessClient<T> : IDisposable
    where T : ClientBase<T>
    {
        class UnixDomainSocketConnectionFactory
        {
            private readonly EndPoint endPoint;

            public UnixDomainSocketConnectionFactory(EndPoint endPoint)
            {
                this.endPoint = endPoint;
            }

            public async ValueTask<Stream> ConnectAsync(SocketsHttpConnectionContext _,
                CancellationToken cancellationToken = default)
            {
                var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);

                try
                {
                    await socket.ConnectAsync(endPoint, cancellationToken).ConfigureAwait(false);
                    return new NetworkStream(socket, true);
                }
                catch
                {
                    socket.Dispose();
                    throw;
                }
            }
        }

        private readonly GrpcChannel channel;
        public T Instance { get; }
        public string Handle { get; }

        public InterprocessClient(string handle)
        {
            Handle = handle;
            var udsEndPoint = new UnixDomainSocketEndPoint(handle);
            var connectionFactory = new UnixDomainSocketConnectionFactory(udsEndPoint);
            var socketsHttpHandler = new SocketsHttpHandler
            {
                ConnectCallback = connectionFactory.ConnectAsync
            };

            channel = GrpcChannel.ForAddress("http://localhost", new GrpcChannelOptions
            {
                HttpHandler = socketsHttpHandler
            });
            Instance = (T)typeof(T).GetConstructor(new[] { typeof(GrpcChannel) })?.
                Invoke(new object[] { channel });
        }

        public void Dispose()
        {
            channel.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}