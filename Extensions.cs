public static class Extensions
{
    public static bool IsNumeric(this string input)
    {
        int number;
        return int.TryParse(input, out number);
    }
}