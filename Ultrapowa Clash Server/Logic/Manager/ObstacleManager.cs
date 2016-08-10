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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using UCS.Core;
using UCS.Files.Logic;

namespace UCS.Logic.Manager
{
    internal class ObstacleManager
    {
        #region Public Constructors

        public ObstacleManager(Level level)
        {
            m_vLevel = level;
            if (m_vObstacleLimit == -1)
            {
                m_vObstacleLimit = ObjectManager.DataTables.GetGlobals().GetGlobalData("OBSTACLE_COUNT_MAX").NumberValue;
                m_vObstacleRespawnSeconds = ObjectManager.DataTables.GetGlobals().GetGlobalData("OBSTACLE_RESPAWN_SECONDS").NumberValue;
            }
            if (!m_vSpawnAbleObstacles.Any())
            {
                var dt = ObjectManager.DataTables.GetTable(7);
                for (var i = 0; i < dt.GetItemCount(); i++)
                {
                    var od = (ObstacleData) dt.GetItemAt(i);
                    if (!od.IsTombstone)
                    {
                        if (!od.GetName().Contains("Gembox"))
                        {
                            if (od.RespawnWeight > 0)
                            {
                                m_vSpawnAbleObstacles.Add(od);
                                SumWeights += od.RespawnWeight;
                            }
                        }
                        else
                            m_vGemBoxes.Add(od);
                    }
                }
            }
            m_vNormalTimer = new Timer();
            m_vGemBoxTimer = new Timer();
            m_vSpecialTimer = new Timer();
            m_vNormalTimer.StartTimer(m_vObstacleRespawnSeconds, level.GetTime());
            m_vGemBoxTimer.StartTimer(m_vObstacleRespawnSeconds * 2, level.GetTime());
            m_vSpecialTimer.StartTimer(m_vObstacleRespawnSeconds, level.GetTime());
            m_vObstacleClearCount = 0;
            m_vRespawnSeed = new Random().Next();
        }

        #endregion Public Constructors

        #region Private Fields

        static readonly List<ObstacleData> m_vGemBoxes = new List<ObstacleData>();

        static readonly List<ObstacleData> m_vSpawnAbleObstacles = new List<ObstacleData>();

        static int m_vObstacleLimit = -1;

        static int m_vObstacleRespawnSeconds = -1;

        static int SumWeights;

        readonly Timer m_vGemBoxTimer;

        readonly Level m_vLevel;

        readonly Timer m_vNormalTimer;

        readonly Timer m_vSpecialTimer;

        volatile int m_vObstacleClearCount;

        int m_vRespawnSeed;

        #endregion Private Fields

        #region Public Methods

        public void IncreaseObstacleClearCount()
        {
            m_vObstacleClearCount++;
            m_vObstacleClearCount = Math.Min(m_vObstacleClearCount, 40);
        }

        public void Load(JObject jsonObject)
        {
            var jToken = jsonObject["respawnVars"];
            if (jToken != null)
            {
                var jObj = jToken.ToObject<JObject>();
                m_vRespawnSeed = jObj["respawnSeed"].ToObject<int>();
                m_vObstacleClearCount = jObj["obstacleClearCounter"].ToObject<int>();
                if (jObj["normal_t"] != null)
                {
                    m_vNormalTimer.StartTimer(
                        m_vObstacleRespawnSeconds - jObj["secondsFromLastRespawn"].ToObject<int>(),
                        jObj["normal_t"].ToObject<DateTime>());
                    m_vGemBoxTimer.StartTimer(jObj["time_to_gembox_drop"].ToObject<int>(),
                        jObj["gembox_t"].ToObject<DateTime>());
                    m_vSpecialTimer.StartTimer(jObj["time_to_special_drop"].ToObject<int>(),
                        jObj["special_t"].ToObject<DateTime>());
                }
            }
            else
            {
                m_vNormalTimer.StartTimer(m_vObstacleRespawnSeconds, m_vLevel.GetTime());
                m_vGemBoxTimer.StartTimer(m_vObstacleRespawnSeconds * 2, m_vLevel.GetTime());
                m_vSpecialTimer.StartTimer(m_vObstacleRespawnSeconds, m_vLevel.GetTime());
            }
        }

        public JObject Save(JObject jsonData)
        {
            //"respawnVars":{"secondsFromLastRespawn":369,"respawnSeed":-212853765,"obstacleClearCounter":0,"time_to_gembox_drop":359631,"time_in_gembox_period":244800,"time_to_special_drop":248031,"time_to_special_period":97200}

            var jobj = new JObject();

            jobj.Add("respawnSeed", m_vRespawnSeed);
            jobj.Add("obstacleClearCounter", m_vObstacleClearCount);
            if (m_vNormalTimer != null)
            {
                jobj.Add("secondsFromLastRespawn",
                    m_vObstacleRespawnSeconds - m_vNormalTimer.GetRemainingSeconds(m_vLevel.GetTime()));
                jobj.Add("time_to_gembox_drop", m_vGemBoxTimer.GetRemainingSeconds(m_vLevel.GetTime()));
                jobj.Add("time_to_special_drop", m_vSpecialTimer.GetRemainingSeconds(m_vLevel.GetTime()));
                jobj.Add("normal_t", m_vNormalTimer.GetStartTime());
                jobj.Add("gembox_t", m_vGemBoxTimer.GetStartTime());
                jobj.Add("special_t", m_vSpecialTimer.GetStartTime());
            }

            jsonData.Add("respawnVars", jobj);

            return jsonData;
        }

