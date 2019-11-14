using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public enum Level
{
    Init = 0,
    Splash = 1,
    Menu = 2,
    MainScene = 3
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
