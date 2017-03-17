using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using eiddata;
using eiddataentityframework;
using mindblog.Models;
using System.Text.RegularExpressions;
using System.IO;
namespace mindblog.Controllers
{
    [Authorize]
    public partial class ForgeController : Controller
    {

        private Forge forge = new Forge(new EntityFrameworkForgeStorage());

        // GET: Forge
        public ActionResult Index()
        {
            Atom place;
            
            var places = forge.GetAtom(AtomType.PlaceKeystone.GenerateId());
            if (places.Count() < 1)
                place = StandardDataInit.Init(forge);
            else
                place = places.First();

            return Details(place.Id,false);
        }

        public ActionResult Details(string id, bool? minimal)
        {
            var model = new WikiPage(forge);
            Atom atom = null;

            if (id == "Trash")
            {
                atom = getSourceChild(AtomType.SourceTrash);
                forge.SaveChanges();
            }
            if (id == "Inventory")
            {
                atom = getSourceChild(AtomType.SourceInventory);
                forge.SaveChanges();
            }
            if (id == "Notebook")
            {
                atom = getSourceChild(AtomType.SourceNotebook);
                forge.SaveChanges();
            }
           

            if (id == null) return RedirectToAction("Index");

            if (atom == null)
            {
                if (!model.Load(id)) return new HttpNotFoundResult(id);
            }
            else
            {
                model.Atom = atom;
            }
            ViewBag.MarkdownHtml = parseHtml(model.Markdown, model.Atom);

            if (minimal.HasValue) ViewBag.Minimal = minimal.Value;

            return View("Details", model);
        }


        //public ActionResult Details(string id, bool? minimal)
        //{
        //    var model = getWikiPageForId(id);

        //    ViewBag.MarkdownHtml = parseHtml(model.Markdown, model.Atom);

        //    if (minimal.HasValue) ViewBag.Minimal = minimal.Value;
          
        //    return View("Details", model);
        //}
         
        public PartialViewResult PartialDetails(string id, bool? minimal)
        {
            var model = getWikiPageForId(id);

            ViewBag.MarkdownHtml = parseHtml(model.Markdown, model.Atom);
 
            return PartialView("~/Views/Forge/PartialDetails.cshtml");
            
        }

        private WikiPage getWikiPageForId(string id)
        {
            var model = new WikiPage(forge);
            Atom atom = null;

            if (id == "Trash")
            {
                atom = getSourceChild(AtomType.SourceTrash);
                forge.SaveChanges();
            }
            if (id == "Inventory")
            {
                atom = getSourceChild(AtomType.SourceInventory);
                forge.SaveChanges();
            }



            if (atom == null)
            {
                if (!model.Load(id)) throw new HttpException(404, "Node not found");
            }
            else
            {
                model.Atom = atom;
            }
            return model;
        }





        private string parseHtml(string theText, Atom atom)
        {


            // Do standard markdown parsing

            var md = new MarkdownDeep.Markdown();
            md.ExtraMode = true;
            theText = md.Transform(theText);

            // Transform [[showchildlist:link id]]

            //var transformedText = Regex.Replace(theText, "(\\[\\[)(showchildlist:)(.*)(\\]\\])([^\\(])", delegate(Match match)
            //{
            //    var theOut = new StringBuilder();
            //    var theItems = findDeChildens.Invoke(match.Groups[3].ToString());
            //    foreach (var item in theItems)
            //    {
            //        theOut.AppendLine(" * [" + item.Abstract + "](/Anvil/See/" + item.Id + ")");
            //    }
            //    return theOut.ToString().TrimEnd('\n');
            //});


            // Transform [[attachment:attachmentname]]
            var transformedText = Regex.Replace(theText, "(\\[\\[)(attachment:)(.*)(\\]\\])([^\\(])", delegate(Match match)
            {

                var attachmentName = match.Groups[3].ToString();
                //TODO: This is DOES NOT WORK!!!!! NEEDS TO BE ELABORATED
                return String.Format("<a href=\"#\">{0}</a>", attachmentName);

            });


            // Transform [[title]]
            theText = Regex.Replace(theText, "(\\[\\[)title(\\]\\])", String.Format("{0}", atom.Name));


            // Transform [[doors]]
            theText = Regex.Replace(theText, "(\\[\\[)doors(\\]\\])", getShowDoorsHtml(atom.Id));

            // Transform [[attachments]]
            theText = Regex.Replace(theText, "(\\[\\[)attachments(\\]\\])", getAttachmentsHtml(atom.Id));

            // Transform [[This Link|This Link]]
            //theText = Regex.Replace(theText, "(\\[\\[)(.*)\\|(.*)(\\]\\])([^\\(])", "[$3](/Forge/Detail/$2)$5");
            theText = Regex.Replace(theText, "(\\[\\[)(.*)\\|(.*)(\\]\\])([^\\(])", "<a href='/Forge/Detail/$2'>$3</a>$5");
            // Transform [[This Text]]
            //theText = Regex.Replace(theText, "(\\[\\[)(.*)(\\]\\])([^\\(])", "[$2](/Forge/Detail/$2)$4");
            theText = Regex.Replace(theText, "(\\[\\[)(.*)(\\]\\])([^\\(])", "<a href='/Forge/Detail/$2'>$2</a>$4");
 
            return theText;

        }

 
        private string getShowDoorsHtml(string id)
        {   
            using(var x = new StringWriter())
            {
                GetHtmlHelper(x).RenderAction("RenderLinks", new {id=id});
                return x.ToString();
            }
        }

