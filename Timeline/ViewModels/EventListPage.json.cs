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
            //DefaultTemplate.EventGroups.Bind = nameof(bindEventGroups);
        }

        protected override void OnData()
        {
            base.OnData();

            //List<EventListEventGroups> testList = new List<EventListEventGroups>();
            //var test = Db.SQL<Event>("SELECT e FROM Simplified.Ring1.Event e").OrderByDescending(x => x.EventInfo.Created).ToList().GroupBy(x => x.EventInfo.Created.ToString("dd MMMM, yyyy")).Select(x => x.ToList()).ToList();
            //if (!string.IsNullOrEmpty(this.PersonId))
            //{
            //    Person thisPerson = DbHelper.FromID(DbHelper.Base64DecodeObjectID(this.PersonId)) as Person;
            //    test = test.Select(x => x.Where(y => y.Participants.Contains(thisPerson) || y.Participants.First == null).ToList()).ToList();
            //}
            //if (!string.IsNullOrEmpty(HelperFunctions.CurrentSortSelection))
            //{
            //    test = test.Select(x => x.Where(y => y.Name == HelperFunctions.CurrentSortSelection).ToList()).ToList();
            //}
            //foreach (var item in test)
            //{
            //    if (item.Count == 0)
            //    {
            //        break;
            //    }
            //    EventListEventGroups newGroup = new EventListEventGroups();
            //    newGroup.Date.Day = item[0].EventInfo.Created.Day.ToString();
            //    newGroup.Date.Month = item[0].EventInfo.Created.Month.ToString("MMM");
            //    newGroup.Date.Year = item[0].EventInfo.Created.Year.ToString();
            //    newGroup.Events.Data = item;
            //    testList.Add(newGroup);
            //}

            //this.EventGroups.Data = testList;
        }

        //public List<EventListEventGroups> bindEventGroups
        //{
        //    get
        //    {
        //        List<EventListEventGroups> testList = new List<EventListEventGroups>();
        //        var test = Db.SQL<Event>("SELECT e FROM Simplified.Ring1.Event e").OrderByDescending(x => x.EventInfo.Created).ToList().GroupBy(x => x.EventInfo.Created.ToString("dd MMMM, yyyy")).Select(x => x.ToList()).ToList();
        //        if (!string.IsNullOrEmpty(this.PersonId))
        //        {
        //            Person thisPerson = DbHelper.FromID(DbHelper.Base64DecodeObjectID(this.PersonId)) as Person;
        //            test = test.Select(x => x.Where(y => y.Participants.Contains(thisPerson) || y.Participants.First == null).ToList()).ToList();
        //        }
        //        if (!string.IsNullOrEmpty(HelperFunctions.CurrentSortSelection))
        //        {
        //            test = test.Select(x => x.Where(y => y.Name == HelperFunctions.CurrentSortSelection).ToList()).ToList();
        //        }
        //        foreach (var item in test)
        //        {
        //            if (item.Count == 0)
        //            {
        //                break;
        //            }
        //            EventListEventGroups newGroup = new EventListEventGroups();
        //            newGroup.Date.Day = item[0].EventInfo.Created.Day.ToString();
        //            newGroup.Date.Month = item[0].EventInfo.Created.Month.ToString("MMM");
        //            newGroup.Date.Year = item[0].EventInfo.Created.Year.ToString();
        //            newGroup.Events.Data = item;
        //            testList.Add(newGroup);
        //        }

        //        return testList;
        //    }
        //}

        public List<Event> bindEvents
        {
            // How should the Create button behave?
            // If a Meeting event is created when the view is set to "view only Notes events". How should the creation of the new meeting event behave?
            // The commented code below displays the new event no matter which view is currently enabled. Once the event has been saved, it no longer
            // counts as a new event, and will then disappear from the view (if it for example is a meeting event and the current view is notes)

            //get
            //{
            //    List<Event> allEvents = Db.SQL<Event>("SELECT e FROM Simplified.Ring1.Event e").ToList();
            //    List<Event> newEvents = allEvents.Where(x => x.EventInfo.Updated == DateTime.MinValue && x.Name != HelperFunctions.CurrentSortSelection).ToList();
            //    if (!string.IsNullOrEmpty(this.PersonId))
            //    {
            //        Person thisPerson = DbHelper.FromID(DbHelper.Base64DecodeObjectID(this.PersonId)) as Person;
            //        List<EventParticipation> allParticipations = Db.SQL<EventParticipation>("SELECT ep FROM Simplified.Ring6.EventParticipation ep").ToList();
            //        List<EventParticipation> allOtherParticipations = allParticipations.Where(x => x.Participant?.Key != thisPerson.Key && x.Participant != null).ToList();
            //        List<Event> allOtherEvents = allOtherParticipations.Select(x => x.Event).ToList();

            //        if (string.IsNullOrEmpty(HelperFunctions.CurrentSortSelection))
            //        {
            //            return allEvents.Except(allOtherEvents).OrderByDescending(x => x.EventInfo.Created).ToList();
            //        }
            //        List<Event> allSpecificEvents = allEvents.Where(x => x.Name == HelperFunctions.CurrentSortSelection).ToList();
            //        allSpecificEvents.AddRange(newEvents);
            //        return allSpecificEvents.Except(allOtherEvents).OrderByDescending(x => x.EventInfo.Created).ToList();
            //    }
            //    if (string.IsNullOrEmpty(HelperFunctions.CurrentSortSelection))
            //    {
            //        return Db.SQL<Event>("SELECT e FROM Simplified.Ring1.Event e ORDER BY e.EventInfo.Created DESC").ToList();
            //    }
            //    List<Event> returnEvents = allEvents.Where(x => x.Name == HelperFunctions.CurrentSortSelection).ToList();
            //    returnEvents.AddRange(newEvents);
            //    return returnEvents.OrderByDescending(x => x.EventInfo.Created).ToList();
            //}
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

    //[EventListPage_json.EventGroups]
    //partial class EventListEventGroups
    //{
    //    public DateTimeFormatInfo DateInfo = new DateTimeFormatInfo();

    //    public EventListPage ParentPage
    //    {
    //        get
    //        {
    //            return this.Parent.Parent as EventListPage;
    //        }
    //    }

    //    public string PersonId { get { return ""; } }
    //}

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
            DefaultTemplate.Participant.Bind = nameof(bindParticipant);
            DefaultTemplate.DisplayBreak.Bind = nameof(bindDisplayBreak);
            //DefaultTemplate.Name.Bind = nameof(bindName);
        }

        //public string bindName
        //{
        //    get
        //    {
        //        return this.Name.ToLower();
        //    }
        //}

        public bool bindDisplayBreak
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

        public string bindDate
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

        public string bindParticipant
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

    //[EventListPage_json.EventGroups.Events]
    //partial class EventListEventGroupsEvent
    //{
    //    public EventListPage ParentPage
    //    {
    //        get
    //        {
    //            return this.Parent.Parent.Parent.Parent as EventListPage;
    //        }
    //    }

    //    static EventListEventGroupsEvent()
    //    {
    //        //DefaultTemplate.DisplayedDate.Bind = nameof(bindDate);
    //        DefaultTemplate.Participant.Bind = nameof(bindParticipant);
    //    }

    //    public string bindDate
    //    {
    //        get
    //        {
    //            ParentPage.DateInfo.LongDatePattern = "HH:mm dddd dd MMMM";
    //            DateTime currentDate = (DbHelper.FromID(DbHelper.Base64DecodeObjectID(this.Key)) as Event).EventInfo.Created;
    //            this.Date.Day = currentDate.Day.ToString();
    //            this.Date.Month = currentDate.ToString("MMM");
    //            this.Date.Year = currentDate.Year.ToString();
    //            return currentDate.ToString(ParentPage.DateInfo.LongDatePattern);
    //        }
    //    }

    //    public string bindParticipant
    //    {
    //        get
    //        {
    //            Event thisEvent = DbHelper.FromID(DbHelper.Base64DecodeObjectID(this.Key)) as Event;
    //            //What if there are multiple participants?
    //            Person participant = thisEvent?.Participants?.First as Person;
    //            return participant?.Name;
    //        }
    //    }

    //    protected override void OnData()
    //    {
    //        base.OnData();
    //        if (string.IsNullOrEmpty(this.Key))
    //        {
    //            return;
    //        }
    //        this.TimelineEventPage = Self.GET($"/timeline/timeline-item/{this.Key}");

    //        Event thisEvent = DbHelper.FromID(DbHelper.Base64DecodeObjectID(this.Key)) as Event;
    //        List<Event> allEvents = Db.SQL<Event>("SELECT ep.Event FROM Simplified.Ring6.EventParticipation ep").ToList();

    //        //If an event does not have a relation > attach it to "this user".
    //        //The only time this will happen is when an event has just been created. = The event will be attached to the correct person
    //        if (!allEvents.Contains(thisEvent) && !string.IsNullOrEmpty(this.ParentPage.PersonId))
    //        {
    //            Person thisPerson = DbHelper.FromID(DbHelper.Base64DecodeObjectID(this.ParentPage.PersonId)) as Person;
    //            Db.Transact(() =>
    //            {
    //                EventParticipation eventParticipation = new EventParticipation();
    //                eventParticipation.Event = thisEvent;
    //                eventParticipation.Participant = thisPerson;
    //            });
    //        }
    //        else if (!allEvents.Contains(thisEvent) && string.IsNullOrEmpty(this.ParentPage.PersonId)) // If an event is created outside of a person scope, it should displayed for every user
    //        {
    //            Db.Transact(() =>
    //            {
    //                EventParticipation eventParticipation = new EventParticipation();
    //                eventParticipation.Event = thisEvent;
    //            });
    //        }
    //    }

    //    public void Handle(Input.EditTrigger Action)
    //    {
    //        Event thisEvent = DbHelper.FromID(DbHelper.Base64DecodeObjectID(this.Key)) as Event;
    //        Db.Transact(() =>
    //        {
    //            thisEvent.EventInfo.Updated = DateTime.Now;
    //        });
    //    }

    //    public void Handle(Input.DeleteTrigger Action)
    //    {
    //        Event thisEvent = DbHelper.FromID(DbHelper.Base64DecodeObjectID(this.Key)) as Event;
    //        HelperFunctions.DeleteEvent(thisEvent);
    //    }
    //}
}
