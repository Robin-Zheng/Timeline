using Starcounter;

namespace NewTimeLine
{
    internal class MainHandlers
    {
        public void Register()
        {
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            Handle.GET("/NewTimeLine", () =>
            {
                return Self.GET("/newTimeLine/eventList");
            });

            Handle.GET("/newTimeLine/standalone", () =>
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

            Handle.GET("/newTimeLine/eventList", () =>
            {
                return Db.Scope<StandalonePage>(() => {
                    var master = (StandalonePage)Self.GET("/newTimeLine/standalone");

                    master.ActionRowPage = Self.GET("/newTimeLine/partials/action-row");
                    master.CurrentPage = Self.GET("/newTimeLine/partials/event-list");

                    return master;
                });
            });

            Handle.GET("/newTimeLine/partials/event-list", () =>
            {
                EventListPage page = new EventListPage();
                return page;
            });

            Handle.GET("/newTimeLine/partials/action-row", () =>
            {
                ActionsPage page = new ActionsPage();
                page.LoadContributions();
                return page;
            });

            Handle.GET("/newTimeLine/contributions", () =>
            {
                // Used for test purposes, will be used until there are more apps which responds to /contributions
                TestLinkPage page = new TestLinkPage();
                return page;
            });
            Blender.MapUri("/newTimeLine/contributions", "/contributions");


            Handle.GET("/newTimeLine/input-contributions", () =>
            {
                return new Json();
            });
            Blender.MapUri("/newTimeLine/input-contributions", "/input-contributions");

        }
    }
}
