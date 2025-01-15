using System;
using Bloomberglp.Blpapi;

class Program
{
    static void Main(string[] args)
    {
        const string serverHost = "127.0.0.1"; // Replace with your server host
        const int serverPort = 8194;          // Replace with your server port

        const string security = "AAPL US Equity"; // Replace with your desired security
        const string field = "PX_LAST";          // Replace with your desired field (e.g., last price)

        // Create a session
        SessionOptions sessionOptions = new SessionOptions
        {
            ServerHost = serverHost,
            ServerPort = serverPort
        };

        Session session = new Session(sessionOptions);

        if (!session.Start())
        {
            Console.WriteLine("Failed to start session.");
            return;
        }

        if (!session.OpenService("//blp/refdata"))
        {
            Console.WriteLine("Failed to open //blp/refdata service.");
            return;
        }

        Service refDataService = session.GetService("//blp/refdata");
        Request request = refDataService.CreateRequest("ReferenceDataRequest");

        // Add securities and fields
        request.GetElement("securities").AppendValue(security);
        request.GetElement("fields").AppendValue(field);

        Console.WriteLine("Sending Request: " + request.ToString());
        session.SendRequest(request, null);

        // Process the response
        while (true)
        {
            Event eventObj = session.NextEvent();
            foreach (Message message in eventObj)
            {
                if (eventObj.Type == Event.EventType.RESPONSE ||
                    eventObj.Type == Event.EventType.PARTIAL_RESPONSE)
                {
                    Console.WriteLine(message.ToString());
                }
            }

            if (eventObj.Type == Event.EventType.RESPONSE)
            {
                break; // Exit loop after receiving the final response
            }
        }

        session.Stop();
    }
}
