using UnityEngine;

public class UIStateDisplay : MonoBehaviour
{
    public UINetwork m_UINetwork;

    public void DisplayNetwork()
    {
        m_UINetwork.gameObject.SetActive(true);
        m_UINetwork.Initialize();
    }
}
