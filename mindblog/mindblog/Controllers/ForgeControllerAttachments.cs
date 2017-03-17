using eiddata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
using mindblog.Models;
namespace mindblog.Controllers
{
    public partial class ForgeController 
    {
        [Authorize]
        public ActionResult UploadFreemind(string id)
        {
            // Load Tier
            var atom = this.forge.GetSingleAtom(id);

            var freemind = new Freemind(this.forge,
                getStandardParentRelationships(id)
                );
           



            for (var i = 0; i < this.Request.Files.Count; i++)
            {
                if (Request.Files[i].FileName.EndsWith(".mm"))
                    freemind.Import(atom, Request.Files[i].InputStream);
            }



            return RedirectToAction("See", new { id = id });
        }

        [Authorize]
        public ActionResult DownloadFreemind(string id)
        {
            // Load Tier
            var atom = this.forge.GetSingleAtom(id);

            var freemind = new Freemind(this.forge,null);
 
            byte[] fileBytes;
            using (var memoryStream = new MemoryStream())
            {
                freemind.Export(atom).CopyTo(memoryStream);
                fileBytes = memoryStream.ToArray();
            }
            return File(
                fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "export.mm");


        }

        [Authorize]
        public string GetAttachmentAsAscii(string id, string key)
        {
            try
            {
                // Load Tier
                var atom = this.forge.GetSingleAtom(id);

                // Get Filename
                var attachmentEntry = (from r in atom.Data
                                       where r.Key == key || r.Name == key
                                       select r).Single();
                var filename = attachmentEntry.Name;

                using (var memoryStream = new MemoryStream())
                using (var img = System.Drawing.Bitmap.FromStream(this.forge.GetDatumStream(atom, attachmentEntry.Key)))
                {

                    return ASCIIConverter.GrayscaleImageToASCII(img, 150, 150);
                }

            }
            catch
            {

                return null;
            }



        }

        [Authorize]
        public ActionResult GetAttachmentThumbnail(string id, string key)
        {
            try
            {
                // Load Atom
                var atom = this.forge.GetSingleAtom(id);

                // Get Filename
                var attachmentEntry = (from r in atom.Data
                                       where r.Key == key || r.Name == key
                                       select r).Single();
                var filename = attachmentEntry.Name.ToLower();

                if(filename.EndsWith(".png") || filename.EndsWith(".jpg"))
                {
                     byte[] fileBytes;
                     using(var img = Image.FromStream(this.forge.GetDatumStream(atom, attachmentEntry.Key)))
                     {
                         var smallImage = img.GetThumbnailImage(48, 48, null, IntPtr.Zero);

                         using (var memoryStream = new MemoryStream())
                         {
                                    
                            smallImage.Save(memoryStream,System.Drawing.Imaging.ImageFormat.Png);
                            smallImage.Dispose();
                            fileBytes = memoryStream.ToArray();
                             
                            return File(
                                fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
                                     }
                        }


                }

               return null;

            }
            catch
            {
                this.Response.StatusCode = 401;
                return null;
            }
        }

        [Authorize]
        public ActionResult DownloadAttachment(string id, string key)
        {
            try
            {
                // Load Atom
                var atom = this.forge.GetSingleAtom(id);

                // Get Filename
                var attachmentEntry = (from r in atom.Data
                                       where r.Key == key || r.Name == key
                                       select r).Single();
                var filename = attachmentEntry.Name;

                byte[] fileBytes;
                using (var memoryStream = new MemoryStream())
                {
                    this.forge.GetDatumStream(atom, attachmentEntry.Key).CopyTo(memoryStream);                   
                    fileBytes = memoryStream.ToArray();
                }
                return File(
                    fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
            }
            catch
            {
                this.Response.StatusCode = 401;
                return null;
            }
        }

        [Authorize]
        public ActionResult MoveAttachment(string id, string key, string toId)
        {
            // Load Atom
            var atom = this.forge.GetSingleAtom(id);
            var toAtom = this.forge.GetSingleAtom(toId);

            // Get Filename
            var sourceAttachmentEntry = (from r in atom.Data
                                   where r.Key == key || r.Name == key
                                   select r).Single();
           
            // Copy
            this.forge.PutDatum(toAtom, DatumType.Image, null, sourceAttachmentEntry.Name, this.forge.GetDatumStream(atom, sourceAttachmentEntry.Key));

            // Delete
            this.forge.DeleteDatum(atom, sourceAttachmentEntry.Key);
            
            this.forge.SaveChanges();

            return RedirectToAction("Details", new { id = toAtom.Id });
        }

        [Authorize]
        public ActionResult UploadAttachment(string id)
        {
            // Load Tier
            var atom = this.forge.GetSingleAtom(id);

            for (var i = 0; i < this.Request.Files.Count; i++)
            {
                
                this.forge.PutDatum(atom, DatumType.Image, null, Path.GetFileName(this.Request.Files[i].FileName), this.Request.Files[i].InputStream);
         
            }
 
            this.forge.SaveChanges();
            return RedirectToAction("Details", new { id = atom.Id });
        }

        [Authorize]
        public ActionResult UploadAttachmentAsNewPage(string id)
        {
 
            for (var i = 0; i < this.Request.Files.Count; i++)
            {

                var name = Path.GetFileName(this.Request.Files[i].FileName).ToLower();
                var model = new WikiPage(forge);               
                model.Create(Path.GetFileNameWithoutExtension(name));

                forge.AddLink(getStandardParentRelationships(id), model.Atom);
                forge.SaveChanges();
                
                if (name.EndsWith(".jpg") || name.EndsWith(".png"))
                {
                    using (var img = Image.FromStream(this.Request.Files[i].InputStream))
                    {
                        model.Image = img;
                    }
                }
                
                //this.forge.PutDatum(model.Atom, datumType, null, name, this.Request.Files[i].InputStream);
                this.forge.SaveChanges();

            }

            
            return RedirectToAction("Details", new { id = id });
        }


    }
}