using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Whir.Software.Framework;

public class ResourcesEditor
{
    public static List<string> fileList=new List<string>();
     [MenuItem("WAutoSet/一键Res")]
     public static void CreateRes()
     {
         FileGet.CreateClass("Res", (a) => {FileGet.CreateContent(a,Application.dataPath + "/Develop/Resources","\t");});
     }
     
}
