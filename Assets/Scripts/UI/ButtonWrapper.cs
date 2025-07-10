using UnityEngine;
using UnityEngine.UI;

namespace BusinessGame.UI
{
    public class ButtonWrapper : MonoBehaviour
	{
        public bool IsClicked { get; private set; }
        [SerializeField] private Button _button;

		void Start()
        {
            _button.onClick.AddListener(OnButtonClicked);
		}

		private void OnButtonClicked()
		{
			IsClicked = true;
		}
		
		public void Unclick()
		{
			IsClicked = false;
		}

		public void SetInteractable(bool interactable) =>
			_button.interactable = interactable;
	}
}