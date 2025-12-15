using LLEAP.UITestAutomation.Infrastructure;
using LLEAP.UITestAutomation.Pages;
using TechTalk.SpecFlow;

namespace LLEAP.UITestAutomation.Steps
{
    [Binding]
    public class InstructorAppSteps
    {
        private readonly InstructorAppPage _instructor;
        private readonly SoftAssert _softAssert;

        public InstructorAppSteps(ScenarioContext scenarioContext)
        {
            _instructor = new InstructorAppPage(scenarioContext);
            _softAssert = scenarioContext["SoftAssert"] as SoftAssert;
        }

        [Given(@"I open the Instructor Application")]
        public void GivenIOpenTheInstructorApplication()
        {
            _instructor.AttachToInstructorWindow();
            Assert.That(_instructor.IsInstructorWindowVisible(), Is.True, "Instructor window was not visible after attaching.");
        }

        [Given(@"I choose to add the license later")]
        public void GivenIChooseToAddTheLicenseLater()
        {
            _instructor.ClickAddLicenseLater();
            _softAssert.That(
               _instructor.IsAddLicenseLaterButtonVisible() == false,
               "Add License Later dialog still visible after clicking."
           );
        }
    }
}
