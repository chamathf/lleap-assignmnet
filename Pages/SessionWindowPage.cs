using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using FlaUI.Core.Tools;
using FlaUI.Core.WindowsAPI;
using System.Globalization;
using TechTalk.SpecFlow;

namespace LLEAP.UITestAutomation.Pages
{
    public class SessionWindowPage : FlaUiPageBase
    {
        public SessionWindowPage(ScenarioContext scenarioContext) : base(scenarioContext) { }

        
        // locators 
        
        private static class Names
        {
            public const string StartSession = "Start session";
            public const string SetHeartRateWindow = "Set Heart Rate";
            public const string Ok = "OK";
            public const string HrLabel = "HR";
            public const string Play = "Play";
        }

        private static class AutoIds
        {
            public const string EyesComboBox = "EyesComboBox";
            public const string LungComplianceRange = "range1";
            public const string HeartRatePane = "11";
            public const string HeartRateEdit = "2093";
            public const string PlayButton = "PlayButton";
        }

        private const string DefaultProcessName = "InstructorApplication";

        
        // This method use for start session
        
        public void StartSession(string startSessionButtonName = Names.StartSession)
        {
            Window? startDialog = null;

            RetryUntilSuccess(() =>
            {
                var desktop = Automation.GetDesktop();
                var windows = desktop.FindAllChildren(cf => cf.ByControlType(ControlType.Window));

                var dialogElement = windows.FirstOrDefault(w =>
                    w.FindFirstDescendant(cf =>
                        cf.ByControlType(ControlType.Button)
                          .And(cf.ByName(startSessionButtonName))) != null);

                if (dialogElement == null)
                    throw new Exception("Start session dialog not found yet.");

                startDialog = dialogElement.AsWindow();

            }, TimeSpan.FromSeconds(50), TimeSpan.FromMilliseconds(250),
               "Timed out waiting for Start session dialog.");

            var startButton = startDialog!
                .FindFirstDescendant(cf => cf.ByControlType(ControlType.Button)
                    .And(cf.ByName(startSessionButtonName)))
                ?.AsButton();

            if (startButton == null)
                throw new Exception($"'{startSessionButtonName}' button not found.");

            startButton.Invoke();
        }

        
        // This method use for get session window
        
        public Window GetInstructorSessionWindow(string processName = DefaultProcessName)
            => FindInstructorWindowByProcess(processName);

        // This method use maximize session wondow
        public void MaximizeSessionWindowIfNeeded(string processName = DefaultProcessName)
        {
            var window = GetInstructorSessionWindow(processName);
            var pattern = window.Patterns.Window.Pattern;

            if (pattern.WindowVisualState.Value != WindowVisualState.Maximized)
            {
                pattern.SetWindowVisualState(WindowVisualState.Maximized);
            }
        }
        // This method use for verify maxmized session window
        public bool IsSessionWindowMaximized(string processName = DefaultProcessName)
        {
            var window = GetInstructorSessionWindow(processName);
            return window.Patterns.Window.Pattern.WindowVisualState.Value
                   == WindowVisualState.Maximized;
        }
        // This method use for verify session window visible
        public bool IsSessionWindowVisible(string processName = DefaultProcessName)
        {
            try
            {
                var window = GetInstructorSessionWindow(processName);
                return window != null
                       && window.IsAvailable
                       && window.Properties.IsOffscreen.ValueOrDefault == false;
            }
            catch
            {
                return false;
            }
        }

        
        // This method use for set eyes closed
      
        public void SetEyes(
            string expected,
            string eyesComboAutomationId = AutoIds.EyesComboBox,
            string processName = DefaultProcessName)
        {
            var window = GetInstructorSessionWindow(processName);

            var eyesComboEl = window.FindFirstDescendant(cf =>
                cf.ByAutomationId(eyesComboAutomationId));

            if (eyesComboEl == null)
                throw new Exception($"Eyes ComboBox not found (AutomationId={eyesComboAutomationId}).");

            var eyesCombo = eyesComboEl.AsComboBox();
            if (eyesCombo == null)
                throw new Exception("Eyes element is not a ComboBox.");

            eyesCombo.Focus();
            eyesCombo.Click();

            try
            {
                eyesCombo.Expand();
            }
            catch
            {
                Keyboard.Press(VirtualKeyShort.ALT);
                Keyboard.Type(VirtualKeyShort.DOWN);
                Keyboard.Release(VirtualKeyShort.ALT);
            }

            var item = eyesCombo.Items.FirstOrDefault(i =>
                string.Equals(i.Text?.Trim(), expected.Trim(),
                    StringComparison.OrdinalIgnoreCase));

            if (item == null)
            {
                var available = string.Join(", ", eyesCombo.Items.Select(i => i.Text));
                try { eyesCombo.Collapse(); } catch { }
                throw new Exception($"Eyes option '{expected}' not found. Available: {available}");
            }

            item.Click();

            RetryUntilSuccess(() =>
            {
                var current = GetEyesValue(eyesComboAutomationId, processName);
                if (!string.Equals(current, expected, StringComparison.OrdinalIgnoreCase))
                    throw new Exception($"Eyes expected '{expected}' but was '{current}'.");
            }, TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(150),
               "Eyes value did not update.");
        }

