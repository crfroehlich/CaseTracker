using System;
using System.Collections.Generic;
using System.Text;

namespace FogBugzNet
{
    public class Case
    {
        public TimeSpan Elapsed;
        public TimeSpan Estimate;
        public int ID;
        public string Name;
        public Project ParentProject;

        public string Area;
        public string AssignedTo;
        public int ParentCaseID = 0;
        public string Category;

        public MileStone ParentMileStone = new MileStone();

        public string ShortDescription { get { return ID + ": " + Name; } }

        private static string FormatTimeSpan(TimeSpan ts)
        {
            return String.Format("{0}:{1}",
                        ts.Hours.ToString("0#"),
                        ts.Minutes.ToString("0#"));
        }

        public string ElapsedTime_h_m // returns elapsed time in h:m format: 20:04
        {
            get
            {
                return FormatTimeSpan(Elapsed);
            }
        }
        public string EstimatedTime_h_m // returns elapsed time in h:m format: 20:04
        {

            get
            {
                return FormatTimeSpan(Estimate);
            }
        }
        public string LongDescription
        {
            get
            {
                return String.Format("{0}:{1}:{2}:{3} - {4}", ParentProject.Name, Area, AssignedTo, ID, Name);
            }
        }

    }

}
