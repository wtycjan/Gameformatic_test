using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Rigidbody ball;
    [SerializeField] private Text ballCounterText;
    Vector3 screenPosition;
    void Awake()
    {
        for(int i=0; i<250; i++)
        {
            Rigidbody newBall = Instantiate(ball, screenPosition, Quaternion.identity);
            GameData.pooledBalls.Add(newBall);
            newBall.gameObject.SetActive(false);
        }
        StartCoroutine(SpawnBall(250));
    }
    IEnumerator SpawnBall(int number)
    {
        int i = 0;
        while (i < number)
        {
            //near clip plane so we can see what's happening
            screenPosition = Camera.main.ScreenToWorldPoint(new Vector3(Random.Range(0, Screen.width), Random.Range(0, Screen.height), Random.Range(Camera.main.nearClipPlane+200, Camera.main.nearClipPlane+400)));
            Rigidbody newBall = GameData.pooledBalls[0];
            newBall.transform.localScale = new Vector3(ball.transform.localScale.x, ball.transform.localScale.y, ball.transform.localScale.z);
            newBall.mass = ball.mass;
            newBall.transform.position = screenPosition;
            newBall.gameObject.SetActive(true);
            ballCounterText.text = (i+1).ToString();
            i++;
            GameData.pooledBalls.Remove(newBall);
            GameData.ballsList.Add(newBall);
            yield return new WaitForSeconds(.25f);
        }
        GameData.repelBalls = true;
    }

    private void FixedUpdate()
    {
        CheckMergeQueueForPair();
        print("ballActive: " + GameData.ballsList.Count + " ballinActive: " + GameData.pooledBalls.Count+" sum: " + (GameData.pooledBalls.Count + GameData.ballsList.Count));
    }
    void CheckMergeQueueForPair()
    {
        Queue<Rigidbody> ballQueue = new Queue<Rigidbody>(GameData.mergeBallsQueue.ToArray());
        GameData.mergeBallsQueue.Clear();

        while (ballQueue.Count % 2 == 0 && ballQueue.Count > 0)
        {
            //check if first ball still exists oor has been merged
            if(ballQueue.Peek().gameObject.activeSelf)
                MergeBalls(ballQueue, ballQueue.Peek());
            else
            {
                ballQueue.Dequeue();
                ballQueue.Dequeue();
            }
        }
    }
    private void MergeBalls(Queue<Rigidbody> ballQueue, Rigidbody rb2d1)
    {
        ballQueue.Dequeue();
        //check if second ball still exists oor has been merged
        if (ballQueue.Peek().gameObject.activeSelf)
        {
            Rigidbody rb2d2 = ballQueue.Peek();
            
            rb2d1.mass += rb2d2.mass;
            rb2d1.transform.localScale = new Vector3(rb2d1.transform.localScale.x + rb2d2.transform.localScale.x, rb2d1.transform.localScale.y + rb2d2.transform.localScale.y,
                                                                                                          rb2d1.transform.localScale.z + rb2d2.transform.localScale.z);
            GameData.ballsList.Remove(rb2d2);
            GameData.pooledBalls.Add(rb2d2);
            ballQueue.Dequeue();
            rb2d2.gameObject.SetActive(false);

        }
        else
        {
            ballQueue.Dequeue();

        }
    }

}