        // This method use for get eyes values
        public string GetEyesValue(
            string eyesComboAutomationId = AutoIds.EyesComboBox,
            string processName = DefaultProcessName)
        {
            var window = GetInstructorSessionWindow(processName);

            var eyesComboEl = window.FindFirstDescendant(cf =>
                cf.ByAutomationId(eyesComboAutomationId));

            if (eyesComboEl == null)
                throw new Exception($"Eyes ComboBox not found (AutomationId={eyesComboAutomationId}).");

            var eyesCombo = eyesComboEl.AsComboBox();
            if (eyesCombo == null)
                throw new Exception("Eyes element is not a ComboBox.");

            try
            {
                return (eyesCombo.Patterns.Value.Pattern.Value.Value ?? "").Trim();
            }
            catch
            {
                return (eyesCombo.Name ?? "").Trim();
            }
        }

        
        // This methoduse for set lung compliance
        public void SetLungCompliancePercent(
            int targetPercent,
            int maxClicks = 50,
            string rangeAutomationId = AutoIds.LungComplianceRange,
            string processName = DefaultProcessName)
        {
            var window = GetInstructorSessionWindow(processName);

            var rangeEl = window.FindFirstDescendant(cf => cf.ByAutomationId(rangeAutomationId));
            if (rangeEl == null)
                throw new Exception($"Range label not found (AutomationId={rangeAutomationId}).");

            var rect = rangeEl.BoundingRectangle;

            int minusX = (int)(rect.Left - 14);
            int y = (int)(rect.Top + rect.Height / 2);

            Mouse.MoveTo(minusX, y);
            Mouse.Click();

            Retry.WhileFalse(
                () =>
                {
                    var el = window.FindFirstDescendant(cf => cf.ByAutomationId(rangeAutomationId));
                    return el != null && el.Name?.Trim() == "67";
                },
                TimeSpan.FromSeconds(20),
                TimeSpan.FromMilliseconds(150)
            );
        }

        // This method use for verify Lung compliance percent
        public int GetLungCompliancePercent(
            string rangeAutomationId = AutoIds.LungComplianceRange,
            string processName = DefaultProcessName)
        {
            try
            {
                var window = GetInstructorSessionWindow(processName);

                var valueElement = window.FindFirstDescendant(cf =>
                    cf.ByAutomationId(rangeAutomationId));

                if (valueElement == null)
                    throw new Exception($"Lung compliance value element not found (AutomationId={rangeAutomationId}).");

                if (!string.IsNullOrWhiteSpace(valueElement.Name)
                    && int.TryParse(valueElement.Name, NumberStyles.Any, CultureInfo.InvariantCulture, out int nameValue))
                {
                    return nameValue;
                }

                if (valueElement.Patterns.Value.IsSupported)
                {
                    var raw = valueElement.Patterns.Value.Pattern.Value.Value;
                    if (int.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out int valuePattern))
                        return valuePattern;
                }

                throw new Exception("Unable to read Lung Compliance percent from UI element.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Failed to read Lung Compliance percent.", ex);
            }
        }

        
        // This method use for set heart rate
        
        public void SetHeartRate(
            int target,
            string hrPaneAutomationId = AutoIds.HeartRatePane,
            string setHrWindowName = Names.SetHeartRateWindow,
            string hrEditAutomationId = AutoIds.HeartRateEdit,
            string okButtonName = Names.Ok,
            string processName = DefaultProcessName)
        {
            var window = GetInstructorSessionWindow(processName);

            var hrPane = window.FindFirstDescendant(cf => cf.ByAutomationId(hrPaneAutomationId));
            if (hrPane == null)
                throw new Exception($"HR pane not found (AutomationId={hrPaneAutomationId}).");

            var r = hrPane.BoundingRectangle;
            int x = (int)(r.Left + (r.Width * 0.45));
            int y = (int)(r.Top + (r.Height * 0.50));

            Mouse.MoveTo(x, y);
            Mouse.Click();

            var setHrWindow = Retry.WhileNull(
                () => window.FindFirstDescendant(cf =>
                        cf.ByControlType(ControlType.Window).And(cf.ByName(setHrWindowName)))
                    ?.AsWindow(),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromMilliseconds(150)
            ).Result;

            if (setHrWindow == null)
                throw new Exception($"{setHrWindowName} window not found.");

            var hrEdit = setHrWindow.FindFirstDescendant(cf =>
                cf.ByControlType(ControlType.Edit).And(cf.ByAutomationId(hrEditAutomationId)))
                ?.AsTextBox();

            if (hrEdit == null)
                throw new Exception($"Heart Rate edit box not found (AutomationId={hrEditAutomationId}).");

            hrEdit.Focus();
            hrEdit.Text = target.ToString(CultureInfo.InvariantCulture);

            var okBtn = setHrWindow.FindFirstDescendant(cf =>
                cf.ByControlType(ControlType.Button).And(cf.ByName(okButtonName)))
                ?.AsButton();

            if (okBtn == null)
                throw new Exception($"OK button not found in {setHrWindowName} window (Name={okButtonName}).");

            okBtn.Invoke();

            Retry.WhileTrue(
                () => setHrWindow.Properties.IsOffscreen.ValueOrDefault == false,
                TimeSpan.FromSeconds(5),
                TimeSpan.FromMilliseconds(150)
            );
        }

