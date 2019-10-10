using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public enum Level
{
    Init = 0,
    TestScene = 1
}

public class GameManager : Singleton<GameManager>
{
	#region Variables
	/// <summary>
	/// The Rewired structure representing a single player
	/// </summary>
	private Rewired.Player rewiredPlayer;

	[MinValue(0)]
    [SerializeField, Tooltip("The Rewired Player ID to use for getting input")]
    public int rewiredPlayerId = 0;
	#endregion Variables

	#region Properties
	/// <summary>
	/// The Rewired structure representing a single player
	/// </summary>
	public Rewired.Player RewiredPlayer
	{
		get { return rewiredPlayer; }
	}
	#endregion Properties

	protected override void CustomAwake()
    {
        rewiredPlayer = Rewired.ReInput.players.GetPlayer(rewiredPlayerId);
    }

}
