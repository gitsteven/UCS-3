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
    //Commande 0x1FB 507
    internal class ClearObstacleCommand : Command
    {
        #region Public Constructors

        public ClearObstacleCommand(BinaryReader br)
        {
            ObstacleId = br.ReadInt32WithEndian(); //ObstacleId - 0x1DFB2BC0;
            Unknown1 = br.ReadUInt32WithEndian();
        }

        #endregion Public Constructors

        #region Public Methods

        public override void Execute(Level level)
        {
            var ca = level.GetPlayerAvatar();
            var go = level.GameObjectManager.GetGameObjectByID(ObstacleId);

            var o = (Obstacle) go;
            var od = o.GetObstacleData();
            if (ca.HasEnoughResources(od.GetClearingResource(), od.ClearCost))
            {
                if (level.HasFreeWorkers())
                {
                    var rd = od.GetClearingResource();
                    ca.SetResourceCount(rd, ca.GetResourceCount(rd) - od.ClearCost);
                    o.StartClearing();
                }
            }
        }

        #endregion Public Methods

        #region Public Properties

        public int ObstacleId { get; set; }

        //00 00 E1 83
        //1D FB 2B C1
        public uint Unknown1 { get; set; }

        #endregion Public Properties
    }
}