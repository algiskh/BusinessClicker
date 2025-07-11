using BusinessGame.UI;
using ConfigHolder = BusinessGame.Configs.ConfigHolder;
using Config = BusinessGame.Configs.Config;
using UpgradeConfig = BusinessGame.Configs.Config.UpgradeConfig;
using BusinessGame.Serialization;

namespace BusinessGame.ECS.Components
{
	#region StartComponents
	public struct ConfigHolderComponent
	{
		public ConfigHolder Value;
	}

	public struct ConfigComponent
	{
		public Config Value;
	}
	public struct Upgrades
	{
		public UpgradeConfig[] Value;
	}

	public struct ObjectsParent
	{
		public UnityEngine.Transform Parent;
	}

	public struct BusinessViewComponent
	{
		public BusinessView Value;
	}
	public struct TopPanelComponent
	{
		public TopPanel Value;
	}
	#endregion

	#region SaveLoadComponents
	public struct RequestSave
	{
		public bool IgnoreCooldown;
	}

	public struct LoadedDataComponent
	{
		public SerializationData Value;
	}
	public struct SaveCooldown
	{
		public float UntilTime;
	}

	public struct AutoSaveTimer
	{
		public float NextSaveTime;
	}
	#endregion

	#region BusinessComponents
	public struct Timer
	{
		public float Value;
		public int Target;
	}
	public struct Level
	{
		public int Value;
	}

	public struct Income
	{
		public long Value;
	}

	public struct SoftCurrency
	{
		public long Value;
	}
	#endregion

	#region Requests
	public struct AddSoftRequest
	{
		public long Amount;
	}


	public struct RequestSpendSoft
	{
		public long Amount;
		public int TargetEntity;
		public SpendPurpose Purpose;
		public int AdditionalTarget;

		public bool IsApproved;
	}

	public struct ApproveSpendSoft
	{
		public long Amount;
		public int Target;
		public SpendPurpose Purpose;
		public int AdditionalTarget;
	}

	public struct UpdateSoftUI
	{
		public long Value;
	}

	public struct UpdateViewRequest
	{
		public bool IsGlobal;
		public int Target;
	}
	#endregion
}