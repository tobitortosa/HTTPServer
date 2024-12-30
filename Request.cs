namespace HTTPServer
{
    public class Request
    {
        public string Type { get; set; }
        public string Url { get; set; }
        public string Host { get; set; }

        private Request(string type, string url, string host)
        {
            Type = type;
            Url = url;
            Host = host;
        }

        public static Request GetRequest(StreamReader reader)
        {
            string request = string.Empty;
            string line;

            while (!string.IsNullOrEmpty(line = reader.ReadLine()))
            {
                request += line + "\n";
            }

            string[] tokens = request.Split(' ');
            if (tokens.Length < 2) throw new InvalidDataException("Invalid HTTP request format");

            string type = tokens[0];
            string url = tokens[1];
            string host = "Unknown Host";

            foreach (string headerLine in request.Split('\n'))
            {
                if (headerLine.StartsWith("Host: "))
                {
                    host = headerLine.Substring(6);
                    break;
                }
            }

            return new Request(type, url, host);
        }
    }
}