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

        //This method use for start the session
        public void StartSession()
        {
            Window? startDialog = null;

            RetryUntilSuccess(() =>
            {
                var desktop = Automation.GetDesktop();
                var windows = desktop.FindAllChildren(cf => cf.ByControlType(ControlType.Window));

                var dialogElement = windows.FirstOrDefault(w =>
                    w.FindFirstDescendant(cf =>
                        cf.ByControlType(ControlType.Button)
                          .And(cf.ByName("Start session"))) != null);

                if (dialogElement == null)
                    throw new Exception("Start session dialog not found yet.");

                startDialog = dialogElement.AsWindow();

            }, TimeSpan.FromSeconds(50), TimeSpan.FromMilliseconds(250),
               "Timed out waiting for Start session dialog.");

            var startButton = startDialog!
                .FindFirstDescendant(cf => cf.ByControlType(ControlType.Button)
                    .And(cf.ByName("Start session")))
                ?.AsButton();

            if (startButton == null)
                throw new Exception("'Start session' button not found.");

            startButton.Invoke();
        }

        // This method use for get instructor session window
        public Window GetInstructorSessionWindow(string processName = "InstructorApplication")
            => FindInstructorWindowByProcess(processName);

        public void MaximizeSessionWindowIfNeeded()
        {
            var window = GetInstructorSessionWindow();
            var pattern = window.Patterns.Window.Pattern;

            if (pattern.WindowVisualState.Value != WindowVisualState.Maximized)
            {
                pattern.SetWindowVisualState(WindowVisualState.Maximized);
            }
        }

        // This method use for get maximized session window
        public bool IsSessionWindowMaximized()
        {
            var window = GetInstructorSessionWindow();
            return window.Patterns.Window.Pattern.WindowVisualState.Value
                   == WindowVisualState.Maximized;
        }

        // This method use for set eyes as closed
        public void SetEyes(string expected)
        {
            var window = GetInstructorSessionWindow();

            var eyesComboEl = window.FindFirstDescendant(cf =>
                cf.ByAutomationId("EyesComboBox"));

            if (eyesComboEl == null)
                throw new Exception("Eyes ComboBox not found (AutomationId=EyesComboBox).");

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
                var current = GetEyesValue();
                if (!string.Equals(current, expected, StringComparison.OrdinalIgnoreCase))
                    throw new Exception($"Eyes expected '{expected}' but was '{current}'.");
            }, TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(150),
               "Eyes value did not update.");
        }

        //This method use for get eyes value
        public string GetEyesValue()
        {
            var window = GetInstructorSessionWindow();

            var eyesComboEl = window.FindFirstDescendant(cf =>
                cf.ByAutomationId("EyesComboBox"));

            if (eyesComboEl == null)
                throw new Exception("Eyes ComboBox not found (AutomationId=EyesComboBox).");

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

        // This method use for set lung compilence
        public void SetLungCompliancePercent(int targetPercent, int maxClicks = 50)
        {
            var window = GetInstructorSessionWindow();

            var range67 = window.FindFirstDescendant(cf => cf.ByAutomationId("range1"));
            if (range67 == null)
                throw new Exception("Range label (67) not found (AutomationId=range1).");

            var rect = range67.BoundingRectangle;

            int minusX = (int)(rect.Left - 14);
            int y = (int)(rect.Top + rect.Height / 2);

            Mouse.MoveTo(minusX, y);
            Mouse.Click();
            Retry.WhileFalse(
                () =>
                {
                    var el = window.FindFirstDescendant(cf => cf.ByAutomationId("range1"));
                    return el != null && el.Name?.Trim() == "67";
                },
                TimeSpan.FromSeconds(20),
                TimeSpan.FromMilliseconds(150)
            );
        }

        // This method use for set heart rate 
        public void SetHeartRate(int target)
        {
            var window = GetInstructorSessionWindow();

            var hrPane = window.FindFirstDescendant(cf => cf.ByAutomationId("11"));
            if (hrPane == null)
                throw new Exception("HR pane not found (AutomationId=11).");

            var r = hrPane.BoundingRectangle;
            int x = (int)(r.Left + (r.Width * 0.45));
            int y = (int)(r.Top + (r.Height * 0.50));

            Mouse.MoveTo(x, y);
            Mouse.Click();


            var setHrWindow = Retry.WhileNull(
                () => window.FindFirstDescendant(cf =>
                        cf.ByControlType(ControlType.Window).And(cf.ByName("Set Heart Rate")))
                    ?.AsWindow(),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromMilliseconds(150)
            ).Result;

            if (setHrWindow == null)
                throw new Exception("Set Heart Rate window not found.");


            var hrEdit = setHrWindow.FindFirstDescendant(cf =>
                cf.ByControlType(ControlType.Edit).And(cf.ByAutomationId("2093")))
                ?.AsTextBox();

            if (hrEdit == null)
                throw new Exception("Heart Rate edit box not found (AutomationId=2093).");

            hrEdit.Focus();
            hrEdit.Text = target.ToString(CultureInfo.InvariantCulture); // clears old value and sets new


            var okBtn = setHrWindow.FindFirstDescendant(cf =>
                cf.ByControlType(ControlType.Button).And(cf.ByName("OK")))
                ?.AsButton();

            if (okBtn == null)
                throw new Exception("OK button not found in Set Heart Rate window.");

            okBtn.Invoke();


            Retry.WhileTrue(
                () => setHrWindow.Properties.IsOffscreen.ValueOrDefault == false,
                TimeSpan.FromSeconds(5),
                TimeSpan.FromMilliseconds(150)
            );
        }

        // This method use for Get heart rate
        public int GetHeartRate()
        {
            var win = GetInstructorSessionWindow();

            var hrLabel = win.FindFirstDescendant(cf =>
                cf.ByControlType(ControlType.Text).And(cf.ByName("HR")));

            if (hrLabel == null)
                throw new Exception("HR label not found.");

            var hrRect = hrLabel.BoundingRectangle;
            var texts = win.FindAllDescendants(cf => cf.ByControlType(ControlType.Text));

            AutomationElement? best = null;
            double bestScore = double.MaxValue;

            foreach (var t in texts)
            {
                var name = (t.Name ?? "").Trim();
                if (!int.TryParse(name, out var val)) continue;

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

        // This method use for select coughing
        public void SelectVocal(string vocalName)
        {
            var window = GetInstructorSessionWindow();

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

        // This method use for Play coughing sound
        public void PlayVocalOnce()
        {
            var window = GetInstructorSessionWindow();

            var playBtn = window.FindFirstDescendant(cf =>
                cf.ByControlType(ControlType.Button).And(cf.ByAutomationId("PlayButton")))
                ?.AsButton();

            if (playBtn == null)
                throw new Exception("Play button not found.");

            playBtn.Invoke();
        }
        // This method use for verify session window
        public bool IsSessionWindowVisible()
        {
            try
            {
                var window = GetInstructorSessionWindow();
                return window != null
                       && window.IsAvailable
                       && window.Properties.IsOffscreen.ValueOrDefault == false;
            }
            catch
            {
                return false;
            }
        }

        // This method use for Lung compilence
        public int GetLungCompliancePercent()
        {
            try
            {
                var window = GetInstructorSessionWindow();

                var valueElement = window.FindFirstDescendant(cf =>
                    cf.ByAutomationId("range1"));

                if (valueElement == null)
                    throw new Exception("Lung compliance value element not found (AutomationId=range1).");


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
        // This method use for select coughing
        public string GetSelectedVocalName()
        {
            var window = GetInstructorSessionWindow();

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

        // This method use for verify playbutton is enabled
        public bool IsPlayButtonEnabled()
        {
            try
            {
                var window = GetInstructorSessionWindow();

                var playButton =
                    window.FindFirstDescendant(cf =>
                        cf.ByControlType(ControlType.Button)
                          .And(cf.ByText("Play")))
                    ??
                    window.FindFirstDescendant(cf =>
                        cf.ByControlType(ControlType.Button)
                          .And(cf.ByAutomationId("PlayButton")));

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
        // This method use for close the window
        public void CloseSessionWindow()
        {
            var window = GetInstructorSessionWindow();
            window.Close();
        }
    }
}

