using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using TechTalk.SpecFlow;

namespace LLEAP.UITestAutomation.Pages
{
    public class SimulationHomePage : FlaUiPageBase
    {
        
        // locators
       
        private static class Paths
        {
            // Installation path
            public const string LaunchPortalExePath =
                @"C:\Program Files (x86)\Laerdal Medical\Laerdal Simulation Home\LaunchPortal.exe";
        }

        private static class ScenarioKeys
        {
            public const string App = "App";
            public const string Automation = "Automation";
            public const string HomeWindow = "HomeWindow";
        }

        private static class Names
        {
            public const string InstructorTileText = "LLEAP - Instructor Application";
        }

        public SimulationHomePage(ScenarioContext scenarioContext) : base(scenarioContext) { }

        
        // This method use for launch simulation home
        
        public void LaunchSimulationHome(string? launchPortalExePath = null)
        {
            var exePath = launchPortalExePath ?? Paths.LaunchPortalExePath;

            var app = Application.Launch(exePath);
            var automation = new UIA3Automation();

            var homeWindow = app.GetMainWindow(automation);
            if (homeWindow == null)
                throw new InvalidOperationException("Could not find the LLEAP Simulation Home main window.");

            Scenario[ScenarioKeys.App] = app;
            Scenario[ScenarioKeys.Automation] = automation;
            Scenario[ScenarioKeys.HomeWindow] = homeWindow;
        }

       
        // This method Start Instructor Application
        
        public void StartInstructorApplication(string instructorTileText = Names.InstructorTileText)
        {
            var homeWindow = RequireWindow(ScenarioKeys.HomeWindow);

            var instructorTile = homeWindow
                .FindFirstDescendant(cf => cf.ByText(instructorTileText))
                ?.AsButton();

            if (instructorTile == null)
                throw new InvalidOperationException(
                    $"Could not find '{instructorTileText}' tile in Simulation Home window.");

            instructorTile.Invoke();
        }

       
        // This method use for Verify home window is visible
        
        public bool IsHomeWindowVisible(string homeWindowKey = ScenarioKeys.HomeWindow)
        {
            try
            {
                var w = RequireWindow(homeWindowKey);
                return w.IsAvailable && w.Properties.IsOffscreen.ValueOrDefault == false;
            }
            catch
            {
                return false;
            }
        }
        // This method use for verify instructor clickable
        public bool IsInstructorTileClickable(string instructorTileText = Names.InstructorTileText)
        {
            try
            {
                var homeWindow = RequireWindow(ScenarioKeys.HomeWindow);
                var tile = homeWindow
                    .FindFirstDescendant(cf => cf.ByText(instructorTileText))
                    ?.AsButton();

                return tile != null && tile.IsEnabled;
            }
            catch
            {
                return false;
            }
        }
    }
}
