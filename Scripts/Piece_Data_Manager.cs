using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.IO;
using System.Globalization;

public class Piece_Data_Manager : MonoBehaviour

{   
    //links
    [SerializeField]
    private TextAsset TXTLegoTopConnections;
    [SerializeField]
    private TextAsset TXTLegoMidConnection;
    [SerializeField]
    private TextAsset TXTLegoBottomConnections;
    [SerializeField]
    private Main_Script Main_Script;

    //public


    //private
    private List<string> SizeDataAsString = new List<string>();
    private List<string> PossibleConnectionPointsAsString = new List<string>();
    private int PieceID;
    private int StartIndex;
    private int EndIndex;


    //PRIMARY DATA COMPILERS
    
    /*public Vector3 SizeData(int ID)
    {
        LoadTextIntoList(TXTLegoTopConnections, SizeDataAsString, agent_main.TotalNumberOfPieces);
        return TurnStringIntoVectorParenthesis(SizeDataAsString, ID);
    }*/

    public void TXTToVectorList(TextAsset TextReference, List<Vector3> ListToWriteTo, int NumberofPoints)
    {
        List<string> stringlist = new List<string>();
        TurnTXTToStringList(TextReference, stringlist, NumberofPoints);
        TurnStringListIntoVectorList(stringlist, ListToWriteTo);
    }



    public void TurnTXTToStringList(TextAsset OriginalTXTFile, List<string> ListToWriteTo, int NumberofPoints)
    {
        ListToWriteTo.Clear();
        var content = OriginalTXTFile.text;
        var AllWords = content.Split("\r\n");

        for (int i = 0; i < NumberofPoints; i++)
        {
            ListToWriteTo.Add(AllWords[i]);
        }
    }
    
    public List<Vector3> TurnStringListIntoVectorList(List<string> OriginalStringList, List<Vector3> ListToWriteTo)
    {
        //load and compile connection point data: returns PossibleConnectionPointsPerPiece

        ListToWriteTo.Clear ();
        //LoadTextIntoList(TXTConnectionPoints, PossibleConnectionPointsAsString, (agent_main.TotalNumberOfPieces * NumberofConnectionPointsPerPiece));
        
        StartIndex = 0;
        EndIndex = StartIndex + OriginalStringList.Count;
        for (int i = StartIndex; i < EndIndex; i++)
        {
            ListToWriteTo.Add(TurnStringIntoVectorBrackets(OriginalStringList, i));
        }

        return ListToWriteTo;
    }

   

    Vector3 TurnStringIntoVectorParenthesis(List<string> DataAsString, int ListItem)
    {
        char[] splittingcharacters = { '(', ')', ',' };
        string[] StringVector = DataAsString[ListItem].Split(splittingcharacters);

        float vectorx = float.Parse(StringVector[1], CultureInfo.InvariantCulture.NumberFormat);
        float vectory = float.Parse(StringVector[2], CultureInfo.InvariantCulture.NumberFormat);
        float vectorz = float.Parse(StringVector[3], CultureInfo.InvariantCulture.NumberFormat);

        return new Vector3(vectorx, vectory, vectorz);
    }

    Vector3 TurnStringIntoVectorBrackets(List<string> DataAsString, int ListItem)
    {
        char[] splittingcharacters = { '[', ']', ',' };
        string[] StringVector = DataAsString[ListItem].Split(splittingcharacters);

        float vectorx = float.Parse(StringVector[1], CultureInfo.InvariantCulture.NumberFormat);
        float vectory = float.Parse(StringVector[2], CultureInfo.InvariantCulture.NumberFormat);
        float vectorz = float.Parse(StringVector[3], CultureInfo.InvariantCulture.NumberFormat);

        return new Vector3(vectorx, vectory, vectorz);
    }



}

