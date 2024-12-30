namespace HTTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 4222;
            Console.WriteLine($"Starting server on port: {port}");
            HTTPServer server = new HTTPServer(port);
            server.Start();
        }
    }
}