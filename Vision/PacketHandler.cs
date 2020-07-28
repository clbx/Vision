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
        }

    }
}
