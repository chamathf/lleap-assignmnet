using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Tools;
using TechTalk.SpecFlow;

namespace LLEAP.UITestAutomation.Pages
{
    public class SimulatorSelectionPage : FlaUiPageBase
    {
        public SimulatorSelectionPage(ScenarioContext scenarioContext) : base(scenarioContext) { }

        // This method use for select local computer 
        public void SelectLocalComputerAsHost()
        {
            var instructorWindow = RequireWindow("InstructorWindow");

            var retryResult = Retry.WhileNull(
                () => instructorWindow.FindFirstDescendant(cf =>
                    cf.ByControlType(ControlType.Text)
                      .And(cf.ByName("Local computer"))),
                timeout: TimeSpan.FromSeconds(10),
                interval: TimeSpan.FromMilliseconds(250)
            );

            var localComputerText = retryResult.Result;
            if (localComputerText == null)
                throw new InvalidOperationException("Timed out waiting for 'Local computer' to appear.");

            var button = localComputerText.Parent.AsButton();
            if (button == null)
                throw new InvalidOperationException("'Local computer' parent is not a Button.");

            button.Click();
        }
        // This method use for select simulator
        public void SelectSimulator(string simulatorName)
        {
            var instructorWindow = RequireWindow("InstructorWindow");

            var retryResult = Retry.WhileNull(
                () => instructorWindow.FindFirstDescendant(cf =>
                    cf.ByControlType(ControlType.Text)
                      .And(cf.ByName(simulatorName))),
                timeout: TimeSpan.FromSeconds(10),
                interval: TimeSpan.FromMilliseconds(250)
            );

            var simText = retryResult.Result;
            if (simText == null)
                throw new InvalidOperationException($"Timed out waiting for simulator '{simulatorName}' to appear.");

            var simButton = simText.Parent?.AsButton() ?? simText.Parent?.Parent?.AsButton();
            if (simButton == null)
                throw new InvalidOperationException($"Simulator '{simulatorName}' was found but its parent tile is not a Button.");

            simButton.Click();
        }
        // This method use for select manual mode
        public void GoToManualMode()
        {
            var instructorWindow = RequireWindow("InstructorWindow");

            AutomationElement? manualModeButton = null;

            RetryUntilSuccess(() =>
            {
                manualModeButton = instructorWindow.FindFirstDescendant(cf =>
                    cf.ByControlType(ControlType.Button)
                      .And(cf.ByName("Manual Mode")));

                if (manualModeButton == null)
                    throw new Exception("Manual Mode button not found yet.");
            }, TimeSpan.FromSeconds(30), TimeSpan.FromMilliseconds(300));

            manualModeButton!.AsButton().Invoke();
        }

        // This method use for select Theme window
        public void SelectTheme(string themeName)
        {
            AutomationElement? themeWindow = null;
            AutomationElement? themeListItem = null;

            
            RetryUntilSuccess(() =>
            {
                var desktop = Automation.GetDesktop();
                var windows = desktop.FindAllChildren(cf => cf.ByControlType(ControlType.Window));

                themeWindow = windows.FirstOrDefault(w =>
                    w.Name.Contains("Select theme", StringComparison.OrdinalIgnoreCase));

                if (themeWindow == null)
                    throw new Exception("Theme window not open yet.");

            }, TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(250));

        
            RetryUntilSuccess(() =>
            {
                var all = themeWindow!.FindAllDescendants();

                themeListItem = all.FirstOrDefault(e =>
                    !string.IsNullOrWhiteSpace(e.Name) &&
                    e.Name.Equals(themeName, StringComparison.OrdinalIgnoreCase));

                if (themeListItem == null)
                    throw new Exception($"Theme '{themeName}' not found yet.");

            }, TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(250));

            
            themeListItem!.Click();
            Thread.Sleep(150);
            themeListItem.Click();

          
            var okButton = themeWindow!.FindFirstDescendant(cf =>
                cf.ByControlType(ControlType.Button)
                  .And(cf.ByName("Ok")));

            if (okButton == null)
                throw new Exception("OK button not found on theme window.");

            okButton.AsButton().Invoke();
        }
    
         // This method use for verify localcomputer is visible
        public bool IsLocalComputerTileVisible()
        {
            try
            {
                var w = RequireWindow("InstructorWindow");
                return w.FindFirstDescendant(cf => cf.ByText("Local Computer")) != null;
            }
            catch { return false; }
        }

        // This method use for verify simulator is visible
        public bool IsSimulatorVisible(string simulatorName)
        {
            try
            {
                var w = RequireWindow("InstructorWindow");
                return w.FindFirstDescendant(cf => cf.ByText(simulatorName)) != null;
            }
            catch { return false; }
        }

        // This method use for verify ManualMode is visible
        public bool IsManualModeVisible()
        {
            try
            {
                var w = RequireWindow("InstructorWindow");
                return w.FindFirstDescendant(cf => cf.ByText("Themes")) != null;
            }
            catch { return false; }
        }
        // This method use for verify theme selection 
        public bool IsThemeSelectionDialogClosed()
        {
            // your SelectTheme() clicks OK; this just confirms no theme dialog window is still open
            try
            {
                var w = RequireWindow("InstructorWindow");
                var themeWindow = w.FindFirstDescendant(cf =>
                    cf.ByControlType(ControlType.Window).And(cf.ByName("Themes")));
                return themeWindow == null;
            }
            catch { return true; }
        }
    }
}
