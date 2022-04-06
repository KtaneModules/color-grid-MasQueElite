using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using rnd = UnityEngine.Random;

namespace buttonColors
	{
		public enum colorNames {Red, Orange, Blue, Green};
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

	colorBehavior[] colors = new colorBehavior[]{new redBehavior(), new orangeBehavior(), new blueBehavior(), new greenBehavior()};

	string[] colorBlindColors = {"Red", "Orange", "Blue", "Green"};

	static int moduleIdCounter = 1;
	int moduleId;
	private bool moduleSolved;

	private bool colorBlindActive;

	int[] gridColors = new int[25];

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
		buttonBehavior();

		

		for (int i = 0; i < 25; i++)
		{
			if (colorBlindActive)
			{
				cbTexts[i].text = colorBlindColors[gridColors[i]][0].ToString();
			}
			else
			{
				cbTexts[i].text = "";
			}
		}

    }

	void randomColorSelection()
	{
		for (int i = 0; i < 25; i++)
		{
			gridColors[i] = rnd.Range(0, 4);

			buttonLEDS[i].material = gridColorMats[gridColors[i]];

			

			colors[gridColors[i]].determineRules(new redBehavior());

			Debug.Log("The color is: " + (buttonColors.colorNames)gridColors[i]);

		}
	}

	void buttonBehavior()
	{
		redBehavior rb = new redBehavior();
		orangeBehavior ob = new orangeBehavior();
		blueBehavior bb = new blueBehavior();
		greenBehavior gb = new greenBehavior();

		rb.determineRules(rb);

		
	}

	void buttonPress(KMSelectable button)
	{
		Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
		button.AddInteractionPunch(0.5f);
		if (moduleSolved) return;



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





