using Xunit;
namespace TechMove.Tests;
public class CurrencyCalculationTests { [Theory] [InlineData(100,18.5,1850)] [InlineData(10,18.5,185)] [InlineData(1.25,18.5,23.13)] public void UsdToZar_IsRoundedToTwoDecimals(decimal usd, decimal rate, decimal expected) { var actual=Math.Round(usd*rate,2,MidpointRounding.AwayFromZero); Assert.Equal(expected,actual); } }