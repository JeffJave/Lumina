using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWebhook
{
    class LineReceivedMessage
    {
        public List<Event> events;
        public LineReceivedMessage()
        {
            events = new List<Event>();
        }
        public class Event
        {
            public string type { get; set; }
            public Source source { get; set; }
            public EventMessage message { get; set; }
            public string replyToken { get; set; }
            public Event()
            {
                source = new Source();
                message = new EventMessage();
            }
        }
        public class Source
        {
            public string userId { get; set; }
        }
        public class EventMessage
        {
            public string id { get; set; }
            public string type { get; set; }
            public string text { get; set; }
        }
    }
}
