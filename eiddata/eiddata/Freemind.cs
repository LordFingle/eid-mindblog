
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
namespace eiddata
{
    public class Freemind
    {
        private Forge forge;
        private IEnumerable<Atom> parentAtoms;

        public Freemind(Forge theForge, IEnumerable<Atom> parents)
        {
            forge = theForge;
            parentAtoms = parents;
        }

        public void Import(Atom rootNode, Stream stream)
        {
            
            XDocument doc = XDocument.Load(stream);
            addNodes(rootNode, doc.Root);
            forge.SaveChanges();
            
        }

        private void addNodes(Atom currentNode, XElement element)
        {
            foreach (var childElement in element.Elements("node"))
            {    
                Atom childNode;
                var mindmapId = childElement.Attribute("ID").Value;
                if (!String.IsNullOrEmpty(mindmapId) && mindmapId.Length == 34)
                    childNode = forge.LoadOrCreate(childElement.Attribute("ID").Value, childElement.Attribute("TEXT").Value);
                else
                    childNode = forge.Add(AtomType.Concept,childElement.Attribute("TEXT").Value);

                var parents = new List<Atom>(parentAtoms);
                parents.Add(currentNode);

                forge.AddLink(parents, childNode);
 
                addNodes(childNode, childElement);
            }

            forge.SaveChanges();
        }

        public Stream Export(Atom rootNode)
        {

            var doc = new XDocument(new XElement("map"));
            doc.Root.SetAttributeValue("version", "0.9.0");
            var ar = new XElement("attribute_registry");
            ar.SetAttributeValue("SHOW_ATTRIBUTES", "hide");
            doc.Root.Add(ar);
            var firstNode = createElement(rootNode);
            doc.Root.Add(firstNode);
            fieldCount = 0;

            exportNodes((Atom)rootNode, firstNode);
            
            // Save without xml declr
            var sb = new StringBuilder();
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.OmitXmlDeclaration = true;

            using (XmlWriter xw = XmlWriter.Create(sb, xws))
            {
              
                doc.Save(xw);
            }

            return GenerateStreamFromString(sb.ToString());
         

        }
        private MemoryStream GenerateStreamFromString(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));
        }
        private int fieldCount;
        private void exportNodes(Atom currentNode, XElement element)
        {
            foreach (var childNode in forge.GetChildren(currentNode))
            {
                
                var childElement = createElement(childNode);

                element.Add(childElement);
                exportNodes(childNode,childElement);
            }

        }

        private XElement createElement(Atom childNode)
        {
            fieldCount++;
            var childElement = new XElement("node");
            childElement.SetAttributeValue("TEXT", childNode.Name);
           // childElement.SetAttributeValue("ID", String.Format("Freemind_Link_{0}", fieldCount));
            childElement.SetAttributeValue("ID", String.Format("{0}", childNode.Id));
            childElement.SetAttributeValue("STYLE", "bubble");
            childElement.SetAttributeValue("COLOR", "#000000");
            childElement.SetAttributeValue("BACKGROUND_COLOR", "#B2B2FE");
             
            return childElement;
        }




    }
}
