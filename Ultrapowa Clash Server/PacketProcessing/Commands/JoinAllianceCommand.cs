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

using System.Collections.Generic;
using System.IO;
using UCS.Logic;

namespace UCS.PacketProcessing.Commands
{
    //Commande 0x001
    internal class JoinAllianceCommand : Command
    {
        #region Private Fields

        Alliance m_vAlliance;

        #endregion Private Fields

        #region Public Constructors

        public JoinAllianceCommand()
        {
            //AvailableServerCommandMessage
        }

        public JoinAllianceCommand(BinaryReader br)
        {
            br.ReadInt64();
            br.ReadString();
            br.ReadInt32();
            br.ReadByte();
            br.ReadInt32();
            br.ReadInt32();
            br.ReadInt32();
        }

        #endregion Public Constructors

        #region Public Methods

        public override byte[] Encode()
        {
            var data = new List<byte>();
            data.AddRange(m_vAlliance.EncodeHeader());
            return data.ToArray();
        }

        public void SetAlliance(Alliance alliance)
        {
            m_vAlliance = alliance;
        }

        #endregion Public Methods
    }
}