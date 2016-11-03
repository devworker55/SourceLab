﻿using MassTransit;
using System;
using System.Configuration;

namespace MassTransitSample
{
    public sealed class BusInitializer
    {
        public static IBusControl CreateBus(Uri queueUri, Action<IRabbitMqBusFactoryConfigurator> appendConfigurations)
        {
            var userName = ConfigurationManager.AppSettings["RabbitMQ.UserName"];
            var password = ConfigurationManager.AppSettings["RabbitMQ.Password"];

            IBusControl _mqServiceBus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                if (queueUri != null && queueUri.Scheme.Equals("rabbitmq", StringComparison.InvariantCultureIgnoreCase))
                {
                    var host = sbc.Host(queueUri, h =>
                    {
                        h.Username(userName);
                        h.Password(password);
                    });

                    sbc.UseRetry(Retry.Immediate(5));

                    sbc.ReceiveEndpoint(host, "input_queue", ep =>
                    {
                        //ep.Consumer<MyConsumer>();
                    });
                }
                else
                {
                    throw new NotSupportedException("RabbitMQ is the only supported message broker at this time.");
                }

                appendConfigurations.Invoke(sbc);
            });

            return _mqServiceBus;
        }
    }
}
