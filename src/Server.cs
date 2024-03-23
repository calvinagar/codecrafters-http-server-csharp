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

            Task.Run(() => {
                byte[] requestBuffer = new byte[1024];
                socket.Receive(requestBuffer);

                HttpRequest request = new HttpRequest(requestBuffer);

                HttpResponse response = HandleRequest(request);
                Console.WriteLine(response);

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
                response.setBody(Get(request));
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

    public static string Get(HttpRequest request)
    {
        // parse path
        string path = request.Path[1..];
        string[] splitPath = path.Split("/");
        string basePath = splitPath[0];

        switch (basePath)
        {
            case "":
                return "";

            case "echo":
                return path[(basePath.Length + 1)..];

            case "user-agent":
                return request.FindHeader("User-Agent");

            default:
                throw new Exception($"Path {path} not found.");
        }
    }
}