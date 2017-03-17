using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
 
namespace eiddata
{
    public class Atom
    {
     
        /// <summary>
        /// Globally unique identifier
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Textual type
        /// </summary>
        public string AtomTypeCode { get; set; }

        /// <summary>
        /// UTC Date time this atom was created
        /// </summary>
        public DateTime CreateDateTime { get; set; }
        
        /// <summary>
        /// UTC date time this atom was last update
        /// </summary>
        public DateTime LastModDateTime { get; set; }

        /// <summary>
        /// Used for search
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// For atoms that utilise a name
        /// </summary>
        private string _name;
        public string Name {
            get { return _name; } 
            set {
                _name = value;
                UpdateKeywords(_name);
            } 
        }

        public void UpdateKeywords(string someKeywords)
        {
            if (String.IsNullOrEmpty(this.Keywords)) this.Keywords = "";

            // Add name to keywords
            if (!this.Keywords.Contains(someKeywords))
                this.Keywords = this.Keywords + " " + someKeywords;
        }

        /// <summary>
        /// If this is a link, where the link originates
        /// </summary>
        public string FromId { get; set; }

        /// <summary>
        /// If this is a link, where the link goes to
        /// </summary>
        public string ToId { get; set; }

        /// <summary>
        /// Data streams
        /// </summary>
        private ICollection<Datum> _data;
        public ICollection<Datum> Data
        {
            get { return _data ?? (_data = new Collection<Datum>()); }
            set { _data = value; }
        }

        public void AddDatum(Datum datum)
        {
            datum.Id = this.Id;
            _data.Add(datum);
        }

    }
     
    public class Datum
    {
        
        public string Id { get; set; }      
        public string Key { get; set; }

        // Human readable name
        public string Name { get; set; }
        /// <summary>
        /// Textual type
        /// </summary>
        public string DatumTypeCode { get; set; }
    }

}
