using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace ClientServer
{
    public class Server
    {
        private Socket socket;
        private IPEndPoint ipPoint;
        public List<Client> handler { get; }

        public List<Action<int>> actions { set; get; }
        public Dictionary<int,List<byte>> lastData { set; get; }

        public Server(string ip, int port)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ipPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            handler = new List<Client>();
            actions = new List<Action<int>>();
            lastData = new Dictionary<int, List<byte>>();
        }

        public void Start()
        {
            socket.Bind(ipPoint);
            socket.Listen(10);

        }
        public void AddClient(string ip, int port)
        {
            handler.Add(new Client(socket.Accept(), "127.0.0.1", 8000));
            this.Send(Server.FromStringToBytes("Connected"), handler.Count - 1);
        }
        public void Send(List<byte> data, int index)
        {
            handler[index].socket.Send(data.ToArray());
        }
        public List<byte> Get(int index)
        {

            List<byte> data = new List<byte>();
            int bytes = 0;
            byte[] array = new byte[255];

            try
            {

            do
            {
                bytes = handler[index].socket.Receive(array, array.Length, 0);
                for (int i = 0; i < bytes; i++)
                {
                    data.Add(array[i]);
                }


            } while (handler[index].socket.Available > 0);
            }
            catch (Exception e)
            {

            }
            if (lastData.ContainsKey(index))
            {
                lastData[index] = data;
            }
            else {
                lastData.Add(index,data);
            }

            return data;
        }
        public void RemoveClient(int index)
        {
            this.Send(Server.FromStringToBytes("Disconnected"), index);
            this.handler[index].socket.Close();
            this.handler.RemoveAt(index);
        }
        public void Close()
        {
            for (int i = 0; i < handler.Count; i++)
            {
                handler[i].socket.Shutdown(SocketShutdown.Both);
                handler[i].Close();
            }
        }
        public void ConnectionUpdate()
        {
           
                this.AddClient("127.0.0.1", 8000);

         
        }
        public static string FromBytesToString(List<byte> bytes)
        {
            return Encoding.Unicode.GetString(bytes.ToArray());
        }
        public static List<byte> FromStringToBytes(string str)
        {
            return Encoding.Unicode.GetBytes(str).ToList();
        }
    }
}
