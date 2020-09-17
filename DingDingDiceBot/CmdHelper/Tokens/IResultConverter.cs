using System;
using System.Collections.Generic;
using System.Text;

namespace DingDingDiceBot.CmdHelper.Tokens
{
	internal interface IResultConverter
	{
		CalcResult ToCalcResult();
	}
}
