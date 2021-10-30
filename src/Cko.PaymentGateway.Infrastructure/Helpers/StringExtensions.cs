using System.Linq;

namespace Cko.PaymentGateway.Infrastructure.Helpers
{
    public static class StringExtensions
    {
        public static string ReplaceWithStars(this string input, int skipChars, bool skipAtStart = false)
        {
            var result = string.Join("", Enumerable.Repeat('*', input.Length - skipChars));
            if (skipChars > 0)
            {
                if (skipAtStart)
                {
                    var original = input.Substring(0, skipChars);
                    result = string.Join("", new[] { original, result });
                }
                else
                {
                    var original = input.Substring(result.Length, input.Length - result.Length);
                    result = string.Join("", new[] { result, original });
                }
            }
            return result;
        }
    }
}