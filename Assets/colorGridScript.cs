using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using rnd = UnityEngine.Random;
using colorButtons;
using behaviors;

namespace colorButtons
{
    public enum colorNames { Red, Orange, Blue, Green };
}
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

	colorBehavior[] colors = new colorBehavior[] { new colorBehavior(0), new colorBehavior(1), new colorBehavior(2), new colorBehavior(3) };

	string[] colorBlindColors = {"Red", "Orange", "Blue", "Green"};

	static int moduleIdCounter = 1;
	int moduleId;
	private bool moduleSolved;

	private bool colorBlindActive;

	colorBehavior[,] gridColors = new colorBehavior[5,5];

	bool[] firstColorCondition, secondColorCondition, thirdColorCondition = new bool[25];

	


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

		

		for (int i = 0; i < 25; i++)
		{
			if (colorBlindActive)
			{
				cbTexts[i].text = colorBlindColors[gridColors[i/5,i%5].indexReference][0].ToString();
			}
			else
			{
				cbTexts[i].text = "";
			}
		}

    }

	colorBehavior[] getAdjacents(int x, int y)
	{
		colorBehavior up    = x == 5 ? null : gridColors[x+1, y],
		              down  = x == 0 ? null : gridColors[x-1, y],
					  left  = y == 0 ? null : gridColors[x, y-1],
					  right = y == 5 ? null : gridColors[x, y+1];
		return new colorBehavior[] { up, down, left, right };
	}

	void randomColorSelection()
	{
		for (int i = 0; i < 25; i++)
		{
			int r = rnd.Range(0, 4), x = i/5, y = i%5;
			gridColors[x,y] = colors[r];
			Debug.LogFormat("Random is {0}, x is {1}, and y is {2}", r, x, y);
			Debug.Log("The index is " + gridColors[x,y].indexReference);

			buttonLEDS[i].material = gridColorMats[gridColors[x,y].indexReference];

			//Debug.Log(gridColors[x,y].checkForAdjacent(getAdjacents(x,y)));
			//Debug.Log("The color is: " + (colorNames)gridColors[x,y].indexReference);

		}
	}

	void buttonPress(KMSelectable button)
	{
		Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
		button.AddInteractionPunch(0.5f);
		if (moduleSolved) return;

		for (int i = 0; i < 25; i++)
		{
			if(button == gridColors[i/5,i%5])
			{
				if (buttonsToPress[0] == i)
				{
					buttonsToPress.RemoveAt(0);
					buttonLEDS[i].material = gridUnlitColor;
				}
			}
		}




	}
	
	
	void Update()
    {

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





