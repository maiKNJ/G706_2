using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class Client : MonoBehaviour
{
    public static Client instance;
    public static int dataBufferSize = 4096;
    //
    public string ip = "127.0.0.1";
    public int port = 26950;
    public int myId = 0;
    public TCP tcp;
    public UDP udp;

    private bool isConnected = false;

    private delegate void PacketHandler(Packet _packet);
    private static Dictionary<int, PacketHandler> packetHandlers;
    //

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Start()
    {
        tcp = new TCP();
        udp = new UDP();
        
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }

    public void ConnectToServer()
    {
        //instance.ip = UIManager.instance.usernameField.text;
        InitializeClientData();

        isConnected = true;
        tcp.Connect();
        //udp.Connect(instance.port);
        //Why not udp.Connect()????
    }

    public class TCP
    {
        public TcpClient socket;

        private NetworkStream stream;
        private Packet receivedData;
        private byte[] receiveBuffer;

        public void Connect()
        {
            //instance.ip = UIManager.instance.usernameField.text;
            socket = new TcpClient
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };

            Debug.Log("socket connect before tcp" + instance.ip);
            receiveBuffer = new byte[dataBufferSize];
            socket.BeginConnect(instance.ip, instance.port, ConnectCallback, socket);
            Debug.Log("socket connect" + instance.ip);
        }

        private void ConnectCallback(IAsyncResult _result)
        {
            socket.EndConnect(_result);

            if (!socket.Connected)
            {
                return;
            }

            stream = socket.GetStream();

            receivedData = new Packet();

            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }

        public void SendData(Packet _packet)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error sending data to server via TCP: {_ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                int _byteLength = stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    instance.Disconnect();
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(receiveBuffer, _data, _byteLength);

                receivedData.Reset(HandleData(_data));
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch
            {
                Disconnect();
            }
        }

        private bool HandleData(byte[] _data)
        {
            int _packetLength = 0;
            receivedData.SetBytes(_data);

            if (receivedData.UnreadLength() >= 4)
            {
                _packetLength = receivedData.ReadInt();
                if (_packetLength <= 0)
                {
                    return true;
                }

            }
            while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
            {
                byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        packetHandlers[_packetId](_packet);
                    }
                });
                _packetLength = 0;
                if (receivedData.UnreadLength() >= 4)
                {
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }

                }
            }
            if (_packetLength <= 1)
            {
                return true;
            }
            return false;

        }

        private void Disconnect()
        {
            instance.Disconnect();
            stream = null;
            receivedData = null;
            receiveBuffer = null;
            socket = null;
        }

    }

    public class UDP
    {
        public UdpClient socket;
        public IPEndPoint endPoint;
        public IPAddress address;

        public UDP()
        {
            //address = IPAddress.Parse(Client.instance.ip);
            //endPoint = new IPEndPoint(address, instance.port);
        }

        public void Connect(int _localPort)
        {
            address = IPAddress.Parse(Client.instance.ip);
            endPoint = new IPEndPoint(address, instance.port);
            
            Debug.Log("ip UDP: " + Client.instance.ip);
            Debug.Log("ip UDP parse" + address);

            socket = new UdpClient(_localPort);
            Debug.Log("Endpoint: " + endPoint);

            socket.Connect(endPoint);
            socket.BeginReceive(ReceiveCallback, null);
            Debug.Log("Endpoint after: " + endPoint);

            using (Packet _packet = new Packet())
            {
                SendData(_packet);
            }
        }

        public void SendData(Packet _packet)
        {
            try
            {
                _packet.InsertInt(instance.myId);
                if (socket != null)
                {
                    socket.BeginSend(_packet.ToArray(), _packet.Length(), null, null);
                }
            }
            catch (Exception _ex)
            {
                Debug.LogError($"Error sending data to server via UDP: {_ex}");

            }
        }

        private void ReceiveCallback(IAsyncResult _result)
        {
            Debug.Log("inside receiveCallback");
            try
            {
                byte[] _data = socket.EndReceive(_result, ref endPoint);
                Debug.Log("data length: " + _data.Length);
                socket.BeginReceive(ReceiveCallback, null);

                if (_data.Length < 4)
                {
                    Debug.Log("if data length: " + _data.Length);
                    instance.Disconnect();
                    return;
                }
            }
            catch
            {
                Debug.Log("something went wrong");
                Disconnect();
            }
        }

        private void HandleData(byte[] _data)
        {
            using (Packet _packet = new Packet(_data))
            {
                int _packetLength = _packet.ReadInt();
                _data = _packet.ReadBytes(_packetLength);

            }
            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet _packet = new Packet(_data))
                {
                    int _packetId = _packet.ReadInt();
                    packetHandlers[_packetId](_packet);
                }
            });
        }

        private void Disconnect()
        {
            instance.Disconnect();
            endPoint = null;
            socket = null;
        }
    }
    private void InitializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            {(int)ServerPackets.welcome, ClientHandler.Welcome },
            //{(int)ServerPackets.spawnPlayer, ClientHandler.SpawnPlayer },
            {(int)ServerPackets.playerPosition, ClientHandler.PlayerPosition },
            {(int)ServerPackets.playerRoation, ClientHandler.PlayerRotation },
        };
        Debug.Log("Initialized packets");
    }

    private void Disconnect()
    {
        if (isConnected)
        {
            isConnected = false;
            tcp.socket.Close();
            udp.socket.Close();

            Debug.Log("disconnected from server");
        }
    }
}
