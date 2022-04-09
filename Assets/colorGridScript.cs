using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using rnd = UnityEngine.Random;

public class colorGridScript : MonoBehaviour {

	public KMBombInfo Bomb;
	public KMAudio Audio;
	public KMBombModule Module;

	public KMColorblindMode Colorblind;

	public KMSelectable[] gridButtons;
	public Material[] gridColorMats;
	public Material gridUnlitColor;

	public MeshRenderer[] buttonLEDS;

	public TextMesh[] cbTexts;

	private List<int> buttonsToPress = new List<int>();

	colorConditions[] colors = new colorConditions[5];

	string[] colorBlindColors = {"Red", "Orange", "Blue", "Green", ""};

	static int moduleIdCounter = 1;
	int moduleId; //should you be using pragma disable 414 here as well? Hm?
	private bool moduleSolved;

	private bool colorBlindActive;

	colorConditions[,] gridColors = new colorConditions[5,5];

	bool[] firstColorCondition = new bool[25]
	     , secondColorCondition = new bool[25]
	     , thirdColorCondition = new bool[25];

	List<int> blackSquares = new List<int>();

	private int[] prematureChange = new int[25];

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
		for (int i = 0; i < 5; i++ ) colors[i] = new colorConditions(i);
		//for (int i = 0; i < 5; i++) Debug.Log(colors[i]);
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
		setColorblindMode();
		
	}

	void setColorblindMode()
	{
		if (!colorBlindActive) return;
		for (int i = 0; i < 25; i++)
		{
			cbTexts[i].text = colorBlindColors[gridColors[i / 5, i % 5].indexReference][0].ToString();
		}
	}

	void randomColorSelection()
	{
		for (int i = 0; i < 25; i++)
		{
			int r = rnd.Range(0, 4), x = i / 5, y = i % 5;
			gridColors[x,y] = colors[r];
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
			firstColorCondition[i] = gridColors[x, y].checkSameAdjacent(getAdjacents(x, y));
			//Debug.Log("Is the first condition fulfilled? " + firstColorCondition[i]);
		}
	}

	void checkSecondRule()
    {
		for (int i = 0; i < 25; i++)
        {
			int x = i / 5, y = i % 5;
			secondColorCondition[i] = gridColors[x, y].checkBlackAdjacents(getAdjacents(x, y));
			//Debug.Log("Is the SECOND condition fulfilled? " + secondColorCondition[i]);
		}
	}

	void checkThirdRule()
	{
		for (int i = 0; i < 25; i++)
		{
			int x = i / 5, y = i % 5;
			thirdColorCondition[i] = gridColors[x, y].checkForAdjacents(getAdjacents(x, y));
			//Debug.Log("Is the <<third>> condition fulfilled? " + thirdColorCondition[i]);
		}
	}

	void buttonPress(KMSelectable button)
	{
		/* Styling */ 
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

					buttonLEDS[i].material = gridUnlitColor;
					if (colorBlindActive && buttonLEDS[i].material == gridUnlitColor)
					{
						cbTexts[i].text = "";
					}
					//blackSquares.Add(i);
					gridColors[x, y] = colors[4];
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
				}
			}
		}
		if (buttonsToPress.Count == 0)
        {
			Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
			GetComponent<KMBombModule>().HandlePass();
			for (int i = 0; i < 25; i++)
            {
				buttonLEDS[i].material = gridUnlitColor;
                if (colorBlindActive && buttonLEDS[i].material == gridUnlitColor)
                {
					cbTexts[i].text = "";
                }
            }
			moduleSolved = true;
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
			bool f = firstColorCondition[i]
			   , s = secondColorCondition[i]
			   , t = thirdColorCondition[i];

			if (!(f && s && t || !f && !s && !t)) //TODO: we might change this for logistics problems
            {
				buttonsToPress.Add(i);
				Debug.Log("New button to press! It is number " + i + " ::: f: " + f + "; s: " + s + "; t: " + t);
			}
		}
    }

	// Twitch Plays

#pragma warning disable 414
	private readonly string TwitchHelpMessage = @"Use <!{0} foobar> to do something.";
#pragma warning restore 414

	IEnumerator ProcessTwitchCommand (string command)
    {
		command = command.Trim().ToUpperInvariant();
		List<string> parameters = command.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
		yield return null;
    }

	IEnumerator TwitchHandleForceSolve()
	{
		yield return null;
	}
}





