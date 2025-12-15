namespace LLEAP.UITestAutomation.Infrastructure
{
    public class SoftAssert
    {
        private readonly List<string> _failures = new();
        private readonly Func<string>? _takeScreenshot;

        public SoftAssert()
        {
        }

       
        public SoftAssert(Func<string> takeScreenshot)
        {
            _takeScreenshot = takeScreenshot;
        }

        public void That(bool condition, string message)
        {
            if (!condition)
            {
                var screenshotPath = _takeScreenshot?.Invoke();
                _failures.Add(
                    string.IsNullOrWhiteSpace(screenshotPath)
                        ? message
                        : $"{message}\nScreenshot: {screenshotPath}"
                );
            }
        }

        public void That<T>(T actual, T expected, string message)
        {
            if (!EqualityComparer<T>.Default.Equals(actual, expected))
            {
                var screenshotPath = _takeScreenshot?.Invoke();
                var failure = $"{message} | Expected: {expected}, Actual: {actual}";

                _failures.Add(
                    string.IsNullOrWhiteSpace(screenshotPath)
                        ? failure
                        : $"{failure}\nScreenshot: {screenshotPath}"
                );
            }
        }

        public void AssertAll()
        {
            if (_failures.Any())
            {
                Assert.Fail(
                    "Soft assertion failures:\n\n" +
                    string.Join("\n\n", _failures.Select((f, i) => $"{i + 1}. {f}"))
                );
            }
        }
    }
}
