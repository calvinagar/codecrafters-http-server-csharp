using System.Net;
using System.Net.Sockets;

static class Server
{
    private static TcpListener Listener = new TcpListener(IPAddress.Any, 4221);

    public static void Main(string[] args)
    {
        // You can use print statements as follows for debugging, they'll be visible when running tests.
        Console.WriteLine("Logs from your program will appear here!");

        Listener.Start();

        while (true)
        {
            Socket socket = Listener.AcceptSocket();

            byte[] requestBuffer = new byte[1024];
            socket.Receive(requestBuffer);

            HttpRequest request = new HttpRequest(requestBuffer);

            HttpResponse response = HandleRequest(request);
        
            socket.Send(response.ToBytes());
        }
    }

    public static HttpResponse HandleRequest(HttpRequest request)
    {
        HttpResponse response = new HttpResponse();

        if (request.Method == HttpRequestMethods.GET)
        {
            try
            {
                response.setBody(Get(request.Path));
                response.setCode(HttpResponseCodes.OK);
            }
            catch (Exception e)
            {
                response.setCode(HttpResponseCodes.NOT_FOUND);
            }
        } 
        else
        {
            response.setCode(HttpResponseCodes.NOT_ALLOWED);
        }

        return response;
    }

    public static string Get(string path)
    {
        // parse path
        string[] parsedPath = path.Split("/");
        string basePath = parsedPath[0];

        switch (basePath)
        {
            case "/":
                return "";

            case "echo":
                return parsedPath[1];

            default:
                throw new Exception($"Path {path} not found.");
        }
    }
}