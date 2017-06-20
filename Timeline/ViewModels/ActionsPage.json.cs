using System;
using System.Collections.Generic;
using System.Linq;
using Starcounter;
using Simplified.Ring1;

namespace Timeline
{
    partial class ActionsPage : Json
    {
        protected override void OnData()
        {
            base.OnData();
        }

        public void LoadContributions()
        {
            // Will recieve contributions from a lot of different apps. Will be used to create different events
            this.Contributions = Self.GET("/timeline/contributions");
        }

        public void Handle(Input.CreateTrigger Action)
        {
            this.AreaExpanded = !this.AreaExpanded;
            Action.Cancel();
        }

        public void Handle(Input.CleanupTrigger Action)
        {
            List<Event> allEvents = Db.SQL<Event>("SELECT p FROM Simplified.Ring1.Event p ORDER BY p.EventInfo.Created DESC").ToList();
            Db.Transact(() =>
            {
                if (allEvents[0].EventInfo.Updated == DateTime.MinValue)
                {
                    allEvents[0].EventInfo.Delete();
                    allEvents[0].Delete();
                }
            });
            Action.Cancel();
        }
    }
}
