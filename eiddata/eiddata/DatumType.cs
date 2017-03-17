using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace eiddata
{

    public class DatumType
    {
        public static DatumType Markdown
        {
            get
            {
                return new DatumType { DatumTypeCode = "MARKDOWN" };
            }
        }
 
        public static DatumType Text
        {
            get
            {
                return new DatumType { DatumTypeCode = "TEXT" };
            }
        }

        public static DatumType Image
        {
            get
            {
                return new DatumType { DatumTypeCode = "IMAGE" };
            }
        }
        public static DatumType ImageThumbnail
        {
            get
            {
                return new DatumType { DatumTypeCode = "THUMBNAIL" };
            }
        }
        public static GeoDatumType GeospacialCoordinates
        {
            get
            {
                return new GeoDatumType { DatumTypeCode = "GEO" };
            }
        }

        
 
        public string DatumTypeCode { get; set; }
 
        public string GenerateKey()
        {
            return DateTime.UtcNow.ToString("hhmmssfff");
        }

        public virtual string FromStream(Stream stream)
        {
            return (new StreamReader(stream).ReadToEnd());
        }

        public virtual Stream ToStream(string stringToPut)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(stringToPut));
        }


    }

    public class GeoDatumType : DatumType
    {

        public GeospatialCoordinates GeoFromStream(Stream stream)
        {

            return JsonConvert.DeserializeObject<GeospatialCoordinates>(base.FromStream(stream));
        }

        public Stream ToStream(GeospatialCoordinates coordinates)
        {

            return base.ToStream(JsonConvert.SerializeObject(coordinates));
       
        }
 
    }
    public class GeospatialCoordinates
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

}
