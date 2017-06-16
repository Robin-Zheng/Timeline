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

    [EventListPage_json.Events]
    partial class EventListPageEvents
    {
        static EventListPageEvents()
        {
            DefaultTemplate.OriginPage.Bind = nameof(bindOriginPage);
        }

        public Json bindOriginPage
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Name))
                {
                    return Self.GET($"/timeline/timeline-item/{this.Key}");
                }
                return new Json();
            }
        }
    }
}
