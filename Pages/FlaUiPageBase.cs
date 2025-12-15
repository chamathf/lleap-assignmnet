using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using System.Diagnostics;
using TechTalk.SpecFlow;

namespace LLEAP.UITestAutomation.Pages
{
    
    // Base class for FlaUI page objects.
    public abstract class FlaUiPageBase
    {
        protected readonly ScenarioContext Scenario;

        protected FlaUiPageBase(ScenarioContext scenarioContext)
        {
            Scenario = scenarioContext;
        }

        protected UIA3Automation Automation
        {
            get
            {
                if (!Scenario.TryGetValue("Automation", out var automationObj) || automationObj is not UIA3Automation automation)
                    throw new InvalidOperationException("Automation not found in ScenarioContext.");
                return automation;
            }
        }

        protected Application App
        {
            get
            {
                if (!Scenario.TryGetValue("App", out var appObj) || appObj is not Application app)
                    throw new InvalidOperationException("App not found in ScenarioContext.");
                return app;
            }
        }

        protected Window RequireWindow(string key)
        {
            if (!Scenario.TryGetValue(key, out var winObj) || winObj is not Window win)
                throw new InvalidOperationException($"{key} not found or invalid in ScenarioContext.");
            return win;
        }

        protected void RetryUntilSuccess(Action action, TimeSpan timeout, TimeSpan interval, string? timeoutMessage = null)
        {
            var start = DateTime.UtcNow;
            Exception? last = null;

            while (DateTime.UtcNow - start < timeout)
            {
                try
                {
                    action();
                    return;
                }
                catch (Exception ex)
                {
                    last = ex;
                    Thread.Sleep(interval);
                }
            }

            throw new TimeoutException(timeoutMessage ?? "Timed out waiting for UI element/window.", last);
        }

        
        // Finds the Instructor window by process name.
        
        protected Window FindInstructorWindowByProcess(string processName = "InstructorApplication")
        {
            var desktop = Automation.GetDesktop();
            var windows = desktop.FindAllChildren(cf => cf.ByControlType(ControlType.Window));

            var winEl = windows.FirstOrDefault(w =>
            {
                try
                {
                    var pid = w.Properties.ProcessId.Value;
                    var proc = Process.GetProcessById(pid);
                    return proc.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase);
                }
                catch
                {
                    return false;
                }
            });

            if (winEl == null)
                throw new Exception($"{processName} window not found.");

            return winEl.AsWindow();
        }
    }
}
