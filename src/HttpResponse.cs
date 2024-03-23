using System.Text;

class HttpResponse
{
    private string HttpVersion { get; }
    private string Code { get; set; }

    private string? ContentType { get; set; }
    private int? ContentLength { get; set; }
    private string? Body { get; set; }

    public HttpResponse()
    {
        HttpVersion = "HTTP/1.1";
    }

    public void setCode(string code)
    {
        Code = code;
    }

    public void setBody(string body)
    {
        if (body == "") 
            return;

        ContentType = ContentTypes.Text;
        ContentLength = body.Length;
        Body = body;
    }

    public byte[] ToBytes()
    {
        StringBuilder response = new StringBuilder();
        
        response.Append($"{HttpVersion} {Code}\r\n");

        if (Body != null)
        {
            response.Append($"Content-Type: {ContentType}\r\n");
            response.Append($"Content-Length: {ContentLength}\r\n");
            response.Append($"\r\n");

            response.Append(Body);
            response.Append($"\r\n");
        }

        response.Append($"\r\n");

        return Encoding.ASCII.GetBytes(response.ToString());
    }
}

public static class HttpResponseCodes
{
    public static string OK = "200 OK";
    public static string NOT_FOUND = "404 NOT FOUND";
    public static string NOT_ALLOWED = "405 METHOD NOT ALLOWED";
}

public static class ContentTypes
{
    public static string Text = "text/plain";
}