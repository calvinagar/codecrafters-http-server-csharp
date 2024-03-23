using System.Text;

class HttpResponse
{
    private string HttpVersion { get; }
    private string? Code { get; set; }

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

    public void setBodyAsText(string body)
    {
        if (body == "") 
            return;

        ContentType = ContentTypes.Text;
        ContentLength = body.Length;
        Body = body;
    }

    public void setBodyAsFile(string body)
    {
        if (body == "") 
            return;

        ContentType = ContentTypes.File;
        ContentLength = body.Length;
        Body = body;
    }

    public void clearBody()
    {
        ContentType = null;
        ContentLength = null;
        Body = null;
    }

    public override string ToString()
    {
        StringBuilder response = new StringBuilder();
        
        response.Append($"{HttpVersion} {Code}\r\n");
        
        // via Hint on bottom of page of "Get a file" step
        response.Append($"Content-Length: {ContentLength ?? 0}\r\n");

        if (Body != null)
        {
            response.Append($"Content-Type: {ContentType}\r\n");
            response.Append($"\r\n");

            response.Append(Body);
            response.Append($"\r\n");
        }

        response.Append($"\r\n");

        return response.ToString();
    }

    public byte[] ToBytes()
    {
        return Encoding.ASCII.GetBytes(this.ToString());
    }
}

public static class HttpResponseCodes
{
    public static string OK = "200 OK";
    public static string CREATED = "201 CREATED";
    public static string BAD_REQUEST = "400 BAD REQUEST";
    public static string NOT_FOUND = "404 NOT FOUND";
    public static string NOT_ALLOWED = "405 METHOD NOT ALLOWED";
}

public static class ContentTypes
{
    public static string Text = "text/plain";
    public static string File = "application/octet-stream";
}