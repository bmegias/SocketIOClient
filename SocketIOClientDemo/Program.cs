using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using SocketIOClient;

namespace SocketIOClientDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting TestSocketIOClient Example...");

            var socket = new Client("http://192.168.80.1:3000/"); // url to nodejs 
            socket.Opened += (a, b) => { Console.WriteLine("Opened"); };
            socket.Message += (a, b) => { Console.WriteLine("Message"); };
            socket.SocketConnectionClosed +=  (a, b) => { Console.WriteLine("SocketConnectionClosed"); };
            socket.Error +=  (a, b) => { Console.WriteLine("Error"); };

            // register for 'connect' event with io server
            socket.On("connect", (fn) =>
            {
                Console.WriteLine("socket.On(connect)");
                // emit Json Serializable object, anonymous types, or strings
                var msg = new Message() { body = "Hello world!" };
                socket.Emit("msg", msg);
            });

            // register for 'update' events - message is a json 'Part' object
            socket.On("msg", (data) =>
            {
                //Console.WriteLine("recv [socket].[update] event");
                //Console.WriteLine("  raw message:      {0}", data.RawMessage);
                //Console.WriteLine("  string message:   {0}", data.MessageText);
                //Console.WriteLine("  json data string: {0}", data.Json.ToJsonString());
                //Console.WriteLine("  json raw:         {0}", data.Json.Args[0]);

                // cast message as Part - use type cast helper
                var msg = data.Json.GetFirstArgAs<Message>();
                Console.WriteLine(">> {0}", msg.body);
            });

            // make the socket.io connection
            socket.Connect();

            while (true) {
                Console.Write("> ");
                var txt = Console.ReadLine();
                var msg = new Message { body = txt };

                socket.Emit("msg", msg);       
            }
        }
    }
}
