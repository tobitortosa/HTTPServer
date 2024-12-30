using System.Text;

namespace HTTPServer
{
    public class Response
    {
        private string _status;
        private string _mime;
        private byte[] _data;

        private Response(string status, string mime, byte[] data)
        {
            _status = status;
            _mime = mime;
            _data = data;
        }

        public static Response From(Request request)
        {
            if (request == null)
                return MakeNullRequest();

            if (request.Type == "GET")
            {
                string filePath = Environment.CurrentDirectory + HTTPServer.WEB_DIR + request.Url.TrimStart('/');
                Console.WriteLine($"File Path: {filePath}");

                if (File.Exists(filePath))
                {
                    return MakeFromFile(new FileInfo(filePath));
                }

                string[] defaultFiles = { "index.html", "index.htm", "default.html", "default.htm" };

                foreach (string defaultFile in defaultFiles)
                {
                    string defaultFilePath = Environment.CurrentDirectory + HTTPServer.WEB_DIR + request.Url.TrimStart('/') + defaultFile;
                    if (File.Exists(defaultFilePath))
                    {
                        return MakeFromFile(new FileInfo(defaultFilePath));
                    }
                }

                return MakePageNotFound();
            }

            return MakeMethodNotAllowed();
        }

        public void Post(StreamWriter writer)
        {
            try
            {
                string responseHeaders = string.Format(
                    "{0} {1}\r\n" +
                    "Server: {2}\r\n" +
                    "Content-Type: {3}\r\n" +
                    "Content-Length: {4}\r\n" +
                    "Connection: close\r\n\r\n",
                    HTTPServer.Version, _status, HTTPServer.Name, _mime, _data.Length
                );

                writer.Write(responseHeaders);
                writer.Flush();

                Console.WriteLine(responseHeaders);

                if (_data != null && _data.Length > 0)
                {
                    writer.BaseStream.Write(_data, 0, _data.Length);
                    writer.BaseStream.Flush();
                    Console.WriteLine("Response Body Sent.");
                }
                else
                {
                    Console.WriteLine("Response Body is NULL.");
                }

            }
            catch (IOException ioEx)
            {
                Console.WriteLine($"IO Error sending response: {ioEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending response: {ex.Message}");
            }
        }

        private static Response MakeFromFile(FileInfo fileInfo)
        {
            byte[] fileBytes = File.ReadAllBytes(fileInfo.FullName);
            string mimeType = GetMimeType(fileInfo.Extension); // Obtener el tipo MIME según la extensión del archivo
            return new Response("200 OK", mimeType, fileBytes);
        }

        private static string GetMimeType(string fileExtension)
        {
            // Asignar el tipo MIME adecuado según la extensión del archivo
            return fileExtension.ToLower() switch
            {
                ".html" => "text/html",
                ".htm" => "text/html",
                ".css" => "text/css",
                ".js" => "application/javascript",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".svg" => "image/svg+xml",
                _ => "application/octet-stream", // Tipo por defecto
            };
        }

        private static Response MakeNullRequest()
        {
            string errorPage = Path.Combine(Environment.CurrentDirectory, HTTPServer.MSG_DIR, "400.html");
            byte[] fileBytes = File.Exists(errorPage) ? File.ReadAllBytes(errorPage) : Encoding.UTF8.GetBytes("400 Bad Request");
            return new Response("400 Bad Request", "text/html", fileBytes);
        }

        private static Response MakePageNotFound()
        {
            string errorPage = Environment.CurrentDirectory + HTTPServer.MSG_DIR + "404.html";

            Console.WriteLine(errorPage);

            byte[] fileBytes = File.Exists(errorPage) ? File.ReadAllBytes(errorPage) : Encoding.UTF8.GetBytes("404 Not Found");
            return new Response("404 Not Found", "text/html", fileBytes);
        }

        private static Response MakeMethodNotAllowed()
        {
            string errorPage = Path.Combine(Environment.CurrentDirectory, HTTPServer.MSG_DIR, "405.html");
            byte[] fileBytes = File.Exists(errorPage) ? File.ReadAllBytes(errorPage) : Encoding.UTF8.GetBytes("405 Method Not Allowed");
            return new Response("405 Method Not Allowed", "text/html", fileBytes);
        }
    }
}
