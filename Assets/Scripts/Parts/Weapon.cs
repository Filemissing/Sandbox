using System.Collections.Generic;
using UnityEngine;

namespace RobotGame
{
	public abstract class Weapon : Part
	{
		protected bool isActive = false;

        public virtual void Activate()
		{
			isActive = true;
        }
    }
}