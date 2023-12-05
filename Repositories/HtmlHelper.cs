using System.Text.RegularExpressions;

namespace Repositories;

public static class HtmlHelper
{
    public static string GetYear(string str)
    {
        return Regex.Match(str, @"\d{4}").Value;
    }
}
