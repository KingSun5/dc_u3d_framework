using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class TmxLayer
{
	public string name = "";
	public int width = 0;
	public int height = 0;
	public bool visible = true;
	public float opacity = 1f;

	public List<int> ListGID = new List<int>();

	public void Parse(XmlNode node)
	{		
		///1.properties
		if(node.Attributes.GetNamedItem("name") != null)
			name = node.Attributes.GetNamedItem("name").Value;
		if(node.Attributes.GetNamedItem("width") != null)
			width = System.Convert.ToInt32(node.Attributes.GetNamedItem("width").Value);
		if(node.Attributes.GetNamedItem("height") != null)
			height = System.Convert.ToInt32(node.Attributes.GetNamedItem("height").Value);
		if(node.Attributes.GetNamedItem("visible") != null)
			visible = System.Convert.ToInt32(node.Attributes.GetNamedItem("visible").Value) != 0 ? true : false;
		if(node.Attributes.GetNamedItem("opacity") != null)
			opacity = System.Convert.ToSingle(node.Attributes.GetNamedItem("opacity").Value);

		///2.tile
		XmlNode data_node = node.SelectSingleNode("data");
		XmlNode encode_node = data_node.Attributes.GetNamedItem("encoding");
		if(encode_node == null)
		{
			XmlNodeList tile_nodes = data_node.SelectNodes("tile");
			foreach(XmlNode obj in tile_nodes)
			{
				string gid = obj.Attributes.GetNamedItem("gid").Value;
				ListGID.Add(System.Convert.ToInt32(gid));
			}
		}
		else
		{
			string encode_type = encode_node.Value;
			if(encode_type == "csv")
			{

			}
			else if(encode_type == "base64")
			{

			}
		}
	}
}
