using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class colorConditions : MonoBehaviour
{
	public int indexReference;
	public colorConditions(int colorRef) { indexReference = colorRef; }
	public bool Equals(colorConditions color) { return this.indexReference == color.indexReference; }
	public bool checkForAdjacent(colorConditions[] adjacent)
	{   // This is just to check whether or not each button is adjacent orthogonally to the same color.
		for (int i = 0; i < 4; i++)
		{
			if (!typeof(UnityEngine.Object).IsInstanceOfType(adjacent[i])) continue;
			if (adjacent[i].Equals(this)) return true;
		}
		return false;
	}
}