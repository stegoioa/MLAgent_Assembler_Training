using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;


public class Main_Script : Agent
{
    //links
    public GameObject Agent;
    public GameObject Piece_Parent;
    
    public Piece_Data_Manager PieceDataManager;

    [SerializeField]
    private TextAsset LegoTopTXT;
    [SerializeField]
    private TextAsset LegoMid;
    [SerializeField]
    private TextAsset LegoBottom;
    public Camera DepthCam;

    



    //public
    List<GameObject> LegoCatalogue = new List<GameObject>(); 
    
    public float speed = 0.01f;
    public int finalconstructionsize = 1;
    public float ConstructionLimit = 0.12f;
    public float ConstructionLimitInset = 0.02f;
    public GameObject ConnectionPrefab;


    //private
    List<GameObject> LegosInScene = new List<GameObject>();
    private GameObject PreviousPiece;
    private GameObject CurrentPiece;
    //private bool IsAgentRotated = false;

    float? ClosestConnectionDistanceTopToBottom;
    float? ClosestConnectionDistanceBottomToTop;
    float? closestConnectionDistanceMid;



    private int PieceCount = 1;
    private BoxCollider CurrentPieceBoxCollider;

    List<Vector3> GlobalTopConnections = new List<Vector3>();
    List<Vector3> GlobalMidConnections = new List<Vector3>();
    List<Vector3> GlobalBottomConnections = new List<Vector3>();

    public List<Vector3> TopConnectionsInScene = new List<Vector3>();
    public List<Vector3> MidConnectionsInScene = new List<Vector3>();
    public List<Vector3> BottomConnectionsInScene = new List<Vector3>();

    public List<GameObject> CurrentTopConnections = new List<GameObject>();
    public List<GameObject> CurrentMidConnections = new List<GameObject>();
    public List<GameObject> CurrentBottomConnections = new List<GameObject>();

    //materials

    
    private void Awake()
    {
        LoadPrefabsIntoList("StreamingPrefabs/p", LegoCatalogue, 1);

        PieceDataManager.TXTToVectorList(LegoTopTXT, GlobalTopConnections, 3);
        PieceDataManager.TXTToVectorList(LegoMid, GlobalMidConnections, 6);
        PieceDataManager.TXTToVectorList(LegoBottom, GlobalBottomConnections, 3);

        //add first piece and add it to the list
        PreviousPiece = Instantiate(LegoCatalogue[0], Piece_Parent.transform);
        LegosInScene.Add(PreviousPiece);
        
        PreviousPiece.layer = 3;

        //PreviousPiece.transform.localPosition = new Vector3(Random.Range(-StartingPointLimitationsXZ, StartingPointLimitationsXZ), 0.55f, Random.Range(-StartingPointLimitationsXZ, StartingPointLimitationsXZ));
        int rotation = Random.Range(0f, 1.0f) > 0.5 ? 0 : 90;
        PreviousPiece.transform.localRotation = Quaternion.Euler(0, rotation, 0);
        InstanceCurrentConnectionPoints(PreviousPiece);

        DepthCam.gameObject.AddComponent<CameraDepthView>();

    }


