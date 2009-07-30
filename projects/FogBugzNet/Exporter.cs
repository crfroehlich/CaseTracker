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
        private Dictionary<int, XmlNode> _caseToNode = new Dictionary<int, XmlNode>();
        private Dictionary<int, XmlNode> _milestoneToNode = new Dictionary<int, XmlNode>();

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

            XmlElement rootNode = _doc.CreateElement("node");
            rootNode.Attributes.Append(_doc.CreateAttribute("TEXT")).Value = "FogBugz Cases";

            _doc.DocumentElement.AppendChild(rootNode);
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
                _caseToNode.Add(c.ID, _doc.DocumentElement.AppendChild(CaseToNode(c)));

                VerifyMileStoneExists(c);
            }
        }

        private void VerifyMileStoneExists(Case c)
        {
            if (!_milestoneToNode.ContainsKey(c.ParentMileStone.ID))
                _milestoneToNode.Add(c.ParentMileStone.ID, _doc.DocumentElement.AppendChild(CreateMileStoneNode(c)));
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
            XmlNode ms = _milestoneToNode[c.ParentMileStone.ID];
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
    }
}
