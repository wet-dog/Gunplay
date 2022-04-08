using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Gunplay.KeyboardState;

public class Network : NetworkBehaviour
{
    private const int NETWORK_INPUT_HISTORY_SIZE = 60;
    private const int NETWORK_SEND_HISTORY_SIZE = 5;

    [Flags]
    public enum InputCode
    {
        Up = 1,
        Down = 2,
        Left = 4,
        Right = 8,
        Attack = 16
    }
    public struct NetworkInputData : INetworkSerializable
    {
        public int LocalTickDelta;
        public int Tick;
        public InputCode[] History;

        public NetworkInputData(int delta, int tick, InputCode[] history)
        {
            LocalTickDelta = delta;
            Tick = tick;
            History = history;
        }

        void INetworkSerializable.NetworkSerialize<T>(BufferSerializer<T> serializer)
        {
            serializer.SerializeValue(ref LocalTickDelta);
            serializer.SerializeValue(ref Tick);
            serializer.SerializeValue(ref History);
        }
    }

    public struct NetworkSyncData : INetworkSerializable
    {
        public int Tick;
        public int SyncData;

        public NetworkSyncData(int tick, int syncData)
        {
            Tick = tick;
            SyncData = syncData;
        }

        void INetworkSerializable.NetworkSerialize<T>(BufferSerializer<T> serializer)
        {
            serializer.SerializeValue(ref Tick);
            serializer.SerializeValue(ref SyncData);
        }
    }

    public int ConfirmedTick = 0;

    public InputCode[] InputHistory = new InputCode[NETWORK_INPUT_HISTORY_SIZE];
    public InputCode[] RemoteInputHistory = new InputCode[NETWORK_INPUT_HISTORY_SIZE];

    public int LocalSyncData = 0;    // Sync data for now is an int representing player's x coord  
    public int RemoteSyncData = 0;   // Can change sync data to something more complex

    public int LastSyncedTick = -1;

    public int LocalTickDelta = 0;
    public int RemoteTickDelta = 0;

    public float TickOffset = 0.0f;
    public bool TickSyncing = false;

    public int DesyncCheckRate = 20;

    public int LocalSyncDataTick = -1;
    public int RemoteSyncDataTick = -1;
    
    public bool IsDesynced = false;

    public int SyncData = 0;

    //public SyncDataHistoryLocal;
    //public SyncDataHistoryRemote;
    //public SyncDataTicks;

    public KeyboardState GetRemoteInputState(int tick)
    {
        if (tick > ConfirmedTick)
        {
            tick = ConfirmedTick;
        }

        int historyIndex = (NETWORK_INPUT_HISTORY_SIZE + tick) % NETWORK_INPUT_HISTORY_SIZE;
        return DecodeInput(RemoteInputHistory[historyIndex]);
    }

    public KeyboardState GetLocalInputState(int tick)
    {
        int historyIndex = (NETWORK_INPUT_HISTORY_SIZE + tick) % NETWORK_INPUT_HISTORY_SIZE;
        return DecodeInput(InputHistory[historyIndex]);
    }

    public InputCode GetLocalInputEncoded(int tick)
    {
        int historyIndex = (NETWORK_INPUT_HISTORY_SIZE + tick) % NETWORK_INPUT_HISTORY_SIZE;
        return InputHistory[historyIndex];
    }

    public void SetLocalSyncData(int tick, int syncData)
    {
        if (!IsDesynced)
        {
            LocalSyncData = syncData;
            LocalSyncDataTick = tick;
        }
    }

    // Can the returns be refactored to just one return IsDesynced?
    public bool CheckDesync()
    {
        if (LocalSyncDataTick < 0) return false;

        if (IsDesynced || LocalSyncDataTick == RemoteSyncDataTick)
        {
            if (LocalSyncData != RemoteSyncData)
            {
                IsDesynced = true;
                return true;
            }
        }

        return false;
    }

    // REPLACE WITH RPC
    //public void SendInputData(int tick)
    //{

    //}

    public void SetLocalInput(KeyboardState inputState, int tick)
    {
        InputCode encodedInput = EncodeInput(inputState);
        int historyIndex = (NETWORK_INPUT_HISTORY_SIZE + tick) % NETWORK_INPUT_HISTORY_SIZE;
        InputHistory[historyIndex] = encodedInput;
    }

    public void SetRemoteEncodedInput(InputCode encodedInput, int tick)
    {
        int historyIndex = (NETWORK_INPUT_HISTORY_SIZE + tick) % NETWORK_INPUT_HISTORY_SIZE;
        RemoteInputHistory[historyIndex] = encodedInput;
    }

    // Send packet, receive packet, process packet functions

    // todo function Receive data

    // todo function Send input data

    // todo function Send sync data

    InputCode EncodeInput(KeyboardState state)
    {
        InputCode data = 0;

        if (state.up) data |= InputCode.Up;
        if (state.down) data |= InputCode.Down;
        if (state.left) data |= InputCode.Left;
        if (state.right) data |= InputCode.Right;
        if (state.attack) data |= InputCode.Attack;

        return data;
    }

    KeyboardState DecodeInput(InputCode data)
    {
        KeyboardState state = new KeyboardState
        {
            up = data.HasFlag(InputCode.Up),
            down = data.HasFlag(InputCode.Down),
            left = data.HasFlag(InputCode.Left),
            right = data.HasFlag(InputCode.Right),
            attack = data.HasFlag(InputCode.Attack)
        };

        return state;
    }

    public NetworkInputData MakeNetworkInputData(int tick)
    {
        int historyIndexStart = tick - NETWORK_SEND_HISTORY_SIZE + 1;
        InputCode[] history = new InputCode[NETWORK_SEND_HISTORY_SIZE];

        int historyIndex;
        for (int i = 0; i < history.Length; i++)
        {
            historyIndex = (NETWORK_INPUT_HISTORY_SIZE + historyIndexStart + i) % NETWORK_INPUT_HISTORY_SIZE;
            history[i] = InputHistory[historyIndex];
        }

        NetworkInputData data = new NetworkInputData(tick, LocalTickDelta, history);
        return data;
    }

    public NetworkSyncData MakeNetworkSyncData(int tick)
    {
        NetworkSyncData data = new NetworkSyncData(tick, LocalSyncData);
        return data;
    }
}
