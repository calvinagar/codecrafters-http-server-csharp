using System.Text;

class HttpRequest
{
    public string Method { get; }
    public string Path { get; }
    public string HttpVersion { get; }
    public Dictionary<string, string> Headers { get; }

    public HttpRequest(byte[] request)
    {
        string[] requestLines = Encoding.ASCII.GetString(request).Split("\r\n");
        string[] firstLine = requestLines[0].Split(" ");

        Method = firstLine[0];
        Path = firstLine[1];
        HttpVersion = firstLine[2];

        Headers = new Dictionary<string, string>();
        foreach (string line in requestLines[1..])
        {
            if (!line.Contains(":"))
                break;
                
            string[] splitHeader = line.Split(":");
            string key = splitHeader[0].Trim();
            string value = splitHeader[1].Trim();

            Headers.Add(key, value);
        }
    }

    public string FindHeader(string key)
    {
        return Headers[key];
    }
}

public static class HttpRequestMethods
{
    public static string GET = "GET";
}