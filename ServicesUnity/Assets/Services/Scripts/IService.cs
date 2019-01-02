﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
	public abstract class IService 
	{
		protected SettingDef def;
		protected ServiceEvents serviceE;

		public virtual IService Init(SettingDef def, ServiceEvents serviceE)//return IService to register to mgr
		{
			this.def = def;
			this.serviceE = serviceE;

			return null;
		}

		public abstract void Dispose();
	}
}
