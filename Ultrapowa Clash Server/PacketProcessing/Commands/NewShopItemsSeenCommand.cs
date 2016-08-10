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

namespace UCS.PacketProcessing.Commands
{
    //Commande 0x0214
    internal class NewShopItemsSeenCommand : Command
    {
        #region Public Constructors

        public NewShopItemsSeenCommand(BinaryReader br)
        {
            /*
            var NewShopItemNumber = br.ReadUInt32WithEndian();
            var Unknown1 = br.ReadUInt32WithEndian();
            var Unknown2 = br.ReadUInt32WithEndian();
            var Unknown3 = br.ReadUInt32WithEndian();
            */
        }

        #endregion Public Constructors

        #region Public Properties

        public uint NewShopItemNumber { get; set; }
        public uint Unknown1 { get; set; }
        public uint Unknown2 { get; set; }
        public uint Unknown3 { get; set; }

        #endregion Public Properties
    }
}