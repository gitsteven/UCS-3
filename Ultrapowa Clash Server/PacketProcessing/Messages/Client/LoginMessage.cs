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
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using UCS.Core;
using UCS.Core.Network;
using UCS.Helpers;
using UCS.Logic;
using UCS.Logic.AvatarStreamEntry;
using UCS.PacketProcessing.Messages.Server;

namespace UCS.PacketProcessing.Messages.Client
{
    //Packet 10101
    internal class LoginMessage : Message
    {
        #region Public Constructors

        public LoginMessage(PacketProcessing.Client client, BinaryReader br) : base(client, br)
        {

        }

        #endregion Public Constructors

        #region Public Fields

        public string AdvertisingGUID;
        public string AndroidDeviceID;
        public string ClientVersion;
        public int ContentVersion;
        public string DeviceModel;
        public string FacebookDistributionID;
        public bool IsAdvertisingTrackingEnabled;
        public string Region;
        public int LocaleKey;
        public string MacAddress;
        public int MajorVersion;
        public string MasterHash;
        public int MinorVersion;
        public string OpenUDID;
        public string OSVersion;
        public int Seed;
        public int Unknown;
        public string Unknown1;
        public byte Unknown2;
        public string Unknown3;
        public byte Unknown4;
        public string Unknown5;
        public string Unknown6;
        public long UserID;
        public string UserToken;
        public string VendorGUID;
        public Level level;

        #endregion Public Fields

        #region Public Methods

        public override void Decode()
        {
            if (Client.CState == 1)
            {
                try
                {
                    using (var reader = new CoCSharpPacketReader(new MemoryStream(GetData())))
                    {
                        UserID = reader.ReadInt64();
                        UserToken = reader.ReadString();
                        MajorVersion = reader.ReadInt32();
                        ContentVersion = reader.ReadInt32();
                        MinorVersion = reader.ReadInt32();
                        MasterHash = reader.ReadString();
                        Unknown1 = reader.ReadString();
                        OpenUDID = reader.ReadString();
                        MacAddress = reader.ReadString();
                        DeviceModel = reader.ReadString();
                        LocaleKey = reader.ReadInt32();
                        Region = reader.ReadString();
                        AdvertisingGUID = reader.ReadString();
                        OSVersion = reader.ReadString();
                        Unknown2 = reader.ReadByte();
                        Unknown3 = reader.ReadString();
                        AndroidDeviceID = reader.ReadString();
                        FacebookDistributionID = reader.ReadString();
                        IsAdvertisingTrackingEnabled = reader.ReadBoolean();
                        VendorGUID = reader.ReadString();
                        Seed = reader.ReadInt32();
                        Unknown4 = reader.ReadByte();
                        Unknown5 = reader.ReadString();
                        Unknown6 = reader.ReadString();
                        ClientVersion = reader.ReadString();
                    }
                }
                catch (Exception e)
                {
                    //Debugger.WriteLine("[UCS]    Exception occured when reading packet", e);
                    Client.CState = 0;
                }
            }
        }

