using Starcounter;

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

            Handle.GET("/timeline/partials/event-list", () =>
            {
                EventListPage page = new EventListPage() { Data = null };
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
                // Used for test purposes, will be used until there are more apps which responds to /contributions
                TestLinkPage page = new TestLinkPage();
                return page;
            });
            Blender.MapUri("/timeline/contributions", "contributions");

            Handle.GET("/timeline/timeline-item/{?}", (string eventId) =>
            {
                return new Json();
            });

            Blender.MapUri("/timeline/timeline-item/{?}", "timeline-item"); // {?} is the event Id

        }
    }
}
