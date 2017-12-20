using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugUITest : DualBehaviour 
{
    #region Public Members

    //public DemoDebug m_demoDebug;

    #endregion

    #region System

    void Awake()
    {
        //m_demoDebug = GameObject.Find("DemoDebug").GetComponent<DemoDebug>();
    }

    void Start () 
    {
        //m_messagesId.Add("Value", m_demoDebug.AddMessage("ceci est mon message " + Time.time));
    }
	
	void Update () 
    {
        //m_demoDebug.UpdateMessage(m_messagesId["Value"], "ceci est mon message " + Time.time);
	}

    #endregion

    #region Private and Protected Members

    //private Dictionary<string, int> m_messagesId = new Dictionary<string, int>();

    #endregion

}
