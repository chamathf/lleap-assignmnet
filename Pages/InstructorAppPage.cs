using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using System.Diagnostics;
using TechTalk.SpecFlow;

namespace LLEAP.UITestAutomation.Pages
{
    public class InstructorAppPage : FlaUiPageBase
    {
        public InstructorAppPage(ScenarioContext scenarioContext) : base(scenarioContext) { }

        //This method use for Open instructor window
        public void AttachToInstructorWindow(string processName = "InstructorApplication", TimeSpan? timeout = null)
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

            Scenario["InstructorWindow"] = instructorWindow!;
        }

        //This method use for click License later
        public void ClickAddLicenseLater()
        {
            var instructorWindow = RequireWindow("InstructorWindow");

            var button = instructorWindow
                .FindFirstDescendant(cf =>
                    cf.ByControlType(ControlType.Button)
                      .And(cf.ByText("Add license later")))
                ?.AsButton();

            if (button == null)
                throw new InvalidOperationException("Could not find 'Add license later' button.");

            button.Invoke();
        }

        // This method use for verify visibleof instructor window
        public bool IsInstructorWindowVisible()
        {
            try
            {
                var w = RequireWindow("InstructorWindow");
                return w.IsAvailable && w.Properties.IsOffscreen.ValueOrDefault == false;
            }
            catch { return false; }
        }

        // This method use for verify Add license later button 
        public bool IsAddLicenseLaterButtonVisible()
        {
            try
            {
                var w = RequireWindow("InstructorWindow");
                var btn = w.FindFirstDescendant(cf =>
                    cf.ByControlType(ControlType.Button).And(cf.ByText("Add license later")));
                return btn != null;
            }
            catch { return false; }
        }
    }
}
