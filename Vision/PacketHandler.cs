using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Vision.Mobs;

namespace Vision
{
    class PacketHandler : IPhotonPackageHandler
    {
        PlayerHandler playerHandler;
        HarvestableHandler harvestableHandler;
        MobsHandler mobsHandler;

        public PacketHandler(PlayerHandler playerHandler, HarvestableHandler harvestableHandler, MobsHandler mobsHandler)
        {
            this.playerHandler = playerHandler;
            this.harvestableHandler = harvestableHandler;
            this.mobsHandler = mobsHandler;
        }
        public void OnEvent(byte code, Dictionary<byte, object> parameters)
        {
            if (code == 2)
            {
                //player movement comes with binary format - not normal.
                onPlayerMovement(parameters);
                return;
            }

            object val;
            parameters.TryGetValue((byte)252, out val);
            if (val == null) return;

            int iCode = 0;
            if (!int.TryParse(val.ToString(), out iCode)) return;

            EventCodes eventCode = (EventCodes)iCode;

            List<EventCodes> ignoreList = new List<EventCodes>();
            /*
            ignoreList.Add(EventCodes.evKilledPlayer);
            ignoreList.Add(EventCodes.evSiegeCampClaimCancel);
            ignoreList.Add(EventCodes.evNewHarvestableObject);
            ignoreList.Add(EventCodes.evNewChatChannels);
            ignoreList.Add(EventCodes.evHarvestableChangeState);
            ignoreList.Add(EventCodes.evLeftChatChannel);
            ignoreList.Add(EventCodes.evNewTravelpoint);
            ignoreList.Add(EventCodes.evLeave);
            ignoreList.Add(EventCodes.evDebugDiminishingReturnInfo);
            ignoreList.Add(EventCodes.evNewOutpostObject);
            */
            //Console.WriteLine(eventCode);
            if (!ignoreList.Contains(eventCode)){
                //Console.WriteLine(eventCode);
            }

            switch (eventCode)
            {
                case EventCodes.evNewTravelpoint:
                    onTestCase(parameters);
                    break;
                case EventCodes.evHarvestableChangeState:
                    onHarvestableChangeState(parameters);
                    break;
                case EventCodes.evHarvestFinished:
                    onHarvestFinished(parameters);
                    break;
                case EventCodes.evNewCharacter:
                    onNewCharacterEvent(parameters);
                    break;
                case EventCodes.evNewHarvestableObject:
                    onNewHarvestableObject(parameters);
                    break;
                case EventCodes.evNewSimpleHarvestableObjectList:
                    onNewSimpleHarvestableObjectList(parameters);
                    break;
                case EventCodes.evLeave:
                    onLeave(parameters);
                    break;
                case EventCodes.evNewMob:
                    onNewMob(parameters);
                    break;
                case EventCodes.evJoinFinished:
                    onJoinFinished(parameters);
                    break;
                case EventCodes.evInCombatStateUpdate:
                    onInCombatStateUpdate(parameters);
                    break;
                case EventCodes.evCastSpell:
                    onCastSpell(parameters);
                    break;
                case EventCodes.evMobChangeState:
                    onMobChangeState(parameters);
                    break;
                case EventCodes.evUpdateMatchDetails:
                    onTestCase(parameters);
                    break;
                default:
                    break;
            }
        }

        private void onTestCase(Dictionary<byte, object> parameters)
        {
            foreach (KeyValuePair<byte, object> kvp in parameters)
                 Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            foreach (String str in (String[])parameters[4])
            {
                Console.WriteLine(str);
            }
            foreach (String str in (String[])parameters[5])
            {
                Console.WriteLine(str);
            }
            foreach (String str in (String[])parameters[6])
            {
                Console.WriteLine(str);
            }
        }

        private void onMobChangeState(Dictionary<byte, object> parameters)
        {
            int mobId = 0;
            byte enchantmentLevel = 0;

            if (!int.TryParse(parameters[0].ToString(), out mobId)) return;
            if (!byte.TryParse(parameters[1].ToString(), out enchantmentLevel)) return;
            mobsHandler.UpdateMobEnchantmentLevel(mobId, enchantmentLevel);

        }


        public void OnResponse(byte operationCode, short returnCode, Dictionary<byte, object> parameters)
        {
            //    Console.WriteLine("OnResponse: " + operationCode + " returnCode: " + returnCode);
        }
        public void OnRequest(byte operationCode, Dictionary<byte, object> parameters)
        {
            //OperationCodes code = (OperationCodes)parameters[253];
            int iCode = 0;
            if (!int.TryParse(parameters[253].ToString(), out iCode)) return;
            OperationCodes code = (OperationCodes)iCode;

            //Console.WriteLine("OnRequest: " + code);
            switch (code)
            {
                case OperationCodes.opMove:
                    onLocalPlayerMovement(parameters);
                    break;
            }
        }

        private void onCastSpell(Dictionary<byte, object> parameters)
        {
            //  foreach (KeyValuePair<byte, object> kvp in parameters)
            //       Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
        }

