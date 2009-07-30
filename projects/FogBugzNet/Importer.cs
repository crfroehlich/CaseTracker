using System;
using System.Collections.Generic;
using System.Text;


/*
 * 
 * The plan for analyzing the XML
 * ==============================
 * Parse XML into tree of project/milestone/case
 * 
 * From the root element only Projects can exist. Fail otherwise.
 * From each project only Milestones can exist, fail otherwise.
 * Under each milestones only cases can exist.
 * 
 * Project nodes begin with Project: 
 * Milestone nodes begin with MileStone:
 * Cases begin with either Bug/Feature/..etc.
 * 
 * Anything not identified as a milestone or project is a case (Task)
 * 
 * Treat every project that has a link as an existing project. If no such project found, fail (we're not into creating new projects)
 * 
 * Treat every milestone that has a link as an existing milestone. If no link, create a new milestone for the project.
 * If link exists and points to a milestone that has a different name, rename the milestone.
 * 
 * Any case that has a number (i.e. "Feature 234: ...") is an existing case.
 * If the title is different thatn the existing title, rename it.
 * If the case has no number, create a new case.
 * If the case has a parent that's different than the existing parent, update the parent.
 * Traverse children, recur.
 * 
 * Ignore cases when parsing prefixes (project, milestone, bug, features, task, etc.)
 * 
 * ==== First Stage ====
 * Implement only parsing and moving cases between parents.
 * The root node should contain the link to the original search that yielded the mind map. Use it as comparison for changes.
 * The result of analysis should be a list of action objects (for now, just "Move to Parent" actions).
 * These actions need to be shown to the user for approval and then executed as batch. Basically, it's a diff before committing.
 * 
 * 
 */
namespace FogBugzNet
{
    class Importer
    {
        public Importer(string server, string importer)
    }
}
