using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using eiddata;
using Moq;
using System.Linq;
using eiddataentityframework;
using System.IO;
using System.Text;
namespace eiddatatests
{
    [TestClass]
    public class EntityFrameworkForgeTests
    {


        private Forge forge;  
        [TestInitialize]
        public void Setup()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", System.IO.Directory.GetCurrentDirectory());

            var storage = new EntityFrameworkForgeStorage(true);
      
            forge = new Forge(storage);

        }


        [TestMethod]
        public void AddTwoTextAttachments_NoErrors()
        {
  
            var forge = new Forge(new EntityFrameworkForgeStorage());
            var h1 = forge.Add(AtomType.Concept, "ConceptAtom1");
            var key = forge.PutDatum(h1, new MemoryStream(Encoding.UTF8.GetBytes("Hello World" ?? "")));

            forge.SaveChanges();
            var retrievedAtom = forge.GetAtom(h1.Id).Single();
            var key2 = forge.PutDatum(h1, new MemoryStream(Encoding.UTF8.GetBytes("Hello You" ?? "")));
            forge.SaveChanges();

      
 
            Assert.AreEqual(2, retrievedAtom.Data.Count);
            Assert.AreEqual("Hello World", new StreamReader(forge.GetDatumStream(retrievedAtom, key)).ReadToEnd());
            Assert.AreEqual("Hello You", new StreamReader(forge.GetDatumStream(retrievedAtom, key2)).ReadToEnd());



        }
        [TestMethod]
        public void AddMarkdownAttachment_NoErrors()
        {

            var forge = new Forge(new EntityFrameworkForgeStorage());
            var h1 = forge.Add(AtomType.Concept, "ConceptAtom1");
            var key = forge.PutDatum(h1, DatumType.Markdown,null,  "", new MemoryStream(Encoding.UTF8.GetBytes("Hello World" ?? "")));

            forge.SaveChanges();
            var retrievedAtom = forge.GetAtom(h1.Id).Single();
            Assert.AreEqual("Hello World", new StreamReader(forge.GetDatumStream(retrievedAtom, key)).ReadToEnd());
            forge.SaveChanges();
 

        }


        [TestMethod]
        public void StoreTextAttachment_NoErrors()
        {

            var forge = new Forge(new EntityFrameworkForgeStorage());
            var h1 = forge.Add(AtomType.Concept, "ConceptAtom1");
            var key = forge.PutDatum(h1, new MemoryStream(Encoding.UTF8.GetBytes("Hello World" ?? "")));

            forge.SaveChanges();

            // Assert
            var retrievedAtom = forge.GetAtom(h1.Id).Single();

            Assert.AreEqual(1, retrievedAtom.Data.Count);
            var readString = new StreamReader(forge.GetDatumStream(retrievedAtom, key)).ReadToEnd();
            Assert.AreEqual("Hello World", readString);
        }

        [TestMethod]
        public void RemoveTextAttachment_NoErrors()
        {

            var forge = new Forge(new EntityFrameworkForgeStorage());
            var h1 = forge.Add(AtomType.Concept, "ConceptAtom1");
            var key = forge.PutDatum(h1, new MemoryStream(Encoding.UTF8.GetBytes("Hello World" ?? "")));

            forge.SaveChanges();

            // Assert
            var retrievedAtom = forge.GetAtom(h1.Id).Single();

            Assert.AreEqual(1, retrievedAtom.Data.Count);
            var readString = new StreamReader(forge.GetDatumStream(retrievedAtom, key)).ReadToEnd();
            Assert.AreEqual("Hello World", readString);
 
            // Perform delete
            forge.DeleteDatum(retrievedAtom, key);
            forge.SaveChanges();

            // Check
            var retrievedAtom2 = forge.GetAtom(h1.Id).Single();

            Assert.AreEqual(0, retrievedAtom2.Data.Count);




        }
        [TestMethod]
        public void AddAAtom_NoErrors()
        {

            //AtomSpace* atomSpace = CogServer::getAtomSpace();
            //SimpleTruthValue tv1(0.5f, 0.99f);
            //Handle h1 = atomSpace->addAtom(CONCEPT_Atom, "ConceptAtom1", tv1);

            var forge = new Forge(new EntityFrameworkForgeStorage());
            var h1 = forge.Add(AtomType.Concept, "ConceptAtom1");
            forge.SaveChanges();

            // Assert
            var checkh1 = forge.GetAtom(h1.Id);
            Assert.AreEqual<string>(h1.Name, checkh1.First().Name);
        }


        [TestMethod]
        public void UpdateAtomName_NoErrors()
        {

            //AtomSpace* atomSpace = CogServer::getAtomSpace();
            //SimpleTruthValue tv1(0.5f, 0.99f);
            //Handle h1 = atomSpace->addAtom(CONCEPT_Atom, "ConceptAtom1", tv1);

            var forge = new Forge(new EntityFrameworkForgeStorage());
            var h1 = forge.Add(AtomType.Concept, "ConceptAtom1");
            forge.SaveChanges();
 
            var h2 = forge.GetAtom(h1.Id).First();
            Assert.AreEqual<string>(h1.Name, h2.Name);
            h2.Name = "Commando";
            forge.SaveChanges();

            var h3 = forge.GetAtom(h1.Id).First();
            Assert.AreEqual("Commando", h3.Name);
        }


