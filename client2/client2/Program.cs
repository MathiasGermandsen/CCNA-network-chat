using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Client
{
  static void Main(string[] args)
  {
    Int32 port = 13000;
    string server = "127.0.0.1";

    try
    {
      TcpClient client = new TcpClient(server, port);

      NetworkStream stream = client.GetStream();

      Console.WriteLine("Connected to the server. Please enter your name:");
      string name = Console.ReadLine();

      Byte[] nameBytes = Encoding.ASCII.GetBytes(name);
      stream.Write(nameBytes, 0, nameBytes.Length);

      Thread receiveThread = new Thread(() =>
      {
        while (true)
        {
          Byte[] data = new Byte[256];
          string responseData = String.Empty;

          Int32 bytes = stream.Read(data, 0, data.Length);
          responseData = Encoding.ASCII.GetString(data, 0, bytes);
          Console.WriteLine("{0}", responseData);
        }
      });
      receiveThread.Start();

      while (true)
      {
        Console.Write(" ");
        string message = Console.ReadLine();

        Byte[] data = Encoding.ASCII.GetBytes(message);
        stream.Write(data, 0, data.Length);
      }

      client.Close();
    }
    catch (Exception e)
    {
      Console.WriteLine("Exception: {0}", e);
    }

    Console.WriteLine("\n Press Enter to continue...");
    Console.Read();
  }
}