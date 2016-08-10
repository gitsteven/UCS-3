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

using System.IO;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers;
using UCS.Logic;
using UCS.PacketProcessing.Commands;
using UCS.PacketProcessing.Messages.Server;

namespace UCS.PacketProcessing.Messages.Client
{
    //Packet 14301
    internal class CreateAllianceMessage : Message
    {
        #region Public Constructors

        public CreateAllianceMessage(PacketProcessing.Client client, BinaryReader br) : base(client, br)
        {
            Decrypt();
        }

        #endregion Public Constructors

        #region Private Fields

        int m_vAllianceBadgeData;
        string m_vAllianceDescription;
        string m_vAllianceName;
        int m_vAllianceOrigin;
        int m_vAllianceType;
        int m_vRequiredScore;
        int m_vWarFrequency;

        #endregion Private Fields

        #region Public Methods

        //00 00 00 04 6E 61 6D 65 00 00 00 0B 64 65 73 63 72 69 70 74 69 6F 6E 5B 00 02 52 00 00 00 01 00 00 07 D0 00 00 00 02 01 E8 48 39
        //00 00 00 04 6E 61 6D 65 00 00 00 0B 64 65 73 63 72 69 70 74 69 6F 6E 00 00 00 00 00 00 00 02 00 00 07 D0 00 00 00 02 01 E8 48 3A
        public override void Decode()
        {
            using (var br = new BinaryReader(new MemoryStream(GetData())))
            {
                m_vAllianceName = br.ReadScString(); //6E 61 6D 65
                m_vAllianceDescription = br.ReadScString(); //64 65 73 63 72 69 70 74 69 6F 6E
                m_vAllianceBadgeData = br.ReadInt32WithEndian(); //5B 00 02 52
                m_vAllianceType = br.ReadInt32WithEndian(); //00 00 00 01
                m_vRequiredScore = br.ReadInt32WithEndian(); //00 00 07 D0
                m_vWarFrequency = br.ReadInt32WithEndian(); //00 00 00 02
                m_vAllianceOrigin = br.ReadInt32WithEndian(); //01 E8 48 39
            }
        }

        public override void Process(Level level)
        {
            //Clan creation
            var alliance = ObjectManager.CreateAlliance(0);
            alliance.SetAllianceName(m_vAllianceName);
            alliance.SetAllianceDescription(m_vAllianceDescription);
            alliance.SetAllianceType(m_vAllianceType);
            alliance.SetRequiredScore(m_vRequiredScore);
            alliance.SetAllianceBadgeData(m_vAllianceBadgeData);
            alliance.SetAllianceOrigin(m_vAllianceOrigin);
            alliance.SetWarFrequency(m_vWarFrequency);

            //Set player clan
            level.GetPlayerAvatar().SetAllianceId(alliance.GetAllianceId());
            var member = new AllianceMemberEntry(level.GetPlayerAvatar().GetId());
            member.SetRole(2);
            alliance.AddAllianceMember(member);

            var joinAllianceCommand = new JoinAllianceCommand();
            joinAllianceCommand.SetAlliance(alliance);
            var availableServerCommandMessage = new AvailableServerCommandMessage(Client);
            availableServerCommandMessage.SetCommandId(1);
            availableServerCommandMessage.SetCommand(joinAllianceCommand);
            PacketManager.ProcessOutgoingPacket(availableServerCommandMessage);
            PacketManager.ProcessOutgoingPacket(new AllianceStreamMessage(Client, alliance));
            PacketManager.ProcessOutgoingPacket(new AllianceFullEntryMessage(Client, alliance));
            //PacketManager.ProcessOutgoingPacket(new OutOfSyncMessage(level.GetClient()));
            // Necessary to display the "Customize" Button.
        }

        #endregion Public Methods
    }
}