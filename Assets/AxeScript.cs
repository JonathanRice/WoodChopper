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
    private float randomBooster = 0.25f;
    private float timeToClick = 2.0f;
    private float minTimeToClick = 0.5f;
    private float timeDecrement = 0.1f;

    public float barDisplay = 0.5f; //current progress
    public Vector2 pos = new Vector2(20, 40);
    private Vector2 size = new Vector2(Screen.width - 40, 20);
    public Texture2D emptyTex;
    public Texture2D fullTex;

    // Use this for initialization
    void Start()
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
            GUI.Label(new Rect(Screen.width / 2 - 200, Screen.height / 2 - 25, 100, 90), "You lost!", lostStyle);
        }
        else
        {
            GUI.Label(new Rect(Screen.width / 2 - 200, Screen.height - 100, 100, 90),
                "Score: " + score + "\n" +
                " Time: " + (Time.time - timeLastClicked) + "\n" +
                " ToClick: " + timeToClick, scoreStyle);
        }
        GUI.BeginGroup(new Rect(pos.x, pos.y, size.x, size.y));
        GUI.Box(new Rect(0, 0, size.x, size.y), emptyTex);

        //draw the filled-in part:
        barDisplay = (Time.time - timeLastClicked) / timeToClick;
        if (chopping || !active)
        {
            barDisplay = 0.0f;
        }
        GUI.BeginGroup(new Rect(0, 0, size.x * barDisplay, size.y));
        GUI.Box(new Rect(0, 0, size.x, size.y), fullTex);
        GUI.EndGroup();
        GUI.EndGroup();
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
        timeToClick = 2.0f;
        timeLastClicked = Time.time;
    }

    IEnumerator ChopEnded()
    {
        chopping = true;
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
    void Update()
    {
        //We have some problems, basically we need to go into this if when time is exceeded.
        // But we need to make sure time being exceeded only occurs during actual play, not during the animation frames, or lost reset frames
        if (((Time.time - timeLastClicked) > timeToClick) && active && !chopping && !lost)
        {
            if (isDolphinEnabled())
            {
                StartCoroutine("ChopEnded");
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
            ActivateGame();
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

    private void ActivateGame()
    {
        active = true;
        GameObject.Find("WoodChopperText").GetComponent<SpriteRenderer>().enabled = false;
    }
}
