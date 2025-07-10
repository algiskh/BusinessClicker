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
	#endregion

	#region SaveLoadComponents
	public struct OnRequestSave
	{
	}

	public struct OnRequestLoad
	{
	}

	public struct LoadedDataComponent
	{
		public SerializationData Value;
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
		public long Amount;
		public int TargetEntity;
		public SpendPurpose Purpose;
		public int AdditionalTarget; // Optional, used for upgrades

		public bool IsApproved;
	}

	public struct ApproveSpendSoft
	{
		public long Amount;
		public int Target;
		public SpendPurpose Purpose;
		public int AdditionalTarget;
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

	public struct UpdateViewRequest
	{
		public bool IsGlobal;
		public int Target;
	}

	public struct Timer
	{
		public float Value;
		public int Target;
	}
}