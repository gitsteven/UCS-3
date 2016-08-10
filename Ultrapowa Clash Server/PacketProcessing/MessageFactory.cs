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
using System.IO;
using UCS.Helpers;
using UCS.PacketProcessing.Messages.Client;

namespace UCS.PacketProcessing
{
    //Command list: LogicCommand::createCommand
    internal static class MessageFactory
    {

        static readonly Dictionary<int, Type> m_vMessages;

        static MessageFactory()
        {
            m_vMessages = new Dictionary<int, Type>();
            m_vMessages.Add(10100, typeof(HandshakeRequest));//Works
            m_vMessages.Add(10101, typeof(LoginMessage));//Works
            m_vMessages.Add(10105, typeof(AskForFriendListMessage));
            m_vMessages.Add(10108, typeof(KeepAliveMessage)); //Works
            m_vMessages.Add(10117, typeof(ReportPlayerMessage)); 
            m_vMessages.Add(10113, typeof(GetDeviceTokenMessage));
            m_vMessages.Add(10212, typeof(ChangeAvatarNameMessage)); //Works
            m_vMessages.Add(14101, typeof(GoHomeMessage)); //Works
            m_vMessages.Add(14102, typeof(ExecuteCommandsMessage));
            m_vMessages.Add(14113, typeof(VisitHomeMessage));//Works
            m_vMessages.Add(14134, typeof(AttackNpcMessage));//Works
            m_vMessages.Add(14201, typeof(FacebookLinkMessage));
            m_vMessages.Add(14316, typeof(EditClanSettingsMessage));//Needed
            m_vMessages.Add(14301, typeof(CreateAllianceMessage));//Works 1/2
            m_vMessages.Add(14302, typeof(AskForAllianceDataMessage));//Works
            m_vMessages.Add(14303, typeof(AskForJoinableAlliancesListMessage));//Not tested
            m_vMessages.Add(14305, typeof(JoinAllianceMessage));//Not tested
            m_vMessages.Add(14306, typeof(PromoteAllianceMemberMessage));//Not tested
            m_vMessages.Add(14308, typeof(LeaveAllianceMessage));// Can't test
            m_vMessages.Add(14315, typeof(ChatToAllianceStreamMessage));//Works
            m_vMessages.Add(14317, typeof(JoinRequestAllianceMessage));// Not tested
            m_vMessages.Add(14321, typeof(TakeDecisionJoinRequestMessage));
            m_vMessages.Add(14322, typeof(AllianceInviteMessage));
            m_vMessages.Add(14324, typeof(SearchAlliancesMessage));//Works
            m_vMessages.Add(14325, typeof(AskForAvatarProfileMessage));
            m_vMessages.Add(14331, typeof(AskForAllianceWarDataMessage));
            m_vMessages.Add(14336, typeof(AskForAllianceWarHistoryMessage));
            m_vMessages.Add(14341, typeof(AskForBookmarkMessage));//Works
            m_vMessages.Add(14343, typeof(AddToBookmarkMessage));//Works
            m_vMessages.Add(14344, typeof(RemoveFromBookmarkMessage));//Can't test
            m_vMessages.Add(14715, typeof(SendGlobalChatLineMessage));//Works
            m_vMessages.Add(14401, typeof(TopGlobalAlliancesMessage));//Works
            m_vMessages.Add(14402, typeof(TopLocalAlliancesMessage));//Works
            m_vMessages.Add(14403, typeof(TopGlobalPlayersMessage));//Works
            m_vMessages.Add(14404, typeof(TopLocalPlayersMessage));//Works
            m_vMessages.Add(14406, typeof(TopPreviousGlobalPlayersMessage));//Works
            m_vMessages.Add(14503, typeof(TopLeaguePlayersMessage));//Works
            m_vMessages.Add(14600, typeof(RequestAvatarNameChange));//Can't test
        }

        public static object Read(Client c, BinaryReader br, int packetType)
        {
            if (m_vMessages.ContainsKey(packetType))
                return Activator.CreateInstance(m_vMessages[packetType], c, br);
            Console.WriteLine("[UCS]    The message '" + packetType + "' is unhandled");
            return null;
        }

    }
}