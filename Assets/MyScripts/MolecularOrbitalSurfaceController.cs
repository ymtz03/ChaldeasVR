using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MolecularOrbitalSurfaceController : MonoBehaviour {

    [SerializeField]
    MeshFilter meshFilter;

    [SerializeField]
    GameObject Sphere;

    public void LoadX3DAndCreateMOMesh(string filefullpath)
    {
        var mesh = new Mesh();

        string xmlString = null;
        var fileInfo = new System.IO.FileInfo(filefullpath);

        // Read X3D file
        try
        {
            using (System.IO.StreamReader sr = new System.IO.StreamReader(fileInfo.OpenRead(), System.Text.Encoding.UTF8))
            {
                xmlString = sr.ReadToEnd();
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("Error raised in reading file -> " + e.Message);
        }

        // Parse X3D string
        try
        {
            Debug.Log("ReadToEnd -> " + xmlString);
            var xmlDoc = new System.Xml.XmlDocument
            {
                XmlResolver = new System.Xml.XmlUrlResolver()
            };
            //xmlDoc.PreserveWhitespace = true;
            xmlDoc.LoadXml(xmlString);
            //xmlDoc.Load(new System.IO.StringReader(xmlString));

            System.Xml.XmlNode root = xmlDoc.DocumentElement;//xmlDoc.FirstChild;
            System.Xml.XmlNodeList shapes = xmlDoc.GetElementsByTagName("Shape");
            foreach (System.Xml.XmlNode shape in shapes)
            {
                System.Xml.XmlNodeList shapechildren = shape.ChildNodes;
                string coordIndexStr = null;
                string pointStr = null;
                string colorStr = null;
                string normalVecStr = null;

                foreach (System.Xml.XmlNode shapechild in shapechildren)
                {
                    if (shapechild.Name != "IndexedFaceSet") { continue; }
                    var ifs = shapechild; //Rename ("ifs" stands for "indexedFaceSet")

                    coordIndexStr = ifs.Attributes["coordIndex"].Value;

                    System.Xml.XmlNodeList ifschildren = ifs.ChildNodes;
                    foreach (System.Xml.XmlNode ifschild in ifschildren)
                    {
                        switch (ifschild.Name)
                        {
                            case "Coordinate":
                                pointStr = ifschild.Attributes["point"].Value;
                                break;
                            case "ColorRGBA":
                                colorStr = ifschild.Attributes["color"].Value;
                                break;
                            case "Normal":
                                normalVecStr = ifschild.Attributes["vector"].Value;
                                break;
                            default:
                                break;
                        } //End switch(ifschild.Name)
                    } //End foreach(ifschild in ifschildren)

                    Debug.Log("coordIndexStr -> " + coordIndexStr);
                    Debug.Log("pointStr -> " + pointStr);
                    Debug.Log("colorStr -> " + colorStr);
                    Debug.Log("normalVecStr -> " + normalVecStr);

                    string[] coordIndexStrList = coordIndexStr.Split(' ');
                    string[] pointStrList = pointStr.Split(' ');
                    string[] colorStrList = colorStr.Split(' ');
                    string[] normalVecStrList = normalVecStr.Split(' ');

                    var coordIndexList = new List<int>();
                    var pointList = new List<Vector3>();
                    var colorList = new List<Color>();
                    var normalVecList = new List<Vector3>();

                    for (int i = 0; i < coordIndexStrList.Length; ++i)
                    {
                        if (i % 4 != 3) coordIndexList.Add(int.Parse(coordIndexStrList[i]));
                    }

                    //foreach (string tmp in coordIndexStrList)
                    //    coordIndexList.Add(int.Parse(tmp));
                    for (int i = 0; i < pointStrList.Length / 3; ++i)
                    {
                        var p = new Vector3(
                            float.Parse(pointStrList[i * 3]),
                            float.Parse(pointStrList[i * 3 + 1]),
                            float.Parse(pointStrList[i * 3 + 2])
                        );
                        pointList.Add(p);
                    }
                    for (int i = 0; i < colorStrList.Length / 4; ++i)
                    {
                        var c = new Color(
                            float.Parse(colorStrList[i * 4]),
                            float.Parse(colorStrList[i * 4 + 1]),
                            float.Parse(colorStrList[i * 4 + 2]),
                            /*float.Parse(colorStrList[i * 4 + 3])*/0.5f
                        );
                        colorList.Add(c);
                    }
                    for (int i = 0; i < normalVecStrList.Length / 3; ++i)
                    {
                        var nv = new Vector3(
                            float.Parse(normalVecStrList[i * 3]),
                            float.Parse(normalVecStrList[i * 3 + 1]),
                            float.Parse(normalVecStrList[i * 3 + 2])
                        );
                        normalVecList.Add(nv);
                    }

                    mesh.SetVertices(pointList);//meshに頂点群をセット
                    //mesh.SetUVs(0, uvList);//meshにテクスチャのuv座標をセット（今回は割愛)
                    mesh.SetIndices(coordIndexList.ToArray(), MeshTopology.Triangles, 0);//メッシュにどの頂点の順番で面を作るかセット
                    mesh.SetColors(colorList);
                    //mesh.SetNormals(normalVecList);

                    mesh.RecalculateBounds();
                    mesh.RecalculateNormals();
                    mesh.RecalculateTangents();

                    Debug.Log("CoordIndexStrList.Count -> " + coordIndexStrList.Length);
                    Debug.Log("CoordIndexList.Count -> " + coordIndexList.Count);

                } //End foreach(ifs in shape)
            }// End foreach(shape in shapes)


            Debug.Log("Root : " + root.Name);

        }
        catch (System.Exception e)
        {

            Debug.Log("Error raised in parsing xml string");
            Debug.Log("Message -> " + e.Message);
            Debug.Log("HelpLink -> " + e.HelpLink);
            Debug.Log("ToString -> " + e.ToString());
        }

        meshFilter.mesh = mesh;
    }

    public void GenerateSurfaceFromCubeData(CubeData cubeData, int iMO){
        Debug.Log("Begin GenerateSurfaceFromCubeData");
        var data = cubeData.Data;
        //var moVal = new double[data.GetLength(0), data.GetLength(1), data.GetLength(2)];
        //for (int ix = 0; ix < data.GetLength(0); ++ix)
        //for (int iy = 0; iy < data.GetLength(1); ++iy)
        //for (int iz = 0; iz < data.GetLength(2); ++iz)
        //    moVal[ix, iy, iz] = data[ix, iy, iz, iMO];

        var surfacePosi = IsosurfaceGenerator.IsosurfaceGen(data[iMO], cubeData.Origin, cubeData.Axes, +0.08, false);
        var surfaceNega = IsosurfaceGenerator.IsosurfaceGen(data[iMO], cubeData.Origin, cubeData.Axes, -0.08, true);

        var vertexList = new List<Vector3>();
        var indexList = new List<int>();
        var colorList = new List<Color32>();

        foreach (double[] vertPosi in surfacePosi.VertCoordinates){
            vertexList.Add(new Vector3((float)vertPosi[0], (float)vertPosi[1], (float)vertPosi[2]));
            colorList.Add(new Color32(255, 0, 0, 255));
        }
        foreach(double[] vertNega in surfaceNega.VertCoordinates){
            vertexList.Add(new Vector3((float)vertNega[0], (float)vertNega[1], (float)vertNega[2]));
            colorList.Add(new Color32(0, 0, 255, 255));
        }

        foreach (int[] indexPosi in surfacePosi.Polygons){
            indexList.Add(indexPosi[0]);
            indexList.Add(indexPosi[1]);
            indexList.Add(indexPosi[2]);
        }
        var nVertPosi = surfacePosi.VertCoordinates.Count;
        foreach (int[] indexNega in surfaceNega.Polygons){
            indexList.Add(indexNega[0] + nVertPosi);
            indexList.Add(indexNega[1] + nVertPosi);
            indexList.Add(indexNega[2] + nVertPosi);
        }

        var mesh = new Mesh();
        mesh.SetVertices(vertexList);//meshに頂点群をセット
        mesh.SetIndices(indexList.ToArray(), MeshTopology.Triangles, 0);//メッシュにどの頂点の順番で面を作るかセット
        mesh.SetColors(colorList);

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        meshFilter.mesh = mesh;
        //meshFilter.mesh = CreatePlaneMesh();

        Debug.Log("End GenerateSurfaceFromCubeData");
        Debug.Log("Length surfaceNega.VertCoordinates : " + surfaceNega.VertCoordinates.Count);
        Debug.Log("Length surfaceNega.Polygons : " + surfaceNega.Polygons.Count);
        Debug.Log("Length vertexList : " + vertexList.Count);
        Debug.Log("Length indexList : " + indexList.Count);
        Debug.Log("Length colorList : " + colorList.Count);
    }

    public void SetEmptyMesh(){ meshFilter.mesh = new Mesh(); }
}