        private void onInCombatStateUpdate(Dictionary<byte, object> parameters)
        {
            // foreach (KeyValuePair<byte, object> kvp in parameters)
            //     Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
        }
        private void onJoinFinished(Dictionary<byte, object> parameters)
        {
            this.harvestableHandler.HarvestableList.Clear();
            this.mobsHandler.MobList.Clear();
            // this.playerHandler.PlayersInRange.Clear();
        }
        private void onNewMob(Dictionary<byte, object> parameters)
        {
            /*
                Rhino Data (4258 HP)
                Key = 0, Value = 12694, Type= System.Int16 // long Object Id ao5
                Key = 1, Value = 41, Type= System.Byte  // short Type Id ao6
                Key = 2, Value = 255, Type= System.Byte // Flagging status ao7
                                Blue = 0,
	                            Highland = 1,
	                            Forest = 2,
	                            Steppe = 3,
	                            Mountain = 4,
	                            Swamp = 5,
	                            Red = byte.MaxValue
                Key = 6, Value = , Type= System.String // apb?
                Key = 7, Value = System.Single[], Type= System.Single[] // Pos // arg apc
                Key = 8, Value = System.Single[], Type= System.Single[] // Pos Target // arg apd
                Key = 9, Value = 26835839, Type= System.Int32 // GameTimeStamp ape
                Key = 10, Value = 171.1836, Type= System.Single // apf (float)
                Key = 11, Value = 2, Type= System.Single // apg (float)
                Key = 13, Value = 4258, Type= System.Single // Health api (float)
                Key = 14, Value = 4258, Type= System.Single // apj (float)
                Key = 16, Value = 26665619, Type= System.Int32 // GameTimeStamp app
                Key = 17, Value = 245, Type= System.Single // float apm
                Key = 18, Value = 245, Type= System.Single // float apn
                Key = 19, Value = 7, Type= System.Single // float apo
                Key = 20, Value = 26835811, Type= System.Int32 // GameTimeStamp app
                Key = 252, Value = 106, Type= System.Int16
             */


            int id = int.Parse(parameters[0].ToString());
            int typeId = int.Parse(parameters[1].ToString());
            Single[] loc = (Single[])parameters[8];
            // Console.WriteLine("Loc Locs: " + loc.Length);
            DateTime timeA = new DateTime(long.Parse(parameters[9].ToString()));
            DateTime timeB = new DateTime(long.Parse(parameters[16].ToString()));
            DateTime timeC = new DateTime(long.Parse(parameters[20].ToString()));
            Single posX = (Single)loc[0];
            Single posY = (Single)loc[1];
            int health = int.Parse(parameters[13].ToString());
            int rarity = int.Parse(parameters[20].ToString());

            mobsHandler.AddMob(id, typeId, posX, posY, health);
        }
        private void onNewSimpleHarvestableObjectList(Dictionary<byte, object> parameters)
        {
            //return;
            // foreach (KeyValuePair<byte, object> kvp in parameters)
            //    Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);

            List<int> a0 = new List<int>();
            if (parameters[0].GetType() == typeof(Byte[]))
            {
                Byte[] typeListByte = (Byte[])parameters[0]; //list of types
                foreach (Byte b in typeListByte)
                    a0.Add(b);
            }
            else if (parameters[0].GetType() == typeof(Int16[]))
            {
                Int16[] typeListByte = (Int16[])parameters[0]; //list of types
                foreach (Int16 b in typeListByte)
                    a0.Add(b);
            }
            else
            {
                Console.WriteLine("onNewSimpleHarvestableObjectList type error: " + parameters[0].GetType());
                return;
            }
            try
            {
                /*
                Key = 0, Value = System.Int16[] //id
                Key = 1, Value = System.Byte[] // type WOOD etc
                Key = 2, Value = System.Byte[] // tier
                Key = 3, Value = System.Single[] //location
                Key = 4, Value = System.Byte[] // size
                Key = 252, Value = 29
                 */
                Byte[] a1 = (Byte[])parameters[1]; //list of types
                Byte[] a2 = (Byte[])parameters[2]; //list of tiers
                Single[] a3 = (Single[])parameters[3]; //list of positions X1, Y1, X2, Y2 ...
                Byte[] a4 = (Byte[])parameters[4]; //size

                for (int i = 0; i < a0.Count; i++)
                {
                    int id = (int)a0.ElementAt(i);
                    byte type = (byte)a1[i];
                    byte tier = (byte)a2[i];
                    Single posX = (Single)a3[i * 2];
                    Single posY = (Single)a3[i * 2 + 1];
                    Byte count = (byte)a4[i];
                    byte charges = (byte)0;
                    harvestableHandler.AddHarvestable(id, type, tier, posX, posY, charges, count);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("eL: " + e.ToString());
            }
        }
        private void onNewHarvestableObject(Dictionary<byte, object> parameters)
        {
            // Console.WriteLine("onNewHarvestableObject");
            //  foreach (KeyValuePair<byte, object> kvp in parameters)
            //        Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            /*
                //Key = 10, Value = 2 //count. If not set its empty
                Key = 0, Value = 7589
                Key = 2, Value = 636712223127023853
                Key = 5, Value = 11
                Key = 6, Value = -1
                Key = 7, Value = 6
                Key = 8, Value = System.Single[]
                Key = 9, Value = 270
                Key = 11, Value = 1
                Key = 252, Value = 30
             */
            int id = 0;
            byte type = 0;
            byte tier = 0;
            try
            {
                id = int.Parse(parameters[0].ToString());
                type = byte.Parse(parameters[5].ToString()); //TODO - check if 5 is type
                tier = byte.Parse(parameters[7].ToString()); //Tier
            }
            catch(Exception e)
            {
                Console.WriteLine("Unparseable Harvestable Object Found");
                return;
            }
            Single[] loc = (Single[])parameters[8];
            Single posX = (Single)loc[0];
            Single posY = (Single)loc[1];
            byte charges = 0;
            byte size = 0;


            if (!byte.TryParse(parameters[10].ToString(), out size))
                size = 0; //nothink in stack

            if (!byte.TryParse(parameters[11].ToString(), out charges))
                charges = 0; // charge 

            harvestableHandler.AddHarvestable(id, type, tier, posX, posY, charges, size);
        }
        private void onHarvestFinished(Dictionary<byte, object> parameters)
        {//
         // foreach (KeyValuePair<byte, object> kvp in parameters)
         //     Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            int harvestableId = 0;
            try
            {
                harvestableId = int.Parse(parameters[3].ToString());
                //Int32 count = Int32.Parse(parameters[2].ToString());
            }
            catch(Exception e)
            {
                Console.WriteLine("Error updating harvest status");
                return;
            }
            
            harvestableHandler.RemoveHarvestable(harvestableId);
            //harvestableHandler.UpdateHarvestable(harvestableId, count);
        }
        private void onHarvestableChangeState(Dictionary<byte, object> parameters)
        {
            /*
             onHarvestableChangeState
                Key = 0, Value = 5803
                Key = 1, Value = 2 //how much more to mine
                Key = 2, Value = 1 //tier
                Key = 252, Value = 33
             */
            // Console.WriteLine("onHarvestableChangeState");
            //foreach (KeyValuePair<byte, object> kvp in parameters)
            //    Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
        }
        private void onLeave(Dictionary<byte, object> parameters)
        {

            /*
             onLeave contains strange data. It should delete Harvestable + Players + Monsters. But its sketchy.
             */
            int id = int.Parse(parameters[0].ToString());
            //  if (harvestableHandler.RemoveHarvestable(id))
            //      Console.WriteLine("Removed harvestable: " + id);
            //  else 
            if (playerHandler.RemovePlayer(id))
                playerHandler.RemovePlayer(id);
            //Console.WriteLine("Removed player: " + id);
            // else
            //     Console.WriteLine("None removed: " + id);

        }
        private void onLocalPlayerMovement(Dictionary<byte, object> parameters)
        {
            Single[] location = (Single[])parameters[1]; //if we switch to [3] we will have future position of player instead of 'right now'
            Single posX = Single.Parse(location[0].ToString());
            Single posY = Single.Parse(location[1].ToString());
            // Console.WriteLine("onLocalPlayerMovement: " +posX + " " + posY);
            // 373,6958 -358,3227 top of map
            //-375,2436 366,6795 bottom of map

            playerHandler.UpdateLocalPlayerPosition(posX, posY);
            //Console.WriteLine("Player Found Local Movement");
        }
        private void onPlayerMovement(Dictionary<byte, object> parameters)
        {
            int id = int.Parse(parameters[0].ToString());
            Byte[] a = (Byte[])parameters[1];
            //string nick = parameters[1].ToString();
            //string nick = Encoding.Default.GetString(parameters[1]);
     

            Single posX = BitConverter.ToSingle(a, 9);
            Single posY = BitConverter.ToSingle(a, 13);

            if (id == 539)
            {
                //Console.WriteLine(id + ": (" + posX + "," + posY);
            }
            
            //Console.WriteLine("X:" + posX + " Y:" + posY);
            playerHandler.UpdatePlayerPosition(id, posX, posY);
            if (playerHandler.inPlayersInRange(id))
            {
                playerHandler.UpdatePlayerPosition(id, posX, posY);
            }

            else
            {
                //new Thread(() => Console.Beep(1000, 1000)).Start();
                playerHandler.AddPlayer(40,40, "nicky", "peepo", "cheeko", 5);
            }
            
            //Console.WriteLine("Player Found Movement");
        
        }
        private void onNewCharacterEvent(Dictionary<byte, object> parameters)
        {

            if (Settings.PlaySoundOnPlayer())
                new Thread(() => Console.Beep(1000, 1000)).Start();


            int id = int.Parse(parameters[0].ToString());
            string nick = parameters[1].ToString();
            object oGuild = "";

            //parameters.TryGetValue((byte)8, out oGuild);
            //string guild = oGuild == null ? "" : oGuild.ToString();

            //string guild = parameters[8].ToString() || null;
            //string alliance = parameters[44].ToString();

            Single[] a13 = (Single[])parameters[13]; //pos1
            //Console.WriteLine("Player Found Char event");

            playerHandler.AddPlayer(a13[0], a13[1], nick, "peepo", "cheeko", id);
        }
    }
}
