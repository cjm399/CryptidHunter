using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	#region VARIABLES
	private static T instance;
	#endregion VARIABLES

	#region GETTERS_SETTERS
	public static T Instance
	{
		get { return instance; }
	}
	#endregion GETTERS_SETTERS

	#region MONOBEHAVIOUR
	private void Awake()
	{
		// If the instance has not yet been declared, then set it
		if(instance == null)
		{
			instance = GetComponent<T>();

			CustomAwake();
		}
		// There is already a Singleton, so destroy this one
		else
		{
			Destroy(this.gameObject);
		}
	}
	#endregion MONOBEHAVIOR

	#region PROTECTED
	/// <summary>
	/// Function called after the Singleton instance has been resolved. Override in child classes instead of using Awake(), which would block the Singleton instance from being set.
	/// </summary>
	protected virtual void CustomAwake() { }
	#endregion PROTECTED
}