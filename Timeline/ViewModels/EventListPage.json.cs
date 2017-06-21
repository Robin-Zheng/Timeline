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

        public List<Event> bindEvents
        {
            get
            {
                if (!string.IsNullOrEmpty(this.PersonId))
                {
                    // Perhaps the Event class should have a "connection" to EventParticipation similar to how it has to EventInfo
                    Person thisPerson = DbHelper.FromID(DbHelper.Base64DecodeObjectID(this.PersonId)) as Person;
                    List<Event> allEvents = Db.SQL<Event>("SELECT p FROM Simplified.Ring1.Event p").ToList();
                    List<EventParticipation> allParticipations = Db.SQL<EventParticipation>("SELECT ep FROM Simplified.Ring6.EventParticipation ep").ToList();
                    List<EventParticipation> allOtherParticipations = allParticipations.Where(x => x.Participant?.Key != thisPerson.Key && x.Participant != null).ToList();
                    List<Event> allOtherEvents = allOtherParticipations.Select(x => x.Event).ToList();

                    //Returns this specific users events and all "empty" events.
                    return allEvents.Except(allOtherEvents).OrderByDescending(x => x.EventInfo.Created).ToList();
                }
                return Db.SQL<Event>("SELECT p FROM Simplified.Ring1.Event p ORDER BY p.EventInfo.Created DESC").ToList();
            }
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
            DefaultTemplate.DisplayedDate.Bind = nameof(bindDate);
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
            else if (string.IsNullOrEmpty(this.ParentPage.PersonId)) // If an event is created outside of a person scope, it should displayed for every user
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
