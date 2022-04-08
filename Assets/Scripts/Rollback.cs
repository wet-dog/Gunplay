using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Rollback : NetworkBehaviour
{
    private const int MAX_ROLLBACK_FRAMES = 10;
    private const int FRAME_ADVANTAGE_LIMIT = 2;
    private const int INITIAL_FRAME = 1;

    private int m_LocalFrame = INITIAL_FRAME;
    private int m_RemoteFrame = INITIAL_FRAME;
    private int m_SyncFrame = INITIAL_FRAME;
    private int m_RemoteFrameAdvantage = 0;

    public int m_Tick;

    void Start()
    {
        NetworkManager.NetworkTickSystem.Tick += Tick;
    }

    public override void OnDestroy()
    {
        if (NetworkManager.NetworkTickSystem != null)
        {
            NetworkManager.NetworkTickSystem.Tick -= Tick;
        }
    }

    void StoreGameState() {}

    void RestoreGameState() {}

    void UpdateInput() {}

    void UpdateGame() {}

    bool RollbackCondition() 
    {
        return m_LocalFrame > m_SyncFrame && m_RemoteFrame > m_SyncFrame;    // No need to rollback if we don't have a frame after the previous sync frame to synchronize to
    }

    bool TimeSynced()
    {
        int localFrameAdvantage = m_LocalFrame - m_RemoteFrame;    // How far the client is ahead of the last reported frame by the remote client
        int frameAdvantageDifference = localFrameAdvantage - m_RemoteFrameAdvantage;    // How different is the frame advantage reported by the remote client and this one
        return localFrameAdvantage < MAX_ROLLBACK_FRAMES && frameAdvantageDifference <= FRAME_ADVANTAGE_LIMIT;    // Only allow the local client to get so far ahead of remote
    }

    // Receive information from remote client
    void UpdateNetwork()
    {
        //m_RemoteFrame = ;    // Latest frame received from the remote client
        //m_RemoteFrameAdvantage = ;    // (clientInfo.LocalFrame - clientInfo.RemoteFrame) sent from the remote client
    }

    void DetermineSyncFrame()
    {
        int finalFrame = m_RemoteFrame;    // We will only check inputs until the remote_frame, since we don't have inputs after
        if (m_RemoteFrame > m_LocalFrame)
        {
            finalFrame = m_LocalFrame;
        }

        // Select frames from (sync_frame + 1) through final_frame and find the first frame where predicted and remote inputs don't match
        bool found = false;
        int foundFrame = 0;

        if (found)
        {
            m_SyncFrame = foundFrame - 1;    // The found frame is the first frame where inputs don't match, so assume the previous frame is synchronized
        }
        else
        {
            m_SyncFrame = finalFrame;    // All remote inputs matched the predicted inputs since the last synchronized frame
        }
    }

    void ExecuteRollbacks()
    {
        RestoreGameState();
        //  select inputs from(sync_frame +1) through local_frame
        UpdateInput();
        UpdateGame();
        StoreGameState();
    }

    void UpdateSynchronization()
    {
        DetermineSyncFrame();
        
        if (RollbackCondition())
        {
            ExecuteRollbacks();
        }
    }

    void NormalUpdate()
    {
        m_LocalFrame++;

        // Get local player input

        // Send the input and the local_frame to the remote client

        UpdateInput();
        UpdateGame();
        StoreGameState();
    }

    private void Tick()
    {
        m_Tick++;

        UpdateNetwork();
        UpdateSynchronization();

        if (TimeSynced())
        {
            NormalUpdate();
        }
    }
}
