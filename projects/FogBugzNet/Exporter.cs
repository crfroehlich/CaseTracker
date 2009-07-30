using System;
using System.Collections.Generic;
using System.Text;
using FogBugzNet;
using System.Xml;

namespace FogBugzNet
{
    class Exporter
    {
        private String _server;
        private XmlElement _rootNode;
        private Dictionary<int, XmlNode> _caseToNode = new Dictionary<int, XmlNode>();
        private Dictionary<int, ProjectNode> _projectIdToNode = new Dictionary<int, ProjectNode>();

        private Case[] _cases;

        public Exporter(string Server, Case[] cases)
        {
            _cases = cases;
            _server = Server;
        }

        private XmlDocument _doc = new XmlDocument();

        private XmlElement NewElement()
        {
            XmlElement node = _doc.CreateElement("node");
            node.Attributes.Append(_doc.CreateAttribute("POSITION")).Value = "right";
            return node;
        }

        public XmlElement CaseToNode(Case c)
        {
            XmlElement node = NewElement();
            node.Attributes.Append(_doc.CreateAttribute("TEXT")).Value = String.Format("{0} {1}: {2}", c.Category, c.ID, c.Name);
            node.Attributes.Append(_doc.CreateAttribute("LINK")).Value = _server + "?" + c.ID.ToString();
            
            return node;
        }

        public XmlDocument CasesToMindMap()
        {
            GenerateDocumentSkeleton();

            CreateFlatElements();

            RelocateElements();
            return _doc;
        }

        private void GenerateDocumentSkeleton()
        {
            _doc.LoadXml("<map version=\"0.8.1\"></map>");

            _rootNode = _doc.CreateElement("node");
            _rootNode.Attributes.Append(_doc.CreateAttribute("TEXT")).Value = "FogBugz Cases";


            _doc.DocumentElement.AppendChild(_rootNode);
        }

        private void RelocateElements()
        {
            foreach (Case c in _cases)
                RelocateCaseInDOM(c);
        }

        private void CreateFlatElements()
        {
            foreach (Case c in _cases)
            {
                _caseToNode.Add(c.ID, _rootNode.AppendChild(CaseToNode(c)));
                VerifyProjectNodeExists(c);
                VerifyMileStoneExists(c);
            }
        }

        private void VerifyProjectNodeExists(Case c)
        {
            if (!_projectIdToNode.ContainsKey(c.ParentProject.ID))
            {
                ProjectNode pn = CreateProjectNode(c);
                _rootNode.AppendChild(pn.Node);
                _projectIdToNode.Add(c.ParentProject.ID, pn);
            }
        }

        private ProjectNode CaseProjectNode(Case c)
        {
            return _projectIdToNode[c.ParentProject.ID];
        }

        private void VerifyMileStoneExists(Case c)
        {
            ProjectNode pn = CaseProjectNode(c);
            
            if (!pn.MileStoneIdToNode.ContainsKey(c.ParentMileStone.ID))
                pn.MileStoneIdToNode.Add(c.ParentMileStone.ID, pn.Node.AppendChild(CreateMileStoneNode(c)));
        }

        private void RelocateCaseInDOM(Case c)
        {

            if (NoParentCaseAvailable(c))
                MoveCaseToMileStone(c);
            else
                MoveCaseToParent(c);
                
        }

        private void MoveCaseToParent(Case c)
        {
            _caseToNode[c.ParentCase].AppendChild(_caseToNode[c.ID]);
        }
        private void MoveCaseToMileStone(Case c)
        {
            XmlNode ms = CaseProjectNode(c).MileStoneIdToNode[c.ParentMileStone.ID];
            ms.AppendChild(_caseToNode[c.ID]);
        }
        private bool NoParentCaseAvailable(Case c)
        {
            return (c.ParentCase == 0 || !_caseToNode.ContainsKey(c.ParentCase));
        }
        private XmlElement CreateMileStoneNode(Case c)
        {
            XmlElement mileStoneNode = NewElement();
            mileStoneNode.Attributes.Append(_doc.CreateAttribute("TEXT")).Value = string.Format("MileStone: {0}", c.ParentMileStone.Name);
            mileStoneNode.Attributes.Append(_doc.CreateAttribute("LINK")).Value = string.Format("{0}?pg=pgList&pre=preSaveFilterFixFor&ixFixFor={1}&ixStatus=-2", _server, c.ParentMileStone.ID);

            return mileStoneNode;
        }

        private ProjectNode CreateProjectNode(Case c)
        {
            ProjectNode ret = new ProjectNode();
            ret.Node = NewElement();
            ret.Node.Attributes.Append(_doc.CreateAttribute("TEXT")).Value = string.Format("Project: {0}", c.ParentProject.Name);
            ret.Node.Attributes.Append(_doc.CreateAttribute("LINK")).Value = 
                string.Format("{0}?pre=preMultiSearch&pg=pgList&pgBack=pgSearch&search=2&searchFor=status:%22active%22+project:%22{1}%22+OrderBy:%22FixFor%22+View:%22Outline%22", _server, c.ParentProject.Name);

            return ret;
        }


    }
}
