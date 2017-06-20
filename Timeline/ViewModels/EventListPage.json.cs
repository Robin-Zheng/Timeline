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
                    Person thisPerson = DbHelper.FromID(DbHelper.Base64DecodeObjectID(this.PersonId)) as Person;
                    List<EventParticipation> allParticipations = Db.SQL<EventParticipation>("SELECT ep FROM Simplified.Ring6.EventParticipation ep").ToList();
                    List<EventParticipation> thisUsersParticipations = allParticipations.Where(x => x?.Participant?.Key == thisPerson.Key).ToList();
                    List<Event> thisUsersEvents = thisUsersParticipations.Select(x => x.Event).ToList();
                    List<Event> allEvents = Db.SQL<Event>("SELECT p FROM Simplified.Ring1.Event p ORDER BY p.EventInfo.Created DESC").ToList();
                    //return thisUsersParticipations.Select(x => x.Event).OrderByDescending(x => x.EventInfo.Created).ToList();

                    List<EventParticipation> notThisParticipations = allParticipations.Where(x => x.Participant?.Key != thisPerson.Key).ToList();
                    List<Event> notThisEvents = thisUsersParticipations.Select(x => x.Event).ToList();

                    return allEvents.Except(notThisEvents).ToList();
                    //Need to find a way to Display: All this users events, and all empty events.
                    //In other words: All events, except events that have a relation to someone else
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
            if (thisEvent.EventInfo.Owner == null)
            {
                if (!string.IsNullOrEmpty(this.ParentPage.PersonId))
                {
                    Person thisPerson = DbHelper.FromID(DbHelper.Base64DecodeObjectID(this.ParentPage.PersonId)) as Person;
                    //Db.Transact(() =>
                    //{
                    //    EventParticipation eventParticipation = new EventParticipation();
                    //    eventParticipation.Event = thisEvent;
                    //    eventParticipation.Participant = thisPerson;
                    //});
                }
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
            Db.Transact(() =>
            {
                thisEvent.EventInfo.Delete();
                thisEvent.Delete();
            });
        }
    }
}
