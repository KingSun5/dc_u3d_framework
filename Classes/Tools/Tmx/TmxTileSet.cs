using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;


/// <summary>
/// TMX图集
/// @author hannibal
/// @time 2015-8-13
/// </summary>
public class TmxTileSet
{
	public int firstGID = 0;
    public string source = "";

    public void Parse(XmlNode node)
    {
        ///1.properties
        if (node.Attributes.GetNamedItem("firstgid") != null)
            firstGID = System.Convert.ToInt32(node.Attributes.GetNamedItem("firstgid").Value);
        if (node.Attributes.GetNamedItem("source") != null)
            source = node.Attributes.GetNamedItem("source").Value;
    }
}