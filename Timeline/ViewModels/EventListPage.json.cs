using System;
using System.Globalization;
using Starcounter;
using Simplified.Ring1;
using System.Collections.Generic;
using System.Linq;

namespace Timeline
{
    partial class EventListPage : Json
    {
        public DateTimeFormatInfo DateInfo = new DateTimeFormatInfo();

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
            DefaultTemplate.DisplayedDate.Bind = nameof(bindDate);
        }

        protected override void OnData()
        {
            base.OnData();
            this.OriginPage = Self.GET($"/timeline/timeline-item/{this.Key}");
        }

        public EventListPage ParentPage
        {
            get
            {
                return this.Parent.Parent as EventListPage;
            }
        }

        public string bindDate
        {
            get
            {
                ParentPage.DateInfo.LongDatePattern = "HH:mm dddd dd MMMM";
                DateTime currentDate = (DbHelper.FromID(DbHelper.Base64DecodeObjectID(this.Key)) as Event).EventInfo.Created;
                return currentDate.ToString(ParentPage.DateInfo.LongDatePattern);
            }
        }
    }
}
