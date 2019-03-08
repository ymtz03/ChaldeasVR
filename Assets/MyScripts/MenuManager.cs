using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {

    [SerializeField]
    UnityEngine.UI.Text uiToggleSwitchText;

    [SerializeField]
    GameObject Molecule;


    [SerializeField]
    GameObject[] slider_file;

    [SerializeField]
    UnityEngine.UI.Text fileSliderPageNumber;


    [SerializeField]
    GameObject[] slider_MO;

    [SerializeField]
    UnityEngine.UI.Text moSliderPageNumber;


    string pwdPath;
    List<string> filenamelist;
    int nPage_file;
    int currentPage_file;

    int nPage_MO;
    int currentPage_MO;

    public static bool exist_running_coroutine;

	// Use this for initialization
    void Start () {
        pwdPath = Application.persistentDataPath;
        filenamelist = GetFilenamelist(pwdPath);
        Debug.Log(pwdPath);
        nPage_file = filenamelist.Count / slider_file.Length + 1;
        currentPage_file = 0;

        foreach (GameObject gobj in slider_file)
        {
            gobj.transform.Find("Text").gameObject
                .GetComponent<UnityEngine.UI.Text>()
                .text = "NO FILE";
        }

        SetFilenameToSlider();
        ChangePage_file(0);

        SetDsetIdToSlider();
        ChangePage_MO(0);
	}
	
    void SetFilenameToSlider(){
        for (int i = 0; i < slider_file.Length; ++i)
        {
            int ifile = currentPage_file * slider_file.Length + i;
            
            if (ifile < filenamelist.Count){
                slider_file[i].SetActive(true);

                var uitext = slider_file[i].transform.Find("Text")
                           .gameObject
                           .GetComponent<UnityEngine.UI.Text>();
                string[] filename_split = filenamelist[ifile].Split('/');
                uitext.text = filename_split[filename_split.Length - 1];
                uitext.fontSize = 50;
            }else{
                slider_file[i].SetActive(false);
            }
        }
    }

	// Update is called once per frame
	void Update () {
		
	}

    List<string> GetFilenamelist(string directory){
        var retval = new List<string>();

        string[] files = System.IO.Directory.GetFiles(directory);
        foreach(var filefullpath in files){
            var extension = System.IO.Path.GetExtension(filefullpath);

            if(extension == ".cube" ||
               extension == ".fchk" ||
               extension == ".x3d")
                retval.Add(filefullpath);
        }

        return retval;
    }

    public void LoadFile(int sliderid){
        var ifile = currentPage_file * slider_file.Length + sliderid;
        Debug.Log("UpdateMO : " + ifile + " : " + filenamelist[ifile]);

        if (!(ifile < filenamelist.Count)) { return; } 
            
        var filefullpath = filenamelist[ifile];
        var extension = System.IO.Path.GetExtension(filefullpath);
        TextController.SetText("Extensiton : " + extension);

        var molCtr = Molecule.GetComponent<MoleculeController>();
        switch (extension)
        {
            case ".x3d":
                molCtr.LoadX3D(filefullpath);
                break;
            case ".fchk":
                //moleculeController.LoadFchk(filefullpath);
                if (exist_running_coroutine) { return; }
                molCtr.StartCoroutine(molCtr.LoadFchkCoroutine(filefullpath));
                exist_running_coroutine = true;
                break;
            case ".cube":
                //Molecule.GetComponent<MoleculeController>().LoadCube(filefullpath);
                if (exist_running_coroutine) { return; }
                molCtr.StartCoroutine(molCtr.LoadCubeCoroutine(filefullpath));
                exist_running_coroutine = true;
                break;
        }
    }

    public void ChangePage_file(int diff){
        if (nPage_file == 0) { return; }

        var newPage = (currentPage_file + diff + nPage_file) % nPage_file;
        Debug.Log("currentPage changed from " + currentPage_file + " to " + newPage + "  nPage="+nPage_file);
        if(newPage != currentPage_file){
            currentPage_file = newPage;
            SetFilenameToSlider();
        }
        fileSliderPageNumber.text = string.Format("{0,2} / {1,2}", currentPage_file + 1, nPage_file);
    }

    public void ToggleDraw(){
        gameObject.SetActive(!gameObject.activeSelf);

        if (gameObject.activeSelf){
            uiToggleSwitchText.text = "Hide Menu";
        }else{
            uiToggleSwitchText.text = "Open Menu";
        }
    }

    public void SetDsetIdToSlider(){
        var molCtr = Molecule.GetComponent<MoleculeController>();
        nPage_MO = molCtr.nMO / slider_MO.Length + 1;
        SetPageNumber_MO();


        for (int i = 0; i < slider_MO.Length; ++i){
            int iMO = currentPage_MO * slider_MO.Length + i;

            if (iMO < molCtr.DsetIds.Length)
            {
                slider_MO[i].SetActive(true);

                var uitext = slider_MO[i].transform.Find("Text")
                                         .gameObject
                                         .GetComponent<UnityEngine.UI.Text>();
                //string[] filename_split = filenamelist[ifile].Split('/');
                uitext.text = "MO" + molCtr.DsetIds[iMO].ToString();
                uitext.fontSize = 50;

                var panelColor = new Color(0.5f, 0.5f, 0.5f);
                if (molCtr.IsLoaded[iMO]) { panelColor = new Color(1f, 1f, 1f); }
                if (iMO == molCtr.iMO) { panelColor = new Color(0.8f, 1f, 0.8f); }
                slider_MO[i].transform.Find("Background").gameObject
                            .GetComponent<UnityEngine.UI.Image>()
                            .color = panelColor;
            }
            else{
                slider_MO[i].SetActive(false);
            }
        }

        UpdateSliderColor();
    }

    public void UpdateSliderColor(){
        var molCtr = Molecule.GetComponent<MoleculeController>();
        nPage_MO = molCtr.nMO / slider_MO.Length + 1;
        SetPageNumber_MO();

        for (int i = 0; i < slider_MO.Length; ++i){
            int iMO = currentPage_MO * slider_MO.Length + i;
            if (iMO >= molCtr.DsetIds.Length){ continue; }
          
            var panelColor = new Color(1f, 1f, 1f);
            if (iMO == molCtr.iMO) { panelColor = new Color(0.8f, 1f, 0.8f); }
            if (!molCtr.IsLoaded[iMO]) { panelColor = new Color(0.5f, 0.5f, 0.5f); }
            slider_MO[i].transform.Find("Background").gameObject
                        .GetComponent<UnityEngine.UI.Image>()
                        .color = panelColor;
        }
    }

    public void ChangePage_MO(int diff)
    {
        var newPage = currentPage_MO;
        if (nPage_MO > 0){
            newPage = (currentPage_MO + diff + nPage_MO) % nPage_MO;
        }

        if (newPage != currentPage_MO){
            Debug.Log("currentPage changed from " + currentPage_MO + " to " + newPage + "  nPage=" + nPage_MO);
            currentPage_MO = newPage;
            SetDsetIdToSlider();
        }

        //moSliderPageNumber.text = string.Format("{0,2} / {1,2}", currentPage_MO + 1, nPage_MO);
        SetPageNumber_MO();
    }

    void SetPageNumber_MO(){
        var v = currentPage_MO + 1;
        if (nPage_MO == 0) { v = 0; }
        moSliderPageNumber.text = string.Format("{0,2} / {1,2}", v, nPage_MO);
    }

    public void UpdateMO(int sliderid){
        var iMO = currentPage_MO * slider_MO.Length + sliderid;
        Debug.Log("UpdateMO : " + iMO);

        var molCtr = Molecule.GetComponent<MoleculeController>();
        molCtr.DrawSurface(iMO);

        UpdateSliderColor();
    }

}
