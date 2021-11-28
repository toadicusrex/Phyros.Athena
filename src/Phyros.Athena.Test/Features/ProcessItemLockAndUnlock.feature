Feature: ProcessItemLockAndUnlock

@tag ProcessItemLockAndUnlock
Scenario Outline: Simple process with lock and unlock
	Given process '<process>'
	And start action '<startAction>' is triggered
	And an notification subscriber has been configured
	And a process item notification has been received
	And a lock has been requested with principalId '<userId>'
	And a process item notification has been received
	When a lock has been requested with principalId '<otherUserId>'
	Then an exception of type '<exception>' should be thrown

	Examples:
		| process                  | startAction  | userId   | otherUserId | exception                  |
		| Process With Single Step | First Action | LockUser | WrongUser   | ProcessItemLockedException |