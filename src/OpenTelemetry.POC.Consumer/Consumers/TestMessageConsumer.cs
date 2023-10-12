using Contracts;
using MassTransit;

namespace OpenTelemetry.POC.Consumer.Consumers;

public class TestMessageConsumer : IConsumer<ITestMessage>
{
	private readonly ILogger<TestMessageConsumer> _logger;

	public TestMessageConsumer(ILogger<TestMessageConsumer> logger)
		=> _logger = logger;
	
	public Task Consume(ConsumeContext<ITestMessage> context)
	{
		_logger.LogInformation("Log from OpenTelemetry.POC.Consumer");
		return Task.CompletedTask;
	}
}