        private string getAttachmentsHtml(string id)
        {
            using(var x = new StringWriter())
            {
                GetHtmlHelper(x).RenderAction("RenderAttachments", new {id=id});
                return x.ToString();
            }
        }

        public HtmlHelper GetHtmlHelper(TextWriter textWriter)
        {
            var viewContext = new ViewContext(this.ControllerContext, new FakeView(), this.ViewData, this.TempData, textWriter);
            return new HtmlHelper(viewContext, new ViewPage());
        }

        public class FakeView : IView
        {
            public void Render(ViewContext viewContext, TextWriter writer)
            {
                throw new InvalidOperationException();
            }
        }

        public PartialViewResult RenderLinks(string id)
        {
            var model = new WikiPage(forge);
            model.Load(id);
            
            var theList = model.GetChildren().ToList();
            foreach(var item in theList) item.ParentAtomId = id;
            return PartialView("RenderLinks", theList);
        }


        public PartialViewResult RenderAttachments(string id)
        {
            var model = new WikiPage(forge);
            if (!model.Load(id)) return null;

            return PartialView("RenderAttachments", model.Atom.Data);
        }
         
        public JsonResult Move(string id, string oldParentId, string newParentId)
        {

            var atomToCopy = forge.GetSingleAtom(id);
            var atomToCopyTo = forge.GetSingleAtom(newParentId);

            forge.AddLink(atomToCopyTo, atomToCopy);

            // Delete Parent Link
            if (oldParentId != null)
            {
                var oldAtom = forge.GetSingleAtom(oldParentId);
                if (oldAtom != null) forge.DeleteLink(oldAtom, atomToCopy);
            }
            forge.SaveChanges();
            return Json(new object());
        }

        public JsonResult Copy(string id, string newParentId)
        {
            return this.Move(id, null, newParentId);
        }

        public JsonResult MoveToTrash(string id,string oldParentId)
        {

            var atomToCopy = forge.GetSingleAtom(id);
            var atomToCopyTo = getSourceChild(AtomType.SourceTrash);
            if (atomToCopy == null || atomToCopyTo == null) throw new HttpException("Invalid items moved to trash");

            forge.AddLink(atomToCopyTo, atomToCopy);

            // Delete Parent Link
            if (oldParentId != null)
            {
                var oldAtom = forge.GetSingleAtom(oldParentId);
                if (oldAtom != null) forge.DeleteLink(oldAtom, atomToCopy);
            }
            forge.SaveChanges();
            return Json(new object());
        }

