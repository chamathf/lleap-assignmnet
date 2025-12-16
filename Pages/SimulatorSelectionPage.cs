using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Tools;
using TechTalk.SpecFlow;

namespace LLEAP.UITestAutomation.Pages
{
    public class SimulatorSelectionPage : FlaUiPageBase
    {
        public SimulatorSelectionPage(ScenarioContext scenarioContext) : base(scenarioContext) { }

        
        // locators 
       
        private static class ScenarioKeys
        {
            public const string InstructorWindow = "InstructorWindow";
        }

        private static class Names
        {
            public const string LocalComputer = "Local computer"; 
            public const string LocalComputerAlt = "Local Computer";
            public const string ManualMode = "Manual Mode";
            public const string SelectThemeWindowContains = "Select theme";
            public const string ThemeOk = "Ok";

            public const string Themes = "Themes"; 
        }

        
        // This method use for select local computer
        
        public void SelectLocalComputerAsHost(
            string instructorWindowKey = ScenarioKeys.InstructorWindow,
            string localComputerName = Names.LocalComputer)
        {
            var instructorWindow = RequireWindow(instructorWindowKey);

            var retryResult = Retry.WhileNull(
                () => instructorWindow.FindFirstDescendant(cf =>
                    cf.ByControlType(ControlType.Text)
                      .And(cf.ByName(localComputerName))),
                timeout: TimeSpan.FromSeconds(10),
                interval: TimeSpan.FromMilliseconds(250)
            );

            var localComputerText = retryResult.Result;
            if (localComputerText == null)
                throw new InvalidOperationException($"Timed out waiting for '{localComputerName}' to appear.");

            var button = localComputerText.Parent.AsButton();
            if (button == null)
                throw new InvalidOperationException($"'{localComputerName}' parent is not a Button.");

            button.Click();
        }

        
        // This method use for select simulator
        
        public void SelectSimulator(
            string simulatorName,
            string instructorWindowKey = ScenarioKeys.InstructorWindow)
        {
            var instructorWindow = RequireWindow(instructorWindowKey);

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
                throw new InvalidOperationException(
                    $"Simulator '{simulatorName}' was found but its parent tile is not a Button.");

            simButton.Click();
        }

        
        // This method use for select manual mode
       
        public void GoToManualMode(
            string instructorWindowKey = ScenarioKeys.InstructorWindow,
            string manualModeButtonName = Names.ManualMode)
        {
            var instructorWindow = RequireWindow(instructorWindowKey);

            AutomationElement? manualModeButton = null;

            RetryUntilSuccess(() =>
            {
                manualModeButton = instructorWindow.FindFirstDescendant(cf =>
                    cf.ByControlType(ControlType.Button)
                      .And(cf.ByName(manualModeButtonName)));

                if (manualModeButton == null)
                    throw new Exception("Manual Mode button not found yet.");
            }, TimeSpan.FromSeconds(30), TimeSpan.FromMilliseconds(300));

            manualModeButton!.AsButton().Invoke();
        }

        
        // This method use for select Theme window
        
        public void SelectTheme(
            string themeName,
            string themeWindowTitleContains = Names.SelectThemeWindowContains,
            string okButtonName = Names.ThemeOk)
        {
            AutomationElement? themeWindow = null;
            AutomationElement? themeListItem = null;

           
            RetryUntilSuccess(() =>
            {
                var desktop = Automation.GetDesktop();
                var windows = desktop.FindAllChildren(cf => cf.ByControlType(ControlType.Window));

                themeWindow = windows.FirstOrDefault(w =>
                    (w.Name ?? string.Empty).Contains(themeWindowTitleContains, StringComparison.OrdinalIgnoreCase));

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
                  .And(cf.ByName(okButtonName)));

            if (okButton == null)
                throw new Exception($"OK button not found on theme window (Name={okButtonName}).");

            okButton.AsButton().Invoke();
        }

        
        // This method use for verify local computer tile is visible 
        
        public bool IsLocalComputerTileVisible(
            string instructorWindowKey = ScenarioKeys.InstructorWindow,
            string localComputerText = Names.LocalComputerAlt)
        {
            try
            {
                var w = RequireWindow(instructorWindowKey);
                return w.FindFirstDescendant(cf => cf.ByText(localComputerText)) != null;
            }
            catch
            {
                return false;
            }
        }
        // This method use for verify simulator is visible 
        public bool IsSimulatorVisible(
            string simulatorName,
            string instructorWindowKey = ScenarioKeys.InstructorWindow)
        {
            try
            {
                var w = RequireWindow(instructorWindowKey);
                return w.FindFirstDescendant(cf => cf.ByText(simulatorName)) != null;
            }
            catch
            {
                return false;
            }
        }
        // This method use for manual mode is visible 
        public bool IsManualModeVisible(
            string instructorWindowKey = ScenarioKeys.InstructorWindow,
            string themesText = Names.Themes)
        {
            try
            {
                var w = RequireWindow(instructorWindowKey);
                return w.FindFirstDescendant(cf => cf.ByText(themesText)) != null;
            }
            catch
            {
                return false;
            }
        }
        // This method use for verify theme selection
        public bool IsThemeSelectionDialogClosed(
            string instructorWindowKey = ScenarioKeys.InstructorWindow,
            string themeWindowName = Names.Themes)
        {
           
            try
            {
                var w = RequireWindow(instructorWindowKey);
                var themeWindow = w.FindFirstDescendant(cf =>
                    cf.ByControlType(ControlType.Window).And(cf.ByName(themeWindowName)));

                return themeWindow == null;
            }
            catch
            {
                return true;
            }
        }
    }
}
