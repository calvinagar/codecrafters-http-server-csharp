using System.Text;

class HttpRequest
{
    public string Method { get; }
    public string Path { get; }
    public string HttpVersion { get; }
    public string[] Headers { get; }

    public HttpRequest(byte[] request)
    {
        string[] requestLines = Encoding.ASCII.GetString(request).Split("\r\n");
        string[] firstLine = requestLines[0].Split(" ");

        Method = firstLine[0];
        Path = firstLine[1];
        HttpVersion = firstLine[2];
        Headers = requestLines.Skip(1).ToArray();
    }
}

public static class HttpRequestMethods
{
    public static string GET = "GET";
}