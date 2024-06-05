using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class S_TrainingPentagon : MonoBehaviour
{
    //made following tutorial https://www.youtube.com/watch?v=twjMW7CxIKk&ab_channel=CodeMonkey
    public float pentagonSize = 145f;
    
    [Header("Debug")]
    [SerializeField] private float debugPentagonSize = -1f;

    [SerializeField] private float debugPentagonAtk = -1f;
    [SerializeField] private float debugPentagonDef = -1f;
    [SerializeField] private float debugPentagonChem = -1f;
    [SerializeField] private float debugPentagonFK = -1f;
    [SerializeField] private float debugPentagonRes = -1f;

    private CanvasRenderer cv;
    [SerializeField] private Material cvMaterial;
    [SerializeField] private Texture2D cvTexture;

    private Vector3 currAtk=Vector3.zero;
    private Vector3 currDef=Vector3.zero;
    private Vector3 currChem=Vector3.zero;
    private Vector3 currFK=Vector3.zero;
    private Vector3 currRes=Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        cv = GetComponent<CanvasRenderer>();
        //UpdateStatsVisual();
    }

    // Update is called once per frame
    void Update()
    {

        UpdateStatsVisual();

    }

    private void UpdateStatsVisual()
    {

        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[6]; //size = pentagon+1
        Vector2[] UVs = new Vector2[6];
        int[] triangles = new int[15]; //size = 3 * pentagon

        int angleIncrement = 360 / 5;

        int attackVertexIndex = 1;
        Vector3 atkVertex = Quaternion.Euler(0, 0, -angleIncrement * (attackVertexIndex-1)) * Vector3.up *  FindStatLength("atk");
        currAtk = S_CoolFuncs.V3Lerpoler(currAtk, atkVertex);

        int defVertexIndex = 2;
        Vector3 defVertex = Quaternion.Euler(0, 0, -angleIncrement * (defVertexIndex-1)) * Vector3.up * FindStatLength("def");
        currDef = S_CoolFuncs.V3Lerpoler(currDef, defVertex);

        int chemVertexIndex = 3;
        Vector3 chemVertex = Quaternion.Euler(0, 0, -angleIncrement * (chemVertexIndex - 1)) * Vector3.up * FindStatLength("chem");
        currChem = S_CoolFuncs.V3Lerpoler(currChem, chemVertex);

        int fkVertexIndex = 4;
        Vector3 fkVertex = Quaternion.Euler(0, 0, -angleIncrement * (fkVertexIndex - 1)) * Vector3.up * FindStatLength("fk");
        currFK = S_CoolFuncs.V3Lerpoler(currFK, fkVertex);


        int resVertexIndex = 5;
        Vector3 resVertex = Quaternion.Euler(0, 0, -angleIncrement * (resVertexIndex - 1)) * Vector3.up * FindStatLength("res");
        currRes = S_CoolFuncs.V3Lerpoler(currRes, resVertex);


        vertices[0] = Vector3.zero;
        vertices[attackVertexIndex] = currAtk;
        vertices[defVertexIndex] = currDef;
        vertices[chemVertexIndex] = currChem;
        vertices[fkVertexIndex] = currFK;
        vertices[resVertexIndex] = currRes;

        triangles[0] = 0;
        triangles[1] = attackVertexIndex;
        triangles[2] = defVertexIndex;

        triangles[3] = 0;
        triangles[4] = defVertexIndex;
        triangles[5] = chemVertexIndex;
        
        triangles[6] = 0;
        triangles[7] = chemVertexIndex;
        triangles[8] = fkVertexIndex;

        triangles[9] = 0;
        triangles[10] = fkVertexIndex;
        triangles[11] = resVertexIndex;

        triangles[12] = 0;
        triangles[13] = resVertexIndex;
        triangles[14] = attackVertexIndex;

        UVs[0] = Vector2.zero;
        UVs[attackVertexIndex] = Vector2.one;
        UVs[defVertexIndex] = Vector2.one;
        UVs[chemVertexIndex] = Vector2.one;
        UVs[fkVertexIndex] = Vector2.one;
        UVs[resVertexIndex] = Vector2.one;

        mesh.vertices = vertices;
        mesh.uv = UVs;
        mesh.triangles = triangles;

        cv.SetMesh(mesh);
        cv.SetMaterial(cvMaterial, cvTexture);
    }

    private float FindStatLength(string stat)
    {
        float val = 0;
        switch (stat)
        {
            case "atk":
                val=S_PlayerTeamStats.GetAtkBoost();
#if UNITY_EDITOR
                if (debugPentagonAtk != -1) val = debugPentagonAtk;
#endif
                break;

            case "def":
                val = S_PlayerTeamStats.GetDefBoost();
#if UNITY_EDITOR
                if (debugPentagonDef != -1) val = debugPentagonDef;
#endif
                break;

            case "chem":
                val = S_PlayerTeamStats.GetChemistryBoost();
#if UNITY_EDITOR
                if (debugPentagonChem != -1) val = debugPentagonChem;
#endif
                break;

            case "fk":
                val = S_PlayerTeamStats.GetFreeKicksBoost();
#if UNITY_EDITOR
                if (debugPentagonFK != -1) val = debugPentagonFK;
#endif

                break;

            case "res":
                val = S_PlayerTeamStats.GetFitnessBoost();
#if UNITY_EDITOR
                if (debugPentagonRes != -1) val = debugPentagonRes;
#endif

                break;
        }
        val = Mathf.Lerp(pentagonSize/5, pentagonSize, val / (float)S_PlayerTeamStats.MAXBOOSTLEVEL);
#if UNITY_EDITOR
        if (debugPentagonSize != -1) val = debugPentagonSize;
#endif
        return val;
    }
}
