using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using TMPro;


public class BallControl : MonoBehaviour
{
    public TMP_Text blueScoreText, redScoreText;
    private int blueScore, redScore;

    public Agent[] players;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();   
    }

    //Initializes everytime its a new Iteration.Resets the ball
    void InitBall() {
        rb.velocity = rb.angularVelocity = Vector3.zero;
        transform.localPosition = new Vector3(0.0f, 1.5f, 0.0f);
    }

    private void OnCollisionEnter(Collision other)
    {
        //if goes inside the blue goal
        //red team +1 Reward
        //blue team -1 Reward
        //End Episode
        if (other.collider.CompareTag("BLUE_GOAL")) {
            //give reward
            players[1].AddReward(+1.0f);
            players[0].AddReward(-1.0f);

            //end training
            players[0].EndEpisode();
            players[1].EndEpisode();

            //reset ball
            InitBall();

            //save score
            redScoreText.text = (++redScore).ToString();

        }



        //if goes inside the red goal
        //red team -1 Reward
        //blue team +1 Reward
        //End Episode

        if (other.collider.CompareTag("RED_GOAL"))
        {
            //give reward
            players[1].AddReward(-1.0f);
            players[0].AddReward(+1.0f);

            //end training
            players[0].EndEpisode();
            players[1].EndEpisode();

            //reset ball
            InitBall();

            //save score
            blueScoreText.text = (++blueScore).ToString();

        }

    }


}
