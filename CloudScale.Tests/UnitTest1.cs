using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CloudScale.Core.Handlers.Ping;
using FluentAssertions;
using Xunit;

namespace CloudScale.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task ShouldPass()
        {
            var handler = new PingHandler();
            var request = new PingRequest {Message = "Hello World"};
            var response = await handler.Handle(request, CancellationToken.None);

            response.Message.Should().Be("Hello World");
        }

        [Fact]
        public async Task ShouldFailValidation()
        {
            var request = new PingRequest {Message = ""};
            var validator = new PingRequestValidator();
            var result = await validator.ValidateAsync(request, CancellationToken.None);
            result.IsValid.Should().BeFalse();
            result.Errors.Single().ErrorMessage.Should().Be("'Message' must not be empty.");
        }
    }
}