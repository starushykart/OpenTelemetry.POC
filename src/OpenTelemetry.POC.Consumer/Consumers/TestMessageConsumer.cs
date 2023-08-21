using Contracts;
using MassTransit;

namespace OpenTelemetry.POC.Consumer.Consumers;

public class TestMessageConsumer : IConsumer<ITestMessage>
{
	public Task Consume(ConsumeContext<ITestMessage> context)
	{
		return Task.CompletedTask;
	}
}