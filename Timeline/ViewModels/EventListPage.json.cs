using Starcounter;
using Simplified.Ring1;
using System.Collections.Generic;
using System.Linq;

namespace Timeline
{
    partial class EventListPage : Json
    {
        protected override void OnData()
        {
            base.OnData();
            this.Events.Data = Db.SQL<Event>("SELECT p FROM Simplified.Ring1.Event p").ToList();
        }
    }
}
