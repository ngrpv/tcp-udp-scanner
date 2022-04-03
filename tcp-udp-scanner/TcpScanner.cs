using System.Net;
using System.Net.Sockets;

namespace tcp_udp_scanner;

public class TcpScanner
{
    public static void ScanIp(IPAddress ip, int fromPort = MinPort, int toPort = MaxPort)
    {
        if (fromPort is < MinPort or > MaxPort || toPort is < MinPort or > MaxPort)
            throw new ArgumentException($"Port number should be in range [{MinPort}, {MaxPort}]");
        var tasks = new List<Task>();
        for (var port = fromPort; port < toPort + 1; port++)
        {
            var temp = port;
            tasks.Add(Task.Run(async () =>
            {
                using var tcp = new TcpClient();
                tcp.ReceiveTimeout = 2000;
                try
                {
                    await tcp.ConnectAsync(ip, temp);
                }
                catch (Exception)
                {
                    return;
                }

                Console.WriteLine($"{ip}:{temp} -- OPEN");
            }));
        }

        Task.WaitAll(tasks.ToArray());
    }

    private const int MaxPort = 65536;
    private const int MinPort = 1;
}