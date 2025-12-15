using LLEAP.UITestAutomation.Infrastructure;
using LLEAP.UITestAutomation.Pages;
using TechTalk.SpecFlow;

namespace LLEAP.UITestAutomation.Steps
{
    [Binding]
    public class SimulationHomeSteps
    {
        private readonly SimulationHomePage _home;
        private readonly SoftAssert _softAssert;

        public SimulationHomeSteps(ScenarioContext scenarioContext)
        {
            _home = new SimulationHomePage(scenarioContext);
            _softAssert = scenarioContext["SoftAssert"] as SoftAssert;
        }

        [Given(@"LLEAP is installed and configured")]
        public void GivenLLEAPIsInstalledAndConfigured()
        {
            _home.LaunchSimulationHome();
            _softAssert.That(
               _home.IsHomeWindowVisible(),
               "Simulation Home window was not visible after launch."
           );

        }

        [Given(@"I start Laerdal Simulation Home")]
        public void GivenIStartLaerdalSimulationHome()
        {
            _home.StartInstructorApplication();
            _softAssert.That(
               _home.IsInstructorTileClickable(),
               "Instructor Application tile not found/clickable."
           );
        }
    }
}
