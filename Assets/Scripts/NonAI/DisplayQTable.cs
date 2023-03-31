using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayQTable : MonoBehaviour
{
    private TextMeshPro text;
    public QAgent qAgent;
    private float[,] QTable;
    public List<string> states;
    public List<string> actions;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        text.SetText(TableToText());
    }

    private string TableToText(){
        float[,] QTable = qAgent.QTable;
        string output = "";
        output += string.Format("{0,12}","");
        foreach (string action in actions)
        {
            output += string.Format("{0,12}",action);
        }
        output += "\n";
        for (int i = 0; i < states.Count; i++)
        {
            output += string.Format("{0,12}",states[i]);
            output += "(";
            for (int j = 0; j<actions.Count; j++){
                if(j == actions.Count -1){
                    output += string.Format("{0,12:0.00}",QTable[i,j]);
                }
                else{
                    output += string.Format("{0,12:0.00},",QTable[i,j]);
                }
            }
            
            output += ")\n";
        }
        output += "\n\n";
        output += string.Format("Episode: {0}\nTotal Episode Reward: {1}\nRandom Exploration Probability(n/1): {2:0.0000}", 
            qAgent.currentEpisode, qAgent.totalEpisodeReward, qAgent.explorationProbability);
        return output;
    }

}
