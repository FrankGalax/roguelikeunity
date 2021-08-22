using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    public GameObject Player;

    private void Awake()
    {
        GameManager.Instance.Resolve();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            player = Instantiate(Player);
            DontDestroyOnLoad(player);
        }
    }
}
