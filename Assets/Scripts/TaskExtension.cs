using System.Collections;
using System.Threading.Tasks;

public static class TaskExtension
{
	/// <summary>
	/// Firebase Task might not play well with Unity's Coroutine workflow. You can now yield on the task with this.
	/// </summary>
	public static IEnumerator YieldWait(Task task)
	{
		while (!task.IsCompleted)
		{
			yield return null;
		}
		if (task.IsFaulted)
		{
			throw task.Exception;
		}
	}
}
