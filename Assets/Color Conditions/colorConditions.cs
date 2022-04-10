using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class colorConditions
{
	public int indexReference;
	public bool[] conditions = new bool[3];
	public colorConditions(int colorRef) { indexReference = colorRef; }
	public bool isUnityObject(colorConditions instance) { return typeof(colorConditions).IsInstanceOfType(instance); }
	public bool checkSameAdjacent(colorConditions[] adjacent)
	{
		// This is just to check whether or not each button is adjacent orthogonally to the same color.
		for (int i = 0; i < 4; i++)
		{
			if (!isUnityObject(adjacent[i])) continue;
			if (adjacent[i].indexReference == indexReference) return true;
		}
		return false;
	}
	public bool checkBlackAdjacents(colorConditions[] adjacent)
	{
		int counter = 0;
		for (int i = 0; i < 4; i++)
		{
			if (!isUnityObject(adjacent[i])) continue;
			if (adjacent[i].indexReference == 4) counter++;
		}
		return counter == 3 - indexReference;
	}
	public bool checkForAdjacents(colorConditions[] adjacent)
	{
		int[] colorsToCheck = { 1, 3, 0, 2, 5 };
		for (int i = 0; i < 4; i++)
		{
			if (!isUnityObject(adjacent[i])) continue;
			if (adjacent[i].indexReference == colorsToCheck[adjacent[i].indexReference]) return false;
		}
		return true;
	}
}