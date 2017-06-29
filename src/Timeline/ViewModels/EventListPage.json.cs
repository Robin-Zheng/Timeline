using System;
using System.Globalization;
using Starcounter;
using Simplified.Ring1;
using Simplified.Ring2;
using Simplified.Ring6;
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

        protected override void OnData()
        {
            base.OnData();
        }

        public List<Event> bindEvents
        {
            get
            {
                return IdentifyEventList();
            }
        }

        public List<Event> IdentifyEventList()
        {
            List<Event> allEvents = Db.SQL<Event>("SELECT e FROM Simplified.Ring1.Event e").OrderByDescending(x => x.EventInfo.Created).ToList();
            if (!string.IsNullOrEmpty(HelperFunctions.CurrentSortSelection))
            {
                allEvents = allEvents.Where(x => x.Name == HelperFunctions.CurrentSortSelection).OrderByDescending(x => x.EventInfo.Created).ToList();
            }
            if (!string.IsNullOrEmpty(PersonId))
            {
                Person thisPerson = DbHelper.FromID(DbHelper.Base64DecodeObjectID(PersonId)) as Person;
                allEvents = allEvents.Where(x => x.Participants.Contains(thisPerson) || x.Participants.First == null).OrderByDescending(x => x.EventInfo.Created).ToList();
            }
            return allEvents;
        }
    }

    [EventListPage_json.Events]
    partial class EventListPageEvents
    {
        public EventListPage ParentPage
        {
            get
            {
                return this.Parent.Parent as EventListPage;
            }
        }

        static EventListPageEvents()
        {
            DefaultTemplate.DisplayedDate.Bind = nameof(BindDate);
            DefaultTemplate.Participant.Bind = nameof(BindParticipant);
            DefaultTemplate.DisplayDatePoint.Bind = nameof(BindDisplayDatePoint);
        }

        public bool BindDisplayDatePoint
        {
            get
            {
                Event thisEvent = DbHelper.FromID(DbHelper.Base64DecodeObjectID(this.Key)) as Event;
                List<Event> allEvents = ParentPage.IdentifyEventList();
                return DisplayCalendarDate(thisEvent, allEvents);
            }
        }

        private bool DisplayCalendarDate(Event thisEvent, List<Event> events)
        {
            var thisIndex = events.IndexOf(thisEvent);
            if (thisIndex < 0 || events.Count == 0)
            {
                return false;
            }
            if (events[0]?.GetObjectID() == this.Data.GetObjectID())
            {
                return true;
            }
            if (events[thisIndex]?.EventInfo?.Created.ToString("dd MMM, yyyy") != events[thisIndex - 1]?.EventInfo?.Created.ToString("dd MMM, yyyy"))
            {
                return true;
            }
            return false;
        }

        public string BindDate
        {
            get
            {
                ParentPage.DateInfo.LongDatePattern = "HH:mm dddd dd MMMM";
                DateTime currentDate = (DbHelper.FromID(DbHelper.Base64DecodeObjectID(this.Key)) as Event).EventInfo.Created;
                this.Date.Day = currentDate.Day.ToString();
                this.Date.Month = currentDate.ToString("MMM");
                this.Date.Year = currentDate.Year.ToString();
                return currentDate.ToString(ParentPage.DateInfo.LongDatePattern);
            }
        }

        public string BindParticipant
        {
            get
            {
                Event thisEvent = DbHelper.FromID(DbHelper.Base64DecodeObjectID(this.Key)) as Event;
                //What if there are multiple participants?
                Person participant = thisEvent?.Participants?.First as Person;
                return participant?.Name;
            }
        }

        protected override void OnData()
        {
            base.OnData();
            this.TimelineEventPage = Self.GET($"/timeline/timeline-item/{this.Key}");

            Event thisEvent = DbHelper.FromID(DbHelper.Base64DecodeObjectID(this.Key)) as Event;
            List<Event> allEvents = Db.SQL<Event>("SELECT ep.Event FROM Simplified.Ring6.EventParticipation ep").ToList();

            //If an event does not have a relation > attach it to "this user".
            //The only time this will happen is when an event has just been created. = The event will be attached to the correct person
            if (!allEvents.Contains(thisEvent) && !string.IsNullOrEmpty(this.ParentPage.PersonId))
            {
                Person thisPerson = DbHelper.FromID(DbHelper.Base64DecodeObjectID(this.ParentPage.PersonId)) as Person;
                Db.Transact(() =>
                {
                    EventParticipation eventParticipation = new EventParticipation();
                    eventParticipation.Event = thisEvent;
                    eventParticipation.Participant = thisPerson;
                });
            }
            else if (!allEvents.Contains(thisEvent) && string.IsNullOrEmpty(this.ParentPage.PersonId)) // If an event is created outside of a person scope, it should displayed for every user
            {
                Db.Transact(() =>
                {
                    EventParticipation eventParticipation = new EventParticipation();
                    eventParticipation.Event = thisEvent;
                });
            }
        }

        public void Handle(Input.EditTrigger Action)
        {
            Event thisEvent = DbHelper.FromID(DbHelper.Base64DecodeObjectID(this.Key)) as Event;
            Db.Transact(() =>
            {
                thisEvent.EventInfo.Updated = DateTime.Now;
            });
        }
        
        public void Handle(Input.DeleteTrigger Action)
        {
            Event thisEvent = DbHelper.FromID(DbHelper.Base64DecodeObjectID(this.Key)) as Event;
            HelperFunctions.DeleteEvent(thisEvent);
        }
    }
}
