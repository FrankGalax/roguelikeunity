using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    public GameObject Player;
    public int InitialFloor = 0;

    private void Awake()
    {
        if (GameManager.Instance.CurrentFloor == 0)
        {
            GameManager.Instance.CurrentFloor = InitialFloor;
        }
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            player = Instantiate(Player);
            DontDestroyOnLoad(player);
        }
    }
}