    public override void OnEpisodeBegin()
    {
        //reset scene to clean slate
        ResetScene(Piece_Parent);

        float StartingPointLimitationsXZ = ConstructionLimit - ConstructionLimitInset;
        
        //add first piece and add it to the list
        PreviousPiece = Instantiate(LegoCatalogue[0], Piece_Parent.transform);
        LegosInScene.Add(PreviousPiece);
        PreviousPiece.layer = 3;
        PreviousPiece.transform.localPosition = new Vector3(Random.Range(-StartingPointLimitationsXZ, StartingPointLimitationsXZ), 0.55f, Random.Range(-StartingPointLimitationsXZ, StartingPointLimitationsXZ));
        int rotation = Random.Range(0f, 1.0f) > 0.5 ? 0 : 90;
        PreviousPiece.transform.localRotation = Quaternion.Euler(0, rotation, 0);
        InstanceCurrentConnectionPoints(PreviousPiece);
        WriteCurrentConnectionPointstoList();

        //set agent position to its 0;
        Agent.transform.localPosition = new Vector3(0, 30.0f, 0);
        Agent.transform.localRotation = Quaternion.Euler(0, 0, 0);

        //add active piece
        CurrentPiece = Instantiate(LegoCatalogue[0], Piece_Parent.transform);
        CurrentPieceBoxCollider = CurrentPiece.AddComponent<BoxCollider>();
        CurrentPieceBoxCollider.isTrigger = true;
        CurrentPiece.AddComponent<Agent_Collider>();
        CurrentPiece.layer = 3;

        LegosInScene.Add(CurrentPiece);
        InstanceCurrentConnectionPoints(CurrentPiece);
        SnapPiecetoAgent(CurrentPiece);


    }
    

    

