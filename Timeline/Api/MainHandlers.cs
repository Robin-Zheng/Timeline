﻿using Starcounter;

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

            Handle.GET("/timeline/masterpage", () =>
            {
                var session = Session.Current;
                if (session == null)
                {
                    session = new Session(SessionOptions.PatchVersioning);
                }

                MasterPage master = new MasterPage();
                master.Session = session;
                return master;
            });

            Handle.GET("/timeline/eventList", () =>
            {
                return Db.Scope<MasterPage>(() => {
                    var master = (MasterPage)Self.GET("/timeline/masterpage");

                    master.ActionRowPage = Self.GET("/timeline/partials/action-row");
                    master.CurrentPage = Self.GET("/timeline/partials/event-list");

                    return master;
                });
            });

            Handle.GET("/timeline/eventList/{?}", (string personId) =>
            {
                return Db.Scope<MasterPage>(() => {
                    var master = (MasterPage)Self.GET("/timeline/masterpage");

                    master.ActionRowPage = Self.GET("/timeline/partials/action-row");
                    master.CurrentPage = Self.GET("/timeline/partials/event-list/{?}");

                    return master;
                });
            });

            Handle.GET("/timeline/partials/event-list", () =>
            {
                EventListPage page = new EventListPage() { Data = null };
                return page;
            });

            Handle.GET("/timeline/partials/event-list/{?}", (string personId) =>
            {
                EventListPage page = new EventListPage() { PersonId = personId, Data = null };
                return page;
            });

            Handle.GET("/timeline/partials/action-row", () =>
            {
                ActionsPage page = new ActionsPage();
                page.LoadContributions();
                return page;
            });


            Handle.GET("/timeline/contributions", () =>
            {
                return new Json();
            });
            Blender.MapUri("/timeline/contributions", "contributions");

            Handle.GET("/timeline/timeline-item/{?}", (string eventId) =>
            {
                return new Json();
            });
            Blender.MapUri("/timeline/timeline-item/{?}", "timeline-item");


            Handle.GET("/timeline/partial/for-something/{?}", (string personId) => {
                Simplified.Ring2.Person thisPerson = DbHelper.FromID(DbHelper.Base64DecodeObjectID(personId)) as Simplified.Ring2.Person;
                var test = personId;
                return Self.GET("/timeline/eventList/{?}");
            });



            //Add Blender.MapUri<Simplified.Ring2.Person>("/people/partials/persons/{?}"); to the People app > will cause this to open in people app
            Blender.MapUri<Simplified.Ring2.Person>("/timeline/partial/for-something/{?}");

        }
    }
}
