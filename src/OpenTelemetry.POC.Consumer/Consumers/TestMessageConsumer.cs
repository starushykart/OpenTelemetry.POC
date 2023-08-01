using Contracts;
using MassTransit;

namespace OpenTelemetry.POC.Consumer.Consumers;

public class TestMessageConsumer : IConsumer<ITestMessage>
{
	public Task Consume(ConsumeContext<ITestMessage> context)
	{
		Console.Write("received");
		return Task.CompletedTask;
	}
}