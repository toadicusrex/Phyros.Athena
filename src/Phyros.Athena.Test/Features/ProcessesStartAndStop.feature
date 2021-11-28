Feature: ProcessesStartAndStop

Scenario Outline: Simple process with start action without properties
	Given process '<process>'
	When start action '<startAction>' is triggered
	And an notification subscriber has been configured
	And a process item notification has been received
	Then the processItemId should not be non-null and non-empty
	And the process item should be recoverable from the ProcessItemStore and should be at step '<destinationStep>'

	Examples:
		| process          | startAction      | destinationStep |
		| Simplest Process | Start and Finish |                 |

Scenario Outline: Simple process with at least one step, taking multiple actions
	Given process '<process>'
	When start action '<startAction>' is triggered
	And an notification subscriber has been configured
	And a process item notification has been received
	And the process item should be recoverable from the ProcessItemStore and should be at step '<destinationStep>'
	And followup action '<followupAction>' is triggered
	And a process item notification has been received
	Then the processItemId should not be non-null and non-empty
	And the process item should be recoverable from the ProcessItemStore and should be at step '<finalStep>'

	Examples:
		| process                  | startAction  | destinationStep | followupAction | finalStep |
		| Process With Single Step | First Action | First Step      | Continue       |           |