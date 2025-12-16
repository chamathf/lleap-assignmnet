using System;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;

public class FlaUIAppManager : IDisposable
{
    private readonly UIA3Automation _automation;
    private Application _simHomeApp;

    public FlaUIAppManager()
    {
        _automation = new UIA3Automation();
    }

    public Window LaunchSimulationHome(string exePath)
    {
        _simHomeApp = Application.Launch(exePath);
        return _simHomeApp.GetMainWindow(_automation, TimeSpan.FromSeconds(10));
    }

    public UIA3Automation Automation => _automation;

    public void Dispose()
    {
        _simHomeApp?.Close();
        _automation?.Dispose();
    }
    public string TakeScreenshot(AutomationElement window, string scenarioName)
    {
        var screenshotsDir = Path.Combine(
            TestContext.CurrentContext.WorkDirectory,
            "Screenshots",
            scenarioName
        );

        Directory.CreateDirectory(screenshotsDir);

        var fileName = $"Failure_{DateTime.Now:yyyyMMdd_HHmmss}.png";
        var filePath = Path.Combine(screenshotsDir, fileName);

        window.CaptureToFile(filePath);

        return filePath;
    
}
}
