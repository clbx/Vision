using Vision.Mobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision
{
    public static class Settings
    {
        static List<String> tiers = new List<String>();
        static List<HarvestableType> harvestable = new List<HarvestableType>();
        static List<MobType> mobs = new List<MobType>();
        static bool onlyRares = false;
        static bool displayPeople = true;
        static bool soundsOnPlayer = false;

        public static void saveSettings(Form1 form){
            AppSettings s = new AppSettings();

           

        }
        public static void loadSettings(Form1 form)
        {
            AppSettings s = new AppSettings();
        }
        public static bool DisplayPeople
        {
            get { return Settings.displayPeople; }
            set { Settings.displayPeople = value; }
        }

        public static bool PlaySoundOnPlayer()
        {
            return soundsOnPlayer;
        }
        public static void UpdateDisplayPeople(bool val)
        {
            displayPeople = val;
        }
        public static bool IsInTiers(byte tier, byte enchant)
        {
            return tiers.Contains(tier+"."+enchant);
        }
        public static bool IsInMobs(MobType mobType)
        {
            return mobs.Contains(mobType);
        }
        public static bool OnlyRares()
        {
            return onlyRares;
        }
        public static bool IsInHarvestable(HarvestableType ht)
        {
            return harvestable.Contains(ht);
        }
        
        public static void UpdateTier(int tier, byte enchant, bool show)
        {
            byte bTier = (byte) tier;
            String iString = tier + "." + enchant;
            if(show){
                if (!tiers.Contains(iString))
                    tiers.Add(iString);
            }else{
                if (tiers.Contains(iString))
                    tiers.Remove(iString);
            }
        }

        public static void UpdateHarvestable(List<HarvestableType> h, bool show)
        {
            if(show){
                foreach(HarvestableType ht in h)
                    if (!harvestable.Contains(ht)) 
                        harvestable.Add(ht);
            }
            else
            {
                foreach (HarvestableType ht in h)
                    if (harvestable.Contains(ht)) 
                        harvestable.Remove(ht);
            }
        }

        public static void UpdateHarvestableMob(MobType h, bool show)
        {
            if (show)
            {
                if (!mobs.Contains(h))
                    mobs.Add(h);
            }
            else
            {
                if (mobs.Contains(h))
                    mobs.Remove(h);
            }
        }

        internal static void UpdateOnlyRares(bool raresOnly)
        {
            onlyRares = raresOnly;
        }

        internal static void setSoundsOnPlayer(bool p)
        {
            soundsOnPlayer = p;
        }

    }
}
