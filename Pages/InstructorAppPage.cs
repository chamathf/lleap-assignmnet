using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using System.Diagnostics;
using TechTalk.SpecFlow;

namespace LLEAP.UITestAutomation.Pages
{
    public class InstructorAppPage : FlaUiPageBase
    {
        public InstructorAppPage(ScenarioContext scenarioContext) : base(scenarioContext) { }

        // locators
       
        private static class ScenarioKeys
        {
            public const string InstructorWindow = "InstructorWindow";
        }

        private static class Names
        {
            public const string InstructorProcessName = "InstructorApplication";
            public const string AddLicenseLater = "Add license later";
        }

        
        // This method use for Open instructor window
        
        public void AttachToInstructorWindow(
            string processName = Names.InstructorProcessName,
            TimeSpan? timeout = null,
            string instructorWindowKey = ScenarioKeys.InstructorWindow)
        {
            var t = timeout ?? TimeSpan.FromSeconds(20);
            Window? instructorWindow = null;

            RetryUntilSuccess(() =>
            {
                var desktop = Automation.GetDesktop();
                var allWindows = desktop.FindAllChildren(cf => cf.ByControlType(ControlType.Window));

                var element = allWindows.FirstOrDefault(w =>
                {
                    var pid = w.Properties.ProcessId.Value;
                    var procName = Process.GetProcessById(pid).ProcessName;
                    return procName.Equals(processName, StringComparison.OrdinalIgnoreCase);
                });

                if (element == null)
                    throw new Exception("Instructor window not found yet.");

                instructorWindow = element.AsWindow();

            }, t, TimeSpan.FromMilliseconds(250), "Timed out waiting for Instructor window.");

            Scenario[instructorWindowKey] = instructorWindow!;
        }

        
        // This method use for click License later
        
        public void ClickAddLicenseLater(
            string instructorWindowKey = ScenarioKeys.InstructorWindow,
            string addLicenseLaterText = Names.AddLicenseLater)
        {
            var instructorWindow = RequireWindow(instructorWindowKey);

            var button = instructorWindow
                .FindFirstDescendant(cf =>
                    cf.ByControlType(ControlType.Button)
                      .And(cf.ByText(addLicenseLaterText)))
                ?.AsButton();

            if (button == null)
                throw new InvalidOperationException($"Could not find '{addLicenseLaterText}' button.");

            button.Invoke();
        }

        
        // This method use for Instructor window Verifications
        
        public bool IsInstructorWindowVisible(string instructorWindowKey = ScenarioKeys.InstructorWindow)
        {
            try
            {
                var w = RequireWindow(instructorWindowKey);
                return w.IsAvailable && w.Properties.IsOffscreen.ValueOrDefault == false;
            }
            catch
            {
                return false;
            }
        }

        // This method use for Add License Later Button Verification
        public bool IsAddLicenseLaterButtonVisible(
            string instructorWindowKey = ScenarioKeys.InstructorWindow,
            string addLicenseLaterText = Names.AddLicenseLater)
        {
            try
            {
                var w = RequireWindow(instructorWindowKey);
                var btn = w.FindFirstDescendant(cf =>
                    cf.ByControlType(ControlType.Button).And(cf.ByText(addLicenseLaterText)));
                return btn != null;
            }
            catch
            {
                return false;
            }
        }
    }
}
