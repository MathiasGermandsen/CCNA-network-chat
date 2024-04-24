using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
  private static TcpListener server = null;
  private static Dictionary<TcpClient, string> clients = new Dictionary<TcpClient, string>();

  static void Main(string[] args)
  {
    Int32 port = 13000;
    IPAddress localAddr = IPAddress.Parse("127.0.0.1");

    server = new TcpListener(localAddr, port);

    server.Start();

    while (true)
    {
      Console.Write("Waiting for a connection... ");

      TcpClient client = server.AcceptTcpClient();
      Console.WriteLine("Connected!");

      Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
      clientThread.Start(client);
    }
  }

  private static void HandleClient(object obj)
  {
    TcpClient client = (TcpClient)obj;

    Byte[] bytes = new Byte[256];
    String data = null;

    NetworkStream stream = client.GetStream();

    // Ask for the client's name
    byte[] msg = System.Text.Encoding.ASCII.GetBytes("Please enter your name:");
    stream.Write(msg, 0, msg.Length);

    int i;
    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
    {
      data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

      // If the client's name is not set, set it
      if (!clients.ContainsKey(client))
      {
        clients[client] = data;
        Console.WriteLine("{0} has joined the chat", data);
        continue;
      }

      Console.WriteLine("Received: {0}: {1}", clients[client], data);

      msg = System.Text.Encoding.ASCII.GetBytes(clients[client] + ": " + data);

      foreach (var c in clients.Keys)
      {
        if (c != client)
        {
          c.GetStream().Write(msg, 0, msg.Length);
        }
      }

      Console.WriteLine("Sent: {0}", data);
    }
  }
}