        public JsonResult MoveToInventory(string id, string oldParentId)
        {

            var atomToCopy = forge.GetSingleAtom(id);
            var atomToCopyTo = getSourceChild(AtomType.SourceInventory);
            if (atomToCopy == null || atomToCopyTo == null) throw new HttpException("Invalid items moved to inventory");

            forge.AddLink(atomToCopyTo, atomToCopy);

            // Delete Parent Link
            if (oldParentId != null)
            {
                var oldAtom = forge.GetSingleAtom(oldParentId);
                if (oldAtom != null) forge.DeleteLink(oldAtom, atomToCopy);
            }
            forge.SaveChanges();
            return Json(new object());
        }

 
        // GET: Forge/Create
        public ActionResult Create(string parentId)
        {
            ViewBag.ParentId = parentId;
            return View();
        }

        
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            
                var model = new WikiPage(forge);
                
                model.Create(collection["name"]);

                forge.AddLink(getStandardParentRelationships(collection["parentid"]), model.Atom);

                forge.SaveChanges();

                return RedirectToAction("Details", new { id = collection["parentid"] });
          
        }
 
        public ActionResult CreateNote()
        {
            var notebook = getNotebook();
            forge.SaveChanges();

            var model = new WikiPage(forge);

            model.Create("Untitled");
            forge.AddLink(getStandardParentRelationships(notebook.Id), model.Atom);
            forge.SaveChanges();

            return RedirectToAction("Edit", new { id =  model.Id });

        }



        private IEnumerable<Atom> getStandardParentRelationships(string parentId)
        {
            var links = new List<Atom>();
            links.Add(forge.GetAtom(parentId).Single());
            links.Add(getCurrentDay());
            links.Add(getCurrentLocation());
            links.Add(getCurrentSource());
           
            return links;
        }

        private Atom getCurrentDay()
        {
            var days = forge.LoadOrCreate(AtomType.DayKeystone, "Days");

            var atomType = AtomType.Day;
            atomType.CurrentDateTime = DateTime.Now;

            return forge.LoadOrCreate(atomType, days);
        }
        private Atom getCurrentLocation()
        {
            var locations = forge.LoadOrCreate(AtomType.LocationKeystone, "Locations");

            var atomType = AtomType.Location;
            atomType.Latitude = this.Session["latitude"] == null ? 0 : (double)this.Session["latitude"];
            atomType.Longitude = this.Session["longitude"] == null ? 0 : (double)this.Session["longitude"];

            return forge.LoadOrCreate(atomType, locations);
        }

        private Atom getCurrentSource()
        {

            var sources = forge.LoadOrCreate(AtomType.SourceKeystone, "Sources");
            var atomType = AtomType.Source;
            atomType.Id = this.User.Identity.Name;
            return forge.LoadOrCreate(atomType, sources);
        }

        private Atom getSourceChild(AtomTypeUnique atomType)
        {
            var source = getCurrentSource();
            
            atomType.Id = this.User.Identity.Name;
            
            var sourceChild = forge.LoadOrCreate(atomType, source);

            return sourceChild;
        }

        private Atom getNotebook()
        {
  
            var atomType = AtomType.SourceNotebook;
            atomType.Id = this.User.Identity.Name;

            var notebook = forge.LoadOrCreate(atomType, getSourceChild(AtomType.SourceInventory));
             
            return notebook;
        }


        // GET: Forge/Edit/5
        public ActionResult Edit(string id)
        {
            var model = new WikiPage(forge);
            if(!model.Load(id))throw new HttpException(404, "Invalid id passed");
            return View(model);
        }

        // POST: Forge/Edit/5
        [HttpPost]
        public ActionResult Edit(string id, FormCollection collection)
        {
            //try
            //{
                var model = new WikiPage(forge);
                if (!model.Load(id)) throw new HttpException(404, "Invalid id passed");

                model.Title = collection["title"];
                model.Markdown = collection["markdown"];
                 
                forge.SaveChanges();

                return RedirectToAction("Details", new { id = id });
            //}
            //catch
            //{
            //    return RedirectToAction("Details", new { id = id });
            //}
        }

        // GET: Forge/Delete/5
        public ActionResult Delete(string id)
        {
            return View();
        }

        [Authorize]
        public JsonResult SetLocation(double latitude, double longitude)
        {
            this.Session["latitude"] = latitude;
            this.Session["longitude"] = longitude;


            return Json(new object());
        }

        // POST: Forge/Delete/5
        [HttpPost]
        public ActionResult Delete(string id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }




    }
}