        // This method use for get heart rate
        public int GetHeartRate(
            string hrLabelName = Names.HrLabel,
            string processName = DefaultProcessName)
        {
            var win = GetInstructorSessionWindow(processName);

            var hrLabel = win.FindFirstDescendant(cf =>
                cf.ByControlType(ControlType.Text).And(cf.ByName(hrLabelName)));

            if (hrLabel == null)
                throw new Exception($"HR label not found (Name={hrLabelName}).");

            var hrRect = hrLabel.BoundingRectangle;
            var texts = win.FindAllDescendants(cf => cf.ByControlType(ControlType.Text));

            AutomationElement? best = null;
            double bestScore = double.MaxValue;

            foreach (var t in texts)
            {
                var name = (t.Name ?? "").Trim();
                if (!int.TryParse(name, out var _)) continue;

                var r = t.BoundingRectangle;
                if (r.Left < hrRect.Right - 2) continue;
                if (r.Top > hrRect.Bottom + 15 || r.Bottom < hrRect.Top - 15) continue;

                var score = r.Left - hrRect.Right;
                if (score < bestScore)
                {
                    bestScore = score;
                    best = t;
                }
            }

            if (best == null)
                throw new Exception("Heart rate value not found.");

            return int.Parse(best.Name!, CultureInfo.InvariantCulture);
        }

        
        // This method use for select Vocal coughing
        
        public void SelectVocal(string vocalName, string processName = DefaultProcessName)
        {
            var window = GetInstructorSessionWindow(processName);

            var item = Retry.WhileNull(
                () => window.FindFirstDescendant(cf =>
                        cf.ByControlType(ControlType.TreeItem).And(cf.ByName(vocalName)))
                    ?.AsTreeItem(),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromMilliseconds(150)
            ).Result;

            if (item == null)
                throw new Exception($"Vocal item not found: '{vocalName}'");

            item.Select();
        }

        // This method use for play vocal once
        public void PlayVocalOnce(
            string playButtonAutomationId = AutoIds.PlayButton,
            string processName = DefaultProcessName)
        {
            var window = GetInstructorSessionWindow(processName);

            var playBtn = window.FindFirstDescendant(cf =>
                cf.ByControlType(ControlType.Button).And(cf.ByAutomationId(playButtonAutomationId)))
                ?.AsButton();

            if (playBtn == null)
                throw new Exception($"Play button not found (AutomationId={playButtonAutomationId}).");

            playBtn.Invoke();
        }
        // This method use for get vocal name
        public string GetSelectedVocalName(string processName = DefaultProcessName)
        {
            var window = GetInstructorSessionWindow(processName);

            var treeItems = window
                .FindAllDescendants(cf => cf.ByControlType(ControlType.TreeItem))
                .Select(x => x.AsTreeItem())
                .Where(t => t != null);

            var selected = treeItems.FirstOrDefault(t => t.IsSelected == true);

            if (selected == null || string.IsNullOrWhiteSpace(selected.Name))
                throw new InvalidOperationException(
                    "No selected coughing found in the Voices section.");

            return selected.Name.Trim();
        }
        // This method use for verify play button aviable
        public bool IsPlayButtonEnabled(
            string playText = Names.Play,
            string playButtonAutomationId = AutoIds.PlayButton,
            string processName = DefaultProcessName)
        {
            try
            {
                var window = GetInstructorSessionWindow(processName);

                var playButton =
                    window.FindFirstDescendant(cf =>
                        cf.ByControlType(ControlType.Button)
                          .And(cf.ByText(playText)))
                    ??
                    window.FindFirstDescendant(cf =>
                        cf.ByControlType(ControlType.Button)
                          .And(cf.ByAutomationId(playButtonAutomationId)));

                if (playButton == null)
                    return false;

                var button = playButton.AsButton();
                return button != null && button.IsEnabled;
            }
            catch
            {
                return false;
            }
        }

        // This method use for close the session window
        public void CloseSessionWindow(string processName = DefaultProcessName)
        {
            var window = GetInstructorSessionWindow(processName);
            window.Close();
        }
    }
}
