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
    
    // These are the same as m_LocalFrame and m_RemoteFrame - I need to refactor
    public int m_Tick;
    public int m_RemoteTick;

    public int m_TickDelta;    // Test tick delta not actual implementation

    public float posX;
    public float posY;

    public float moveSpeed = 5f;

    public InputSystem m_InputSystem;
    public PlayerMovement m_Movement;

    void Awake()
    {
        PlayerStats.OnInstantiated += OnPlayerSpawned;
    }

    private void OnPlayerSpawned()
    {
        Debug.Log("Player spawned");
    }

    [ServerRpc]
    void SetTickServerRpc(int tick)
    {
        SetTickClientRpc(tick);
    }

    [ClientRpc]
    void SetTickClientRpc(int tick)
    {
        m_Tick = tick;
    }

    int GetRemoteTick()
    {
        var playerGameObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (var gameObject in playerGameObjects)
        {
            bool isRemoteGameObject = !gameObject.GetComponent<NetworkObject>().IsOwner;
            if (isRemoteGameObject)
            {
                return gameObject.GetComponent<Rollback>().m_Tick;
            }
        }

        return 0;
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

    void FixedUpdate()
    {
        if (IsOwner)
        {
            m_Tick++;    // Won't update tick here in actual implementation - look at NormalUpdate()

            SetTickServerRpc(m_Tick);
            m_RemoteTick = GetRemoteTick();
            m_TickDelta = m_Tick - m_RemoteTick;

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            MovePlayerServerRpc(horizontal, vertical);

            UpdateNetwork();
            UpdateSynchronization();

            if (TimeSynced())
            {
                NormalUpdate();
            }
        }

        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
        rb.MovePosition(rb.position + moveSpeed * Time.fixedDeltaTime * new Vector2(posX, posY));
    }

    [ServerRpc]
    void MovePlayerServerRpc(float horizontal, float vertical)
    {
        MovePlayerClientRpc(horizontal, vertical);
    }

    [ClientRpc]
    void MovePlayerClientRpc(float horizontal, float vertical)
    {
        posX = horizontal;
        posY = vertical;
        //gameObject.GetComponent<Rigidbody2D>().MovePosition(new Vector2(horizontal, vertical));
        //m_Movement.MovePlayer(m_InputSystem);
    }

}
