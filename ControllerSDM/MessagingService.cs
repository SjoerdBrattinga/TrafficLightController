using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ControllerSDM
{
    public abstract class MessagingService
    {
        private const string Server = "141.252.237.18";//"localhost";
        private const string VHost = "/10";
        private const string UserName = "softdev";
        private const string Password = "softdev";

        private EventingBasicConsumer _consumer;
        private IConnection _connectionSend, _connectionReceive;
        private IModel _channelSend, _channelReceive;
        private Dictionary<string, object> _args;

        public abstract void ConsumerOnReceived(object sender, BasicDeliverEventArgs ea);

        public void SetupMessaging()
        {
            try
            {
                _connectionSend = GetConnection();
                _connectionReceive = GetConnection();
                _channelSend = _connectionSend.CreateModel();
                _channelReceive = _connectionReceive.CreateModel();
                _consumer = new EventingBasicConsumer(_channelReceive);
                _args = new Dictionary<string, object> { { "x-message-ttl", 10000 } };
                DeclareQueues();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        public void ReceiveTrafficUpdates()
        {
            if (_consumer == null || _channelReceive == null) return;

            _consumer.Received += ConsumerOnReceived;
            _channelReceive.BasicConsume(queue: "controller", autoAck: true, consumer: _consumer);
        }

        public void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channelSend.BasicPublish(exchange: "",
                routingKey: "simulator",
                basicProperties: null,
                body: body);
        }

        private static IConnection GetConnection()
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = Server,
                VirtualHost = VHost,
                UserName = UserName,
                Password = Password
            };

            return connectionFactory.CreateConnection();
        }

        private void DeclareQueues()
        {
            _channelReceive.QueueDeclare(
                queue: "controller",
                durable: false,
                exclusive: false,
                autoDelete: true,
                arguments: _args
            );

            _channelSend.QueueDeclare(
                queue: "simulator",
                durable: false,
                exclusive: false,
                autoDelete: true,
                arguments: _args
            );
        }

        public void CloseConnection()
        {
            _channelSend.Close();
            _channelReceive.Close();
            _connectionSend.Close();
            _channelReceive.Close();
        }
    }
}