    public override void CollectObservations(VectorSensor sensor)
    {
        //its own position, its own rotation, closebyobjects (raycast), camerafromtop?;
        
        //sensor.AddObservation(PreviousPiece.transform.localPosition);
        sensor.AddObservation(Agent.transform.localPosition);
        sensor.AddObservation(Agent.transform.localRotation);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        //evaluate
        float DistanceBefore = Vector3.Distance(PreviousPiece.transform.localPosition, Agent.transform.localPosition);







        //ACTIONS
        int action0 = actionBuffers.DiscreteActions[0];
        //Debug.Log("action0:" + action0);

        switch (action0)
        {
            case 0: Agent.transform.localPosition += new Vector3(speed, 0, 0); break;
            case 1: Agent.transform.localPosition += new Vector3(-speed, 0, 0); break;
            case 2: Agent.transform.localPosition += new Vector3(0, speed, 0); break;
            case 3: Agent.transform.localPosition += new Vector3(0, -speed, 0); break;
            case 4: Agent.transform.localPosition += new Vector3(0, 0, speed); break;
            case 5: Agent.transform.localPosition += new Vector3(0, 0, -speed); break;
            case 6: Agent.transform.localPosition += new Vector3(0, 0, 0); break;
        }

        int action1 = actionBuffers.DiscreteActions[1];
        //Debug.Log("action1:" + action1);

        switch (action1)
        {
            case 0: Agent.transform.localRotation *= Quaternion.Euler(0, 90, 0); break;
            case 1: Agent.transform.localRotation *= Quaternion.Euler(0, -90, 0); break;
            case 2: Agent.transform.localRotation *= Quaternion.Euler(0, 0, 0); break;
                //case 0: Agent.transform.localRotation *= Quaternion.Euler(10,0,0); break;
                //case 1: Agent.transform.localRotation *= Quaternion.Euler(-10, 0, 0); break;
                //case 4: Agent.transform.localRotation *= Quaternion.Euler(0, 0, 10); break;
                //case 5: Agent.transform.localRotation *= Quaternion.Euler(0, 0, -10); break;
        }

        //snap piece to new agent position
        
        SnapPiecetoAgent(CurrentPiece);

        //evaluate
        float DistanceAfter = Vector3.Distance(PreviousPiece.transform.localPosition, Agent.transform.localPosition);

        
        //evaluate closestpoints
        /*
        for (int i = 0; i < CurrentTopConnections.Count; i++)
        {
            for (int j = 0; j < BottomConnectionsInScene.Count; j++)
            {
                float TemporaryDistance = Vector3.Distance(CurrentTopConnections[i].transform.localPosition, BottomConnectionsInScene[j]);

                if (ClosestConnectionDistanceTopToBottom == null)
                {
                    ClosestConnectionDistanceTopToBottom = TemporaryDistance;
                }
                else if (TemporaryDistance < ClosestConnectionDistanceTopToBottom)
                {
                    ClosestConnectionDistanceTopToBottom = TemporaryDistance;
                }
            }
        }

        for (int i = 0; i < CurrentMidConnections.Count; i++)
        {
            for (int j = 0; j < MidConnectionsInScene.Count; j++)
            {
                float TemporaryDistance = Vector3.Distance(CurrentMidConnections[i].transform.localPosition, MidConnectionsInScene[j]);

                if (closestConnectionDistanceMid == null)
                {
                    closestConnectionDistanceMid = TemporaryDistance;
                }
                else if (TemporaryDistance < closestConnectionDistanceMid)
                {
                    closestConnectionDistanceMid = TemporaryDistance;
                }
            }
        }

        for (int i = 0; i < CurrentBottomConnections.Count; i++)
        {
            for (int j = 0; j < TopConnectionsInScene.Count; j++)
            {
                float TemporaryDistance = Vector3.Distance(CurrentBottomConnections[i].transform.localPosition, TopConnectionsInScene[j]);

                if (ClosestConnectionDistanceBottomToTop == null)
                {
                    ClosestConnectionDistanceBottomToTop = TemporaryDistance;
                }
                else if (TemporaryDistance < ClosestConnectionDistanceBottomToTop)
                {
                    ClosestConnectionDistanceBottomToTop = TemporaryDistance;
                }
            }
        }

        Debug.Log("ToptoBottom: " + ClosestConnectionDistanceTopToBottom);
        //Debug.Log("Mid: " + closestConnectionDistanceMid);
        //Debug.Log("BottomtoTop: " + ClosestConnectionDistanceBottomToTop);
        */
        
        //rewards
        if (CurrentPiece.GetComponent<Agent_Collider>().Collision == true)
        {
            Debug.Log("Piece Collided");
            CurrentPiece.GetComponent<Agent_Collider>().Collision = false;
            AddReward(-2.0f);
            EndEpisode();
        }

        if (DistanceAfter < DistanceBefore)
        {
            AddReward(1.0f / MaxStep);
        }
        else
        {
            AddReward(-1.0f / MaxStep);
        }

        //if (closestConnectionDistanceMid < 10.0f || ClosestConnectionDistanceTopToBottom < 10.0f || ClosestConnectionDistanceBottomToTop < 10.0f)
        if (DistanceAfter < 7.0f)
        {
            Debug.Log("Docked Piece");

            WriteCurrentConnectionPointstoList();
            Destroy(CurrentPieceBoxCollider);
            PreviousPiece = CurrentPiece;
            Agent.transform.localPosition = new Vector3(0, 30.0f, 0);

            CurrentPiece = Instantiate(LegoCatalogue[0], Piece_Parent.transform);
            CurrentPieceBoxCollider = CurrentPiece.AddComponent<BoxCollider>();
            CurrentPieceBoxCollider.isTrigger = true;
            CurrentPiece.AddComponent<Agent_Collider>();
            CurrentPiece.layer = 3;

            LegosInScene.Add(CurrentPiece);
            InstanceCurrentConnectionPoints(CurrentPiece);
            SnapPiecetoAgent(CurrentPiece);

            ClosestConnectionDistanceTopToBottom = null;
            ClosestConnectionDistanceBottomToTop = null;
            closestConnectionDistanceMid = null;

            AddReward(2.0f);
            EndEpisode();



            PieceCount += 1;
        }


        if (PieceCount == finalconstructionsize)
        {
            //AddReward(5.0f);
            Debug.Log("Finished Construction");
            EndEpisode();
        }



    }




    
    
    // TOOLS

    private void LoadPrefabsIntoList(string path, List<GameObject> listname, int NumberOfPieces)
    {
        for (int i = 0; i < NumberOfPieces; i++)
        {
            string pathtoGameObjects = path + i;
            listname.Add(Resources.Load<GameObject>(pathtoGameObjects));
        }
    }

