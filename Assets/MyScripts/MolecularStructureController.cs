using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MolecularStructureController : MonoBehaviour
{

    [SerializeField]
    GameObject Sphere;

    [SerializeField]
    GameObject Line;

    GameObject[] AtomList;
    GameObject[] BondList;

    readonly float[] Radius = {
        0.5f, 0.6f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, //   1-10
        1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, //  11-20
        1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, //  21-30
        1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, //  31-40
        1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, //  41-40
        1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, //  51-40
        1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, //  61-40
        1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, //  71-40
        1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, //  81-40
        1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, //  91-40
        1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, // 101-110
        1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, // 111-120
        1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, // 121-130
        1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, // 131-140
        1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f  // 141-150
    };

    readonly Color[] AtomColorMap = 
    {
        new Color(0.07f, 0.50f, 0.70f), // Dummy
    	new Color(0.75f, 0.75f, 0.75f), // Hydrogen
    	new Color(0.85f, 1.00f, 1.00f), // Helium
    	new Color(0.80f, 0.50f, 1.00f), // Lithium
    	new Color(0.76f, 1.00f, 0.00f),
        new Color(1.00f, 0.71f, 0.71f),
        new Color(0.40f, 0.40f, 0.40f),
        new Color(0.05f, 0.05f, 1.00f),
        new Color(1.00f, 0.05f, 0.05f),
        new Color(0.50f, 0.70f, 1.00f),
        new Color(0.70f, 0.89f, 0.96f),
        new Color(0.67f, 0.36f, 0.95f),
        new Color(0.54f, 1.00f, 0.00f),
        new Color(0.75f, 0.65f, 0.65f),
        new Color(0.50f, 0.60f, 0.60f),
        new Color(1.00f, 0.50f, 0.00f),
        new Color(0.70f, 0.70f, 0.00f),
        new Color(0.12f, 0.94f, 0.12f),
        new Color(0.50f, 0.82f, 0.89f),
        new Color(0.56f, 0.25f, 0.83f),
        new Color(0.24f, 1.00f, 0.00f),
        new Color(0.90f, 0.90f, 0.90f),
        new Color(0.75f, 0.76f, 0.78f),
        new Color(0.65f, 0.65f, 0.67f),
        new Color(0.54f, 0.60f, 0.78f),
        new Color(0.61f, 0.48f, 0.78f),
        new Color(0.88f, 0.40f, 0.20f),
        new Color(0.94f, 0.56f, 0.63f),
        new Color(0.31f, 0.82f, 0.31f),
        new Color(0.78f, 0.50f, 0.20f),
        new Color(0.49f, 0.50f, 0.69f),
        new Color(0.76f, 0.56f, 0.56f),
        new Color(0.40f, 0.56f, 0.56f),
        new Color(0.74f, 0.50f, 0.89f),
        new Color(1.00f, 0.63f, 0.00f),
        new Color(0.65f, 0.16f, 0.16f),
        new Color(0.36f, 0.72f, 0.82f),
        new Color(0.44f, 0.18f, 0.69f),
        new Color(0.00f, 1.00f, 0.00f),
        new Color(0.58f, 1.00f, 1.00f),
        new Color(0.58f, 0.88f, 0.88f),
        new Color(0.45f, 0.76f, 0.79f),
        new Color(0.33f, 0.71f, 0.71f),
        new Color(0.23f, 0.62f, 0.62f),
        new Color(0.14f, 0.56f, 0.56f),
        new Color(0.04f, 0.49f, 0.55f),
        new Color(0.00f, 0.41f, 0.52f),
        new Color(0.88f, 0.88f, 1.00f),
        new Color(1.00f, 0.85f, 0.56f),
        new Color(0.65f, 0.46f, 0.45f),
        new Color(0.40f, 0.50f, 0.50f),
        new Color(0.62f, 0.39f, 0.71f),
        new Color(0.83f, 0.48f, 0.00f),
        new Color(0.58f, 0.00f, 0.58f),
        new Color(0.26f, 0.62f, 0.69f),
        new Color(0.34f, 0.09f, 0.56f),
        new Color(0.00f, 0.79f, 0.00f),
        new Color(0.44f, 0.83f, 1.00f),
        new Color(1.00f, 1.00f, 0.78f),
        new Color(0.85f, 1.00f, 0.78f),
        new Color(0.78f, 1.00f, 0.78f),
        new Color(0.64f, 1.00f, 0.78f),
        new Color(0.56f, 1.00f, 0.78f),
        new Color(0.38f, 1.00f, 0.78f),
        new Color(0.27f, 1.00f, 0.78f),
        new Color(0.19f, 1.00f, 0.78f),
        new Color(0.12f, 1.00f, 0.78f),
        new Color(0.00f, 1.00f, 0.61f),
        new Color(0.00f, 0.90f, 0.46f),
        new Color(0.00f, 0.83f, 0.32f),
        new Color(0.00f, 0.75f, 0.22f),
        new Color(0.00f, 0.67f, 0.14f),
        new Color(0.30f, 0.76f, 1.00f),
        new Color(0.30f, 0.65f, 1.00f),
        new Color(0.13f, 0.58f, 0.84f),
        new Color(0.15f, 0.49f, 0.67f),
        new Color(0.15f, 0.40f, 0.59f),
        new Color(0.09f, 0.33f, 0.53f),
        new Color(0.90f, 0.85f, 0.68f),
        new Color(0.80f, 0.82f, 0.12f),
        new Color(0.71f, 0.71f, 0.76f),
        new Color(0.65f, 0.33f, 0.30f),
        new Color(0.34f, 0.35f, 0.38f),
        new Color(0.62f, 0.31f, 0.71f),
        new Color(0.67f, 0.36f, 0.00f),
        new Color(0.46f, 0.31f, 0.27f),
        new Color(0.26f, 0.51f, 0.59f),
        new Color(0.26f, 0.00f, 0.40f),
        new Color(0.00f, 0.49f, 0.00f),
        new Color(0.44f, 0.67f, 0.98f),
        new Color(0.00f, 0.73f, 1.00f),
        new Color(0.00f, 0.63f, 1.00f),
        new Color(0.00f, 0.56f, 1.00f),
        new Color(0.00f, 0.50f, 1.00f),
        new Color(0.00f, 0.42f, 1.00f),
        new Color(0.33f, 0.36f, 0.95f),
        new Color(0.47f, 0.36f, 0.89f),
        new Color(0.54f, 0.31f, 0.89f),
        new Color(0.63f, 0.21f, 0.83f),
        new Color(0.70f, 0.12f, 0.83f),
        new Color(0.70f, 0.12f, 0.73f),
        new Color(0.70f, 0.05f, 0.65f),
        new Color(0.74f, 0.05f, 0.53f),
        new Color(0.78f, 0.00f, 0.40f),
        new Color(0.80f, 0.00f, 0.35f),
        new Color(0.82f, 0.00f, 0.31f),
        new Color(0.85f, 0.00f, 0.27f),
        new Color(0.88f, 0.00f, 0.22f),
        new Color(0.90f, 0.00f, 0.18f),
        new Color(0.92f, 0.00f, 0.15f),
        new Color(0.93f, 0.00f, 0.14f),
        new Color(0.94f, 0.00f, 0.13f),
        new Color(0.95f, 0.00f, 0.12f),
        new Color(0.96f, 0.00f, 0.11f),
        new Color(0.97f, 0.00f, 0.10f),
        new Color(0.98f, 0.00f, 0.09f),
        new Color(0.99f, 0.00f, 0.08f),
        new Color(0.99f, 0.00f, 0.07f),
        new Color(0.99f, 0.00f, 0.06f)
    };

    public void DestroyMolecule(){
        if (AtomList != null){
            foreach (GameObject obj in AtomList){
                Destroy(obj);
            }
            AtomList = null;
        }

        if(BondList != null){
            foreach(GameObject obj in BondList){
                Destroy(obj);
            }
            BondList = null;
        }
    }

    public void SetStructure(CubeData cubeData){
        Debug.Log("MolecularStructure::SetStructure");
        DestroyMolecule();

        AtomList = new GameObject[cubeData.NAtoms];

        var atomPositionVectors = new Vector3[cubeData.NAtoms];

        for (int iatom = 0; iatom < cubeData.NAtoms; ++iatom){
            var position = new Vector3(
                (float)cubeData.AtomPositions[iatom, 0],
                (float)cubeData.AtomPositions[iatom, 1],
                (float)cubeData.AtomPositions[iatom, 2]
            );
            atomPositionVectors[iatom] = position;
            
            var sphere = Instantiate(Sphere, transform);
            var r = Radius[cubeData.AtomicNumbers[iatom]];
            sphere.GetComponent<Renderer>().material.color = AtomColorMap[cubeData.AtomicNumbers[iatom]];
            sphere.transform.localPosition = position;
            sphere.transform.localScale = new Vector3(r,r,r);
            sphere.SetActive(true);

            Debug.Log("Add Sphere : "+ cubeData.AtomicNumbers[iatom].ToString() + " , " + position.ToString());

            AtomList[iatom] = sphere;
        }

        GenerateBonds(atomPositionVectors, cubeData.AtomicNumbers);
        //TextController.SetText("SetStructure");
    }

    void GenerateBonds(Vector3[] atomPositionVectors, int[] atomicNumbers)
    {
        Debug.Log("GenerateBonds");

        var bondList = new List<GameObject>();

        var nAtom = atomicNumbers.Length;
        var sqrDistanceMatrix = new double[nAtom, nAtom];

        for (int iatom = 0; iatom < nAtom; ++iatom)
        for (int jatom = 0; jatom < iatom; ++jatom){
            var sqrDistance = (atomPositionVectors[iatom] - atomPositionVectors[jatom]).sqrMagnitude;
            sqrDistanceMatrix[iatom, jatom] = sqrDistanceMatrix[jatom, iatom] = sqrDistance;
            Debug.Log("sqrDist" + iatom + "," + jatom + "," + sqrDistance);

            if (atomicNumbers[iatom] != 1 && atomicNumbers[jatom] != 1 && sqrDistance < 20){
                var bond = Instantiate(Line, transform);
                bond.SetActive(true);
                bond.GetComponent<LineRenderer>().SetPosition(0, atomPositionVectors[iatom]);
                bond.GetComponent<LineRenderer>().SetPosition(1, atomPositionVectors[jatom]);

                bondList.Add(bond);
                Debug.Log("Bond" + iatom + "," + jatom);
            }
        }

        for (int iatom = 0; iatom < nAtom; ++iatom){
            if (atomicNumbers[iatom] != 1) { continue; } // iatom must be Hydrogen
           
            var nearestNeighborAtom = -1;
            for (int jatom = 0; jatom < nAtom; ++jatom){
                if (atomicNumbers[jatom] == 1) { continue; } // jatom must not be Hydrogen

                if(nearestNeighborAtom == -1 || 
                   sqrDistanceMatrix[iatom,jatom] < sqrDistanceMatrix[iatom,nearestNeighborAtom]){
                    nearestNeighborAtom = jatom;
                }
            }

            Debug.Log("Nearest" + iatom + "," + nearestNeighborAtom);

            if(nearestNeighborAtom != -1 && sqrDistanceMatrix[iatom,nearestNeighborAtom] < 20){
                var bond = Instantiate(Line, transform);
                bond.SetActive(true);
                bond.GetComponent<LineRenderer>().SetPosition(0, atomPositionVectors[iatom]);
                bond.GetComponent<LineRenderer>().SetPosition(1, atomPositionVectors[nearestNeighborAtom]);

                bondList.Add(bond);
                Debug.Log("Bond" + iatom + "," + nearestNeighborAtom);
            }
        }

        BondList = bondList.ToArray();
    }

}
