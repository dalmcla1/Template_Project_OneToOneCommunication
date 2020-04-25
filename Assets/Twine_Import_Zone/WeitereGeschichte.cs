﻿/*
------------------------------------------------
Generated by Cradle 2.0.1.0
https://github.com/daterre/Cradle

Original file: WeitereGeschichte.html
Story format: Harlowe
------------------------------------------------
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cradle;
using IStoryThread = System.Collections.Generic.IEnumerable<Cradle.StoryOutput>;
using Cradle.StoryFormats.Harlowe;

public partial class @WeitereGeschichte: Cradle.StoryFormats.Harlowe.HarloweStory
{
	#region Variables
	// ---------------

	public class VarDefs: RuntimeVars
	{
		public VarDefs()
		{
		}

	}

	public new VarDefs Vars
	{
		get { return (VarDefs) base.Vars; }
	}

	// ---------------
	#endregion

	#region Initialization
	// ---------------

	public readonly Cradle.StoryFormats.Harlowe.HarloweRuntimeMacros macros1;

	@WeitereGeschichte()
	{
		this.StartPassage = "Start";

		base.Vars = new VarDefs() { Story = this, StrictMode = true };

		macros1 = new Cradle.StoryFormats.Harlowe.HarloweRuntimeMacros() { Story = this };

		base.Init();
		passage1_Init();
		passage2_Init();
	}

	// ---------------
	#endregion

	// .............
	// #1: Start

	void passage1_Init()
	{
		this.Passages[@"Start"] = new StoryPassage(@"Start", new string[]{  }, passage1_Main);
	}

	IStoryThread passage1_Main()
	{
		yield return text("Das ist die erste Zeile.");
		yield return lineBreak();
		yield return lineBreak();
		yield return text("Das ist die zweite Zeile.");
		yield return lineBreak();
		yield return lineBreak();
		yield return text("Das ist die dritte Zeile.");
		yield return lineBreak();
		yield return link("ok", "ok", null);
		yield break;
	}


	// .............
	// #2: ok

	void passage2_Init()
	{
		this.Passages[@"ok"] = new StoryPassage(@"ok", new string[]{  }, passage2_Main);
	}

	IStoryThread passage2_Main()
	{
		yield return text("Double-click this passage to edit it.");
		yield break;
	}


}