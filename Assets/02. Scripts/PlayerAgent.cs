using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

public class PlayerAgent : Agent
{
    public enum TEAM { 
        BLUE, RED
    }

    public TEAM team = TEAM.BLUE;
    public Material[] materials;

    //Player's Initial Position, rotation
    private Vector3 initBluePos = new Vector3(-5.5f, 0.5f, 0.0f);
    private Vector3 initRedPos = new Vector3(5.5f, 0.5f, 0.0f);
    private Quaternion initBlueRot = Quaternion.Euler(Vector3.up * 90.0f);
    private Quaternion initRedRot = Quaternion.Euler(-Vector3.up * 90.0f);



    private BehaviorParameters bps;
    private Transform tr;
    private Rigidbody rb;

    public void InitPlayer() {
        tr.localPosition = (team == TEAM.BLUE) ? initBluePos : initRedPos; 
        tr.localRotation = (team == TEAM.RED) ? initBlueRot : initRedRot;
    }


    public override void Initialize() {

        MaxStep = 10000;

        bps = GetComponent<BehaviorParameters>();
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();

        bps.TeamId = (int)team;

        GetComponent<Renderer>().material = materials[bps.TeamId];
    }

    public override void OnEpisodeBegin()
    {
        InitPlayer();
        rb.velocity = rb.angularVelocity = Vector3.zero; //resetting rigidbody
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        /*
         move backward forward: stop, forward, back (0,1,2) (No input, W, S)
        move left right: stop, left, right (0,1,2) (No Input, Q, E)
        rotate: stop, left, right (0,1,2) (No Input, A, D)
        */

        var actions = actionsOut.DiscreteActions;
        actions.Clear();

        //branch 0 forward, backward
        if (Input.GetKey(KeyCode.W)) actions[0] = 1;
        if (Input.GetKey(KeyCode.S)) actions[0] = 2;

        //branch 1 right, left
        if (Input.GetKey(KeyCode.A)) actions[1] = 1;
        if (Input.GetKey(KeyCode.D)) actions[1] = 2;

        //branch 2 turn right, left
        if (Input.GetKey(KeyCode.Q)) actions[2] = 1;
        if (Input.GetKey(KeyCode.E)) actions[2] = 2;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var action = actions.DiscreteActions;
        //Debug.LogFormat("[0]={0}, [1]={1}, [2]={2}", action[0], action[1], action[2]);
        Vector3 dir = Vector3.zero;
        Vector3 rot = Vector3.zero;

        switch (action[0]) {
            case 1: dir = tr.forward; break;
            case 2: dir = -tr.forward; break;
        }

        switch (action[1]) {
            case 1: dir = -tr.right; break;
            case 2: dir = tr.right; break;
        }

        switch (action[2]) {
            case 1: rot = -tr.up; break;
            case 2: rot = tr.up; break;
        }

        tr.Rotate(rot, Time.deltaTime * 100.0f);
        rb.AddForce(dir * 1.0f, ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("ball")) {
            //if ball is touched add a little bit of reward
            AddReward(0.2f);

            //ball kick
            Vector3 kickDir = other.GetContact(0).point - tr.position; // tr.position inglobal axis
            other.gameObject.GetComponent<Rigidbody>().AddForce(kickDir.normalized* 800.0f);


        }
    }
}
