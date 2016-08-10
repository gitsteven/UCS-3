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
using UCS.Helpers;
using UCS.Logic;

namespace UCS.PacketProcessing.Commands
{
    //Commande 0x1F5
    internal class MoveBuildingCommand : Command
    {
        #region Public Constructors

        public MoveBuildingCommand(BinaryReader br)
        {
            X = br.ReadInt32WithEndian();
            Y = br.ReadInt32WithEndian();
            BuildingId = br.ReadInt32WithEndian(); //buildingId - 0x1DCD6500;
            Unknown1 = br.ReadUInt32WithEndian();
        }

        #endregion Public Constructors

        //00 00 00 1F

        #region Public Methods

        public override void Execute(Level level)
        {
            var go = level.GameObjectManager.GetGameObjectByID(BuildingId);
            go.SetPositionXY(X, Y);
        }

        #endregion Public Methods

        //30/08/2014 18:51;S;14102(0);32;00 00 2D BE 01 EB 32 0C 00 00 00 01 00 00 01 F5 00 00 00 13 00 00 00 1F 1D CD 65 06 00 00 2D 7F

        #region Public Properties

        public int BuildingId { get; set; }

        //1D CD 65 06 some unique id
        public uint Unknown1 { get; set; }

        public int X { get; set; } //00 00 00 13
        public int Y { get; set; }

        #endregion Public Properties
    }
}