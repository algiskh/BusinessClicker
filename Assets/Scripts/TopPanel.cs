using TMPro;
using UnityEngine;

public class TopPanel : MonoBehaviour
{
	private const string BALANCE_PREFIX = "Баланс: ";

	[SerializeField] private TMP_Text _text;

	public void UpdateView(long softAmount)
	{
			_text.text = $"{BALANCE_PREFIX}{softAmount}$";
	}
}
