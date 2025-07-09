using BusinessGame.Configs;
using BusinessGame.UI;
using System;

namespace BusinessGame.ECS.Components
{
	#region StartComponents
	public struct ConfigHolderComponent
	{
		public Configs.ConfigHolder ConfigHolder;
	}

	public struct ConfigComponent
	{
		public Configs.Config config;
	}

	public struct ObjectsParent
	{
		public UnityEngine.Transform Parent;
	}

	public struct BusinessViewComponent
	{
		public BusinessView View;
	}
	#endregion

	#region SaveLoadComponents
	public struct OnRequestSave
	{
	}

	public struct OnRequestLoad
	{
	}

	public struct LoadedData
	{
	}
	#endregion

	public struct Level
	{
		public int Value;
	}

	public struct Income
	{
		public long Value;
	}

	public struct IncomeProgress
	{
		public float progress;
	}

	public struct Upgrade
	{
		public bool IsObtained;
		public float Multiplier;
		public int Price;
	}

	public struct LevelButtonWrapper
	{
		public ButtonWrapper buttonWrapper;
	}

	public struct UpgradeButtonWrapper
	{
		public ButtonWrapper buttonWrapper;
	}

	public struct AddSoftRequest
	{
		public long Amount;
	}

	public struct SoftCurrency
	{
		public long Value;
	}

	public struct RequestSpendSoft
	{
		public int Amount;
		public int Target;
	}

	public struct ApproveSpendSoft
	{
		public int Amount;
		public int Target;
	}

	public struct UpgradeRequest
	{
		public int Target; // Entity ID of the target that will receive the currency
	}

	public struct UpdateSoftUI
	{
		public long Value;
	}

	public struct TopPanelComponent
	{
		public TopPanel Value;
	}

	public struct Timer
	{
		public float Value;
		public int Target;
	}

	#region Events
	public struct OnTimerFinish
	{
		public int Target;
	}

#endregion
}