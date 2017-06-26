using System;
using System.Collections.Generic;
using System.Linq;
using Starcounter;
using Simplified.Ring1;
using Simplified.Ring2;
using Simplified.Ring6;

namespace Timeline
{
    partial class ActionsPage : Json
    {
        public void LoadContributions()
        {
            // Will recieve contributions from a lot of different apps. Will be used to create different events
            this.Contributions = Self.GET("/timeline/contributions");
            HelperFunctions.CurrentSortSelection = string.Empty;
        }

        static ActionsPage()
        {
            DefaultTemplate.SortButtons.Bind = nameof(bindSortButtons);
        }

        public List<Event> bindSortButtons
        {
            get
            {
                if (string.IsNullOrEmpty(this.PersonId))
                {
                    return Db.SQL<Event>("SELECT e FROM Simplified.Ring1.Event e").ToList().GroupBy(x => x.Name).Select(x => x.First()).ToList();
                }
                Person thisPerson = DbHelper.FromID(DbHelper.Base64DecodeObjectID(this.PersonId)) as Person;
                List<Event> eventList = Db.SQL<Event>("SELECT e FROM Simplified.Ring1.Event e").ToList();
                return eventList.Where(x => x.Participants.Contains(thisPerson)).GroupBy(x => x.Name).Select(x => x.First()).ToList();
            }
        }

        public void Handle(Input.CreateTrigger Action)
        {
            this.AreaExpanded = !this.AreaExpanded;
            Action.Cancel();
        }

        public void Handle(Input.CleanupTrigger Action)
        {
            Action.Cancel();
            List<Event> allEvents = Db.SQL<Event>("SELECT e FROM Simplified.Ring1.Event e ORDER BY e.EventInfo.Created DESC").ToList();
            if (allEvents.Count == 0)
            {
                return;
            }
            if (allEvents[0].EventInfo.Updated == DateTime.MinValue)
            {
                HelperFunctions.DeleteEvent(allEvents[0]);
            }
        }
    }

    [ActionsPage_json.SortButtons]
    partial class ActionsSortButtons
    {
        public ActionsPage ParentPage
        {
            get
            {
                return this.Parent.Parent as ActionsPage;
            }
        }

        static ActionsSortButtons()
        {
            DefaultTemplate.Amount.Bind = nameof(bindAmount);
        }

        public int bindAmount
        {
            get
            {
                List<Event> eventList = Db.SQL<Event>("SELECT e FROM Simplified.Ring1.Event e WHERE e.Name = ?", this.Name).ToList();
                if (string.IsNullOrEmpty(ParentPage.PersonId))
                {
                    return eventList.ToList().Count;
                }
                Person thisPerson = DbHelper.FromID(DbHelper.Base64DecodeObjectID(ParentPage.PersonId)) as Person;
                return eventList.Where(x => x.Participants.Contains(thisPerson)).ToList().Count + eventList.Where(x => x.Participants.First == null).ToList().Count;
            }
        }

        public void Handle(Input.SortTrigger Action)
        {
            foreach (var item in ParentPage.SortButtons)
            {
                if (item == this)
                {
                    if (HelperFunctions.CurrentSortSelection == this.Name)
                    {
                        HelperFunctions.CurrentSortSelection = string.Empty;
                        item.Selected = false;
                    }
                    else
                    {
                        HelperFunctions.CurrentSortSelection = this.Name;
                        item.Selected = true;
                    }
                }
                else
                {
                   item.Selected = false;
                }
            }
        }
    }
}
