using KModkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Rnd = UnityEngine.Random;

public class LineEquationsScript : MonoBehaviour
{

    static int _moduleIdCounter = 1;
    int _moduleID = 0;

    public KMBombModule Module;
    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMSelectable[] Buttons;
    public TextMesh[] Texts;

    private decimal[] Points = new decimal[] { 0m, 0m, 0m, 0m };
    private decimal Gradient;
    private decimal YIntercept;
    private string InputEquation = "";
    private string Solution = "69";
    private bool Solved;

    void Awake()
    {
        _moduleID = _moduleIdCounter++;
        Texts[2].text = "";
        Texts[3].text = "y =";
        for (int i = 0; i < 17; i++)
        {
            int x = i;
            Buttons[i].OnInteract += delegate { StartCoroutine(ButtonPress(x)); return false; };
        }
        Buttons[16].OnHighlight += delegate { Texts[2].text = "CLR"; Texts[3].text = ""; };
        Buttons[16].OnHighlightEnded += delegate { Texts[2].text = ""; Texts[3].text = "y ="; };
        Texts[1].text = "";
        Calculate();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void Calculate()
    {
        while (Points[0] == Points[2])
            for (int i = 0; i < 4; i++)
                Points[i] = Rnd.Range(-99, 100);
        Texts[0].text = "(" + Points[0] + ", " + Points[1] + "), (" + Points[2] + ", " + Points[3] + ")";
        Gradient = (Points[3] - Points[1]) / (Points[2] - Points[0]);
        Gradient = Math.Round(Gradient, 3, MidpointRounding.AwayFromZero);
        YIntercept = Points[1] - (Points[0] * Gradient);
        string GradientString = Gradient.ToString("G29");
        string YInterceptString = YIntercept.ToString("G29");
        Solution = (Gradient == 0 ? "" : Gradient == 1 ? "x" : Gradient == -1 ? "-x" : GradientString + "x") + (YIntercept > 0 && Gradient != 0 ? "+" : "") + (YIntercept == 0 && Gradient == 0 ? "0" : YIntercept == 0 ? "" : YInterceptString);
        Debug.LogFormat("[Line Equations #{0}] The coordinates given are ({1}, {2}) and ({3}, {4}). The gradient is {5} and the Y-intercept is {6}, making the solution {7}.", _moduleID, Points[0], Points[1], Points[2], Points[3], GradientString, YInterceptString, Solution);
    }

    private IEnumerator ButtonPress(int pos)
    {
        Audio.PlaySoundAtTransform("press", Buttons[pos].transform);
        Buttons[pos].AddInteractionPunch(0.5f);
        for (int i = 0; i < 3; i++)
        {
            Buttons[pos].transform.localPosition -= new Vector3(0, 0.005f / 3f, 0);
            yield return null;
        }
        if (!Solved)
        {
            if (pos == 12 && InputEquation != "")
                InputEquation = InputEquation.Substring(0, InputEquation.Length - 1);
            else if (pos == 16)
                InputEquation = "";
            else if (pos == 15)
            {
                if (InputEquation == Solution)
                {
                    Audio.PlaySoundAtTransform("solve", Buttons[pos].transform);
                    Module.HandlePass();
                    Debug.LogFormat("[Line Equations #{0}] You submitted \"{1}\", which was correct. Module solved!", _moduleID, InputEquation);
                    Solved = true;
                }
                else if (InputEquation == "69" || InputEquation == "69x" || InputEquation == "69x+69" || InputEquation == "420" || InputEquation == "420x" || InputEquation == "420x+420" || InputEquation == "69x+420" || InputEquation == "420x+69")
                {
                    Audio.PlaySoundAtTransform("laugh track", transform);
                    Debug.LogFormat("[Line Equations #{0}] You submitted \"{1}\", which was funny. I can't strike you because of that.", _moduleID, InputEquation);
                    InputEquation = "";
                    Texts[1].text = "";
                }
                else
                {
                    Module.HandleStrike();
                    Debug.LogFormat("[Line Equations #{0}] You submitted \"{1}\", which was incorrect. Strike!", _moduleID, InputEquation);
                    InputEquation = "";
                    Texts[1].text = "";
                }
            }
            else if (pos != 12)
            {
                if (pos == 3)
                    InputEquation += "x";
                else if (pos == 7)
                    InputEquation += "+";
                else if (pos == 11)
                    InputEquation += "-";
                else if (pos == 13)
                    InputEquation += "0";
                else if (pos == 14)
                    InputEquation += ".";
                else if (pos > 7)
                    InputEquation += (pos - 1).ToString();
                else if (pos > 3)
                    InputEquation += pos.ToString();
                else
                    InputEquation += (pos + 1).ToString();
            }
            Texts[1].text = InputEquation;
            Texts[1].transform.localScale = new Vector3(7.2311f, Texts[1].transform.localScale.y, Texts[1].transform.localScale.z);
            float scale = Mathf.Min(0.045f / Texts[1].GetComponent<Renderer>().bounds.extents.x, 1f);
            Texts[1].transform.localScale = new Vector3(scale * 7.2311f, Texts[1].transform.localScale.y, Texts[1].transform.localScale.z);
        }
        for (int i = 0; i < 3; i++)
        {
            Buttons[pos].transform.localPosition += new Vector3(0, 0.005f / 3f, 0);
            yield return null;
        }
    }

#pragma warning disable 414
    private string TwitchHelpMessage = "Use '!{0} -123.123x-123.123' to enter -123.123x-123.123, use '!{0} submit' to press the submit button and use '!{0} clear' to clear your current input.";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        string ValidCommands = "123x456+789- 0.";
        string[] CommandArray = command.Split(' ');
        yield return null;
        for (int i = 0; i < CommandArray.Length; i++)
        {
            if (CommandArray[i] == "submit")
            {
                Buttons[15].OnInteract();
                yield return new WaitForSeconds(0.1f);
            }
            else if (CommandArray[i] == "clear")
            {
                int x = InputEquation.Length;
                for (int j = 0; j < x; j++)
                {
                    Buttons[12].OnInteract();
                    yield return new WaitForSeconds(0.1f);
                }
            }
            else
            {
                for (int j = 0; j < CommandArray[i].Length; j++)
                {
                    if (!ValidCommands.Contains(CommandArray[i][j]) || CommandArray[i][j] == ' ')
                    {
                        yield return "sendtochaterror Invalid command.";
                        yield break;
                    }
                }
                for (int j = 0; j < CommandArray[i].Length; j++)
                {
                    Buttons[ValidCommands.IndexOf(CommandArray[i][j])].OnInteract();
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        string Chars = "123x456+789- 0.";
        TypeAnswer:
        yield return true;
        for (int i = 0; i < Solution.Length; i++)
        {
            Buttons[Chars.IndexOf(Solution[i])].OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
        if (Solution != InputEquation)
        {
            Buttons[16].OnInteract();
            goto TypeAnswer;
        }
        else
            Buttons[15].OnInteract();
    }
}