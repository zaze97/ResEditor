using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Whir.Software.Framework;

public static class FileGet
{
    /// <summary>
    /// 将字符串转为大写或小写
    /// </summary>
    /// <param name="tmp"></param>
    /// <param name="isUpper"></param>
    /// <returns></returns>
    public static string GetUpperOrLower(this string tmp, bool isUpper = true)
    {
        if (isUpper)
            return tmp.ToLower();
        else
            return tmp.ToUpper();
    }

    public static void CreateContent(StringBuilder variable, string path, string tab)
    {
        tab += "\t";
        DirectoryInfo dir = new DirectoryInfo(path);
        DirectoryInfo[] first = dir.GetDirectories();
        FileInfo[] fil = dir.GetFiles();
        if (first.Length > 0)
        {
            foreach (FileInfo f in fil)
            {
                CreateFile(variable, f, path, tab);
            }

            foreach (DirectoryInfo f in first)
            {
                CreateFolder(variable, f, tab);
            }
        }
        else
        {
            foreach (FileInfo f in fil)
            {
                CreateFile(variable, f, path, tab);
            }
        }
    }

    /// <summary>
    /// 生成脚本文件
    /// </summary>
    /// <param name="className">类名</param>
    /// <param name="content">字符串事件</param>
    public static void CreateClass(string className, Action<StringBuilder> content = null)
    {
        string tdClassName = className;
        StringBuilder variable = new StringBuilder();
        //variable.Append("using System;\n");
        variable.Append("namespace " + "R" + "\n");
        variable.Append("{\n");
        variable.Append("\tpublic partial class " + tdClassName + "\n");
        variable.Append("\t{\n");

        content?.Invoke(variable);

        variable.Append("\t}\n");
        variable.Append("}");
        string dirName = Application.dataPath + "/Develop/";
        string outputPath = dirName + tdClassName + ".cs";
        FileHelper.WriteFile(outputPath, variable.ToString());

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 文件夹生成
    /// </summary>
    /// <param name="folderName"></param>
    private static void CreateFolder(StringBuilder variable, DirectoryInfo folderName, string tab)
    {
        string classname = ChineseToPinYin(folderName.Name, false);
        variable.Append(tab + " public static class " + classname + "\n");
        variable.Append(tab + "{\n");
        CreateContent(variable, folderName.FullName, tab);

        variable.Append(tab + "}\n");
    }

    /// <summary>
    /// 文件生成
    /// </summary>
    /// <param name="fileName"></param>
    private static void CreateFile(StringBuilder variable, FileInfo fileName, string path, string tab)
    {
        Debug.Log("FileInfo：" + fileName.Name);
        if (System.IO.Path.GetExtension(fileName.FullName) != ".meta")
        {
            string filepath = fileName.FullName.Substring(path.IndexOf("Resources") + 10).Replace("\\", "/");
            filepath = DeleteLastIndex(filepath, ".");
            Debug.Log(filepath);
            string name = ChineseToPinYin(fileName.Name.Replace(".", "_"));
            variable.Append(tab + "public const string " + name + " = \"" + filepath + "\";\n");
        }
    }

    /// <summary>
    /// 判断是否有中文，有转为全拼
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string ChineseToPinYin(string text, bool isfile = true)
    {
        if (HasChinese(text))
        {
            return ConvertPinYin(text, isfile).GetUpperOrLower().Replace(" ", "");
        }
        else
        {
            if (isfile)
                text = String.Concat("_", text);
            return text.GetUpperOrLower();
        }
    }

    /// <summary>
    /// 判断字符串中是否有中文
    /// </summary>
    private static bool HasChinese(string s)
    {
        return Regex.IsMatch(s, "[\u4e00-\u9fbb]");
    }

    /// <summary>
    /// 汉字转全拼
    /// </summary>
    private static string ConvertPinYin(string text, bool isfile = true)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        try
        {
            var sb = new StringBuilder();
            bool isfirstchinese=false;
            if (HasChinese(text.ToList()[0].ToString()))
            {
                isfirstchinese = true;
                Debug.Log(text.ToList()[0].ToString()+":"+isfirstchinese);
            }
            for (int i = 0; i < text.ToList().Count; i++)
            {
                if (text.ToList()[i] <= 127)
                    sb.Append(text.ToList()[i]);
                else
                    sb.Append($"_{NPinyin.Pinyin.GetPinyin(text.ToList()[i])}_");
            }

            var name = sb.ToString().Replace("__", "_");
            if (!isfile) //裁剪首尾字符“_”
            {
                name = name.Trim('_');
            }
            else
            {
                name = name.TrimEnd('_');
                if(!isfirstchinese)
                    name = String.Concat("_", name);
            }

            return name;
        }
        catch (Exception e)
        {
            Debug.LogError($"拼音转换失败：{text}  {e.Message}");
        }

        return text;
    }

    /// <summary>
    /// 删除指定字符后的字符串
    /// </summary>
    /// <param name="fileName">字符串</param>
    /// <param name="lastIndex">符号例如：“ .”</param>
    /// <returns></returns>
    public static string DeleteLastIndex(string fileName, string lastIndex)
    {
        int index = fileName.LastIndexOf(lastIndex);
        if (index >= 0)
        {
            fileName = fileName.Substring(0, index);
        }

        return fileName;
    }
}