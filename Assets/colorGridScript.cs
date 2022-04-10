using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using rnd = UnityEngine.Random;

public class colorGridScript : MonoBehaviour {

	public KMBombInfo Bomb; //Bomb information
	public KMAudio Audio; //Audio management
	public KMBombModule Module; //Module management
	public KMColorblindMode Colorblind; //Colorblind instance

	public Material[] gridColorMats; //Materials: red, orange, blue, green, black
	string[] colorBlindColors = { "Red", "Orange", "Blue", "Green", " " }; //Names of the colors
	
	public KMSelectable[] gridButtons; //Selectables of the buttons
	public MeshRenderer[] buttonLEDS; //Buttons' materials management

	public TextMesh[] cbTextMeshes; //Buttons' texts (if colorblind is enabled)

	private List<int> buttonsToPress = new List<int>(); //List of correct buttons

	static int moduleIdCounter = 1;
	int moduleId;
	private bool moduleSolved; //basic vars

	private bool colorBlindActive; //bool for colorblind

	colorConditions[,] gridColors = new colorConditions[5,5]; //Each button has assigned an instance of colorConditions

	void Awake()
    {
		moduleId = moduleIdCounter++;

		foreach (KMSelectable button in gridButtons)
		{
			button.OnInteract += delegate () { buttonPress(button); return false; };
		}
		colorBlindActive = Colorblind.ColorblindModeActive;

    }

	void Start()
    {
		randomColorSelection();
		checkForRules();
    }

	void checkForRules()
    {
		buttonsToPress.Clear();
		checkFirstRule();
		checkSecondRule();
		checkThirdRule();
		addToColorsToPress();
		updateColorblindMode();
	}

	void updateColorblindMode()
	{
		if (colorBlindActive)
		{
			for (int i = 0; i < 25; i++)
			{
				cbTextMeshes[i].text = colorBlindColors[gridColors[i / 5, i % 5].indexReference][0].ToString();
			}
		}
        else
		{
			for (int i = 0; i < 25; i++)
			{
				cbTextMeshes[i].text = "";
			}
		}
	}

	void randomColorSelection()
	{
		for (int i = 0; i < 25; i++)
		{
			int r = rnd.Range(0, 4), x = i / 5, y = i % 5;
			gridColors[x, y] = new colorConditions(r);
			buttonLEDS[i].material = gridColorMats[gridColors[x, y].indexReference];
		}
	}

	colorConditions[] getAdjacents(int x, int y)
	{
		colorConditions up    = x == 0 ? null : gridColors[x - 1, y],
					    down  = x == 4 ? null : gridColors[x + 1, y],
					    left  = y == 0 ? null : gridColors[x, y - 1],
					    right = y == 4 ? null : gridColors[x, y + 1];
		return new colorConditions[] { up, down, left, right };
	}

	void checkFirstRule()
	{
		for (int i = 0; i < 25; i++)
		{
			int x = i / 5, y = i % 5;
			gridColors[x, y].conditions[0] = gridColors[x, y].checkSameAdjacent(getAdjacents(x, y));
			//Debug.Log("Is the first condition fulfilled? " + firstColorCondition[i]); DEBUG
		}
	}

	void checkSecondRule()
    {
		for (int i = 0; i < 25; i++)
        {
			int x = i / 5, y = i % 5;
			gridColors[x, y].conditions[1] = gridColors[x, y].checkBlackAdjacents(getAdjacents(x, y));
			//Debug.Log("Is the SECOND condition fulfilled? " + secondColorCondition[i]); DEBUG
		}
	}

	void checkThirdRule()
	{
		for (int i = 0; i < 25; i++)
		{
			int x = i / 5, y = i % 5;
			gridColors[x, y].conditions[2] = gridColors[x, y].checkForAdjacents(getAdjacents(x, y));
			//Debug.Log("Is the <<third>> condition fulfilled? " + thirdColorCondition[i]); DEBUG
		}
	}

