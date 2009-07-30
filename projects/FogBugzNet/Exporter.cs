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
            node.Attributes.Append(_doc.CreateAttribute("TEXT")).Value = String.Format("{0}: {1}", c.id, c.name);
            node.Attributes.Append(_doc.CreateAttribute("POSITION")).Value = "right";
            node.Attributes.Append(_doc.CreateAttribute("LINK")).Value = _server + "?" + c.id.ToString();
            
            return node;
        }


        public string CasesToMindMap(Case[] cases)
        {

            _doc.LoadXml("<map version=\"0.8.1\"></map>");

            XmlElement rootNode = _doc.CreateElement("node");
            rootNode.Attributes.Append(_doc.CreateAttribute("TEXT")).Value = "FogBugz Cases";
            
            _doc.DocumentElement.AppendChild(rootNode);

            Dictionary<int, XmlNode> dict = new Dictionary<int,XmlNode>();
            foreach(Case c in cases)
                dict.Add(c.id, rootNode.AppendChild(CaseToNode(c)));

            foreach (Case c in cases)
            {
                if (c.parentCase == 0)
                    continue;

                if (!dict.ContainsKey(c.parentCase))
                    continue;

                XmlNode parent = dict[c.parentCase];

                parent.AppendChild(dict[c.id]);
            }
        
            return _doc.OuterXml;
        }
    }
}
