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
    //Commande 0x226
    internal class RemoveUnitsCommand : Command
    {
        #region Public Constructors

        public RemoveUnitsCommand(BinaryReader br)
        {
            Unknown1 = br.ReadUInt32WithEndian();
            UnitTypesCount = br.ReadInt32WithEndian();

            UnitsToRemove = new List<UnitToRemove>();
            for (var i = 0; i < UnitTypesCount; i++)
            {
                var unit = (CharacterData) br.ReadDataReference();
                var count = br.ReadInt32WithEndian();
                var level = br.ReadInt32WithEndian();
                UnitsToRemove.Add(new UnitToRemove { Data = unit, Count = count, Level = level });
            }

            Unknown2 = br.ReadUInt32WithEndian();
        }

        #endregion Public Constructors

        #region Public Methods

        public override void Execute(Level level)
        {
            foreach (var unit in UnitsToRemove)
            {
                var components = level.GetComponentManager().GetComponents(0);
                for (var i = 0; i < components.Count; i++)
                {
                    var c = (UnitStorageComponent) components[i];
                    if (c.GetUnitTypeIndex(unit.Data) != -1)
                    {
                        var storageCount = c.GetUnitCountByData(unit.Data);
                        if (storageCount >= unit.Count)
                        {
                            c.RemoveUnits(unit.Data, unit.Count);
                            break;
                        }
                        c.RemoveUnits(unit.Data, storageCount);
                        unit.Count -= storageCount;
                    }
                }
            }
        }

        #endregion Public Methods

        //00 00 17 D6 24 B8 F6 5B 00 00 00 01
        //00 00 02 26 00 00 00 00 00 00 00 02 00 3D 09 00 00 00 00 03 00 00 00 01 00 3D 09 08 00 00 00 02 00 00 00 03 00 00 17 98

        #region Public Properties

        public List<UnitToRemove> UnitsToRemove { get; set; }
        public int UnitTypesCount { get; set; }
        public uint Unknown1 { get; set; }
        public uint Unknown2 { get; set; }

        #endregion Public Properties
    }

    internal class UnitToRemove
    {
        #region Public Properties

        public int Count { get; set; }
        public CharacterData Data { get; set; }
        public int Level { get; set; }

        #endregion Public Properties
    }
}