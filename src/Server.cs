using System.Net;
using System.Net.Sockets;
using System.Text;

// You can use print statements as follows for debugging, they'll be visible when running tests.
Console.WriteLine("Logs from your program will appear here!");

// Uncomment this block to pass the first stage
TcpListener server = new TcpListener(IPAddress.Any, 4221);
server.Start();

var socket = server.AcceptSocket(); // wait for client

// read request from connection
var request = new byte[1024];
socket.Receive(request);

// parse request, explicitly path
string[] requestLines = Encoding.ASCII.GetString(request).Split("\r\n");

string [] startLine = requestLines[0].Split(" ");
string method = startLine[0];
string path = startLine[1];
string httpVersion = startLine[2];

// return ok if path is '/'
// otherwise 404
var ok = "200 OK";
var notFound = "404 NOT FOUND";
socket.Send(Encoding.ASCII.GetBytes($"HTTP/1.1 {(path == "/" ? ok : notFound)}\r\n\r\n"));