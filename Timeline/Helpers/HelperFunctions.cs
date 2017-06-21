﻿using System;
using System.Collections.Generic;
using System.Linq;
using Starcounter;
using Simplified.Ring1;
using Simplified.Ring2;
using Simplified.Ring6;

namespace Timeline
{
    public class HelperFunctions
    {
        /// <summary>
        /// Deletes an event, the events EventInfo, and its relation(EventParticipation)
        /// </summary>
        /// <param name="eventToDelete"></param>
        public static void DeleteEvent(Event eventToDelete)
        {
            List<EventParticipation> participationList = Db.SQL<EventParticipation>("SELECT ep FROM Simplified.Ring6.EventParticipation ep").ToList();
            EventParticipation thisParticipation = participationList.Where(x => x.Event.Key == eventToDelete.Key).FirstOrDefault();
            Db.Transact(() =>
            {
                eventToDelete.EventInfo.Delete();
                eventToDelete.Delete();
                thisParticipation?.Delete();
            });
        }
    }
}
