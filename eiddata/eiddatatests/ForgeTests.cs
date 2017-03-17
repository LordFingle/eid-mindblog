using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using eiddata;
using Moq;
namespace eiddatatests
{

    [TestClass]
    public class ForgeTests
    {
        [TestMethod]
        public void AddAAtom_NoErrors()
        {

  //AtomSpace* atomSpace = CogServer::getAtomSpace();
  //SimpleTruthValue tv1(0.5f, 0.99f);
  //Handle h1 = atomSpace->addAtom(CONCEPT_Atom, "ConceptAtom1", tv1);

            var mock = new Mock<IForgeDataAdapter>();
          
            var forge = new Forge(mock.Object);
            forge.Add(AtomType.Concept,"ConceptAtom1");
        }

        [TestMethod]
        public void AddALink_NoErrors()
        {

  //AtomSpace* atomSpace = CogServer::getAtomSpace();
  //SimpleTruthValue tv1(0.5f, 0.99f);
  //Handle h1 = atomSpace->addAtom(CONCEPT_Atom, "ConceptAtom1", tv1);
  //Handle h2 = atomSpace->addAtom(CONCEPT_Atom, "ConceptAtom2", tv1);  
  //Handle link = atomSpace->addLink(INHERITANCE_LINK, h1, h2, tv1);

            var mock = new Mock<IForgeDataAdapter>();
            mock.Setup(x => x.Add(It.IsNotNull<Atom>()));
            mock.Setup(x => x.SaveChanges());

            var forge = new Forge(mock.Object);
            var h1 = forge.Add(AtomType.Concept, "ConceptAtom1");
            var h2 = forge.Add(AtomType.Concept, "ConceptAtom2");
            var link = forge.AddLink(h1, h2);
        }
    }
}