        public void Tick()
        {
            while (m_vObstacleClearCount > 0 && m_vNormalTimer.GetRemainingSeconds(m_vLevel.GetTime()) <= 0)
            {
                //Debugger.WriteLine("Start adding new Obstacle", null, 5);
                var ob = GetRandomObstacle();
                var pos = GetFreePlace(ob);
                if (pos != null)
                {
                    SpawnObstacle(pos, ob);
                    m_vObstacleClearCount--;
                    if (m_vObstacleClearCount > 0)
                    {
                        m_vNormalTimer.StartTimer(m_vObstacleRespawnSeconds,
                            m_vNormalTimer.GetStartTime().AddSeconds(m_vObstacleRespawnSeconds));
                    }
                    else
                        m_vNormalTimer.StartTimer(m_vObstacleRespawnSeconds, m_vLevel.GetTime());
                    //Debugger.WriteLine("Finished adding new Obstacle " + ob.GetName(), null, 5);
                }
                else
                {
                    m_vNormalTimer.StartTimer(m_vObstacleRespawnSeconds, m_vLevel.GetTime());
                    break;
                }
            }
            if (m_vGemBoxTimer.GetRemainingSeconds(m_vLevel.GetTime()) <= 0)
            {
                if (new Random().Next(0, 4) == 0)
                {
                    //Debugger.WriteLine("Start adding new Obstacle", null, 5);
                    var ob = m_vGemBoxes[new Random().Next(0, m_vGemBoxes.Count)];
                    var pos = GetFreePlace(ob);
                    if (pos != null)
                    {
                        SpawnObstacle(pos, ob);
                        m_vGemBoxTimer.StartTimer(m_vObstacleRespawnSeconds * 2, m_vLevel.GetTime());
                        //Debugger.WriteLine("Finished adding new Obstacle " + ob.GetName(), null, 5);
                    }
                }
                else
                    m_vGemBoxTimer.StartTimer(m_vObstacleRespawnSeconds * 2, m_vLevel.GetTime());
            }
        }

        #endregion Public Methods

        #region Private Methods

        int[] GetFreePlace(ObstacleData od)
        {
            try
            {
                var pos = new int[2];
                var field = new int[46, 46];
                foreach (var list in m_vLevel.GameObjectManager.GetAllGameObjects())
                {
                    foreach (var go in list)
                    {
                        int w = 0, h = 0;
                        int x = 0, y = 0;
                        x = go.X;
                        y = go.Y;

                        switch (go.GetData().GetDataType())
                        {
                            case 0:
                                x--;
                                y--;
                                w = ((BuildingData) go.GetData()).Width + 2;
                                h = ((BuildingData) go.GetData()).Height + 2;
                                break;

                            case 7:
                                w = ((ObstacleData) go.GetData()).Width;
                                h = ((ObstacleData) go.GetData()).Height;
                                break;

                            case 11:
                                x--;
                                y--;
                                w = ((TrapData) go.GetData()).Width + 2;
                                h = ((TrapData) go.GetData()).Height + 2;
                                break;

                            case 17:
                                x--;
                                y--;
                                w = ((DecoData) go.GetData()).Width + 2;
                                h = ((DecoData) go.GetData()).Height + 2;
                                break;
                        }

                        for (var i = 0; i < w; i++)
                        {
                            for (var j = 0; j < h; j++)
                                field[x + i, y + j] = 1;
                        }
                    }
                }
                var freePositions = new List<int[]>();
                for (var i = 2; i < 42 - od.Height; i++)
                {
                    for (var j = 2; j < 42 - od.Width; j++)
                    {
                        if (field[i, j] != 1)
                            freePositions.Add(new[] { i, j });
                    }
                }

                if (freePositions.Count < od.Height * od.Width)
                    return null;

                freePositions.OrderBy(t => new Random().Next());
                var z = 0;
                pos = null;
                while (z < freePositions.Count && pos == null)
                {
                    if (obstacleHasSpace(od, freePositions[z][0], freePositions[z][1], field))
                        pos = freePositions[z];
                    z++;
                }

               return pos;
            }
            catch (Exception e)
            {
                //Debugger.WriteLine("An Exception occured during GetFreePlace", e, 0);
                return null;
            }
        }

        ObstacleData GetRandomObstacle()
        {
            var randomValue = new Random().Next(0, SumWeights);
            foreach (var ob in m_vSpawnAbleObstacles)
            {
                randomValue -= ob.RespawnWeight;
                if (randomValue <= 0)
                    return ob;
            }
            return m_vSpawnAbleObstacles[0];
        }

        bool obstacleHasSpace(ObstacleData od, int x, int y, int[,] field)
        {
            int w = od.Width, h = od.Height;
            for (var i = 0; i < w; i++)
            {
                for (var j = 0; j < h; j++)
                {
                    if (field[x + i, y + j] == 1)
                        return false;
                }
            }

            return true;
        }

        void SpawnObstacle(int[] position, ObstacleData data)
        {
            var o = new Obstacle(data, m_vLevel);
            o.SetPositionXY(position[0], position[1]);
            m_vLevel.GameObjectManager.AddGameObject(o);
        }

        #endregion Private Methods
    }
}