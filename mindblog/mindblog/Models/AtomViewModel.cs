using eiddata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mindblog.Models
{

    public class AtomViewModel
    {
        
        public string Title { get; set; }

        public Atom Atom { get; set; }
        public IEnumerable<Atom> Children { get; set; }
        public IEnumerable<Atom> Parents { get; set; }

        public void Load(Forge forge, string id)
        {
           
            this.Atom = forge.GetAtom(id).First();
            this.Children = forge.GetChildren(this.Atom);
            this.Parents = forge.GetParents(this.Atom);
            this.Title = this.Atom.Name;
        }

    }
}