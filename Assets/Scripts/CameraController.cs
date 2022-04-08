using Cinemachine;
using UnityEngine;
using UnityEngine.Assertions;

public class CameraController : MonoBehaviour
{
    private CinemachineVirtualCamera m_MainCamera;

    public CinemachineVirtualCamera GetCurrentCamera()
    {
        return m_MainCamera;
    }

    void Start()
    {
        AttachCamera();
    }

    private void AttachCamera()
    {
        m_MainCamera = FindObjectOfType<CinemachineVirtualCamera>();
        Assert.IsNotNull(m_MainCamera, "CameraController.AttachCamera: Couldn't find gameplay virtual camera");

        if (m_MainCamera)
        {
            // camera body / aim
            m_MainCamera.Follow = transform;
            m_MainCamera.LookAt = transform;
        }
    }
}
