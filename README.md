# lleap-assignmnet

## Overview

This project contains automated UI tests for the **LLEAP desktop application**, implemented using **C#**, **SpecFlow (BDD)**, and **FlaUI (Windows UI Automation)**.

The purpose of this project is to demonstrate:
- Maintainable desktop UI automation
- Behavior Driven Development (BDD)
- Page Object Model (POM)
- Step-level verification
- Automatic screenshot capture on failures
- Clear structure suitable for developers and non-technical stakeholders

---

## Technology Stack

| Component | Technology |
|---------|------------|
| Language | C# |
| Framework | .NET 6 / .NET 8 |
| BDD | SpecFlow |
| Test Runner | NUnit |
| UI Automation | FlaUI (UIA3) |
| IDE | Visual Studio 2022 |
| OS | Windows 10 / Windows 11 |

All tools and libraries used are **free and open-source**.

---

## Project Structure

```text
LLEAP.Automation/
│
├── LLEAP.Automation.sln
│
└── src/
    └── LLEAP.Tests/
        │
        ├── Features/              # Gherkin feature files
        │     ├── Test1_RunSession.feature
        │
        │
        ├── Steps/                 # SpecFlow step definitions
        │     ├── Test1Steps.cs
        │     ├── Test2Steps.cs
        │     └── Hooks.cs
        │
        ├── Pages/                 # Page Object Model
        │     ├── SimulationHomePage.cs
        │     ├── InstructorAppPage.cs
        │     ├── PatientMonitorPage.cs
        │     └── HelpPage.cs
        │
        ├── Infrastructure/        # Core automation utilities
        │     ├── FlaUIAppManager.cs
        │     ├── ScreenshotHelper.cs
        │     ├── AppConfig.cs
        │     └── ElementExtensions.cs
        │
        ├── appsettings.json       # Configuration
        └── README.md
