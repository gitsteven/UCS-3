/*
 * Program : Ultrapowa Clash Server
 * Description : A C# Writted 'Clash of Clans' Server Emulator !
 *
 * Authors:  Jean-Baptiste Martin <Ultrapowa at Ultrapowa.com>,
 *           And the Official Ultrapowa Developement Team
 *
 * Copyright (c) 2016  UltraPowa
 * All Rights Reserved.
 */

using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using UCS.Logic;
using UCS.PacketProcessing;

namespace UCS.Core.Network
{
    internal class PacketManager : IDisposable
    {
        public PacketManager()
        {
            m_vIncomingPackets = new ConcurrentQueue<Message>();
            m_vOutgoingPackets = new ConcurrentQueue<Message>();
        }

        private static readonly EventWaitHandle m_vIncomingWaitHandle = new AutoResetEvent(false);
        private static readonly EventWaitHandle m_vOutgoingWaitHandle = new AutoResetEvent(false);
        private static ConcurrentQueue<Message> m_vIncomingPackets;
        private static ConcurrentQueue<Message> m_vOutgoingPackets;
        private bool m_vIsRunning;

        private delegate void IncomingProcessingDelegate();

        private delegate void OutgoingProcessingDelegate();

        public static void ProcessIncomingPacket(Message p)
        {
            m_vIncomingPackets.Enqueue(p);
            m_vIncomingWaitHandle.Set();
        }

        public static void ProcessOutgoingPacket(Message p)
        {
            try
            {
                p.Encode();
                p.Process(p.Client.GetLevel());
                Level pl = p.Client.GetLevel();
                string player = " (0, NoNameYet)";
                if (pl != null)
                    player = " (" + pl.GetPlayerAvatar().GetId() + ", " + pl.GetPlayerAvatar().GetAvatarName() + ")";
                //Debugger.WriteLine("[UCS]    Processing " + p.GetType().Name + player);
                m_vOutgoingPackets.Enqueue(p);
                m_vOutgoingWaitHandle.Set();
            }
            catch (Exception ex)
            {
                //Debugger.WriteLine("[UCS]    Exception when processing " + p.GetType().Name, ex);
            }
        }

        public void Dispose()
        {
            m_vIncomingWaitHandle.Dispose();
            GC.SuppressFinalize(this);
            m_vOutgoingWaitHandle.Dispose();
        }

        public void Start()
        {
            IncomingProcessingDelegate incomingProcessing = IncomingProcessing;
            incomingProcessing.BeginInvoke(null, null);
            OutgoingProcessingDelegate outgoingProcessing = OutgoingProcessing;
            outgoingProcessing.BeginInvoke(null, null);
            m_vIsRunning = true;
            Console.WriteLine("[UCS]    Packet Manager started successfully");
        }

        private void IncomingProcessing()
        {
            while (m_vIsRunning)
            {
                m_vIncomingWaitHandle.WaitOne();
                Message p;
                while (m_vIncomingPackets.TryDequeue(out p))
                {
                    p.GetData();
                    p.Decrypt();
                    //Logger.WriteLine(p, "R");
                    MessageManager.ProcessPacket(p);
                }
            }
        }

        private void OutgoingProcessing()
        {
            while (m_vIsRunning)
            {
                m_vOutgoingWaitHandle.WaitOne();
                Message p;
                while (m_vOutgoingPackets.TryDequeue(out p))
                {
                    //Logger.WriteLine(p, "S");
                    try
                    {
                        if (p.Client.Socket != null)
                            p.Client.Socket.Send(p.GetRawData());
                        else
                            ResourcesManager.DropClient(p.Client.GetSocketHandle());
                    }
                    catch (Exception)
                    {
                        try
                        {
                            ResourcesManager.DropClient(p.Client.GetSocketHandle());
                            p.Client.Socket.Shutdown(SocketShutdown.Both);
                            p.Client.Socket.Close();
                        }
                        catch (Exception ex)
                        {
                            //Debugger.WriteLine("[UCS]    Exception thrown when dropping client : ", ex);
                        }
                    }
                }
            }
        }
    }
}