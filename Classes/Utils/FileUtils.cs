using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System;

public class FileUtils
{
    /// <summary>
    /// 获取不带扩展名的文件
    /// </summary>
	static string GetPathWithNoExtend (string path)
	{
		int idx = path.LastIndexOf (".");
		string noext = path.Substring (0, idx);
		return noext;
	}

	/// <summary>
    /// add "/" to the head
	/// </summary>
	public static string ConfirmSlashAhead (string name)
	{
		name = name.Trim ();
		if (name [0] != '/')
			name = "/" + name;
		return name;
	}

	static public string MD5ByPathName(string pathName)
	{
		try
		{
			FileStream file = new FileStream(pathName, FileMode.Open);
			System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
			byte[] retVal = md5.ComputeHash(file);
			file.Close();
			
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < retVal.Length; i++)
			{
				sb.Append(retVal[i].ToString("x2"));
			}
			return sb.ToString();
		}
		catch (Exception ex)
		{
			throw new Exception("MD5ByPathName() fail,error:" + ex.Message);
		}
	}
    /// <summary>
    /// 遍历目录，获取所有文件
    /// </summary>
    /// <param name="dir">查找的目录</param>
    /// <param name="listFiles">文件列表</param>
    static public void GetFullDirectoryFiles(string dir_path, ref List<string> list_files)
    {
        if (!Directory.Exists(dir_path)) return;

        DirectoryInfo dir = new DirectoryInfo(dir_path);
        RecursiveFullDirectory(dir, dir_path + '/', ref list_files);
    }
    static private void RecursiveFullDirectory(DirectoryInfo dir, string parent_path, ref List<string> list_files)
    {
        FileInfo[] allFile = dir.GetFiles();
        foreach (FileInfo fi in allFile)
        {
            string ext = fi.Extension.ToLower();
            if (ext == ".meta" || ext == ".manifest" || ext == ".svn") continue;
            if ((fi.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden) continue;
            list_files.Add(parent_path + fi.Name);
        }
        DirectoryInfo[] allDir = dir.GetDirectories();
        foreach (DirectoryInfo d in allDir)
        {
            if (d.Name == "." || d.Name == "..") continue;
            if ((d.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden) continue;
            RecursiveFullDirectory(d, parent_path+d.Name+'/', ref list_files);
        }
    }
	/// <summary>
	/// 拷贝目录
	/// </summary>
	static public void CopyXMLFolderTo(string srcDir, string dstDir)
	{
#if UNITY_WEBPLAYER
		//undo
		Log.Error("FileUtils::CopyFolderTo - cannot use in web");
#else
		if(!Directory.Exists(dstDir))
		{
			Directory.CreateDirectory(dstDir);
		}
		
		DirectoryInfo info = new DirectoryInfo(srcDir);
		FileInfo[] files = info.GetFiles();
		foreach (FileInfo file in files)
		{
			string srcFile = srcDir + file.Name;
			string dstFile = dstDir + file.Name;
			if(srcFile.Contains(".xml"))
			{
				Log.Debug("srcFile:" + srcFile + " dstFile:" + dstFile);
				File.Copy(srcFile, dstFile, true);
			}
		}
#endif
	}
}
