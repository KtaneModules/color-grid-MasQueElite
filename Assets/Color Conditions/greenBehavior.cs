using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class greenBehavior : colorBehavior
{
	public greenBehavior() { indexReference = 3; }
	override public bool checkForAdjacent(colorBehavior[] adjacent)
	{
		base.currentColor = this;
		return base.checkForAdjacent(adjacent);
	}
	
}
