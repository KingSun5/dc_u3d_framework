using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class TmxObjectGroup
{
	public string name = "";
	public bool visible = true;
	public float opacity = 1f;
	public Dictionary<int, TmxObject> DicObject = new Dictionary<int, TmxObject>();

	public void Parse(XmlNode node)
	{		
		///1.property
		if(node.Attributes.GetNamedItem("name") != null)
			name = node.Attributes.GetNamedItem("name").Value;
		if(node.Attributes.GetNamedItem("visible") != null)
			visible = System.Convert.ToInt32(node.Attributes.GetNamedItem("visible").Value) != 0 ? true : false;
		if(node.Attributes.GetNamedItem("opacity") != null)
			opacity = System.Convert.ToSingle(node.Attributes.GetNamedItem("opacity").Value);

		///2.object
		XmlNodeList node_list = node.SelectNodes("object");
		foreach(XmlNode obj in node_list)
		{
			TmxObject item = new TmxObject();
			item.Parse(obj);
			DicObject.Add(item.id, item);
		}
	}
}

public class TmxObject
{
	public int id;
	public string name;
	public string type;
	public float x;
	public float y;
	public float width;
	public float height;
	public Dictionary<string, string> DicProperty = new Dictionary<string, string>();

	public void Parse(XmlNode node)
	{		
		///1.properties
		id = System.Convert.ToInt32(node.Attributes.GetNamedItem("id").Value);
		name = node.Attributes.GetNamedItem("name").Value;
		type = node.Attributes.GetNamedItem("type").Value;
		x = System.Convert.ToSingle(node.Attributes.GetNamedItem("x").Value);
		y = System.Convert.ToSingle(node.Attributes.GetNamedItem("y").Value);
		width = System.Convert.ToSingle(node.Attributes.GetNamedItem("width").Value);
		height = System.Convert.ToSingle(node.Attributes.GetNamedItem("height").Value);
		
		
		///2.property
		XmlNodeList node_list = node.SelectNodes("properties/property");
		foreach(XmlNode obj in node_list)
		{
			string _name = obj.Attributes.GetNamedItem("name").Value;
			string _value = obj.Attributes.GetNamedItem("value").Value;
			DicProperty.Add(_name, _value);
		}
	}
}
