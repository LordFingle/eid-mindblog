using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using eiddata;
using System.Linq;
using eiddataentityframework;
using System.IO;
using mindblog.Models;
using System.Text;
namespace eiddatatests
{
    [TestClass]
    public class WikiPageTests
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
        public void UpdateMarkupCheckMarkdownDefault_NoErrors()
        {

            // Setup
            var forge = new Forge(new EntityFrameworkForgeStorage());


            // Update wiki page
            var wikiPage = new WikiPage(forge);
            wikiPage.Create("This is a test");
            Assert.AreEqual("",wikiPage.Markdown);
            forge.SaveChanges();

            // Update Markdown
            wikiPage.Load(wikiPage.Id);
            wikiPage.Markdown = "Some Markdown";
            forge.SaveChanges();

            // Assert 
            var testWikiPage = new WikiPage(forge);
            Assert.IsTrue(testWikiPage.Load(wikiPage.Id));
            Assert.AreEqual(wikiPage.Title, testWikiPage.Title);
            Assert.AreEqual( "Some Markdown", testWikiPage.Markdown );

        }
        [TestMethod]
        public void UpdateMarkup_NoErrors()
        {
  
            // Setup
            var forge = new Forge(new EntityFrameworkForgeStorage());
       
            
            // Update wiki page
            var wikiPage = new WikiPage(forge);
            wikiPage.Create("This is a test");       
            wikiPage.Markdown = "This is some markdown";
            forge.SaveChanges();
            
            // Assert 
            var testWikiPage = new WikiPage(forge);
            Assert.IsTrue(testWikiPage.Load(wikiPage.Id));
            Assert.AreEqual(wikiPage.Title, testWikiPage.Title );
            Assert.AreEqual(wikiPage.Markdown, testWikiPage.Markdown);
      
        }
     
    }
}
