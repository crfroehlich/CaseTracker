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

        public Exporter(string Server)
        {
            _server = Server;
        }

        private XmlDocument _doc = new XmlDocument();


        public XmlElement CaseToNode(Case c)
        {
            XmlElement node = _doc.CreateElement("node");
            node.Attributes.Append(_doc.CreateAttribute("TEXT")).Value = String.Format("{0} {1}: {2}", c.Category, c.id, c.name);
            node.Attributes.Append(_doc.CreateAttribute("POSITION")).Value = "right";
            node.Attributes.Append(_doc.CreateAttribute("LINK")).Value = _server + "?" + c.id.ToString();
            
            return node;
        }

        public XmlDocument CasesToMindMap(Case[] cases)
        {

            _doc.LoadXml("<map version=\"0.8.1\"></map>");

            XmlElement rootNode = _doc.CreateElement("node");
            rootNode.Attributes.Append(_doc.CreateAttribute("TEXT")).Value = "FogBugz Cases";
            
            _doc.DocumentElement.AppendChild(rootNode);

            Dictionary<int, XmlNode> caseToNode = new Dictionary<int,XmlNode>();
            Dictionary<int, XmlNode> milestoneToNode = new Dictionary<int, XmlNode>();
            foreach (Case c in cases)
            {
                caseToNode.Add(c.id, rootNode.AppendChild(CaseToNode(c)));
                if (!milestoneToNode.ContainsKey(c.milestone.ID))
                    milestoneToNode.Add(c.milestone.ID, rootNode.AppendChild(CreateMileStoneNode(c)));
            }

            foreach (Case c in cases)
            {
                if (c.parentCase == 0 || !caseToNode.ContainsKey(c.parentCase))
                {
                    XmlNode ms = milestoneToNode[c.milestone.ID];
                    ms.AppendChild(caseToNode[c.id]);
                    continue;
                }

                XmlNode parent = caseToNode[c.parentCase];

                parent.AppendChild(caseToNode[c.id]);
            }
        
            return _doc;
        }

        private XmlElement CreateMileStoneNode(Case c)
        {
            XmlElement ms = _doc.CreateElement("node");
            ms.Attributes.Append(_doc.CreateAttribute("TEXT")).Value = string.Format("MileStone: {0}", c.milestone.Name);
            ms.Attributes.Append(_doc.CreateAttribute("POSITION")).Value = "right";
            ms.Attributes.Append(_doc.CreateAttribute("LINK")).Value = string.Format("{0}?pg=pgList&pre=preSaveFilterFixFor&ixFixFor={1}&ixStatus=-2", _server, c.milestone.ID);

            return ms;
        }
    }
}
