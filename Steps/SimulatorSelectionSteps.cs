using LLEAP.UITestAutomation.Infrastructure;
using LLEAP.UITestAutomation.Pages;
using TechTalk.SpecFlow;

namespace LLEAP.UITestAutomation.Steps
{
    [Binding]
    public class SimulatorSelectionSteps
    {
        private readonly SimulatorSelectionPage _sim;
        private readonly SoftAssert _softAssert;

        public SimulatorSelectionSteps(ScenarioContext scenarioContext)

        {
            _sim = new SimulatorSelectionPage(scenarioContext);
            _softAssert = scenarioContext["SoftAssert"] as SoftAssert;
        }

        [Given(@"I select the Local Computer as simulator host")]
        public void GivenISelectTheLocalComputerAsSimulatorHost()
        {
            _sim.SelectLocalComputerAsHost();
            _softAssert.That(
               _sim.IsLocalComputerTileVisible(),
               "Local Computer tile not visible after selection attempt."
           );
        }

        [Given(@"I select the ""(.*)"" simulator")]
        public void GivenISelectTheSimulator(string simulatorName)
        {
            _sim.SelectSimulator(simulatorName);
            _softAssert.That(
               _sim.IsSimulatorVisible(simulatorName),
               $"Simulator '{simulatorName}' not visible/selected after clicking."
           );
        }

        [Given(@"I go to Manual Mode")]
        public void GivenIGoToManualMode()
        {
            _sim.GoToManualMode();
            _softAssert.That(
                _sim.IsManualModeVisible(),
                "Manual Mode UI was not visible after switching."
            );
        }

        [Given(@"I select the ""(.*)"" theme")]
        public void GivenISelectTheTheme(string themeName)
        {
            _sim.SelectTheme(themeName);
            _softAssert.That(
                _sim.IsThemeSelectionDialogClosed(),
                "Theme dialog did not close after selecting theme and pressing OK."
            );
        }
    }
}
