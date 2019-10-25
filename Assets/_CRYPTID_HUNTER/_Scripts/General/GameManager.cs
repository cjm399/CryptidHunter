using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public enum Level
{
    Init = 0,
    Menu = 1,
    MainScene = 2
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
	
	[ReadOnly]
	[SerializeField, Tooltip("Whether the game is currently ongoing (will be true once an endgame state has been reached")]
	bool hasReachedEnd = false;

    public Menu menu;
	#endregion Variables

	#region Properties
	/// <summary>
	/// The Rewired structure representing a single player
	/// </summary>
	public Rewired.Player RewiredPlayer
	{
		get { return rewiredPlayer; }
	}

	/// <summary>
	/// Whether the game is currently running (will be true once an endgame state has been reached)
	/// </summary>
	public bool HasReachedEnd
	{
		get { return hasReachedEnd; }
		set
		{
			if(hasReachedEnd != value)
			{
				hasReachedEnd = value;

				if(hasReachedEnd)
				{
					OnGameEnd?.Invoke();
				}
			}
		}
	}
	#endregion Properties

	#region Events
	public delegate void GameEndEventHandler();
	public event GameEndEventHandler OnGameEnd;
	#endregion Event

	#region MONOBEHAVIOUR

	private void Start()
    {
        menu = FindObjectOfType<Menu>();
    }

    #endregion

    protected override void CustomAwake()
    {
        rewiredPlayer = Rewired.ReInput.players.GetPlayer(rewiredPlayerId);
    }

}
