namespace TimeTrackerBlazorWitkac.Helpers;

public static class StringGenerator
{
    private static readonly Random Random = new Random();
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public static string GenerateRandomString(int length) =>
        new string(Enumerable.Repeat(Chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
}