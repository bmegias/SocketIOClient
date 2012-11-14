using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Common;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing.v0_9_1;

namespace SocketIOClientPubRabbit
{


    class Program
    {
        static void Main(string[] args)
        {
            var exchg = "socketIOdemo";

            Console.WriteLine("Creating factory...");
            var factory = new ConnectionFactory();

            Console.WriteLine("Creating connection...");
            factory.Protocol = Protocols.FromEnvironment();
            factory.HostName = "localhost";

            var ser = new JavaScriptSerializer();

            using (var conn = factory.CreateConnection())
            using (IModel ch = conn.CreateModel())
            {
                Console.WriteLine("Creating exchange...");
                ch.ExchangeDeclare(exchg, ExchangeType.Fanout, true);
                var props = new BasicProperties()
                {
                    ContentType = "application/json"
                };

                while (true)
                {
                    Console.Write("> ");
                    var txt=Console.ReadLine();
                    var msg = new Message{ body = txt };
                    var serMsg = ser.Serialize(msg);
                    var messageBody = Encoding.UTF8.GetBytes(serMsg);
                    ch.BasicPublish(exchg, string.Empty, props, messageBody);                }
            }
        }
    }
}
