﻿using Starcounter;
using Simplified.Ring1;
using Simplified.Ring2;

namespace Timeline
{
    internal class MainHandlers
    {
        public void Register()
        {
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            Handle.GET("/Timeline", () =>
            {
                return Self.GET("/timeline/eventList");
            });

            Handle.GET("/timeline/eventList", () =>
            {
                return Db.Scope<MasterPage>(() => {
                    var master = this.GetMasterPageFromSession();

                    master.ActionRowPage = Self.GET("/timeline/partials/action-row");
                    master.CurrentPage = Self.GET("/timeline/partials/event-list");

                    return master;
                });
            }, new HandlerOptions { SelfOnly = true });

            Handle.GET("/timeline/partials/event-list", () =>
            {
                EventListPage page = new EventListPage() { Data = null };
                return page;
            }, new HandlerOptions { SelfOnly = true });

            Handle.GET("/timeline/partials/action-row", () =>
            {
                ActionsPage page = new ActionsPage();
                page.LoadContributions();
                return page;
            }, new HandlerOptions { SelfOnly = true });

            Handle.GET("/timeline/eventList/{?}", (string personId) =>
            {
                return Db.Scope<MasterPage>(() => {
                    var master = this.GetMasterPageFromSession();

                    master.ActionRowPage = Self.GET("/timeline/partials/action-row/"  + personId);
                    master.CurrentPage = Self.GET("/timeline/partials/event-list/" + personId);

                    return master;
                });
            }, new HandlerOptions { SelfOnly = true });

            Handle.GET("/timeline/partials/event-list/{?}", (string personId) =>
            {
                EventListPage page = new EventListPage() { PersonId = personId, Data = null };
                return page;
            }, new HandlerOptions { SelfOnly = true });

            Handle.GET("/timeline/partials/action-row/{?}", (string personId) =>
            {
                ActionsPage page = new ActionsPage() { PersonId = personId, Data = null };
                page.LoadContributions();
                return page;
            }, new HandlerOptions { SelfOnly = true });


            Handle.GET("/timeline/contributions", () =>
            {
                return new Json();
            }, new HandlerOptions { SelfOnly = true });
            Blender.MapUri("/timeline/contributions", "contributions");

            Handle.GET("/timeline/timeline-item/{?}", (string eventId) =>
            {
                return new Json();
            }, new HandlerOptions { SelfOnly = true });
            Blender.MapUri("/timeline/timeline-item/{?}", "timeline-item");

            // Provides the People app with a timeline (+ an actions row, where events can be created)
            Handle.GET("/timeline/partial/timeline/{?}", (string personId) => {
                return Self.GET("/timeline/eventList/" + personId);
            }, new HandlerOptions { SelfOnly = true });

            //Add Blender.MapUri<Simplified.Ring2.Person>("/people/partials/persons/{?}"); to the People app > will cause this to open in people app
            Blender.MapUri<Simplified.Ring2.Person>("/timeline/partial/timeline/{?}");


            // Instead of having the "delete this event" code in every app, the call to this handler triggers the deletion event.
            // That way timeline app will have full control of the deletion of events, no matter if the "cancel" button (from inside an event) is clicked,
            // or if the "X" (delete) button from the timeline app is clicked
            Handle.GET("/timeline/partial/delete-event/{?}", (string eventKey) =>
            {
                Event thisEvent = DbHelper.FromID(DbHelper.Base64DecodeObjectID(eventKey)) as Event;
                HelperFunctions.DeleteEvent(thisEvent);
                return new Json();
            }, new HandlerOptions { SelfOnly = true });
            Blender.MapUri("/timeline/partial/delete-event/{?}", "delete-event");
        }

        protected MasterPage GetMasterPageFromSession()
        {
            if (Session.Current == null)
            {
                Session.Current = new Session(SessionOptions.PatchVersioning);
            }

            MasterPage master = Session.Current.Data as MasterPage;

            if (master == null)
            {
                master = new MasterPage();
                Session.Current.Data = master;
            }
            return master;
        }
    }
}
