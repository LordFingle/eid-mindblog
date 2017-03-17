using eiddata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Drawing;

namespace mindblog.Models
{
 
    public static class IEnumerableAtomExtensions
    {
        public static List<WikiPage> ToWikiPage(this IEnumerable<Atom> atoms, Forge forge)
        {
            return (from a in atoms
                   select new WikiPage(forge)
                   {
                       Atom = a
                   }).ToList();
            
        }
    }
    public class WikiPage
    {
        private Forge forge;
        public Atom Atom;
        public string ParentAtomId = null; 
        public WikiPage(Forge forge)
        {
            this.forge = forge;       
        }

        public bool Load(string id)
        {
            var items = forge.GetAtom(id);
            if (items.Count() < 1) return false;
            this.Atom = items.First();
            return true;
        }

        public void Create(string title)
        {          
            this.Atom = forge.Add(AtomType.Concept, title);    
        }
         

        public IEnumerable<WikiPage> GetChildren()
        {
            var children = forge.GetChildren(this.Atom).ToWikiPage(forge);
            foreach (var item in children) item.ParentAtomId = this.Id;
            return children;
        }

        public IEnumerable<WikiPage> GetParents()
        {  
            return forge.GetParents(this.Atom).ToWikiPage(forge);
        }
        

        public string Id
        {
            get
            {
                return this.Atom.Id;
            }
            set
            {
                this.Atom.Id = value;
            }
        }


        public string Title
        {
            get
            {
                return this.Atom.Name;
            }
            set
            {
                this.Atom.Name = value;
            }
        }

        public string Markdown {
            get
            {
              
                do
                {
                   
                    string key = this.Atom.Data.GetKeyByType(DatumType.Markdown);
                    if (key == null) return getDefaultMarkdown(); ;
                    var stream = forge.GetDatumStream(this.Atom, key);
                    if (stream == null)
                    {
                        // Oh dear.  Looks like we have a data corruption.
                        // Remove the offending item and try again.
                        this.Atom.Data.Remove(this.Atom.Data.GetByKey(key));
                        
                    }
                    else
                    {
                        return DatumType.Markdown.FromStream(stream);
                    }

                } while (true);
 
            }
            set
            {
                string key = this.Atom.Data.GetKeyByType(DatumType.Markdown);
                forge.PutDatum(this.Atom, DatumType.Markdown, key, "", DatumType.Markdown.ToStream(value));
            }
        }




        public GeospatialCoordinates Coordinates
        {
            get
            {

                do
                {

                    string key = this.Atom.Data.GetKeyByType(DatumType.GeospacialCoordinates);
                    if (key == null) return null;
                    var stream = forge.GetDatumStream(this.Atom, key);
                    if (stream == null)
                    {
                        // Oh dear.  Looks like we have a data corruption.
                        // Remove the offending item and try again.
                        this.Atom.Data.Remove(this.Atom.Data.GetByKey(key));

                    }
                    else
                    {
                        return DatumType.GeospacialCoordinates.GeoFromStream(stream);
                    }

                } while (true);

            }
            set
            {
                string key = this.Atom.Data.GetKeyByType(DatumType.GeospacialCoordinates);
                if (String.IsNullOrEmpty(key)) key = DatumType.GeospacialCoordinates.GenerateKey();
                forge.PutDatum(this.Atom, DatumType.GeospacialCoordinates, key, "", DatumType.GeospacialCoordinates.ToStream(value));
            }
        }

        public Image Thumbnail
        {
            get
            {
                return getThumbnail();
            }
            set
            {
                setThumbnail(value);
            }
        }

        public bool HasThumbnail()
        {
            return (!String.IsNullOrEmpty(this.Atom.Data.GetKeyByType(DatumType.ImageThumbnail)) ||
                    !String.IsNullOrEmpty(this.Atom.Data.GetKeyByType(DatumType.Image)));

        }

        private Image getThumbnail()
        {
             string key = this.Atom.Data.GetKeyByType(DatumType.ImageThumbnail);
             if (key != null)
             {
                 return getImage(key);
          
             }
             else
             {
                 key = this.Atom.Data.GetKeyByType(DatumType.Image);
                 var img = getImage(key);
                 if (img != null) 
                     return img.GetThumbnailImage(60,60, null, IntPtr.Zero);                
             }
             return null;
        }

        private void setThumbnail(Image someImage)
        {
            string key = this.Atom.Data.GetKeyByType(DatumType.ImageThumbnail);
            if (key == null) key = DatumType.ImageThumbnail.GenerateKey();
            setImage(key, someImage.GetThumbnailImage(60, 60, null, IntPtr.Zero));
        }

        public Image Image
        {
            get
            {
                string key = this.Atom.Data.GetKeyByType(DatumType.Image);
                if (key == null) return null;
                return getImage(key);

            }
            set
            {
                string key = this.Atom.Data.GetKeyByType(DatumType.Image);
                if (String.IsNullOrEmpty(key)) key = DatumType.Image.GenerateKey();
                setImage(key, value);
                setThumbnail(value);              
            }
        }

        private Image getImage(string datumKey)
        {
            do
            {

                string key = this.Atom.Data.GetKeyByType(DatumType.Image);
                if (key == null) return null;
                var stream = forge.GetDatumStream(this.Atom, datumKey);
                if (stream == null)
                {
                    // Oh dear.  Looks like we have a data corruption.
                    // Remove the offending item and try again.
                    this.Atom.Data.Remove(this.Atom.Data.GetByKey(key));

                }
                else
                {
                    return Image.FromStream(stream); // Assume PNG
                }

            } while (true);
        }

        private void setImage(string key, Image value)
        {
            using (var memoryStream = new MemoryStream())
            {

                value.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                memoryStream.Position = 0;
                forge.PutDatum(this.Atom, DatumType.Image, key, "", memoryStream);

            }
        }
      
        private string getDefaultMarkdown()
        {
            return "";
            //return "#[[title]]" + Environment.NewLine
            //             + "[[attachments]]" + Environment.NewLine
            //             + "[[doors]]" + Environment.NewLine;
        }

 




    }
}