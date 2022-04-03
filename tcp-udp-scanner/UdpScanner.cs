using System.Net;
using System.Net.Sockets;
using System.Text;

namespace tcp_udp_scanner;

public class UdpScanner
{
    public static void ScanIp(IPAddress ip, int fromPort = MinPort, int toPort = MaxPort)
    {
        if (fromPort is < MinPort or > MaxPort || toPort is < MinPort or > MaxPort)
            throw new ArgumentException($"Port number should be in range [{MinPort}, {MaxPort}]");
        var tasks = new List<Task>();
        var openCounter = 0;
        var data = new byte[48];
        for (var port = fromPort; port < toPort + 1; port++)
        {
            var port1 = port;
            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    using var udpClient = new UdpClient();
                    udpClient.Connect(ip, port1);
                    udpClient.Client.ReceiveTimeout = Timeout;
                    await udpClient.SendAsync(data, 48);
                    var result = udpClient.ReceiveAsync();
                    if (await Task.WhenAny(result, Task.Delay(Timeout)) == result)
                    {
                        Console.WriteLine(Encoding.Default.GetString(result.Result.Buffer));
                    }
                    else
                    {
                        Console.WriteLine($"{ip}:{port1} -- OPEN");
                        openCounter++;
                    }

                    udpClient.Close();
                }
                catch (Exception)
                {
                    // ignored
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());
        Console.WriteLine($"OPEN: {openCounter}/{toPort - fromPort + 1}");
    }

    private static int Timeout = 5000;
    private const int MaxPort = 65536;
    private const int MinPort = 1;
}