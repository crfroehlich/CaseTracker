using System;
using System.Collections.Generic;
using System.Text;

namespace FogBugzCaseTracker
{
    public class Case
    {
        public TimeSpan elapsed;
        public TimeSpan estimate;
        public int id;
        public string name;
        public string project;
        public string area;
        public string assignedTo;
        public int parentCase;

        public string id_name { get { return id + ": " + name; } }
        public string elapsed_time_h_m // returns elapsed time in h:m format: 20:04
        {

            get
            {
                return String.Format("{0}:{1}",
                        elapsed.TotalHours.ToString("###"),
                        elapsed.Minutes.ToString("0#"));

            }
        }
        public string project_id_name
        {
            get
            {
                return String.Format("{0}:{1}:{2}:{3} - {4}", project, area, assignedTo, id, name);
            }
        }

    }

}
