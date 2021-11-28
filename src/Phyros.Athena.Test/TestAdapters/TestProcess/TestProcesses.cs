using Phyros.Athena.Model.Workflow.Processes;
using System.Collections.Generic;

namespace Phyros.Athena.Test.TestAdapters.TestProcess
{
	public static class TestProcesses
	{
		public static WorkflowProcess SimplestProcess = new WorkflowProcess()
		{
			Name = TestProcessIds.SimplestProcess,
			StartActions = new Dictionary<string, WorkflowProcessAction>
			{
				{ "Start and Finish", new WorkflowProcessAction()
					{
						// this means that the destination is an end point
						ArrivalStep = null,
						DisplayName = "Started and finished, one action.",
					}
				}
			}
		};

		public static WorkflowProcess ProcessWithSingleStep = new WorkflowProcess()
		{
			Name = TestProcessIds.ProcessWithSingleStep,
			StartActions = new Dictionary<string, WorkflowProcessAction>
			{
				{ "First Action", new WorkflowProcessAction()
					{
						// this means that the destination is an end point
						ArrivalStep = "First Step",
						DisplayName = "Started and moved to first step.",
					}
				}
			},
			Steps = new Dictionary<string, WorkflowProcessStep>()
			{
				{
					"First Step", new WorkflowProcessStep()
					{
						Actions = new Dictionary<string, WorkflowProcessAction>()
						{
							{
								"Continue", new WorkflowProcessAction()
								{
									ArrivalStep = null,
									DisplayName = "Continued",
								}
							}
						},
						Name = "First Step"
					}
				}
			}
		};

	}
}
