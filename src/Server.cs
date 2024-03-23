using System.Net;
using System.Net.Sockets;

static class Server
{
    private static TcpListener Listener = new TcpListener(IPAddress.Any, 4221);
    private static string[]? Args;

    public static void Main(string[] args)
    {
        Args = args;

        Listener.Start();

        while (true)
        {
            Socket socket = Listener.AcceptSocket();

            Task.Run(() => {
                byte[] requestBuffer = new byte[1024];
                socket.Receive(requestBuffer);

                HttpRequest request = new HttpRequest(requestBuffer);

                HttpResponse response = HandleRequest(request);

                socket.Send(response.ToBytes());
            });
        }
    }

    public static HttpResponse HandleRequest(HttpRequest request)
    {
        HttpResponse response = new HttpResponse();
        if (request.Method == HttpRequestMethods.GET)
        {
            try
            {
                Get(request, response);
                response.setCode(HttpResponseCodes.OK);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                response.setCode(HttpResponseCodes.NOT_FOUND);
                response.clearBody();
            }
        }
        else if (request.Method == HttpRequestMethods.POST)
        {
            try
            {
                Post(request, response);
                response.setCode(HttpResponseCodes.CREATED);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                response.setCode(HttpResponseCodes.BAD_REQUEST);
                response.clearBody();
            }
        }
        else
        {
            response.setCode(HttpResponseCodes.NOT_ALLOWED);
            response.clearBody();
        }

        return response;
    }

    public static HttpResponse Get(HttpRequest request, HttpResponse response)
    {
        // Parse base path.
        string path = request.Path[1..];
        string[] splitPath = path.Split("/");
        string basePath = splitPath[0];

        if (basePath == "")
        {
            response.setBodyAsText("");
            return response;
        }

        switch (basePath)
        {
            case "echo":
                // Remove basePath from path.
                // + 1 for trailing '/'.
                response.setBodyAsText(path[(basePath.Length + 1)..]);
                break;

            case "user-agent":
                response.setBodyAsText(request.GetHeader("User-Agent"));
                break;

            case "files":
                // Remove basePath from path.
                // + 1 for trailing '/'.
                try { response.setBodyAsFile(GetFile(path[(basePath.Length + 1)..])); }
                catch { throw; }
                break;

            default:
                throw new Exception($"GET path '{basePath}' not found.");
        }

        return response;
    }

    public static string GetFile(string path)
    {
        // Parse "--directory" argument.
        if (!Args.Contains("--directory"))
            throw new Exception($"\"--directory\" command line argument was not passed in.");

        string directory = Args[Array.IndexOf(Args, "--directory") + 1];
        string filePath = directory + path;

        if (!File.Exists(filePath))
            throw new Exception($"File at {filePath} was not found.");

        return File.ReadAllText(filePath);
    }

    public static HttpResponse Post(HttpRequest request, HttpResponse response)
    {
        // Parse base path.
        string path = request.Path[1..];
        string[] splitPath = path.Split("/");
        string basePath = splitPath[0];

        // Remove basePath from path.
        // + 1 for trailing '/'.
        path = path[(basePath.Length + 1)..];
        switch (basePath)
        {
            case "files":
                try { PostFile(path, request.Body); }
                catch { throw; }
                break;

            default:
                throw new Exception($"POST path '{basePath}' not found.");
        }

        return response;
    }

    public static void PostFile(string path, string body)
    {
        // Parse "--directory" argument.
        if (!Args.Contains("--directory"))
            throw new Exception($"\"--directory\" command line argument was not passed in.");

        string directory = Args[Array.IndexOf(Args, "--directory") + 1];
        string filePath = directory + path;

        if (File.Exists(filePath))
            throw new Exception($"File at {filePath} already exists.");

        try
        {
            File.WriteAllText(filePath, body);
        }
        catch
        {
            throw new Exception($"Issue writing contents to '{filePath}'.");
        }

        return;
    }
}