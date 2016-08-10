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
using System.IO;
using System.Linq;
using UCS.Core;
using UCS.Core.Network;
using UCS.Logic;
using UCS.Logic.StreamEntry;
using UCS.PacketProcessing.Messages.Server;

namespace UCS.PacketProcessing.Messages.Client
{
    //Packet 14308
    internal class LeaveAllianceMessage : Message
    {
        #region Public Fields

        public static bool done;

        #endregion Public Fields

        #region Public Constructors

        public LeaveAllianceMessage(PacketProcessing.Client client, BinaryReader br) : base(client, br)
        {
        }

        #endregion Public Constructors

        #region Public Methods

        public override void Decode()
        {
        }

        public override void Process(Level level)
        {
            var avatar = level.GetPlayerAvatar();
            var alliance = ObjectManager.GetAlliance(level.GetPlayerAvatar().GetAllianceId());
            if (avatar.GetAllianceRole() == 2 && alliance.GetAllianceMembers().Count != 0)
            {
                var members = alliance.GetAllianceMembers();
                foreach (var player in members.Where(player => player.GetRole() >= 3))
                {
                    player.SetRole(2);
                    done = true;
                    break;
                }
                if (!done)
                {
                    var count = alliance.GetAllianceMembers().Count;
                    var rnd = new Random();
                    var id = rnd.Next(1, count);
                    while (id != level.GetPlayerAvatar().GetId())
                        id = rnd.Next(1, count);
                    var loop = 0;
                    foreach (var player in members)
                    {
                        loop++;
                        if (loop == id)
                        {
                            player.SetRole(2);
                            break;
                        }
                    }
                }
            }

            alliance.RemoveMember(avatar.GetId());
            avatar.SetAllianceId(0);

            if (alliance.GetAllianceMembers().Count > 0)
            {
                var eventStreamEntry = new AllianceEventStreamEntry();
                eventStreamEntry.SetId((int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                eventStreamEntry.SetAvatar(avatar);
                eventStreamEntry.SetEventType(4);
                eventStreamEntry.SetAvatarId(avatar.GetId());
                eventStreamEntry.SetAvatarName(avatar.GetAvatarName());
                alliance.AddChatMessage(eventStreamEntry);

                foreach (var onlinePlayer in ResourcesManager.GetOnlinePlayers())
                    if (onlinePlayer.GetPlayerAvatar().GetAllianceId() == alliance.GetAllianceId())
                    {
                        var p = new AllianceStreamEntryMessage(onlinePlayer.GetClient());
                        p.SetStreamEntry(eventStreamEntry);
                        PacketManager.ProcessOutgoingPacket(p);
                    }
            }
            else
                DatabaseManager.Singelton.RemoveAlliance(alliance);

            PacketManager.ProcessOutgoingPacket(new LeaveAllianceOkMessage(Client, alliance));
            DatabaseManager.Singelton.Save(level);
        }

        #endregion Public Methods
    }
}