using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace Vision
{
    class PacketHandler : IPhotonPackageHandler
    {


        public PacketHandler()
        {
 
        }
        public void OnEvent(byte code, Dictionary<byte, object> parameters)
        {
            if (code == 2)
            {
                //player movement comes with binary format - not normal.
                return;
            }

            object val;
            parameters.TryGetValue((byte)252, out val);
            if (val == null) return;

            int iCode = 0;
            if (!int.TryParse(val.ToString(), out iCode)) return;

            EventCodes eventCode = (EventCodes)iCode;
            //List<int> Whitelist = new List<int>() { 83}
            List<EventCodes> IgnoreList = new List<EventCodes>();

            IgnoreList.Add(EventCodes.evActionOnBuildingStart);
            IgnoreList.Add(EventCodes.evActiveSpellEffectsUpdate);
            IgnoreList.Add(EventCodes.evAttachItemContainer);
            IgnoreList.Add(EventCodes.evAttack);
            IgnoreList.Add(EventCodes.evAttackBuilding);
            IgnoreList.Add(EventCodes.evCastHit);
            IgnoreList.Add(EventCodes.evCastHits);
            IgnoreList.Add(EventCodes.evCastSpell);
            IgnoreList.Add(EventCodes.evCastStart);
            IgnoreList.Add(EventCodes.evChannelingEnded);
            IgnoreList.Add(EventCodes.evCharacterStatsDeathHistory);
            IgnoreList.Add(EventCodes.evChatWhisper);
            IgnoreList.Add(EventCodes.evDuelReEnteredArea);
            IgnoreList.Add(EventCodes.evDurabilityChanged);
            IgnoreList.Add(EventCodes.evEnergyUpdate);
            IgnoreList.Add(EventCodes.evEnteringArenaCancel);
            IgnoreList.Add(EventCodes.evExpeditionStartEvent);
            IgnoreList.Add(EventCodes.evGuildLogoUpdate);
            IgnoreList.Add(EventCodes.evGuildMemberWorldUpdate);
            IgnoreList.Add(EventCodes.evHealthUpdate);
            IgnoreList.Add(EventCodes.evInCombatStateUpdate);
            IgnoreList.Add(EventCodes.evInitHideoutAttackCancel);
            IgnoreList.Add(EventCodes.evKilledPlayer);
            IgnoreList.Add(EventCodes.evKnockedDown);
            IgnoreList.Add(EventCodes.evLeave);
            IgnoreList.Add(EventCodes.evMatchPlayerJoinedEvent);
            IgnoreList.Add(EventCodes.evMatchPlayerStatsCompleteEvent);
            IgnoreList.Add(EventCodes.evMatchTimeLineEventEvent);
            IgnoreList.Add(EventCodes.evNewEquipmentItem);
            IgnoreList.Add(EventCodes.evNewExit);
            IgnoreList.Add(EventCodes.evNewExpeditionCheckPoint);
            IgnoreList.Add(EventCodes.evNewRealEstate);
            IgnoreList.Add(EventCodes.evNewTreasureChest);
            IgnoreList.Add(EventCodes.evOtherGrabbedLoot);
            IgnoreList.Add(EventCodes.evPartyLootItemsRemoved);
            IgnoreList.Add(EventCodes.evPlayerCounts);
            IgnoreList.Add(EventCodes.evRealEstateListUpdate);
            IgnoreList.Add(EventCodes.evRegenerationCraftingChanged);
            IgnoreList.Add(EventCodes.evRegenerationEnergyChanged);
            IgnoreList.Add(EventCodes.evRegenerationHealthChanged);
            IgnoreList.Add(EventCodes.evRegenerationMountHealthChanged);
            IgnoreList.Add(EventCodes.evServerDebugLog);
            IgnoreList.Add(EventCodes.evSiegeCampClaimFinished);
            IgnoreList.Add(EventCodes.evSiegeCampClaimStart);
            IgnoreList.Add(EventCodes.evSiegeCampScheduleResult);
            IgnoreList.Add(EventCodes.evStopEmote);
            IgnoreList.Add(EventCodes.evTeleport);
            IgnoreList.Add(EventCodes.evTreasureChestUsingStart);
            IgnoreList.Add(EventCodes.evUpdateChatSettings);
            IgnoreList.Add(EventCodes.evUpdateHome);
         
            if (!IgnoreList.Contains(eventCode)){
                //Console.WriteLine(eventCode);
            }
            

            switch (eventCode)
            {
                /**
                case EventCodes.evPartyFinderJoinRequestDeclined:
                    Console.WriteLine("Party Finder");
                    onTestCase(parameters);
                    break;
                case EventCodes.evNewCharacter:
                    Console.WriteLine("New Character");
                    onTestCase(parameters);
                    break;
                case EventCodes.evUpdateFame:
                    Console.WriteLine("Update Fame");
                    onTestCase(parameters);
                    break;
                case EventCodes.evNewIslandAccessPoint:
                    Console.WriteLine("Access Point");
                    onTestCase(parameters);
                    break;
                 **/
                default:
                    break;
            }
        }

        private void onTestCase(Dictionary<byte, object> parameters)
        {
            foreach (KeyValuePair<byte, object> kvp in parameters)
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);


            String[] temp = (String[])parameters[0];

        }


        public void OnResponse(byte operationCode, short returnCode, Dictionary<byte, object> parameters)
        {
            Console.WriteLine("OnResponse: " + operationCode + " returnCode: " + returnCode);
            //foreach (String str in (String[])parameters[0])
            //{
            //    Console.WriteLine(str);
            //}
            foreach (KeyValuePair<byte, object> kvp in parameters)
            {
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                if(kvp.Value is String[])
                {
                    foreach (String str in (String[])kvp.Value)
                    {
                        Console.WriteLine(str);
                    }
                }


            }

            
        }
        public void OnRequest(byte operationCode, Dictionary<byte, object> parameters)
        {
            //OperationCodes code = (OperationCodes)parameters[253];
            int iCode = 0;
            if (!int.TryParse(parameters[253].ToString(), out iCode)) return;
            OperationCodes code = (OperationCodes)iCode;

            //Console.WriteLine("OnRequest: " + code);
        }

    }
}
