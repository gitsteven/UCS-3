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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UCS.Logic.Manager;
using UCS.PacketProcessing;

namespace UCS.Logic
{
    internal class Level
    {
        #region Public Fields

        public GameObjectManager GameObjectManager;
        public WorkerManager WorkerManager;

        #endregion Public Fields

        #region Private Fields

        readonly ClientAvatar m_vClientAvatar;

        //a1 + 44
        byte m_vAccountPrivileges;

        byte m_vAccountStatus;
        Client m_vClient;
        string m_vIPAddress;
        DateTime m_vTime;

        #endregion Private Fields

        //a1 + 40
        //MissionManager
        //AchievementManager
        //CooldownManager

        #region Public Constructors

        public Level()
        {
            WorkerManager = new WorkerManager();
            GameObjectManager = new GameObjectManager(this);
            m_vClientAvatar = new ClientAvatar();
            m_vAccountPrivileges = 0;
            m_vAccountStatus = 0;
            m_vIPAddress = "0.0.0.0";
        }

        public Level(long id, string token)
        {
            WorkerManager = new WorkerManager();
            GameObjectManager = new GameObjectManager(this);
            m_vClientAvatar = new ClientAvatar(id, token);
            m_vTime = DateTime.UtcNow;
            m_vAccountPrivileges = 0;
            m_vAccountStatus = 0;
            m_vIPAddress = "0.0.0.0";
        }

        #endregion Public Constructors

        #region Public Methods

        public byte GetAccountPrivileges()
        {
            return m_vAccountPrivileges;
        }

        public bool Banned()
        {
            if (m_vAccountStatus == 99)
                return true;
            return false;
        }

        public byte GetAccountStatus()
        {
            return m_vAccountStatus;
        }

        public Client GetClient()
        {
            return m_vClient;
        }

        public ComponentManager GetComponentManager()
        {
            return GameObjectManager.GetComponentManager();
        }

        public ClientAvatar GetHomeOwnerAvatar()
        {
            return m_vClientAvatar;
        }

        public string GetIPAddress()
        {
            return m_vIPAddress;
        }

        public ClientAvatar GetPlayerAvatar()
        {
            return m_vClientAvatar;
        }

        public DateTime GetTime()
        {
            return m_vTime;
        }

        public bool HasFreeWorkers()
        {
            return WorkerManager.GetFreeWorkers() > 0;
        }

        public void LoadFromJSON(string jsonString)
        {
            var jsonObject = JObject.Parse(jsonString);
            GameObjectManager.Load(jsonObject);
        }

        public string SaveToJSON()
        {
            return JsonConvert.SerializeObject(GameObjectManager.Save());
        }

        public void SetAccountPrivileges(byte privileges)
        {
            m_vAccountPrivileges = privileges;
        }

        public void SetAccountStatus(byte status)
        {
            m_vAccountStatus = status;
        }

        public void SetClient(Client client)
        {
            m_vClient = client;
        }

        public void SetHome(string jsonHome)
        {
            GameObjectManager.Load(JObject.Parse(jsonHome));
        }

        public void SetIPAddress(string IP)
        {
            m_vIPAddress = IP;
        }

        public void SetTime(DateTime t)
        {
            m_vTime = t;
        }

        public void Tick()
        {
            SetTime(DateTime.UtcNow);
            GameObjectManager.Tick();
        }

        #endregion Public Methods
    }
}