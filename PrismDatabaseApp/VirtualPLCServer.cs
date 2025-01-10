using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class VirtualPLCServer
{
    private double _currentValue;
    private bool _increasing;
    private TcpListener _listener;

    public VirtualPLCServer(int port)
    {
        _currentValue = 50.0;
        _increasing = true;
        _listener = new TcpListener(IPAddress.Any, port);
    }

    public void Start()
    {
        _listener.Start();
        Console.WriteLine("PLC Server started.");

        // Value updater in a separate thread
        new Thread(UpdateValue).Start();

        while (true)
        {
            var client = _listener.AcceptTcpClient();
            var stream = client.GetStream();
            var response = Encoding.ASCII.GetBytes(_currentValue.ToString("F2"));
            stream.Write(response, 0, response.Length);
            client.Close();
        }
    }

    private void UpdateValue()
    {
        var random = new Random();
        while (true)
        {
            _currentValue += _increasing ? random.NextDouble() : -random.NextDouble();
            if (_currentValue >= 100) _increasing = false;
            if (_currentValue <= 0) _increasing = true;

            Console.WriteLine($"Current Value: {_currentValue:F2}");
            Thread.Sleep(1000);
        }
    }
}
