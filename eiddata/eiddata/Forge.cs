using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace eiddata
{
    

    public class Forge
    {

        private IForgeDataAdapter data;

        public Forge(IForgeDataAdapter adapt)
        {
            data = adapt;
                
        }

        public void Delete(params string[] ids)
        {


             data.Delete(ids);
        }

        public Atom GetSingleAtom(params string[] ids)
        {
            var atoms = this.GetAtom(ids);
            if (atoms.Count() > 0) return atoms.Single();
            return null;
        }

        public IEnumerable<Atom> GetAtom(params string[] ids)
        {
            return data.GetAtom(ids);
        }


        public IEnumerable<Atom> GetParents(Atom anAtom)
        {
            return data.GetNodeParents(anAtom.Id);
        }


        public IEnumerable<Atom> GetChildren(Atom anAtom)
        {
            return data.GetNodeChildren(anAtom.Id);
        }

        public Atom LoadOrCreate(AtomType atomType, Atom parentAtom)
        {
            return LoadOrCreate(atomType, atomType.GenerateId(), atomType.GetName(),parentAtom);
        }

        public Atom LoadOrCreate(AtomTypeKeystone atomType, string name)
        {
            return LoadOrCreate(atomType, atomType.GenerateId(), name);
        }

        public Atom LoadOrCreate(string id, string name)
        {
            return LoadOrCreate(AtomType.Concept, id, name);
        }

        public Atom LoadOrCreate(AtomType atomType, string id, string name)
        {
   
            return LoadOrCreate(atomType, id,  name, null);

        }

        public Atom LoadOrCreate(AtomType atomType, string id, string name, Atom parentAtom)
        {
            var nodes = data.GetAtom(id);
            if (nodes.Count() > 0) return nodes.First();

            var atom = Add(atomType, id, name);
            if (parentAtom != null)
            {
                this.AddLink(parentAtom, atom);
            }
            return atom;

        }


        public Atom Add(AtomType atomtype, string name)
        {
            
            return Add(atomtype,atomtype.GenerateId(), name);
             
        }
        public Atom Add(AtomType atomtype, string id,  string name)
        {
            var n = new Atom();
            n.Name = name;
            n.CreateDateTime = DateTime.UtcNow;
            n.LastModDateTime = n.CreateDateTime;
            n.Id = id;
            n.AtomTypeCode = atomtype.Prefix;
           
            data.Add(n);
           
            return n;
        }

        public void AddLink(IEnumerable<Atom> fromAtoms, Atom toAtom)
        {
            // Get Parents
            var existingParents = data.GetNodeParents(toAtom.Id).ToDictionary(x=>x.FromId,y=>y);

            // Add relationships that dont already exist
            foreach (var fromAtom in fromAtoms)
            {
                if (!existingParents.ContainsKey(fromAtom.Id))
                {
                    AddLink(fromAtom, toAtom);
                }
            }
 
        }

        public void AddLink(Atom fromAtom, IEnumerable<Atom> toAtoms)
        {
            // Get Parents
            var existingChildren = data.GetNodeChildren(fromAtom.Id).ToDictionary(x => x.ToId, y => y);

            // Add relationships that dont already exist
            foreach (var toAtom in toAtoms)
            {
                if (!existingChildren.ContainsKey(toAtom.Id))
                {
                    AddLink(fromAtom, toAtom);
                }
            }

        }

        public Atom AddLink(Atom fromAtom, Atom toAtom)
        {
            var l = this.Add(AtomType.Link,"");
            l.Id = AtomType.Link.GenerateId();
            l.FromId = fromAtom.Id;
            l.ToId = toAtom.Id;
        

            data.Update(l);
      
            return l;
        }

        public void DeleteLink(Atom fromAtom, Atom toAtom)
        {

            data.DeleteLink(fromAtom.Id, toAtom.Id);
   
        }


        public Datum GetDatum(Atom atom, string key)
        {
            var findKey = from a in atom.Data
                          where a.Key == key
                          select a;

            if (findKey.Count() < 1) throw new ApplicationException("Datum not found");

            return findKey.First(); 
        }


        public Stream GetDatumStream(Atom atom, string key)
        {
            var findKey = from a in atom.Data
                          where a.Key == key
                          select a;

            if (findKey.Count() < 1) return null;

            return data.GetDatum(atom.Id, key);
        }
        public void DeleteDatum(Atom atom, string key)
        {
            var findKey = from a in atom.Data
                          where a.Key == key
                          select a;

            if (findKey.Count() < 1) throw new ApplicationException("Datum not found");

            atom.Data.Remove(findKey.First());
            data.DeleteDatum(atom.Id, key);
        }
    


        public string PutDatum(Atom atom, Stream sourceStream)
        {
            return PutDatum(atom, DatumType.Text, null,  atom.Name, sourceStream);
        }

        public string PutDatum(Atom atom, DatumType datumType, string key, string name, Stream sourceStream)
        {
            // Passing a null key means it's a new item
            if (key == null) key = datumType.GenerateKey();

            // Update the Data collection
            var findit = atom.Data.GetByKey(key);
            
            if (findit == null)
            { 
                atom.AddDatum(new Datum
                {
                    Id = atom.Id,
                    Key = key,
                    DatumTypeCode = datumType.DatumTypeCode,
                    Name = name,
                });
            }
            else
            { 
                key = findit.Key;
                findit.Name = name;
                findit.DatumTypeCode = datumType.DatumTypeCode;
            }

            // Write out datum
            data.PutDatum(atom.Id, key, sourceStream);

            return key;
        }

        

        public void SaveChanges()
        {
            data.SaveChanges();
        }
         
    }

    

}
