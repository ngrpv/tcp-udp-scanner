// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Sockets;
using tcp_udp_scanner;

IPAddress ipAddress = null;
while (true)
{
    Console.WriteLine("Enter ip address or domain:");
    try
    {
        ipAddress = Dns.GetHostAddresses(Console.ReadLine()).First();
    }
    catch (SocketException e)
    {
        Console.WriteLine(e.Message);
    }

    if (ipAddress != null)
        break;
}

if (ipAddress.ToString() == "::1")
{
    ipAddress = IPAddress.Loopback;
}

Console.WriteLine($"Processing {ipAddress}");
var from = ParseFromConsole("Enter number of port to start in range [1, 65536]: ", 1, 65536);
var to = ParseFromConsole($"Enter number of port to end in range [{from}, 65536]: ", from, 65536);
bool? isTcp = null;
do
{
    Console.WriteLine("Tcp/Udp:");
    var input = Console.ReadLine();
    if (input?.ToLower() == "udp")
        isTcp = false;
    if (input?.ToLower() == "tcp")
        isTcp = true;
} while (isTcp == null);

if (isTcp == true)
{
    TcpScanner.ScanIp(ipAddress, from, to);
}
else
{
    UdpScanner.ScanIp(ipAddress, from, to);
}

int ParseFromConsole(string message, int min, int max)
{
    do
    {
        Console.WriteLine(message);
        if (int.TryParse(Console.ReadLine(), out var val) && val <= max && val >= min)
            return val;
        Console.WriteLine("Invalid input...");
    } while (true);
}