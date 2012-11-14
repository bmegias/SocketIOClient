using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Common;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SocketIOClientSubRabbit
{
    class Program
    {
        static void Main(string[] args)
        {
            var exchg = "socketIOdemo";
            var queue = "socketIOdemoNetQ";

            var ser = new JavaScriptSerializer();
            var factory = new ConnectionFactory();
            factory.Protocol = Protocols.FromEnvironment();
            factory.HostName = "localhost";
            using (var con = factory.CreateConnection())
            using (var mod = con.CreateModel())
            {
                mod.QueueDeclare(queue, true, false, false, null);
                mod.ExchangeDeclare(exchg, ExchangeType.Fanout, true);
                mod.QueueBind(queue, exchg, string.Empty);


                var consumer = new QueueingBasicConsumer(mod);
                mod.BasicConsume(queue, false, consumer);
                Console.WriteLine("Waiting for messages...");
                while (true)
                {
                    var e = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
                    mod.BasicAck(e.DeliveryTag, false);
                    var props = e.BasicProperties;
                    byte[] body = e.Body;
                    var msg = ser.Deserialize<Message>(Encoding.UTF8.GetString(body));
                    Console.WriteLine(DateTime.Now.ToShortTimeString()+">> " + msg.body);
                }
            }
        }

    }

}
