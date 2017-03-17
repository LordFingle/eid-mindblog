using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eiddata
{
    public static class DatumExtensions
    {
        public static Datum GetByKey(this ICollection<Datum> col, string key)
        {

            var items = (from i in col where i.Key == key select i);
            if (items.Count() < 1) return null;
            return items.First();

        }
        public static bool HasKey(this ICollection<Datum> col, string key)
        {

            return ((from i in col where i.Key == key select i).Count() > 0);
        }

        public static string GetKeyByType(this ICollection<Datum> col, DatumType datumType)
        {

            var items = (from i in col where i.DatumTypeCode == datumType.DatumTypeCode select i.Key);
            if (items.Count() < 1) return null;
            return items.First();

        }


    }
}
