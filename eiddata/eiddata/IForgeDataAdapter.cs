using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace eiddata
{
    public interface IForgeDataAdapter
    {
        IEnumerable<Atom> GetAtom(params string[] id);
        IEnumerable<Atom> SearchAtoms(string searchString);
 
        IEnumerable<Atom> GetNodeChildren(string id);
        IEnumerable<Atom> GetNodeParents(string id);

        void DeleteLink(string fromAtomId, string toAtomId);

        void Delete(params string[] id);
        
        void Add(params Atom[] Atom);
        void Update(params Atom[] Atom);
  
        void SaveChanges();

        // Attachments
        Stream GetDatum(string id, string key);
        void DeleteDatum(string id, string key);    
        void PutDatum(string id, string key, Stream sourceStream);

    }
}
