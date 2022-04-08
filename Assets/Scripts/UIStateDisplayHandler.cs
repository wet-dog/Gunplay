using UnityEngine;
using Unity.Netcode;
using UnityEngine.Assertions;

public class UIStateDisplayHandler : NetworkBehaviour
{
    private Transform m_CanvasTransform;

    public UIStateDisplay m_UIStatePrefab;

    // Spawned in world (only one instance of this)
    private UIStateDisplay m_UIState;

    private void Start()
    {
        if (m_CanvasTransform != null) return;

        var canvasGameObject = GameObject.FindWithTag("GameCanvas");
        Assert.IsNotNull(canvasGameObject);
        m_CanvasTransform = canvasGameObject.transform;

        DisplayUINetwork();
    }

    public void DisplayUINetwork()
    {
        if (m_UIState == null)
        {
            SpawnUIState();
        }

        if (m_UIState != null) {
            m_UIState.DisplayNetwork();
        }
    }

    void SpawnUIState()
    {
        if (IsOwner)
        {
            m_UIState = Instantiate(m_UIStatePrefab, m_CanvasTransform);
            // Make in world UI state draw under other UI elements
            m_UIState.transform.SetAsFirstSibling();
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (m_UIState != null)
        {
            Destroy(m_UIState.gameObject);
        }
    }
}
