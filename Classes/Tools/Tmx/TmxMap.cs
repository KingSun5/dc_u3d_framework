using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

/// <summary>
/// TMX文件解析
/// @author hannibal
/// @time 2015-8-13
/// </summary>
public class TmxMap
{
	public string version = "";
	public string orientation = "";
	public string renderorder = "";
	public int tile_rows = 0;
    public int tile_cols = 0;
    public float tile_width = 0;
    public float tile_height = 0;
	public int nextobjectid = 0;

    public Dictionary<string, string> DicProperty = new Dictionary<string, string>();
    public Dictionary<int, TmxTileSet> DicTileSet = new Dictionary<int, TmxTileSet>();
	public Dictionary<string, TmxLayer> DicLayer = new Dictionary<string, TmxLayer>();
	public Dictionary<string, TmxObjectGroup> DicObjectGroup = new Dictionary<string, TmxObjectGroup>();

    public bool Parse(string file_name)
    {
        XmlDocument doc = OpenXML(file_name);
        if (doc == null) return false;
        return this.Parse(doc);
    }
    public bool Parse(XmlDocument doc)
	{
		if(doc == null)return false;
		
		XmlNode root_node = doc.DocumentElement;

		///1.properties
		if(root_node.Attributes.GetNamedItem("version") != null)
			version = root_node.Attributes.GetNamedItem("version").Value;
		if(root_node.Attributes.GetNamedItem("orientation") != null)
			orientation = root_node.Attributes.GetNamedItem("orientation").Value;
		if(root_node.Attributes.GetNamedItem("renderorder") != null)
			renderorder = root_node.Attributes.GetNamedItem("renderorder").Value;
		if(root_node.Attributes.GetNamedItem("height") != null)
            tile_rows = System.Convert.ToInt32(root_node.Attributes.GetNamedItem("height").Value);
		if(root_node.Attributes.GetNamedItem("width") != null)
            tile_cols = System.Convert.ToInt32(root_node.Attributes.GetNamedItem("width").Value);
		if(root_node.Attributes.GetNamedItem("tilewidth") != null)
            tile_width = System.Convert.ToSingle(root_node.Attributes.GetNamedItem("tilewidth").Value);
		if(root_node.Attributes.GetNamedItem("tileheight") != null)
            tile_height = System.Convert.ToSingle(root_node.Attributes.GetNamedItem("tileheight").Value);
		if(root_node.Attributes.GetNamedItem("nextobjectid") != null)
			nextobjectid = System.Convert.ToInt32(root_node.Attributes.GetNamedItem("nextobjectid").Value);

        ///2.properties
        XmlNodeList node_list = root_node.SelectNodes("properties/property");
        foreach (XmlNode obj in node_list)
        {
            string _name = obj.Attributes.GetNamedItem("name").Value;
            string _value = obj.Attributes.GetNamedItem("value").Value;
            DicProperty.Add(_name, _value);
        }

		///3.tileset
		node_list = root_node.SelectNodes("tileset");
		foreach(XmlNode obj in node_list)
		{
			TmxTileSet tile_set = new TmxTileSet();
			tile_set.Parse(obj);
			if(DicTileSet.ContainsKey(tile_set.firstGID))
                Log.Error("TmxMap::Parse the same tileset is exist:" + tile_set.firstGID);
			else
                DicTileSet.Add(tile_set.firstGID, tile_set);
		}

		///4.layer
		node_list = root_node.SelectNodes("layer");
		foreach(XmlNode obj in node_list)
		{
			TmxLayer layer = new TmxLayer();
			layer.Parse(obj);
			if(DicLayer.ContainsKey(layer.name))
				Log.Error("TmxMap::Parse the same layer is exist:" + layer.name);
			else
				DicLayer.Add(layer.name, layer);
		}
		
		///5.objectgroup
		node_list = root_node.SelectNodes("objectgroup");
		foreach(XmlNode obj in node_list)
		{
			TmxObjectGroup objectgroup = new TmxObjectGroup();
			objectgroup.Parse(obj);
			if(DicObjectGroup.ContainsKey(objectgroup.name))
				Log.Error("TmxMap::Parse the same objectgroup is exist:" + objectgroup.name);
			else
				DicObjectGroup.Add(objectgroup.name, objectgroup);
		}

		return true;
	}

    //～～～～～～～～～～～～～～～～～～～～～～～xml～～～～～～～～～～～～～～～～～～～～～～～//
    static public XmlDocument OpenXML(string file_name)
    {
        TextAsset textAsset = Resources.Load(file_name) as TextAsset;
        if (textAsset == null)
        {
            Log.Error("FileUtils::OpenXML - load error:" + file_name);
            return null;
        }
        else
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(textAsset.text);
            
            return xmlDoc;
        }
    }
}
