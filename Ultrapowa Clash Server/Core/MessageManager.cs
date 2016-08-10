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
using System.Threading;
using UCS.PacketProcessing;

namespace UCS.Core
{
    internal class MessageManager
    {
        #region Public Constructors

        /// <summary>
        ///     The loader of the MessageManager class.
        /// </summary>
        public MessageManager()
        {
            m_vPackets = new ConcurrentQueue<Message>();
            m_vIsRunning = false;
        }

        #endregion Public Constructors

        #region Private Methods

        /// <summary>
        ///     This function process packets.
        /// </summary>
        void PacketProcessing()
        {
            while (m_vIsRunning)
            {
                m_vWaitHandle.WaitOne();

                Message p;
                while (m_vPackets.TryDequeue(out p))
                {
                    var pl = p.Client.GetLevel();
                    var player = "";
                    if (pl != null)
                        player += " (" + pl.GetPlayerAvatar().GetId() + ", " + pl.GetPlayerAvatar().GetAvatarName() +
                                  ")";
                    try
                    {
                        //Debugger.WriteLine("[UCS]    Processing " + p.GetType().Name + player);
                        p.Decode();
                        p.Process(pl);
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        //Debugger.WriteLine(
                        // "[UCS]    An exception occured during processing of message " + p.GetType().Name + player,
                        // ex);
                        Console.WriteLine("Error processing message" + p.GetType().Name + player, ex);
                     Console.ResetColor();
                    }
                }
            }
        }

        #endregion Private Methods

        #region Private Delegates

        delegate void PacketProcessingDelegate();

        #endregion Private Delegates

        #region Private Fields

        static readonly EventWaitHandle m_vWaitHandle = new AutoResetEvent(false);
        static ConcurrentQueue<Message> m_vPackets;
        bool m_vIsRunning;

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        ///     This function handle the packet by enqueue him.
        /// </summary>
        /// <param name="p">The message/packet.</param>
        public static void ProcessPacket(Message p)
        {
            m_vPackets.Enqueue(p);
            m_vWaitHandle.Set();
        }

        /// <summary>
        ///     This function start the MessageManager.
        /// </summary>
        public void Start()
        {
            PacketProcessingDelegate packetProcessing = PacketProcessing;
            packetProcessing.BeginInvoke(null, null);
            m_vIsRunning = true;
            Console.WriteLine("[UCS]    Message manager has been successfully started !");
        }

        #endregion Public Methods
    }
}