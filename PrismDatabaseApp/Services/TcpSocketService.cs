using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public interface ITcpSocketService
{
    event Action<string> DataReceived;
    void StartListening(string ip, int port);
    void StartListening(IPAddress any, int v);
    void StopListening();
}

public class TcpSocketService : ITcpSocketService
{
    private TcpListener _listener;
    private bool _isRunning;

    public event Action<string> DataReceived;

    public void StartListening(string ip, int port)
    {
        _listener = new TcpListener(IPAddress.Parse(ip), port);
        _listener.Start();
        _isRunning = true;

        Task.Run(async () =>
        {
            while (_isRunning)
            {
                try
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    _ = HandleClientAsync(client);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Listener error: {ex.Message}");
                    await Task.Delay(1000); // 재시도 대기 시간
                }
            }
        });
    }

    public void StartListening(IPAddress any, int v)
    {
        _listener = new TcpListener(IPAddress.Any, v); // 모든 IP에서 연결 수락
        _listener.Start();
        _isRunning = true;

        Task.Run(async () =>
        {
            while (_isRunning)
            {
                try
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    _ = HandleClientAsync(client);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Listener error: {ex.Message}");
                    await Task.Delay(1000); // 재시도 대기 시간
                }
            }
        });
    }

    public void StopListening()
    {
        _isRunning = false;
        _listener?.Stop();
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        try
        {
            var stream = client.GetStream();
            var buffer = new byte[1024];
            while (_isRunning && client.Connected)
            {
                var byteCount = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (byteCount == 0) break;

                var data = Encoding.UTF8.GetString(buffer, 0, byteCount);
                DataReceived?.Invoke(data);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Client error: {ex.Message}");
        }
        finally
        {
            client.Close();
        }
    }
}
