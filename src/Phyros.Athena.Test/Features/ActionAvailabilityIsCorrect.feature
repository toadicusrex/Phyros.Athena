Feature: ActionAvailabilityIsCorrect
	Simple calculator for adding two numbers
Feature: Action Availability Is Correct

Scenario Outline: Simple process where we attempt to access an action that doesn't exist
	Given process '<process>'
	When start action '<startAction>' is triggered
	And an notification subscriber has been configured
	And a process item notification has been received
	And the process item should be recoverable from the ProcessItemStore and should be at step '<destinationStep>'
	And followup action '<missingAction>' is triggered
	Then an exception of type '<exception>' should be thrown

	Examples:
		| process                  | startAction  | destinationStep | missingAction | finalStep | exception               |
		| Process With Single Step | First Action | First Step      | Vomit         |           | ActionNotFoundException |