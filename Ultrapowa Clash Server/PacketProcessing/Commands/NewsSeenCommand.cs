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
using UCS.Logic;

namespace UCS.PacketProcessing.Commands
{
    //Commande 0x21B
    internal class NewsSeenCommand : Command
    {
        #region Public Fields

        public byte[] packet;

        #endregion Public Fields

        #region Public Constructors

        public NewsSeenCommand(BinaryReader br)
        {
            //packet = br.ReadAllBytes();
            //Unknown1 = br.ReadUInt32WithEndian();
            //Unknown2 = br.ReadUInt32WithEndian();
        }

        #endregion Public Constructors

        //00 00 00 00

        #region Public Methods

        public override void Execute(Level level)
        {
        }

        #endregion Public Methods

        //00 00 00 02 00 00 02 1B 00 00 00 0C 00 00 00 00 00 00 02 1B 00 00 00 0D 00 00 00 00

        #region Public Properties

        public uint Unknown1 { get; set; } //00 00 00 0C
        public uint Unknown2 { get; set; }

        #endregion Public Properties
    }
}