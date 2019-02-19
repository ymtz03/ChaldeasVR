using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyMesh : MonoBehaviour {

    [SerializeField]
    private MeshFilter meshFilter;

    private Mesh mesh;
    private List<Vector3> vertextList = new List<Vector3>();
    private List<Vector2> uvList = new List<Vector2>();
    private List<int> indexList = new List<int>();
    private List<Color32> colorList = new List<Color32>();

    private string loadPath;

    void Start()
    {
        mesh = CreatePlaneMesh();
        meshFilter.mesh = mesh;

        loadPath = UnityEngine.Application.persistentDataPath;
        Debug.Log("loadPath : " + loadPath);

        Writedata();

        //LoadX3DAndCreateMOMesh(loadPath + "/test.xml");
    }

    private Mesh CreatePlaneMesh()
    {
        var mesh = new Mesh();

        vertextList.Add(new Vector3(-1, -1, 0));//0番頂点
        vertextList.Add(new Vector3(1, -1, 0)); //1番頂点
        vertextList.Add(new Vector3(-1, 1, 0)); //2番頂点
        vertextList.Add(new Vector3(1, 1, 0));  //3番頂点

        uvList.Add(new Vector2(0, 0));
        uvList.Add(new Vector2(1, 0));
        uvList.Add(new Vector2(0, 1));
        uvList.Add(new Vector2(1, 1));

        indexList.AddRange(new[] { 0, 2, 1, 1, 2, 3 });//0-2-1の頂点で1三角形。 1-2-3の頂点で1三角形。

        colorList.Add(new Color32(255,   0,   0, 255));
        colorList.Add(new Color32(  0, 255,   0, 255));
        colorList.Add(new Color32(  0,   0, 255, 255));
        colorList.Add(new Color32(255, 255,   0, 255));

        mesh.SetVertices(vertextList);//meshに頂点群をセット
        mesh.SetUVs(0, uvList);//meshにテクスチャのuv座標をセット（今回は割愛)
        mesh.SetIndices(indexList.ToArray(), MeshTopology.Triangles, 0);//メッシュにどの頂点の順番で面を作るかセット
        mesh.SetColors(colorList);

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        //var normalList = new List<Vector3>();
        //normalList.Add(new Vector3(0f, 0f,  1f));
        //normalList.Add(new Vector3(0f, 0f, -1f));
        //mesh.SetNormals(normalList);

        return mesh;
    }

    //for debug
    private void Writedata()
    {
        Debug.Log("Begin writedata(for debug)");
        try{
            System.IO.StreamWriter sw;
            var fi = new System.IO.FileInfo(loadPath + "/samplefile.txt");
            using(sw = fi.AppendText()){
                sw.WriteLine("Vamos Yokohama");
                sw.Flush();
            }
        }catch(System.Exception e){
            Debug.Log("Error raised in writedada -> " + e.Message);
        }
        Debug.Log("End writedata(for debug)");
    }

    public Mesh LoadX3DAndCreateMOMesh(string filefullpath)
    {
        var mesh = new Mesh();

        string xmlString = null;
        //const string filename = "test.xml";
        //System.IO.FileInfo fileInfo = new System.IO.FileInfo(loadPath + "/" + filename);
        var fileInfo = new System.IO.FileInfo(filefullpath);

        // Read X3D file
        try{
            using (System.IO.StreamReader sr = new System.IO.StreamReader(fileInfo.OpenRead(), System.Text.Encoding.UTF8))
            {
                xmlString = sr.ReadToEnd();
            }
        }catch(System.Exception e){
            Debug.Log("Error raised in reading file -> " + e.Message);
        }

        // Parse X3D string
        try{
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
            foreach(System.Xml.XmlNode shape in shapes){
                System.Xml.XmlNodeList shapechildren = shape.ChildNodes;
                string coordIndexStr = null;
                string pointStr = null;
                string colorStr = null;
                string normalVecStr = null;

                foreach(System.Xml.XmlNode shapechild in shapechildren){
                    if (shapechild.Name != "IndexedFaceSet") { continue; }
                    var ifs = shapechild; //Rename ("ifs" stands for "indexedFaceSet")

                    coordIndexStr = ifs.Attributes["coordIndex"].Value;
                            
                    System.Xml.XmlNodeList ifschildren = ifs.ChildNodes;
                    foreach(System.Xml.XmlNode ifschild in ifschildren){
                        switch(ifschild.Name){
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

                    for (int i = 0; i < coordIndexStrList.Length; ++i){
                        if (i % 4 != 3) coordIndexList.Add(int.Parse(coordIndexStrList[i]));
                    }

                    //foreach (string tmp in coordIndexStrList)
                    //    coordIndexList.Add(int.Parse(tmp));
                    for (int i = 0; i < pointStrList.Length / 3; ++i){
                        var p = new Vector3(
                            float.Parse(pointStrList[i * 3    ]),
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

        }catch(System.Exception e){
            
            Debug.Log("Error raised in parsing xml string");
            Debug.Log("Message -> " + e.Message);
            Debug.Log("HelpLink -> " + e.HelpLink);
            Debug.Log("ToString -> " + e.ToString());
        }

        meshFilter.mesh = mesh;
        return mesh;
    }

    // Update is called once per frame
    void Update()
    {
       //this.transform.position
       // = Camera.main.transform.position
       // + Camera.main.transform.forward * 10.0f;
    }

}
