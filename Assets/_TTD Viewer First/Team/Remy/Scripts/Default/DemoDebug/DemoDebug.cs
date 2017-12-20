using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoDebug : DualBehaviour 
{
    #region Public Members

    public List<string> m_messageToShow = new List<string>();
    #endregion

    #region Public void

    public int AddMessage(string _msg)
    {
        m_messageToShow.Add(_msg);

        return m_messageToShow.Count;
    }

    public void UpdateMessage(int _id, string _msg)
    {
        m_messageToShow[_id - 1] = _msg;
    }

    #endregion

    #region System

    void OnGUI()
    {
        foreach(string msg in m_messageToShow)
        {
            GUILayout.Button(msg);
        }
    }

    #endregion
}
