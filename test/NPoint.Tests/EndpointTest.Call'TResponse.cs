using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace NPoint.Tests
{
    public partial class EndpointTest
    {
        public class CallOfTResponse
        {
            [Fact]
            public void ShouldThrowWhenArgumentIsNull()
            {
                // Arrange
                var converter = default(Func<string, ResponseDto>);

                // Act
                var sut = new Endpoint();
                Func<Task<ResponseDto>> activity = async () => await sut.Call(converter);

                // Assert
                activity.ShouldThrowExactly<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo(nameof(converter));
            }

            public class ResponseDto
            {
                public string Name { get; set; }
            }
        }
    }
}
