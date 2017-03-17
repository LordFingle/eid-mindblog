using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eiddata
{
    public class StandardDataInit
    {
        public static Atom Init(Forge forge)
        {

            // create the keystones  
            var root = forge.LoadOrCreate(AtomType.PlaceKeystone, "Home");
            forge.SaveChanges();
          
            var days = forge.LoadOrCreate(AtomType.DayKeystone, "Days");
            var locations = forge.LoadOrCreate(AtomType.LocationKeystone, "Locations");
            var sources = forge.LoadOrCreate(AtomType.SourceKeystone, "Sources");
            forge.SaveChanges();

            // Root points to the other keystones
            forge.AddLink(root, new[] { days, locations, sources });

          
            forge.SaveChanges();

            return root;

        }
    }
}
