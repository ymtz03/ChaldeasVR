using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleculeController : MonoBehaviour {

    [SerializeField]
    MenuManager menuManager;

    [SerializeField]
    MolecularStructureController MolStructureCtr;

    [SerializeField]
    MolecularOrbitalSurfaceController MOSurfaceCtr;

    float zoomrate = 1.0f;
    public int nMO { get; private set; }
    public int iMO { get; private set; }
    public int[] DsetIds { get; private set; }
    public bool[] IsLoaded { get; private set; }

    FchkData m_FchkData;
    CubeData m_CubeData;

	// Use this for initialization
	void Start () {
        iMO = -1;
        DsetIds = new int[0];
        IsLoaded = new bool[0];
	}

    // Update is called once per frame
    void Update()
    {
        var cameraForward = Camera.main.transform.forward;
        //var rotOfCamera = Quaternion.FromToRotation(initialCameraVec, nowCameraVec);
        var rotOfCamera = Quaternion.FromToRotation(new Vector3(0, 0, 1), cameraForward);
        var rotY = rotOfCamera.eulerAngles.y;
        var cy2 = cameraForward.y * cameraForward.y;
        var rotPitch = System.Math.Atan(cameraForward.y / System.Math.Sqrt(1 - cy2)) * 180.0 / System.Math.PI;

        //TextController.SetText(rotPitch.ToString() + "  " + cameraForward.ToString() + "  " + rotPitch.ToString());

        transform.rotation = Quaternion.Euler(0, -rotY * 3, 0);
        transform.rotation *= Quaternion.Euler((float)rotPitch * 3, 0, 0);

        //var invrot = Quaternion.Inverse(rotOfCamera);
        //this.transform.rotation = invrot * invrot * invrot;
        //this.transform.rotation = rotOfCamera;//invrot;

        var cameraRollY = Camera.main.transform.right.y;
        //var rotRoll = System.Math.Atan();
        if (cameraRollY > +0.2) { Zoom(+0.5f); }
        if (cameraRollY < -0.2) { Zoom(-0.5f); }

        transform.position = Camera.main.transform.position + cameraForward * 10;
    }

    public void Zoom(float diff)
    {
        zoomrate += diff * Time.deltaTime;
        zoomrate = System.Math.Max(zoomrate, 0.2f);
        zoomrate = System.Math.Min(zoomrate, 2.0f);
        transform.localScale = new Vector3(zoomrate, zoomrate, zoomrate);
    }

    public void LoadX3D(string filefullpath){
        //MOSurfaceCtr.GetComponent<MolecularOrbitalSurfaceController>().LoadX3DAndCreateMOMesh(filefullpath);
        MolStructureCtr.DestroyMolecule();
        nMO = 0;
    }

    public void LoadFchk(string filefullpath){
        m_FchkData = FchkData.Load(filefullpath);

        nMO = System.Math.Min(m_FchkData.NAtomicOrbs,10);
        iMO = 0;
        var iLoadMO = new int[nMO];
        for (int i = 0; i < nMO; ++i){ iLoadMO[i] = i; }
        m_CubeData = CubeGenerator.CubeGen(m_FchkData, iLoadMO, 60);

        MolStructureCtr.GetComponent<MolecularStructureController>().SetStructure(m_CubeData);
        DrawSurface(iMO);
    }

    public IEnumerator LoadFchkCoroutine(string filefullpath){
        FchkData new_fchkData = new FchkData();

        foreach (float progress_parcent in new_fchkData.LoadCoroutine(filefullpath, 10000)){
            TextController.SetText(string.Format("Loading fchk... {0,6:P1}", progress_parcent));
            yield return null;
        }

        int nMO_ = new_fchkData.NAtomicOrbs;
        //int nMO_initialLoad = System.Math.Min(nMO_,10);
        //var iLoadMO = new int[nMO_initialLoad];
        //for (int i = 0; i < nMO_initialLoad; ++i) { iLoadMO[i] = i; }

        //m_CubeData = CubeGenerator.CubeGen(m_FchkData, iLoadMO, 60);
        //CubeData new_cubeData = new CubeData();
        //foreach(float progress_parcent in CubeGenerator.CubeGen(new_cubeData, new_fchkData, iLoadMO, 60)){
        //    TextController.SetText(string.Format("Generating cube... {0,6:P1}", progress_parcent));
        //    yield return null;
        //}

        m_FchkData = new_fchkData;
        //m_CubeData = new_cubeData;
        m_CubeData = CubeGenerator.CubeGen_noMO(m_FchkData, 60);

        nMO = nMO_;
        iMO = 0;
        DsetIds = new int[nMO];
        for (int i = 0; i < nMO; ++i) { DsetIds[i] = i + 1; }
        IsLoaded = new bool[nMO];
        //for (int i = 0; i < nMO_initialLoad; ++i) { IsLoaded[i] = true; }

        MolStructureCtr.SetStructure(m_CubeData);
        MOSurfaceCtr.SetEmptyMesh();
        //MOSurfaceCtr.gameObject.GetComponent<Renderer>().
        //DrawSurface();
        menuManager.SetDsetIdToSlider();
    }

    public IEnumerable<float> CubeGenCoroutine_Core(int[] iLoadMO){
        CubeData new_cubeData = new CubeData();
        foreach (float progress_parcent in CubeGenerator.CubeGen(new_cubeData, m_FchkData, iLoadMO, 60)){
            //TextController.SetText(string.Format("Generating cube... {0,6:P1}", progress_parcent));
            yield return progress_parcent;
        }

        // insert new dsetIds and data to m_cubedata
        for (int i = 0; i < iLoadMO.Length; ++i){
            var new_dsetId = new_cubeData.DsetIds[i];
            var new_data = new_cubeData.Data[i];
            int j = 0;
            for (; j < m_CubeData.DsetIds.Count && new_dsetId < m_CubeData.DsetIds[j]; ++j) { }
            m_CubeData.DsetIds.Insert(j, new_dsetId);
            m_CubeData.Data.Insert(j, new_data);
        }

        foreach (int iMO_ in iLoadMO) { IsLoaded[iMO_] = true; }
    }

    public IEnumerator CubeGenCoroutine(int[] iLoadMO){
        foreach (float progress_parcent in CubeGenCoroutine_Core(iLoadMO)){
            TextController.SetText(string.Format("Generating cube... {0,6:P1}", progress_parcent));
            yield return null;
        }

        iMO = iLoadMO[0];
        DrawSurface(iMO);
        menuManager.UpdateSliderColor();
    }

    /*
    public void LoadCube(string filefullpath){
        //var cubeData = CubeData.Load(filefullpath);
        m_CubeData = CubeData.LoadNew(filefullpath);
        nMO = m_CubeData.Data.Count;
        iMO = 0;

        MolecularStructure.GetComponent<MolecularStructureController>().SetStructure(m_CubeData);
        DrawSurface();
    }
    */

    public IEnumerator LoadCubeCoroutine(string filefullpath){
        CubeData new_cubeData = new CubeData();

        foreach(float progress_parcent in new_cubeData.LoadCoroutine(filefullpath)){
            TextController.SetText(string.Format("Loading cube... {0,6:P1}",progress_parcent));
            yield return null;
        }

        //yield return StartCoroutine(new_cubeData.LoadCoroutine(filefullpath));

        m_CubeData = new_cubeData;
        nMO = m_CubeData.Data.Count;
        iMO = 0;
        DsetIds = m_CubeData.DsetIds.ToArray();
        IsLoaded = new bool[nMO];
        for (int i = 0; i < nMO; ++i) { IsLoaded[i] = true; }

        //MolStructureCtr.GetComponent<MolecularStructureController>().SetStructure(m_CubeData);
        MolStructureCtr.SetStructure(m_CubeData);
        DrawSurface(iMO);
        menuManager.SetDsetIdToSlider();
    }

    public void DrawSurface(int iMO){
        if (IsLoaded[iMO]){
            var dsetId = DsetIds[iMO];
            var kMO = m_CubeData.DsetIds.IndexOf(dsetId);

            MOSurfaceCtr.GenerateSurfaceFromCubeData(m_CubeData, kMO);

            this.iMO = iMO;
        }
        else{
            TextController.SetText(iMO.ToString() + " is not loaded.");
            StartCoroutine(CubeGenCoroutine(new int[] { iMO }));
        }
        //TextController.SetText("MO" + m_CubeData.DsetIds[iMO]);
    }

    public void ChangeSurface(){
        if (nMO>0){
            iMO = (iMO + 1) % nMO;
            DrawSurface(iMO);
        }
    }

    public void ChangeSurface(int iMO){
        if (0 <= iMO && iMO < nMO){
            DrawSurface(iMO);
        }
    }

}
