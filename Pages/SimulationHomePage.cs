using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using TechTalk.SpecFlow;

namespace LLEAP.UITestAutomation.Pages
{
    public class SimulationHomePage : FlaUiPageBase
    {
        // This is  installation path .
        private const string LaunchPortalExePath = @"C:\Program Files (x86)\Laerdal Medical\Laerdal Simulation Home\LaunchPortal.exe";

        public SimulationHomePage(ScenarioContext scenarioContext) : base(scenarioContext) { }

        // This method use for simulation home window
        public void LaunchSimulationHome()
        {
            var app = Application.Launch(LaunchPortalExePath);
            var automation = new UIA3Automation();

            var homeWindow = app.GetMainWindow(automation);
            if (homeWindow == null)
                throw new InvalidOperationException("Could not find the LLEAP Simulation Home main window.");

            Scenario["App"] = app;
            Scenario["Automation"] = automation;
            Scenario["HomeWindow"] = homeWindow;
        }

        //This method use for start instructor application
        public void StartInstructorApplication()
        {
            var homeWindow = RequireWindow("HomeWindow");

            var instructorTile = homeWindow
                .FindFirstDescendant(cf => cf.ByText("LLEAP - Instructor Application"))
                ?.AsButton();

            if (instructorTile == null)
                throw new InvalidOperationException("Could not find 'LLEAP - Instructor Application' tile in Simulation Home window.");

            instructorTile.Invoke();
        }
        // This method Verify Home window is visisble
        public bool IsHomeWindowVisible()
        {
            try
            {
                var w = RequireWindow("HomeWindow");
                return w.IsAvailable && w.Properties.IsOffscreen.ValueOrDefault == false;
            }
            catch { return false; }
        }
        // This method Verify Instructor title is clickabale
        public bool IsInstructorTileClickable()
        {
            try
            {
                var homeWindow = RequireWindow("HomeWindow");
                var tile = homeWindow.FindFirstDescendant(cf => cf.ByText("LLEAP - Instructor Application"))?.AsButton();
                return tile != null && tile.IsEnabled;
            }
            catch { return false; }
        }
    }
}
