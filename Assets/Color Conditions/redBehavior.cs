using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class redBehavior : colorBehavior
{
	public redBehavior() { indexReference = 0; }
	override public bool checkForAdjacent(colorBehavior[] adjacent)
	{
		
		base.currentColor = this;
		return base.checkForAdjacent(adjacent);
	}

}