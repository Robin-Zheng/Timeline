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

            Handle.GET("/timeline/standalone", () =>
            {
                var session = Session.Current;
                if (session == null)
                {
                    session = new Session(SessionOptions.PatchVersioning);
                }

                StandalonePage standalone = new StandalonePage();
                standalone.Session = session;
                return standalone;
            });

            Handle.GET("/timeline/eventList", () =>
            {
                return Db.Scope<StandalonePage>(() => {
                    var master = (StandalonePage)Self.GET("/timeline/standalone");

                    master.ActionRowPage = Self.GET("/timeline/partials/action-row");
                    master.CurrentPage = Self.GET("/timeline/partials/event-list");

                    return master;
                });
            });

            Handle.GET("/timeline/partials/event-list", () =>
            {
                EventListPage page = new EventListPage();
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


            Handle.GET("/timeline/input-contributions", () =>
            {
                return new Json();
            });
            Blender.MapUri("/timeline/input-contributions", "input-contributions");

        }
    }
}