    private void ResetScene(GameObject Parent)
    {
        foreach (Transform child in Parent.transform)
        {
            Destroy(child.gameObject);
        }
        PieceCount = 1;

        TopConnectionsInScene.Clear();
        MidConnectionsInScene.Clear();
        BottomConnectionsInScene.Clear();
    }

    private void SnapPiecetoAgent(GameObject Piece)
    {
        Piece.transform.localPosition = Agent.transform.localPosition;
        Piece.transform.localRotation = Agent.transform.localRotation;
    }

    private void InstanceCurrentConnectionPoints(GameObject CurrentPiece)
    {
        CurrentTopConnections.Clear();
        CurrentMidConnections.Clear();
        CurrentBottomConnections.Clear();
        
        for (int i = 0; i < GlobalTopConnections.Count; i++)
        {
            var CurrentConnectionPrefab = Instantiate(ConnectionPrefab, CurrentPiece.transform);
            CurrentConnectionPrefab.transform.localPosition = CurrentPiece.transform.localPosition + GlobalTopConnections[i];
            CurrentConnectionPrefab.transform.localRotation = CurrentPiece.transform.localRotation;
            CurrentTopConnections.Add(CurrentConnectionPrefab);
        }

        for (int i = 0; i < GlobalMidConnections.Count; i++)
        {
            var CurrentConnectionPrefab = Instantiate(ConnectionPrefab, CurrentPiece.transform);
            CurrentConnectionPrefab.transform.localPosition = CurrentPiece.transform.localPosition + GlobalMidConnections[i];
            CurrentConnectionPrefab.transform.localRotation = CurrentPiece.transform.localRotation;
            CurrentMidConnections.Add(CurrentConnectionPrefab);
        }

        for (int i = 0; i < GlobalBottomConnections.Count; i++)
        {
            var CurrentConnectionPrefab = Instantiate(ConnectionPrefab, CurrentPiece.transform);
            CurrentConnectionPrefab.transform.localPosition = CurrentPiece.transform.localPosition + GlobalBottomConnections[i];
            CurrentConnectionPrefab.transform.localRotation = CurrentPiece.transform.localRotation;
            CurrentBottomConnections.Add(CurrentConnectionPrefab);
        }
    }

    private void WriteCurrentConnectionPointstoList()
    {
        for (int i = 0; i < CurrentTopConnections.Count; i++)
        {

            Vector3 CurrentPosition = CurrentTopConnections[i].transform.position;
            
            //turn into localspace coordinate
            CurrentPosition -= this.transform.position;
            
            //add coordinates to list
            TopConnectionsInScene.Add(CurrentPosition);

            Destroy(CurrentTopConnections[i]);
        }
        
        for (int i = 0; i < CurrentMidConnections.Count; i++)
        {
            Vector3 CurrentPosition = CurrentMidConnections[i].transform.position;

            //turn into localspace coordinate
            CurrentPosition -= this.transform.position;

            //add coordinates to list
            MidConnectionsInScene.Add(CurrentPosition);


            Destroy(CurrentMidConnections[i]);
        }

        for (int i = 0; i < CurrentBottomConnections.Count; i++)
        {
            Vector3 CurrentPosition = CurrentBottomConnections[i].transform.position;
            
            //turn into localspace coordinate
            CurrentPosition -= this.transform.position;

            //add coordinates to list
            BottomConnectionsInScene.Add(CurrentPosition);


            Destroy(CurrentBottomConnections[i]);
        }
    }

    private void CreateCurrentPiece(GameObject CurrentPiece)
    {
        CurrentPiece = Instantiate(LegoCatalogue[0], Piece_Parent.transform);
        CurrentPieceBoxCollider = CurrentPiece.AddComponent<BoxCollider>();
        CurrentPieceBoxCollider.isTrigger = true;
        CurrentPiece.AddComponent<Agent_Collider>();
        
        LegosInScene.Add(CurrentPiece);
        InstanceCurrentConnectionPoints(CurrentPiece);
        SnapPiecetoAgent(CurrentPiece);
    }

    
}
