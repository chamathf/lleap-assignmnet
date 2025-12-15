Feature: Run session with Virtual SimMan3G without license

  Background:
    Given LLEAP is installed and configured

  Scenario: Run manual session for SimMan3G Plus without license
    Given I start Laerdal Simulation Home
    And I open the Instructor Application
    And I choose to add the license later
    And I select the Local Computer as simulator host
    And I select the "SimMan 3G PLUS" simulator
    And I go to Manual Mode
    And I select the "Healthy patient" theme
    When I start the session
    Then the session window is maximized
    And the eyes control is set to "Closed"
    And the lung compliance is set to 67 percent
    And the heart rate is set to 100
    And the "Coughing" voice is played once
