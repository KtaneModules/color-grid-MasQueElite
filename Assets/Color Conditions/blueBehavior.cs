﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class blueBehavior : colorBehavior
{

	public blueBehavior() { indexReference = 2; }
	override public bool checkForAdjacent(colorBehavior[] adjacent)
	{
		base.currentColor = this;
		return base.checkForAdjacent(adjacent);
	}

}
