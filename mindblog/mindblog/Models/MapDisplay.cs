using eiddata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mindblog.Models
{
    public static class MapDisplay
    {
         
        public static List<MapDisplayNode> GetMapDisplayNodes(this WikiPage wikiPage)
        {
            var ret = new List<MapDisplayNode>();
            var parents = wikiPage.GetParents();
            ret.Add(new MapDisplayNode { Id = wikiPage.Id, Title = wikiPage.Title });
            if (parents.Count() > 0)
            {
                ret.Add(new MapDisplayNode { Id = "parent", Title = "", Shape = "dot", ParentId = wikiPage.Id });
            }
            
            

            foreach (var item in wikiPage.GetChildren())
            {
                ret.Add(new MapDisplayNode { Id = item.Id, Title = item.Title, ParentId = wikiPage.Id });
            }

            foreach (var item in parents)
            {
                ret.Add(new MapDisplayNode { Id = item.Id, Title = item.Title, ParentId = "parent" });
            }

            foreach (var att in wikiPage.Atom.Data)
            {
                ret.Add(new MapDisplayNode { 
                    Id = att.Id + att.Key, 
                    Title = "", 
                    Shape = "image",
                    AttachmentId = att.Id,
                    AttachmentKey = att.Key,
                    ImageUrl = "/Forge/GetAttachmentThumbnail",
                    ParentId = wikiPage.Id
                });
            }

            return ret;

        }
 
    }
    public class MapDisplayNode
    {
        public string Id;
        public string Title;
        public string ParentId;
        public string Shape = "box";
        public string ImageUrl = "";
        public string AttachmentId;
        public string AttachmentKey;
    }
}