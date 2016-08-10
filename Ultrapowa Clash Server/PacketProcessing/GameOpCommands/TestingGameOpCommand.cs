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
using UCS.Core;
using UCS.Core.Network;
using UCS.Logic;
using UCS.Logic.StreamEntry;
using UCS.PacketProcessing.Messages.Server;

namespace UCS.PacketProcessing.GameOpCommands
{
    internal class TestingGameOpCommand : GameOpCommand
    {
        #region Private Fields

        readonly string[] m_vArgs;

        #endregion Private Fields

        #region Public Constructors

        public TestingGameOpCommand(string[] args)
        {
            m_vArgs = args;
        }

        #endregion Public Constructors

        #region Public Methods

        public override void Execute(Level level)
        {
            var cm = new ShareStreamEntry();
            cm.SetId((int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
            cm.SetSenderId(0);
            cm.SetHomeId(0);
            cm.SetSenderLeagueId(22);
            cm.SetSenderName("Mimi");
            cm.SetSenderRole(4);
            cm.SetMessage("Look this ! I killed it ahahah");
            cm.SetEnemyName("Mimi");
            cm.SetReplayjson("{'lol': 'mdr'}");
            var all = ObjectManager.GetAlliance(level.GetPlayerAvatar().GetAllianceId());
            all.AddChatMessage(cm);

            foreach (var onlinePlayer in ResourcesManager.GetOnlinePlayers())
            {
                if (onlinePlayer.GetPlayerAvatar().GetAllianceId() == level.GetPlayerAvatar().GetAllianceId())
                {
                    var p = new AllianceStreamEntryMessage(onlinePlayer.GetClient());
                    p.SetStreamEntry(cm);
                    PacketManager.ProcessOutgoingPacket(p);
                }
            }
        }

        #endregion Public Methods
    }
}