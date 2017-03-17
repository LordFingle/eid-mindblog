using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mindblog.Models
{
  
    public class MindBlogSession
    {
        private HttpSessionStateBase _session;
        public MindBlogSession(HttpSessionStateBase sess)
        {
            _session = sess;
        }
         
        public double Latitude
        {
            get
            {
                return _session["latitude"] == null ? 0 : (double)_session["latitude"];

            }
            set
            {
                _session["latitude"] = value;
            }
        }
        public double Longitude
        {
            get
            {
                return _session["longitude"] == null ? 0 : (double)_session["longitude"];

            }
            set
            {
                _session["longitude"] = value;
            }
        }

    }
}