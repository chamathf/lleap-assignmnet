using FlaUI.Core.AutomationElements;
using TechTalk.SpecFlow;

namespace LLEAP.UITestAutomation.Infrastructure
{
    [Binding]
    public class Hooks
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly FlaUIAppManager _appManager = new();

        public Hooks(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            var scenarioName = _scenarioContext.ScenarioInfo.Title;

          
            _scenarioContext["SoftAssert"] = new SoftAssert(() =>
            {
                
                if (_scenarioContext.TryGetValue("SessionWindow", out var winObj)
                    && winObj is AutomationElement window)
                {
                    return _appManager.TakeScreenshot(window, scenarioName);
                }

                
                return "No window available for screenshot.";
            });
        }

        [AfterScenario]
        public void AfterScenario()
        {
            if (_scenarioContext.TryGetValue("SoftAssert", out var saObj)
                && saObj is SoftAssert softAssert)
            {
                softAssert.AssertAll();
            }
        }
    }
}
