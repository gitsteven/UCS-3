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
using UCS.Files.Logic;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.PacketProcessing.Commands
{
    //Commande 0x20E
    internal class BoostBuildingCommand : Command
    {
        #region Public Constructors

        public BoostBuildingCommand(BinaryReader br)
        {
            BuildingIds = new List<int>();
            BoostedBuildingsCount = br.ReadInt32WithEndian();
            for (var i = 0; i < BoostedBuildingsCount; i++)
            {
                BuildingIds.Add(br.ReadInt32WithEndian()); //buildingId - 0x1DCD6500;
            }
        }

        #endregion Public Constructors

        #region Public Methods

        public override void Execute(Level level)
        {
            var ca = level.GetPlayerAvatar();
            foreach (var buildingId in BuildingIds)
            {
                var go = level.GameObjectManager.GetGameObjectByID(buildingId);

                var b = (ConstructionItem) go;
                var costs = ((BuildingData) b.GetConstructionItemData()).BoostCost[b.UpgradeLevel];
                if (ca.HasEnoughDiamonds(costs))
                {
                    b.BoostBuilding();
                    ca.SetDiamonds(ca.GetDiamonds() - costs);
                }
            }
        }

        #endregion Public Methods

        //00 00 02 0E 1D CD 65 05 00 00 8C 52

        #region Public Properties

        public int BoostedBuildingsCount { get; set; }
        public List<int> BuildingIds { get; set; }

        #endregion Public Properties
    }
}