using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent_Main_Pre_Agentize : MonoBehaviour
{
    //links
    public Piece_Data_Manager PieceData;
    public TextAsset TXTSizeOfPiece;
    public GameObject ParentOfAllPieces;


    //public variables
    public int TotalNumberOfPieces = 25;
    public int CurrentID;
    public float ConstructionLimit = 25;
    public float ConstructionLimitInset = 15;

    //private variables
    private List<GameObject> ListOfAllPieces = new List<GameObject>();
    private List<string> ListOfSizeData = new List<string>();
    private GameObject CurrentPiece;

    public GameObject connectionpointprefab;

    private void Awake()
    {
        LoadPrefabsIntoList("streamingprefabs/p", ListOfAllPieces, TotalNumberOfPieces);
    }

    private void Start()
    {
        CurrentPiece = Instantiate(ListOfAllPieces[0], ParentOfAllPieces.transform);
        float StartingPointLimitationsXZ = ConstructionLimit - ConstructionLimitInset;
        CurrentPiece.transform.position = new Vector3(Random.Range(-StartingPointLimitationsXZ, StartingPointLimitationsXZ), 1.5f, Random.Range(-StartingPointLimitationsXZ, StartingPointLimitationsXZ));
        int rotation = Random.Range(0f, 1.0f) > 0.5 ? 0 : 90;
        CurrentPiece.transform.rotation = Quaternion.Euler(0, rotation, 0);
        /*
        for (int i = 0; i < TotalNumberOfPieces; i++)
        {

            Vector3 upmove = new Vector3(0, i * 10, 0);
            CurrentPiece = Instantiate(ListOfAllPieces[i], ParentOfAllPieces.transform);
            CurrentPiece.transform.position = upmove; 
            CurrentID = GetIDFromCurrentGO(CurrentPiece);

            //load data visualizer into inspector of each piece
            CurrentPiece.AddComponent<Piece_Data_Visualizer>();
            var PieceDataVisualizer = CurrentPiece.GetComponent<Piece_Data_Visualizer>();
            PieceDataVisualizer.Size = PieceData.SizeData(CurrentID);
            PieceDataVisualizer.ConnectionPoints = PieceData.ConnectionPointsData(CurrentID);
        }
                       
        */



        /*
        for (int j = 0; j < 6; j++)
        {
            var ConnectionPointInstances = Instantiate(connectionpointprefab, CurrentPiece.transform);
            ConnectionPointInstances.transform.position = PieceDataVisualizer.ConnectionPoints[j] + CurrentPiece.transform.position;
        }*/






        //StringVector = StringVector.spl

        //Vector3 pos = ListOfSizeData[0];
    }

    private void Update()
    {

    }

    private void LoadPrefabsIntoList(string path, List<GameObject> listname, int NumberOfPieces)
    {
        for (int i = 0; i < NumberOfPieces; i++)
        {
            string pathtoGameObjects = path + i;
            listname.Add(Resources.Load<GameObject>(pathtoGameObjects));
        }
    }

    private void LoadTextIntoList(TextAsset textreference, List<string> ListToWriteTo, int NumberOfPiecesInSim)
    {
        var content = textreference.text;
        var AllWords = content.Split("\n");

        for (int i = 0; i < NumberOfPiecesInSim; i++)
        {
            ListToWriteTo.Add(AllWords[i]);
            //Debug.Log(ListToWriteTo[i]);
        }
    }

    int GetIDFromCurrentGO(GameObject GameObject)
    {
        //naming format: p0, p1, p,2,...
        char[] Splitters = { 'p', '(' };
        string[] ID = GameObject.name.Split(Splitters);
        return int.Parse(ID[1]);
    }


}
