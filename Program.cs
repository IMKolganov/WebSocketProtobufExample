using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using Google.Protobuf;
// using GrpcServiceClientCoreExample;

class Program
{
    private static ConcurrentDictionary<Guid, WebSocket> _webSockets = new ConcurrentDictionary<Guid, WebSocket>();

    static async void ProcessWebSocketRequest(HttpListenerContext context)
    {
        var webSocketContext = await context.AcceptWebSocketAsync(null);
        var webSocket = webSocketContext.WebSocket;
        var webSocketId = Guid.NewGuid();
        _webSockets.TryAdd(webSocketId, webSocket);

        Console.WriteLine($"WebSocket connection established. ConnectionId: {webSocketId}");

        var receiveBuffer = new byte[1024];
        while (webSocket.State == WebSocketState.Open)
        {
            var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
            if (receiveResult.MessageType == WebSocketMessageType.Close)
            {
                Console.WriteLine($"WebSocket connection closed. ConnectionId: {webSocketId}");
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed by server.", CancellationToken.None);
                _webSockets.TryRemove(webSocketId, out _);
            }
            else if (receiveResult.MessageType == WebSocketMessageType.Binary)
            {
                // var receivedMessage = YourMessage.Parser.ParseFrom(receiveBuffer, 0, receiveResult.Count);
                //
                // Console.WriteLine($"Received message: {receivedMessage.Name}. ConnectionId: {webSocketId}");
                //
                // // Пример ответного сообщения
                // var responseMessage = new YourMessage
                // {
                //     Name = $"Server received: {receivedMessage.Name}"
                // };

                // var responseBuffer = responseMessage.ToByteArray();
                //
                // foreach (var socket in _webSockets.Values)
                // {
                //     await socket.SendAsync(new ArraySegment<byte>(responseBuffer), WebSocketMessageType.Binary, true, CancellationToken.None);
                // }
            }
        }
    }

    static void Main(string[] args)
    {
        var listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:8080/"); // Установите желаемый URL для WebSocket сервера
        listener.Start();
        Console.WriteLine("WebSocket server started.");

        while (true)
        {
            var context = listener.GetContext();
            if (context.Request.IsWebSocketRequest)
            {
                ProcessWebSocketRequest(context);
            }
            else
            {
                context.Response.StatusCode = 400;
                context.Response.Close();
            }
        }
    }
}