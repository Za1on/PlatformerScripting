using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToolBox : MonoBehaviour
{
    public GameObject playerPrefab_m;
    public Player player_m;
    #region Singleton
    private static ToolBox instance;
    public static ToolBox Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<ToolBox>();
                if (instance == null)
                {
                    var singleton = new GameObject();
                    instance = singleton.AddComponent<ToolBox>();
                    singleton.name = "(Singleton) ToolBox";
                }
            }
            return instance;
        }
    }
    private void Awake()
    {
        instance = this;
    }

    #endregion
}


