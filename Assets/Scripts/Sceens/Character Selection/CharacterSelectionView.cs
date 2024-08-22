using System.Collections.Generic;
using Michsky.MUIP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionView : MonoBehaviour
{
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private Image _avatarImage;
    [SerializeField] private ProgressBar _offenserBar;
    [SerializeField] private ProgressBar _defenseBar;
    [SerializeField] private ProgressBar _rangeBar;
    [SerializeField] private ProgressBar _survivabilityBar;
    [SerializeField] private ProgressBar _flexibilityBar;
    [SerializeField] private List<Image> _skillsImages;

    public void LoadCharacter(CharacterClassData characterClass)
    {
        _name.text = characterClass.Name;
        _description.text = characterClass.Description;
        _avatarImage.sprite = characterClass.Avatar;
        _offenserBar.currentPercent = characterClass.Offense;
        _defenseBar.currentPercent = characterClass.Defense;
        _rangeBar.currentPercent = characterClass.Range;
        _survivabilityBar.currentPercent = characterClass.Survivability;
        _flexibilityBar.currentPercent = characterClass.Flexibility;

        foreach (Image item in _skillsImages)
        {
            item.gameObject.SetActive(false);
        }

        for (int i = 0; i < characterClass.Skills.Count; i++)
        {
            _skillsImages[i].sprite = characterClass.Skills[i].Sprite;
            _skillsImages[i].gameObject.SetActive(true);
        }
    }


}

