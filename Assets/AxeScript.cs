using UnityEngine;
using System.Collections;

public class AxeScript : MonoBehaviour
{

    private int score;
    private float timeLastClicked;
    private bool lost;
    private bool active;
    private bool chopping;
    private GUIStyle lostStyle = new GUIStyle();
    private GUIStyle scoreStyle = new GUIStyle();
    private int fontBooster = 1;
    private double randomBooster = 0.5;
    private double timeToClick = 2.0;
    private double minTimeToClick = 0.3;
    private double timeDecrement = 0.1;

    // Use this for initialization
    void Start ()
    {
        GameObject.Find("DolphinLeft").GetComponent<SpriteRenderer>().enabled = false;
        GameObject.Find("DolphinRight").GetComponent<SpriteRenderer>().enabled = false;
        if (Application.platform == RuntimePlatform.Android)
        {
            fontBooster = 3;
        }
        score = 0;
        timeLastClicked = Time.time;
        lost = false;
        active = false;
        chopping = false;
        lostStyle.normal.textColor = Color.red;
        lostStyle.fontSize = 58 * fontBooster;
        scoreStyle.normal.textColor = Color.black;
        scoreStyle.fontSize = 28 * fontBooster;
    }

    void OnGUI()
    {
        if (lost)
        {
            GUI.Label(new Rect(Screen.width/2 - 200, Screen.height/2 - 25, 100, 90), "You lost!", lostStyle);
        }
        else
        {
            GUI.Label(new Rect(Screen.width / 2 - 200, Screen.height - 100, 100, 90), 
                "Scofffffre: " + score + 
                " Time: " + (Time.time - timeLastClicked) + 
                " ToClick: " + timeToClick, scoreStyle);
        }
    }

    IEnumerator Lost()
    {
        lost = true;
        active = false;
        score = 0;
        DisableAll();
        yield return new WaitForSeconds(3);
        EnableLog();
        lost = false;
        timeToClick = 2.0;
        timeLastClicked = Time.time;
    }

    IEnumerator ChopEnded()
    {
        DisableAll();
        yield return new WaitForSeconds(1);
        EnableOne();
        ReduceTimeToClick();
        timeLastClicked = Time.time;
        chopping = false;
    }

    void LogLeftChopAnimationEnd()
    {
        StartCoroutine("ChopEnded");
    }

	// Update is called once per frame
	void Update ()
    {
        //We have some problems, basically we need to go into this if when time is exceeded.
        // But we need to make sure time being exceeded only occurs during actual play, not during the animation frames, or lost reset frames
        if (((Time.time - timeLastClicked) > timeToClick) && active && !chopping && !lost)
        {
            if (isDolphinEnabled())
            {
                //TODO win the round, we need to blank everything out for a second then start again
                EnableLog();
                ReduceTimeToClick();
                timeLastClicked = Time.time;
                chopping = false;
            }
            else
            {
                StartCoroutine("Lost");
            }
        }
        // We have a problem, we need to go in here whenever animation is not playing and the user touches the screen.
        if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && !chopping && !lost)
        {
            chopping = true;
            active = true;
            score++;
            timeLastClicked = Time.time;
            this.GetComponent<Animator>().Play("AxeAnim", 0);

            GameObject logLeft = GameObject.Find("LogLeft");
            logLeft.GetComponent<Animator>().Play("LogLeftAnim", 0);
            GameObject logRight = GameObject.Find("LogRight");
            logRight.GetComponent<Animator>().Play("LogRightAnim", 0);

            GameObject dolphinLeft = GameObject.Find("DolphinLeft");
            dolphinLeft.GetComponent<Animator>().Play("LogLeftAnim", 0);
            GameObject dolphinRight = GameObject.Find("DolphinRight");
            dolphinRight.GetComponent<Animator>().Play("LogRightAnim", 0);

            if (isDolphinEnabled())
            {
                StartCoroutine("Lost");
            }

        }
    }

    void DisableAll()
    {
        GameObject.Find("LogLeft").GetComponent<SpriteRenderer>().enabled = false;
        GameObject.Find("LogRight").GetComponent<SpriteRenderer>().enabled = false;
        GameObject.Find("DolphinLeft").GetComponent<SpriteRenderer>().enabled = false;
        GameObject.Find("DolphinRight").GetComponent<SpriteRenderer>().enabled = false;
    }

    void EnableOne()
    {
        if (Random.value > randomBooster)
        {
            EnableLog();
        }
        else
        {
            EnableDolphin();
        }
        /*
        randomBooster += Random.Range(-0.1f, 0.1f);
        if (randomBooster < 0)
        {
            randomBooster = 0.1;
        }
        if (randomBooster > 1)
        {
            randomBooster = 0.9;
        }
        */
    }

    private void EnableDolphin()
    {
        GameObject.Find("LogLeft").GetComponent<SpriteRenderer>().enabled = false;
        GameObject.Find("LogRight").GetComponent<SpriteRenderer>().enabled = false;
        GameObject.Find("DolphinLeft").GetComponent<SpriteRenderer>().enabled = true;
        GameObject.Find("DolphinRight").GetComponent<SpriteRenderer>().enabled = true;
    }

    private void EnableLog()
    {
        GameObject.Find("LogLeft").GetComponent<SpriteRenderer>().enabled = true;
        GameObject.Find("LogRight").GetComponent<SpriteRenderer>().enabled = true;
        GameObject.Find("DolphinLeft").GetComponent<SpriteRenderer>().enabled = false;
        GameObject.Find("DolphinRight").GetComponent<SpriteRenderer>().enabled = false;
    }

    private bool isDolphinEnabled()
    {
        return GameObject.Find("DolphinLeft").GetComponent<SpriteRenderer>().enabled;
    }

    private void ReduceTimeToClick()
    {
        timeToClick -= timeDecrement;
        if (timeToClick < minTimeToClick)
        {
            timeToClick = minTimeToClick;
        }
    }
}
