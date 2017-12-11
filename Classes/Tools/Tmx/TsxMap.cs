using System.Collections;
using System.Collections.Generic;
using System.Xml;


/// <summary>
/// TMX tsx文件图集
/// @author hannibal
/// @time 2015-8-13
/// </summary>
public class TsxMap
{
    /// <summary>
    /// 图集列表
    /// </summary>
    public static Dictionary<string, TsxTileSet> TsxSets = new Dictionary<string, TsxTileSet>();


    /// <summary>
    /// 添加图集文件
    /// </summary>
    public static string Add(XmlDocument doc)
    {
        if (doc == null) return "";

        XmlNode root_node = doc.DocumentElement;

        TsxTileSet tsx = new TsxTileSet();
        tsx.Parse(root_node);
        if (!TsxSets.ContainsKey(tsx.name))
        {
            TsxSets.Add(tsx.name, tsx);
        }
        return tsx.name;
    }
}

public class TsxTileSet
{
    /// <summary>
    /// 单个图集对应的tiles
    /// </summary>
    public Dictionary<int, TsxTile> DicTile = new Dictionary<int, TsxTile>();
    /// <summary>
    /// 属性
    /// </summary>
    public Dictionary<string, string> DicProperty = new Dictionary<string, string>();

    public string name = "";
    public int tilewidth = 0;
    public int tileheight = 0;
    public int tilecount = 0;
    public int columns = 0;
    public int spacing = 0;
    public int margin = 0;

    public string image_source = "";
    public int image_width = 0;
    public int image_height = 0;

    public void Parse(XmlNode node)
    {
        ///1.properties
        if (node.Attributes.GetNamedItem("name") != null)
            name = node.Attributes.GetNamedItem("name").Value;
        if (node.Attributes.GetNamedItem("tilewidth") != null)
            tilewidth = System.Convert.ToInt32(node.Attributes.GetNamedItem("tilewidth").Value);
        if (node.Attributes.GetNamedItem("tileheight") != null)
            tileheight = System.Convert.ToInt32(node.Attributes.GetNamedItem("tileheight").Value);
        if (node.Attributes.GetNamedItem("tilecount") != null)
            tilecount = System.Convert.ToInt32(node.Attributes.GetNamedItem("tilecount").Value);
        if (node.Attributes.GetNamedItem("columns") != null)
            columns = System.Convert.ToInt32(node.Attributes.GetNamedItem("columns").Value);
        if (node.Attributes.GetNamedItem("spacing") != null)
            spacing = System.Convert.ToInt32(node.Attributes.GetNamedItem("spacing").Value);
        if (node.Attributes.GetNamedItem("margin") != null)
            margin = System.Convert.ToInt32(node.Attributes.GetNamedItem("margin").Value);

        ///2.property
        XmlNodeList node_list = node.SelectNodes("properties/property");
        foreach (XmlNode obj in node_list)
        {
            string _name = obj.Attributes.GetNamedItem("name").Value;
            string _value = obj.Attributes.GetNamedItem("value").Value;
            DicProperty.Add(_name, _value);
        }

        ///3.image
        XmlNode image_node = node.SelectSingleNode("image");
        if (image_node.Attributes.GetNamedItem("source") != null)
            image_source = image_node.Attributes.GetNamedItem("source").Value;
        if (image_node.Attributes.GetNamedItem("width") != null)
            image_width = System.Convert.ToInt32(image_node.Attributes.GetNamedItem("width").Value);
        if (image_node.Attributes.GetNamedItem("height") != null)
            image_height = System.Convert.ToInt32(image_node.Attributes.GetNamedItem("height").Value);


        ///4.tiles
        node_list = node.SelectNodes("tile");
        foreach (XmlNode obj in node_list)
        {
            TsxTile tile = new TsxTile();
            tile.Parse(obj);
            DicTile.Add(tile.ID, tile);
        }
    }
}

public class TsxTile
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
        foreach (XmlNode obj in node_list)
        {
            string name = obj.Attributes.GetNamedItem("name").Value;
            string value = obj.Attributes.GetNamedItem("value").Value;
            DicProperty.Add(name, value);
        }
    }
}