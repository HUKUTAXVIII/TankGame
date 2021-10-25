using System;
using ClientServer;
using TankLib;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace ServerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server("127.0.0.1",8000);
            List<Task> tankThreads = new List<Task>();
            server.Start();

            Task clientsUpdate = new Task(()=> {
                while (true)
                {
                    server.ConnectionUpdate();
                    tankThreads.Add(new Task(()=> {
                        string ip = server.handler.Last().socket.RemoteEndPoint.ToString();
                        int index = server.handler.IndexOf(server.handler.Where((item)=>item.socket.RemoteEndPoint.ToString() == ip).First());
                        Console.WriteLine(ip);
                        while (true)
                        {

                            //if (!server.handler.Any((item) => item.socket.RemoteEndPoint.ToString() == ip))
                            //{
                            //    server.handler.RemoveAt(index);
                            //    break;
                            //}
                            if (!server.handler[index].socket.Connected) {
                                server.handler.RemoveAt(index);
                                break;
                            }
                            
                                Console.WriteLine(index+" - "+server.handler.Count);
                                var data = server.Get(index);
                                if (data.Count != 0)
                                {


                                    try
                                    {

                                        Tank info = JsonSerializer.Deserialize<Tank>(Server.FromBytesToString(data));

                                        //Console.WriteLine(info.Location.X + " " + info.Location.Y);
                                    }
                                    catch (Exception)
                                    {
                                        Console.WriteLine("Lost Bytes");

                                    }

                                }
                                GC.Collect(GC.GetGeneration(data));

                                List<Tank> tanks = new List<Tank>();
                                foreach (var item in server.lastData.Where(item => item.Key != index).ToList()) {
                                    tanks.Add(JsonSerializer.Deserialize<Tank>(Client.FromBytesToString(item.Value)));        
                                }

                            for (int i = 0; i < server.handler.Count; i++) {
                                if (i != index)
                                {
                                    server.Send(Server.FromStringToBytes(JsonSerializer.Serialize<List<Tank>>(tanks)), i);
                                }
                            }
                            
                        }


                    }));
                    tankThreads.Last().Start();
                }
            });
            clientsUpdate.Start();



            try
            {  
                
                Console.ReadLine();
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }

            server.Close();

        }
    }
}
