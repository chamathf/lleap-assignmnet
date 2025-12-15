using LLEAP.UITestAutomation.Infrastructure;
using LLEAP.UITestAutomation.Pages;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace LLEAP.UITestAutomation.Steps
{
    [Binding]
    public class SessionControlSteps
    {
        private readonly SessionWindowPage _session;
        private readonly SoftAssert _softAssert;

        public SessionControlSteps(ScenarioContext scenarioContext)
        {
            _session = new SessionWindowPage(scenarioContext);
            _softAssert = scenarioContext["SoftAssert"] as SoftAssert;
        }

        [When(@"I start the session")]
        public void WhenIStartTheSession()
        {
            _session.StartSession();
            _softAssert.That(
               _session.IsSessionWindowVisible(),
               "Session window was not visible after starting the session."
           );
        }

        [Then(@"the session window is maximized")]
        public void ThenTheSessionWindowIsMaximized()
        {
            _session.MaximizeSessionWindowIfNeeded();
            _softAssert.That(
                _session.IsSessionWindowMaximized(),
                "Session window is not maximized."
            );
        }

        [Then(@"the eyes control is set to ""(.*)""")]
        public void ThenTheEyesControlIsSetTo(string expected)
        {
            // Action
            _session.SetEyes(expected);

            // Assertion
            _softAssert.That(
                _session.GetEyesValue(),
                expected,
                $"Eyes value mismatch."
            );
        }

            [Then(@"the lung compliance is set to (.*) percent")]
        public void ThenTheLungComplianceIsSetToPercent(int percent)
        {
            _session.SetLungCompliancePercent(percent);
            _softAssert.That(
                _session.GetLungCompliancePercent(),
                percent,
                "Lung compliance percent mismatch."
            );
        }

        [Then(@"the heart rate is set to (.*)")]
        public void ThenTheHeartRateIsSetTo(int expected)
        {
            _session.SetHeartRate(expected);
            _softAssert.That(
               _session.GetHeartRate(),
               expected,
               "Heart rate mismatch."

           );
        }

        [Then(@"the ""(.*)"" voice is played once")]
        public void ThenTheVoiceIsPlayedOnce(string voice)
        {
            var vocalName = voice.Equals("Coughing", StringComparison.OrdinalIgnoreCase)
        ? "Coughing (long)"
        : voice;

            _session.SelectVocal(vocalName);
            _softAssert.That(
                _session.GetSelectedVocalName(),
                vocalName,
                $"Expected vocal '{vocalName}' to be selected."
            );

            // ? Soft assertion: play button should be enabled
            _softAssert.That(
                _session.IsPlayButtonEnabled(),
                "Play button was not enabled before playing the voice."
            );

            _session.PlayVocalOnce();
        }
    }
}