	void buttonPress(KMSelectable button)
	{
		Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
		button.AddInteractionPunch(0.5f);
		if (moduleSolved) return;

		for (int i = 0; i < 25; i++)
		{
			int x = i / 5, y = i % 5;
			if (button == gridButtons[i])
			{
				if (buttonsToPress.Any(buttonID => buttonID == i))
				{
					buttonsToPress.RemoveAt(buttonsToPress.IndexOf((b => b == i)));

					buttonLEDS[i].material = gridColorMats[4];
					gridColors[x, y].indexReference = 4;
					gridColors[x, y] = new colorConditions(4);
					for (int j = 0; j < 25; j++)
					{
						int x2 = j / 5, y2 = j % 5;
						if (gridColors[x2, y2].indexReference != 4 && rnd.Range(0, 10) == 0)
						{
							updateMaterials(rnd.Range(0, 4), x2, y2);
						}
					}
					checkForRules();
				}
				else
				{
					GetComponent<KMBombModule>().HandleStrike();
					Debug.LogFormat("[Color Grid #{0}] You pressed the button number {1}. That's a strike!", moduleId, (x * 5 + y));
					string debug = "<< Here are the buttons you have to press: ";
					foreach (int correctButton in buttonsToPress)
					{
						int x2 = correctButton / 5, y2 = correctButton % 5;
  						debug += string.Format("{0} [{1}] ({2} {3} {4}); ", correctButton + 1, colorBlindColors[gridColors[x2, y2].indexReference], gridColors[x2, y2].conditions[0], gridColors[x2, y2].conditions[1], gridColors[x2, y2].conditions[2]);
					}
					Debug.LogFormat("[Color Grid #{0}] {1} >>", moduleId, debug);
				}
			}
		}
		if (buttonsToPress.Count == 0)
        {
			Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
			GetComponent<KMBombModule>().HandlePass();
			for (int i = 0; i < 25; i++)
            {
				buttonLEDS[i].material = gridColorMats[4];
			}
			updateColorblindMode();
			moduleSolved = true;
			Debug.LogFormat("[Color Grid #{0}] You pressed all the correct buttons. That's a solve!", moduleId);
		}
	}

	void updateMaterials(int randNum, int x, int y)
	{
		buttonLEDS[x*5+y].material = gridColorMats[randNum];
		gridColors[x, y].indexReference = randNum;
	}

	void addToColorsToPress()
    {
		for(int i = 0; i < 25; i++)
		{
			int x = i / 5, y = i % 5;
			bool f = gridColors[x, y].conditions[0],
			     s = gridColors[x, y].conditions[1],
			     t = gridColors[x, y].conditions[2];

			if (gridColors[x, y].indexReference < 4 && ((f && !s && !t) || (!f && s && !t) || (!f && !s && t)))
			{
				buttonsToPress.Add(i);
				//Debug.Log("New button to press! It is number " + i + " ::: f: " + f + "; s: " + s + "; t: " + t); DEBUG
			}
		}
		string debug = "String of buttons to press: ";
		foreach (var b in buttonsToPress) debug += (b+1) + " ";
		Debug.LogFormat("[Color Grid #{0}] {1}", moduleId, debug);
	}

	// Twitch Plays

#pragma warning disable 414
	private readonly string TwitchHelpMessage = @"Use !{0} A/B/C/D/E/1/2/3/4. | !{0} cb to toggle colorblind mode.";
#pragma warning restore 414

	IEnumerator ProcessTwitchCommand(string command)
	{
		command = command.Trim().ToUpper();
		yield return null;
		if (command == "CB")
		{
			colorBlindActive = !colorBlindActive;
			updateColorblindMode();
			yield break;
		}
		if (command.Length != 2 || !"ABCDE".Contains(command[0]) || !"12345".Contains(command[1]))
		{
			yield return "sendtochaterror Please specify what buttons you want to press with coordinates!";
			yield break;
		}

		gridButtons[command[0] - 'A' + 5 * (command[1] - '1')].OnInteract();
	}
   

	IEnumerator TwitchHandleForcedSolve()
	{
		yield return null;
        while (!moduleSolved)
        {
			gridButtons[buttonsToPress[0]].OnInteract();
			yield return new WaitForSeconds(.2f);
        }
	}
}





