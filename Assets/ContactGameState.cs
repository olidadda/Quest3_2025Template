using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactGameState : MonoBehaviour
{
    public void RecordIDNum(int iDNum)
    {
        GameStateManager.Instance.SetIDNumberAsAlreadyChosen(iDNum);
    }
}
