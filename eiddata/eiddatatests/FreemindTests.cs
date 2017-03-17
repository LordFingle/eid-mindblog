using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using eiddata;
using Moq;
using eiddataentityframework;
using System.IO;
using System.Xml.Linq;
using System.Linq;
namespace eiddatatests
{

    [TestClass]
    public class FreemindTests
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
        public void ImportMindmap_NoErrors()
        {
            var someParent = forge.Add(AtomType.Concept,"Generic Parent");
            var someRoot = forge.Add(AtomType.Concept,"Generic Root");
            var freemind = new Freemind(forge, new[] { someParent });
            freemind.Import(someRoot, new MemoryStream(eiddatatests.Properties.Resources.eid));

            var doc = XDocument.Load(freemind.Export(someRoot));
            
            Assert.AreEqual("Generic Root", doc.Root.Element("node").Attribute("TEXT").Value);            
            var eidRoot = from n in doc.Root.Element("node").Descendants("node")
                          where n.Attribute("TEXT").Value == "eid"
                          select n;

            Assert.AreEqual(1, eidRoot.Count());            
        }

        [TestMethod]
        public void ExportMindmap_NoErrors()
        {
            var someParent = forge.Add(AtomType.Concept, "Generic Parent");
            var someRoot = forge.Add(AtomType.Concept, "Generic Root");
            var freemind = new Freemind(forge, new[] { someParent });
            freemind.Import(someRoot, new MemoryStream(eiddatatests.Properties.Resources.eid));
            

 
        }
    }
}
