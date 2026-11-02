using UnityEngine;

public class CharacterVisibility : MonoBehaviour
{
    [SerializeField] private GameObject characterModel;

    public void SetHidden(bool hidden)
    {
        if (characterModel != null)
        {
            characterModel.SetActive(!hidden);
        }
    }
}
