using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;


public class TmxTileSet
{
	public Dictionary<int, TmxTile> DicTile = new Dictionary<int, TmxTile>();

	public int firstGID = 0;
	public string name = "";
	public int tilewidth = 0;
	public int tileheight = 0;
	public int spacing = 0;
	public int margin = 0;

	public string image_source = "";
	public int image_width = 0;
	public int image_height = 0;

	public void Parse(XmlNode node)
	{		
		///1.properties
		if(node.Attributes.GetNamedItem("firstgid") != null)
			firstGID = System.Convert.ToInt32(node.Attributes.GetNamedItem("firstgid").Value);
		if(node.Attributes.GetNamedItem("name") != null)
			name = node.Attributes.GetNamedItem("name").Value;
		if(node.Attributes.GetNamedItem("tilewidth") != null)
			tilewidth = System.Convert.ToInt32(node.Attributes.GetNamedItem("tilewidth").Value);
		if(node.Attributes.GetNamedItem("tileheight") != null)
			tileheight = System.Convert.ToInt32(node.Attributes.GetNamedItem("tileheight").Value);
		if(node.Attributes.GetNamedItem("spacing") != null)
			spacing = System.Convert.ToInt32(node.Attributes.GetNamedItem("spacing").Value);
		if(node.Attributes.GetNamedItem("margin") != null)
			margin = System.Convert.ToInt32(node.Attributes.GetNamedItem("margin").Value);

		///2.image
		XmlNode image_node = node.SelectSingleNode("image");
		if(node.Attributes.GetNamedItem("source") != null)
			image_source = node.Attributes.GetNamedItem("source").Value;
		if(node.Attributes.GetNamedItem("width") != null)
			image_width = System.Convert.ToInt32(node.Attributes.GetNamedItem("width").Value);
		if(node.Attributes.GetNamedItem("height") != null)
			image_height = System.Convert.ToInt32(node.Attributes.GetNamedItem("height").Value);


		///3.tile
		XmlNodeList node_list = node.SelectNodes("tile");
		foreach(XmlNode obj in node_list)
		{
			TmxTile tile = new TmxTile();
			tile.Parse(obj);
			DicTile.Add(tile.ID, tile);
		}
	}
}

public class TmxTile
{
	public int ID = 0;
	public Dictionary<string, string> DicProperty = new Dictionary<string, string>();

	public void Parse(XmlNode node)
	{		
		///1.properties
		string id = node.Attributes.GetNamedItem("id").Value;
		ID = System.Convert.ToInt32(id);

		///2.property
		XmlNodeList node_list = node.SelectNodes("properties/property");
		foreach(XmlNode obj in node_list)
		{
			string name = obj.Attributes.GetNamedItem("name").Value;
			string value = obj.Attributes.GetNamedItem("value").Value;
			DicProperty.Add(name, value);
		}
	}
}
