using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eiddata;
using System.Data.Entity;
using System.IO;

namespace eiddataentityframework
{


    public class ForgeDbContext : DbContext
    {
        public ForgeDbContext()
            : base("EntityFrameworkForgeStorage")
        {
        }

        public DbSet<Atom> Atoms { get; set; }
        public DbSet<Datum> Datum { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Datum>().HasKey(t => new { t.Id, t.Key});
        }
    }

    public class EntityFrameworkForgeStorage : IForgeDataAdapter
    {

        private ForgeDbContext db;
        private EntityAttachments att;

        public EntityFrameworkForgeStorage()
        {
             db = new ForgeDbContext();
             att = new EntityAttachments();
 
             var dataDirectory = (string)AppDomain.CurrentDomain.GetData("DataDirectory");
             att.AttachmentsFolderPath = Path.Combine(dataDirectory, "Attachments");
             db.Database.CreateIfNotExists();
             db.Database.Connection.Open();

        }


        public EntityFrameworkForgeStorage(bool init)
        {
            db = new ForgeDbContext();
            if (init)
            {
                db.Database.Delete();
                db.Database.Initialize(true);
            }
        }

        public IEnumerable<Atom> GetAtom(params string[] id)
        {
            
            return (from r in db.Atoms
                   where id.Contains(r.Id) 
                   select r).Include(b=>b.Data).ToList();
           
        }

        public IEnumerable<Atom> SearchAtoms(string searchText)
        {
            return (from r in db.Atoms
                   where r.Keywords.Contains(" " + searchText + " ")
                      && r.AtomTypeCode != "LN"
                   select r).ToList();  
        }

      

        public IEnumerable<Atom> GetNodeChildren(string id)
        {
            // Get Links
            var theLinks = (from r in db.Atoms
                           where r.FromId == id
                           && r.AtomTypeCode == "LN"
                           select r.ToId).ToArray();

            // Get matching nodes
            return this.GetAtom(theLinks);
        }

        public IEnumerable<Atom> GetNodeParents(string id)
        {
            // Get Links
            var theLinks = (from r in db.Atoms
                            where r.ToId == id
                             && r.AtomTypeCode == "LN"
                            select r.FromId).ToArray();
            // Return atoms for the links
            return this.GetAtom(theLinks);
        }

        public void DeleteLink(string fromAtomId, string toAtomId)
        {
            var theLinks = (from r in db.Atoms
                            where r.FromId == fromAtomId
                             && r.ToId == toAtomId
                             && r.AtomTypeCode == "LN"
                            select r).ToArray();
            
            db.Atoms.RemoveRange(theLinks);

        }

        public void Delete(params string[] id)
        {
            
            // Get Datum
            var datumToRemove = (from r in db.Datum
                                    where id.Contains(r.Id)
                                    select r).ToList();

            // Remove Associated Files
            foreach (var datum in datumToRemove)
            {
                this.DeleteDatum(datum.Id, datum.Key);
            }

            // Remove Datum
            db.Datum.RemoveRange(datumToRemove);

            // Get Atoms
            var theAtomsToRemove = (from r in db.Atoms
                            where id.Contains(r.ToId) 
                              || id.Contains(r.FromId)
                              || id.Contains(r.Id) 
                            select r).ToList();

            // Remove Atoms
            db.Atoms.RemoveRange(theAtomsToRemove);
            
            

        }

        public void Add(params Atom[] Atom)
        {
            db.Atoms.AddRange(Atom);
        }
        public void Update(params Atom[] Atom)
        {
            // Auto tracking should cover this?
             
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }
        public void DeleteDatum(string id, string key)
        {

            att.Delete(id, key);
            
        }
        public System.IO.Stream GetDatum(string id, string key)
        {

            return att.Get(id, key);
        }
 
        public void PutDatum(string id, string key, System.IO.Stream sourceStream)
        {
            att.Put(id, key, sourceStream);
             
        }


    }
}
