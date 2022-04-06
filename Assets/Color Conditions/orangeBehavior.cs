using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class orangeBehavior : colorBehavior
{
	public orangeBehavior() { indexReference = 1; }
	override public bool checkForAdjacent(colorBehavior[] adjacent)
	{
		
		base.currentColor = this;
		return base.checkForAdjacent(adjacent);
	}

}
