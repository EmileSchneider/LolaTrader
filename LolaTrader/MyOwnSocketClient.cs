using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using Binance.Net.Clients;

namespace Lola;

class UserDataStreamSocket
{
    CancellationTokenSource disposalTokenSource = new CancellationTokenSource();
    ClientWebSocket webSocket = new ClientWebSocket();
    private ConcurrentQueue<string> messages;

    public string UserDataStreamKey { get; set; }

    public UserDataStreamSocket(string userDataStreamKey, ConcurrentQueue<string> messageQueue)
    {
        UserDataStreamKey = userDataStreamKey;
        messages = messageQueue;
    }

    public async Task Run()
    {
        await webSocket.ConnectAsync(new Uri($"wss://testnet.binance.vision/ws/{UserDataStreamKey}"),
            disposalTokenSource.Token);
        _ = ReceiveLoop();
    }

    async Task ReceiveLoop()
    {
        var buffer = new ArraySegment<byte>(new byte[1024 * 64]);
        while (!disposalTokenSource.IsCancellationRequested)
        {
            // Note that the received block might only be part of a larger message. If this applies in your scenario,
            // check the received.EndOfMessage and consider buffering the blocks until that property is true.
            // Or use a higher-level library such as SignalR.
            var received = await webSocket.ReceiveAsync(buffer, disposalTokenSource.Token);
            var receivedAsText = Encoding.UTF8.GetString(buffer.Array, 0, received.Count);
            messages.Enqueue(receivedAsText);
        }
    }

    public void Dispose()
    {
        disposalTokenSource.Cancel();
        _ = webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Bye", CancellationToken.None);
    }
}