using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HTTPServer
{
    public class HTTPServer
    {
        public const string MSG_DIR = "/root/msg/";
        public const string WEB_DIR = "/root/web/";
        public const string Version = "HTTP/1.1";
        public const string Name = "Tobi Tortosa HTTP Server";

        private TcpListener _tcpListener;
        private bool _isRunning;

        public HTTPServer(int port)
        {
            _tcpListener = new TcpListener(IPAddress.Any, port);
            _isRunning = false;
        }

        public void Start()
        {
            Thread serverThread = new Thread(Run);
            serverThread.Start();
        }

        private void Run()
        {
            _isRunning = true;
            _tcpListener.Start();

            while (_isRunning)
            {
                Console.WriteLine("Waiting for connection...");
                TcpClient client = _tcpListener.AcceptTcpClient();
                Console.WriteLine("Client connected");

                HandleClient(client);
            }
        }

        private void HandleClient(TcpClient client)
        {
            using (NetworkStream stream = client.GetStream())
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
            {
                try
                {
                    Request req = Request.GetRequest(reader);
                    Response res = Response.From(req);
                    res.Post(writer);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error handling client: {ex.Message}");
                }
                finally
                {
                    client.Close();
                    Console.WriteLine("Client disconnected.");
                }
            }
        }
    }
}