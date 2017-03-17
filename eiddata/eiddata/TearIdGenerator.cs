using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eiddata
{
    public class NodeIdGenerator
    {

        public static string GetSourceKeystone()
        {
            return getFixedId("KS");
        }
        public static string GetDaysKeystone()
        {
            return getFixedId("KD");
        }

        public static string GetPlacesKeystone()
        {
            return getFixedId("KP");
        }

        private static string getFixedId(string idType)
        {
            return idType + "00000000000000000000000000000000";
        }

        private static string getGeneratedId(string idType)
        {
            return idType + Guid.NewGuid().ToString("N");
        }

        public static string GenerateGenericNodeId()
        {
            // 34 char identifier, including 2 char type id
            return getGeneratedId("DS");
        }
        public static string Generate()
        {
            // 34 char identifier, including 2 char type id

            return getGeneratedId("GN");
        }

        public static string GenerateLinkId()
        {
            // 34 char identifier, including 2 char type id

            return getGeneratedId("LN");
        }


        public static string GenerateHumanSourceNodeId()
        {
            // 34 char identifier, including 2 char type id

            return getGeneratedId("SH");
        }

        public static string GenerateSourceNodeId()
        {
            // 34 char identifier, including 2 char type id

            return getGeneratedId("SO");
        }

        public static string GeneratePlace(double latitude, double longitude)
        {
            //TODO: This is wrong at the moment
            return String.Format("PL{0:-0000.00000000}{1:-0000.00000000}",
                 latitude, longitude);
        }

        public static string GenerateDay(DateTime theDay)
        {
            return String.Format("DY{0:0000}{1:00}{2:00}{3}",
                 theDay.Year, theDay.Month, theDay.Day,
                 "000000000000000000000000");
        }




    }
}
