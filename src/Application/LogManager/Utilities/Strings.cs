using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace MicroservicesLogger.Utilities
{
    [ExcludeFromCodeCoverage]
    public static partial class Strings
    {
        public static string Clean(string dirtValue)
        {
            dirtValue = Regex.Replace(dirtValue, @"\r\n?|\n|\r", " ");
            dirtValue = Regex.Replace(dirtValue, @"\t", " ");
            dirtValue = Regex.Replace(dirtValue, @"\s{2,}", " ");

            dirtValue = dirtValue.Trim();

            if (string.IsNullOrWhiteSpace(dirtValue))
                dirtValue = string.Empty;

            return dirtValue;
        }
    }
}
