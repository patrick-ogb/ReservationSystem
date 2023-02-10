using FluentAssertions;
using Xunit;
namespace Reservation.Test
{
    public class CalculatorTest
    {
        [Fact]
        public void Test1() => Sum(2, 2).Should().Be(4);

        int Sum(int left, int right) => left + right ;
    }
}