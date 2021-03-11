using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    GameController gameController;
    Rigidbody rb2d;
    bool isSplitting = false;
    static int ballSizeThreshold = 50;

    void Awake()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        rb2d = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        GravitationalField();
        CheckMass();
    }
    void GravitationalField()
    {
        Vector3 forceOneMe = new Vector3(0, 0, 0);
        foreach (var ball in GameData.ballsList)
        {
            if (ball != rb2d)
            {

                float r = Vector3.Distance(transform.position, ball.position);
                if (r != 0)
                {
                    Vector3 dir = (ball.position - transform.position).normalized;
                    float force = (5000 * ball.mass * rb2d.mass) / Mathf.Pow(r, 2);
                    forceOneMe += (GameData.repelBalls ? -1 : 1) * dir * force;
                }
            }
        }
        rb2d.AddForce(forceOneMe);
    }
    void CheckMass()
    {
        //if two or more collisions happen at once
        if (rb2d.mass >= gameController.ball.mass*ballSizeThreshold && !isSplitting)
            Seperate();
    }
    void Seperate()
    {
        if (rb2d.mass > 1)
        {
            //run only once
            isSplitting = true;
            print("split " + rb2d.mass);
            //mass-1 because last ball will be made from itself
            for (int i = 0; i < rb2d.mass-1; i++)
            {
                CreateSmallBall();
            }
            rb2d.mass = gameController.ball.mass;
            rb2d.transform.localScale = new Vector3(gameController.ball.transform.localScale.x, gameController.ball.transform.localScale.y, gameController.ball.transform.localScale.z);
            GameData.ballsList.Remove(rb2d);
            rb2d.SendMessage("DisableCollision");
            //big number, multiple by either 1 or -1 and random direction
            rb2d.AddForce((Random.Range(0, 2) * 2 - 1) * 8000 * new Vector3(Random.value, Random.value, Random.value));
            isSplitting = false;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(!GameData.repelBalls)
        {
            GameData.mergeBallsQueue.Enqueue(rb2d);
        }
    }

    private void CreateSmallBall()
    {
        Rigidbody smallBall = GameData.pooledBalls[0];
        smallBall.mass = gameController.ball.mass;
        GameData.pooledBalls.Remove(smallBall);
        smallBall.transform.localScale = new Vector3(gameController.ball.transform.localScale.x, gameController.ball.transform.localScale.y, gameController.ball.transform.localScale.z);
        smallBall.transform.position = rb2d.position;
        smallBall.gameObject.SetActive(true);
        smallBall.SendMessage("DisableCollision");

        //big number, multiple by either 1 or -1 and random direction
        smallBall.AddForce((Random.Range(0, 2) * 2 - 1) * 8000 * new Vector3(Random.value, Random.value, Random.value));
    }
    private void DisableCollision()
    {
        StartCoroutine(DisableCollisionCoroutine(0.5f));
    }
    IEnumerator DisableCollisionCoroutine(float time)
    {
        rb2d.detectCollisions = false;
        yield return new WaitForSeconds(time);
        rb2d.detectCollisions = true;
        GameData.ballsList.Add(rb2d);

    }

}