        public override void Process(Level a)
        {
            if (Client.CState >= 1)
            {
                CheckClient();
                // IF THE USER IS TOTALLY NEW, WITH ID 0 AND NO TOKEN
                if (UserID == 0 || string.IsNullOrEmpty(UserToken))
                {
                    NewUser();
                    return;
                }

                level = ResourcesManager.GetPlayer(UserID); // THE USER HAVE AN ID, WE CHECK IF IT'S IN DATABASE
                if (level != null)
                {
                    if (level.Banned()) // IF THE USER IS FOUND BUT BANNED
                    {
                        var p = new LoginFailedMessage(Client);
                        p.SetErrorCode(11);
                        PacketManager.ProcessOutgoingPacket(p);
                        return;
                    }
                    if (String.Equals(level.GetPlayerAvatar().GetUserToken(), UserToken, StringComparison.Ordinal)) // IF THE USER TOKEN MATCH THE CLIENT TOKEN
                    {
                        LogUser();
                    }
                    else // ELSE, HE IS TRYING TO STEAL AN ACCOUNT
                    {
                        var p = new LoginFailedMessage(Client);
                        p.SetErrorCode(11);
                        p.SetReason("We have detected unrecognized token sended from your devices. Please contact server owner for more information");
                        PacketManager.ProcessOutgoingPacket(p);
                        return;
                    }
                }
                else // IF NOTHING IS FOUND IN DATABASE WITH THIS ID, WE CREATE A NEW
                    NewUser();
        
                if (ResourcesManager.IsPlayerOnline(level))
                {
                    var mail = new AllianceMailStreamEntry();
                    mail.SetId((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                    mail.SetSenderId(0);
                    mail.SetSenderAvatarId(0);
                    mail.SetSenderName("Server Manager");
                    mail.SetIsNew(0);
                    mail.SetAllianceId(0);
                    mail.SetSenderLeagueId(22);
                    mail.SetAllianceBadgeData(1728059989);
                    mail.SetAllianceName("Server Admin");
                    mail.SetMessage(ConfigurationManager.AppSettings["AdminMessage"]);
                    mail.SetSenderLevel(500);
                    var p = new AvatarStreamEntryMessage(level.GetClient());
                    p.SetAvatarStreamEntry(mail);
                    PacketManager.ProcessOutgoingPacket(p);
                }
            }
        }

        void LogUser()
        {
            
            ResourcesManager.LogPlayerIn(level, Client);
            level.Tick();
            var loginOk = new LoginOkMessage(Client);
            var avatar = level.GetPlayerAvatar();
            loginOk.SetAccountId(avatar.GetId());
            loginOk.SetPassToken(avatar.GetUserToken());
            loginOk.SetServerMajorVersion(MajorVersion);
            loginOk.SetServerBuild(MinorVersion);
            loginOk.SetContentVersion(ContentVersion);
            loginOk.SetServerEnvironment("prod");
            loginOk.SetDaysSinceStartedPlaying(0);
            loginOk.SetServerTime(Math.Round(level.GetTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds * 1000).ToString(CultureInfo.InvariantCulture));
            loginOk.SetAccountCreatedDate("1414003838000");
            loginOk.SetStartupCooldownSeconds(0);
            loginOk.SetCountryCode(avatar.GetUserRegion().ToUpper());
            PacketManager.ProcessOutgoingPacket(loginOk);
            var alliance = ObjectManager.GetAlliance(level.GetPlayerAvatar().GetAllianceId());
            PacketManager.ProcessOutgoingPacket(new OwnHomeDataMessage(Client, level));

            if (alliance == null)
                level.GetPlayerAvatar().SetAllianceId(0);
            else
            {
                PacketManager.ProcessOutgoingPacket(new AllianceFullEntryMessage(Client, alliance));
                PacketManager.ProcessOutgoingPacket(new AllianceStreamMessage(Client, alliance));
            }
            PacketManager.ProcessOutgoingPacket(new BookmarkMessage(Client));
            PacketManager.ProcessOutgoingPacket(new LeaguePlayersMessage(Client));
        }

        void CheckClient()
        {
            int time = Convert.ToInt32(ConfigurationManager.AppSettings["maintenanceTimeleft"]);
            if (time != 0 || Client.CState == 0)
            {
                var p = new LoginFailedMessage(Client);
                p.SetErrorCode(10);
                p.RemainingTime(time);
                PacketManager.ProcessOutgoingPacket(p);
                return;
            }

            var cv = ClientVersion.Split('.');
            if (cv[0] != "8" || cv[1] != "332")
            {
                var p = new LoginFailedMessage(Client);
                p.SetErrorCode(8);
                p.SetUpdateURL(Convert.ToString(ConfigurationManager.AppSettings["UpdateUrl"]));
                PacketManager.ProcessOutgoingPacket(p);
                return;
            }

            if (Convert.ToBoolean(ConfigurationManager.AppSettings["useCustomPatch"]) &&
                MasterHash != ObjectManager.FingerPrint.sha)
            {
                var p = new LoginFailedMessage(Client);
                p.SetErrorCode(7);
                p.SetResourceFingerprintData(ObjectManager.FingerPrint.SaveToJson());
                p.SetContentURL(ConfigurationManager.AppSettings["patchingServer"]);
                p.SetUpdateURL(ConfigurationManager.AppSettings["UpdateUrl"]);
                PacketManager.ProcessOutgoingPacket(p);
                return;
            }
        }

        void NewUser()
        {
            level = ObjectManager.CreateAvatar(0, null);
            if (string.IsNullOrEmpty(UserToken))
            {
                var tokenSeed = new byte[20];
                new Random().NextBytes(tokenSeed);
                using (SHA1 sha = new SHA1CryptoServiceProvider())
                UserToken = BitConverter.ToString(sha.ComputeHash(tokenSeed)).Replace("-", string.Empty);
            }
            level.GetPlayerAvatar().SetRegion(Region.ToUpper());
            level.GetPlayerAvatar().SetToken(UserToken);
            DatabaseManager.Singelton.Save(level);
            LogUser();
        }

        #endregion Public Methods
    }
}
