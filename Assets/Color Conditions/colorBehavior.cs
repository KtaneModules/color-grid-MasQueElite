using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class colorBehavior : MonoBehaviour
{
	public colorBehavior currentColor { set; get; }
	public int indexReference;
	virtual public bool checkForAdjacent(colorBehavior[] adjacent)
	{
		// This is just to check whether or not each button is adjacent orthogonally to the same color.
		for (int i = 0; i < 4; i++)
		{
			if (adjacent[i] == null) continue;
			if (adjacent[i].Equals(currentColor)) return true;
		}
		return false;
	}
	public bool Equals(colorBehavior color)
	{
        if (color == null) return false;
		return this.indexReference == color.indexReference;
	}
}