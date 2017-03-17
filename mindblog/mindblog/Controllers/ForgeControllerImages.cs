using eiddata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
namespace mindblog.Controllers
{
    public partial class ForgeController 
    {
      

        [Authorize]
        public string GetImageAsAscii(string id)
        {
            try
            {
                // Load Tier

                var model = getWikiPageForId(id);
 
                return ASCIIConverter.GrayscaleImageToASCII(model.Image, 150, 150);
                

            }
            catch
            {

                return null;
            }



        }

       
        [Authorize]
        public ActionResult GetThumbnail(string id)
        {
            try
            {
                // Load Atom       
                var model = getWikiPageForId(id);
                byte[] fileBytes;
                using (var img =  model.Thumbnail)
                {
                     
                    using (var memoryStream = new MemoryStream())
                    {

                        img.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                        img.Dispose();
                        fileBytes = memoryStream.ToArray();

                        return File(
                            fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "thumbnail.png");
                    }
                }
                
            }
            catch
            {
                this.Response.StatusCode = 401;
                return null;
            }
        }

        [Authorize]
        public ActionResult GetImage(string id)
        {
            try
            {

                // Load Atom
                var model = getWikiPageForId(id);
                byte[] fileBytes;
                using (var img = model.Image)
                {

                    using (var memoryStream = new MemoryStream())
                    {

                        img.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                        img.Dispose();
                        fileBytes = memoryStream.ToArray();

                        return File(
                            fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "image.png");
                    }
                }

            }
            catch
            {
                this.Response.StatusCode = 401;
                return null;
            }
        }

    }
}