using Starcounter;
using Simplified.Ring1;
using System.Collections.Generic;
using System.Linq;

namespace Timeline
{
    partial class EventListPage : Json
    {
        static EventListPage()
        {
            DefaultTemplate.Events.Bind = nameof(bindEvents);
        }

        public List<Event> bindEvents
        {
            get
            {
                return Db.SQL<Event>("SELECT p FROM Simplified.Ring1.Event p ORDER BY p.EventInfo.Created DESC").ToList();
            }
        }
    }
}
