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
    //Commande 0x202
    internal class SpeedUpClearingCommand : Command
    {
        #region Private Fields

        readonly int m_vObstacleId;

        #endregion Private Fields

        #region Public Constructors

        public SpeedUpClearingCommand(BinaryReader br)
        {
            m_vObstacleId = br.ReadInt32WithEndian();
            br.ReadInt32WithEndian();
        }

        #endregion Public Constructors

        #region Public Methods

        public override void Execute(Level level)
        {
            var go = level.GameObjectManager.GetGameObjectByID(m_vObstacleId);
            if (go != null)
            {
                if (go.ClassId == 3)
                    ((Obstacle) go).SpeedUpClearing();
            }
        }

        #endregion Public Methods
    }
}