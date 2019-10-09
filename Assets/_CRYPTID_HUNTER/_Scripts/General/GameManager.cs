using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public class GameManager : MonoBehaviour
{
    public Rewired.Player rewiredPlayer;

    [MinValue(0)]
    [SerializeField, Tooltip("The Rewired Player ID to use for getting input")]
    public int rewiredPlayerId = 0;

    public static GameManager Instance;

    private void Awake()
    {
        rewiredPlayer = Rewired.ReInput.players.GetPlayer(rewiredPlayerId);

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

}
