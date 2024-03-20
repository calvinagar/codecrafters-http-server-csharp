using System.Net;
using System.Net.Sockets;
using System.Text;

// You can use print statements as follows for debugging, they'll be visible when running tests.
Console.WriteLine("Logs from your program will appear here!");

// Uncomment this block to pass the first stage
TcpListener server = new TcpListener(IPAddress.Any, 4221);
server.Start();

var socket = server.AcceptSocket(); // wait for client

// read data from connection
var data = new byte[1024];
socket.Receive(data);

// return 'HTTP/1.1 200 OK\r\n\r\n'
var response = "HTTP/1.1 200 OK\r\n\r\n";
socket.Send(Encoding.ASCII.GetBytes(response));