        [TestMethod]
        public void SimpleDelete_NoErrors()
        {

            // Setup

       
            var h1 = forge.Add(AtomType.Concept, "ConceptAtom1");
            forge.SaveChanges();
 
            var retAtom = forge.GetAtom(h1.Id).Single();

            // Delete
            forge.Delete(retAtom.Id);

            // See if result is still there prior to commit
            var checkAtom = forge.GetAtom(h1.Id).Single();
            Assert.AreEqual("ConceptAtom1", checkAtom.Name);

            // Commit
            forge.SaveChanges();

            //Assert
            var results = forge.GetAtom(h1.Id);
            Assert.AreEqual(0, results.Count());
        }

        [TestMethod]
        public void DeleteWithLinks_NoErrors()
        {

            // Setup
            var forge = new Forge(new EntityFrameworkForgeStorage());
            var h1 = forge.Add(AtomType.Concept, "ConceptAtom1");
            var h2 = forge.Add(AtomType.Concept, "ConceptAtom2");
            var link = forge.AddLink(h1, h2);
            forge.SaveChanges();

            var retAtom = forge.GetAtom(h1.Id).Single();
            var childitems = forge.GetChildren(h1);
            Assert.AreEqual(1, childitems.Count());

            // Delete
            forge.Delete(retAtom.Id);

            // See if result is still there prior to commit
            var checkAtom = forge.GetAtom(h1.Id).Single();
            Assert.AreEqual("ConceptAtom1", checkAtom.Name);
            Assert.AreEqual(1, childitems.Count());

            // Commit
            forge.SaveChanges();

            // Assert
            var results = forge.GetAtom(h1.Id);
            Assert.AreEqual(0, results.Count());

            // Try and get children

            var childitemscheck = forge.GetChildren(h1);
            Assert.AreEqual(0, childitemscheck.Count());
 
        }


        [TestMethod]
        public void DeleteWithAttachments_NoErrors()
        {

            // Setup
            var forge = new Forge(new EntityFrameworkForgeStorage());
            var h1 = forge.Add(AtomType.Concept, "ConceptAtom1");    
            var key = forge.PutDatum(h1, new MemoryStream(Encoding.UTF8.GetBytes("Hello World" ?? "")));

            forge.SaveChanges();

            // Assert
            var retrievedAtom = forge.GetAtom(h1.Id).Single();
            Assert.AreEqual(1, retrievedAtom.Data.Count);
            var readString = new StreamReader(forge.GetDatumStream(retrievedAtom, key)).ReadToEnd();
            Assert.AreEqual("Hello World", readString);
  

            var retAtom = forge.GetAtom(h1.Id).Single();
      
        

            // Delete
            forge.Delete(retAtom.Id);

            // See if result is still there prior to commit
            var checkAtom = forge.GetAtom(h1.Id).Single();
            Assert.AreEqual("ConceptAtom1", checkAtom.Name);
           
            // Commit
            forge.SaveChanges();

            // Assert
            var results = forge.GetAtom(h1.Id);
            Assert.AreEqual(0, results.Count());

            // Try and get attachments
            Assert.IsNull(forge.GetDatumStream(h1, key));

            try
            {
                 forge.GetDatum(h1, key);
            }
            catch (ApplicationException e)
            {
                // Just trapping the right exception
                Assert.IsInstanceOfType(e, typeof(ApplicationException));
            }
            

        }

        [TestMethod]
        public void AddALink_NoErrors()
        {

            //AtomSpace* atomSpace = CogServer::getAtomSpace();
            //SimpleTruthValue tv1(0.5f, 0.99f);
            //Handle h1 = atomSpace->addAtom(CONCEPT_Atom, "ConceptAtom1", tv1);
            //Handle h2 = atomSpace->addAtom(CONCEPT_Atom, "ConceptAtom2", tv1);  
            //Handle link = atomSpace->addLink(INHERITANCE_LINK, h1, h2, tv1);
            var forge = new Forge(new EntityFrameworkForgeStorage());
            var h1 = forge.Add(AtomType.Concept,"ConceptAtom1");
            var h2 = forge.Add(AtomType.Concept, "ConceptAtom2");
            var link = forge.AddLink(h1, h2);
            forge.SaveChanges();
            
            // Assert
            Assert.AreEqual<int>(1, forge.GetChildren(h1).Count());
            Assert.AreEqual<int>(0, forge.GetChildren(h2).Count());
            Assert.AreEqual<int>(0, forge.GetParents(h1).Count());
            Assert.AreEqual<int>(1, forge.GetParents(h2).Count());

            Assert.AreEqual(h2.Id, forge.GetChildren(h1).First().Id);
        }
    }
}
