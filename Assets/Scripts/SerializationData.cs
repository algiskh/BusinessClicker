using System;

namespace BusinessGame.Serialization
{
	[Serializable]
	public class SerializationData
	{
		[Serializable]
		public class SerializedBusiness
		{
			public string Id;
			public int Level;
			public float Timer;
			public bool[] Upgrades;
		}

		public long SoftCurrency;
		public SerializedBusiness[] Businesses;
	}
}