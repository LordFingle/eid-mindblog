using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace eiddata
{
    public class AtomType
    {

        public static AtomType Concept
        {
            get
            {
                return new AtomType { Prefix = "CN" };
            }
        }
        public static AtomType Link
        {
            get
            {
                return new AtomType { Prefix = "LN" };
            }
        }

        public static AtomTypeUnique Source
        {
            get
            {
                return new AtomTypeUnique { Prefix = "SO" };
            }
        }

        public static AtomTypeNotebook SourceNotebook
        {
            get
            {
                return new AtomTypeNotebook { Prefix = "SN" };
            }
        }

        public static AtomTypeInventory SourceInventory
        {
            get
            {
                return new AtomTypeInventory { Prefix = "SI" };
            }
        }

        public static AtomTypeTrash SourceTrash
        {
            get
            {
                return new AtomTypeTrash { Prefix = "ST" };
            }
        }



        public static AtomType Place
        {
            get
            {
                return new AtomType { Prefix = "RM" };
            }
        }

        public static AtomTypeLocation Location
        {
            get
            {
                return new AtomTypeLocation { Prefix = "PL" };
            }
        }

        public static AtomTypeDay Day
        {
            get
            {
                return new AtomTypeDay { Prefix = "DY" };
            }
        }

        public static AtomTypeKeystone DayKeystone
        {
            get
            {
                return new AtomTypeKeystone { Prefix = "KD" };
            }
        }
        public static AtomTypeKeystone LocationKeystone
        {
            get
            {
                return new AtomTypeKeystone { Prefix = "KP" };
            }
        }

        public static AtomTypeKeystone SourceKeystone
        {
            get
            {
                return new AtomTypeKeystone { Prefix = "KS" };
            }
        }




        public static AtomTypeKeystone PlaceKeystone
        {
            get
            {
                return new AtomTypeKeystone { Prefix = "KR" };
            }
        }

        internal string Prefix;

        public virtual string GenerateId()
        {
            return getGeneratedId(Prefix);
        }

        private string getGeneratedId(string idType)
        {
            return idType + Guid.NewGuid().ToString("N").ToUpper();
        }
        public virtual string GetName()
        {
            return "";
        }
    }

    public class AtomTypeNotebook : AtomTypeUnique
    {
        public override string GetName()
        {
            return "Notebook";
        }

    }
    public class AtomTypeTrash : AtomTypeUnique
    {
        public override string GetName()
        {
            return "Trash";
        }

    }
    public class AtomTypeInventory : AtomTypeUnique
    {
        public override string GetName()
        {
            return "Inventory";
        }

    }

    public class AtomTypeUnique : AtomType
    {
        public override string GetName()
        {
            return this.Id;
        }

        public string Id { get; set; }
        public override string GenerateId()
        {

            UTF8Encoding encoder = new UTF8Encoding();
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();

            byte[] hashedBytes = md5Hasher.ComputeHash(encoder.GetBytes(this.Id));

            StringBuilder sb = new StringBuilder(hashedBytes.Length * 2);
            for (int i = 0; i < hashedBytes.Length; i++)
            {
                sb.Append(hashedBytes[i].ToString("X2"));
            }
 
            var sanitisedId = sb.ToString().ToUpper();
             
            var id = this.Prefix + sanitisedId.PadRight(32, '0');
            return id.Substring(0, 34);
        }

        
    }


    public class AtomTypeDay : AtomType
    {
        public override string GetName()
        {
            return this.CurrentDateTime.ToLongDateString();
        }

        public DateTime CurrentDateTime { get; set; }
        public override string GenerateId()
        {
            return GenerateDay(CurrentDateTime);
        }

        private static string GenerateDay(DateTime theDay)
        {
            return String.Format("DY{0:0000}{1:00}{2:00}{3}",
                 theDay.Year, theDay.Month, theDay.Day,
                 "000000000000000000000000");
        }

    }

    public class AtomTypeLocation : AtomType
    {
        public override string GetName()
        {
            return String.Format("Earth : {0},{1}" , this.Latitude, this.Longitude);
        }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public override string GenerateId()
        { 
            return GeneratePlace(Latitude, Longitude);
        }
        private static string GeneratePlace(double latitude, double longitude)
        {
            //TODO: This is wrong at the moment
            return String.Format("PL{0:-0000X00000000}{1:-0000X00000000}",
                 latitude, longitude);
        }

    }
    public class AtomTypeKeystone : AtomType
    {

        public override string GenerateId()
        {
            return getFixedId(this.Prefix);
        }

        private static string getFixedId(string idType)
        {
            return idType + "00000000000000000000000000000000";
        }

       
    }

}
