using BusinessGame.Configs;
using BusinessGame.UI;

namespace BusinessGame.Components
{
	public struct ConfigComponent
	{
		public Config config;
	}

	public struct Level
	{
		public int level;
	}
	public struct Income
	{
		public float income;
	}

	public struct IncomeProgress
	{
		public float progress;
	}

	public struct Upgrade
	{
		public bool IsObtained;
		public int upgrade;
	}

	public struct LevelButtonWrapperComponent
	{
		public ButtonWrapper buttonWrapper;
	}

	public struct UpgradeButtonWrapperComponent
	{
		public ButtonWrapper buttonWrapper;
	}

	public struct AddSoftCurrencyComponent
	{
		public int Amount;
	}

	public struct SoftCurrencyComponent
	{
		public int Value;
	}

	public struct SpendSoftCurrencyRequest
	{
		public int Amount;
		public int Target;
	}

	public struct UpgradeRequest
	{
		public int Target; // Entity ID of the target that will receive the currency
	}

}