using Starcounter;

namespace NewTimeLine
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
            this.Contributions = Self.GET("/newTimeLine/contributions");
            this.InputContributions = Self.GET("/newTimeLine/input-contributions");
        }

        public void Handle(Input.CreateTrigger Action)
        {
            this.AreaExpanded = !this.AreaExpanded;
            Action.Cancel();
        }
    }
}
