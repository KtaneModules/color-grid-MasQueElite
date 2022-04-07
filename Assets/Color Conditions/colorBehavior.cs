using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using colorButtons;

namespace behaviors
{
	public class colorBehavior : MonoBehaviour
	{
		public int indexReference;
		public colorBehavior(int colorRef)
		{
			indexReference = colorRef;
			Debug.Log("The color is " + (colorNames)colorRef);
		}
		public bool checkForAdjacent(colorBehavior[] adjacent)
		{
			// This is just to check whether or not each button is adjacent orthogonally to the same color.
			for (int i = 0; i < 4; i++)
			{
				if (adjacent[i] == null) continue;
				if (adjacent[i].Equals(this)) return true;
			}
			return false;
		}
		public bool Equals(colorBehavior color)
		{
			if (color == null) return false;
			return this.indexReference == color.indexReference;
		}
	}
}