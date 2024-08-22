using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CharacterClassData
{
    public string Name;
    [TextArea]
    public string Description;
    public Sprite Avatar;
    public int Hp;
    public int Ap;
    public int Mp;
    public float Offense;
    public float Range;
    public float Defense;
    public float Survivability;
    public float Flexibility;
    public List<ClassSkills> Skills;
}

[System.Serializable]
public struct ClassSkills
{
    public string Name;
    public string Description;
    public Sprite Sprite;
}

public class CharacterSelectionScreenController : MonoBehaviour
{
     [SerializeField] private CharacterClassData _classesData;
     [SerializeField] private CharacterSelectionView _view;

    private void Start()
    {
        _view.LoadCharacter(_classesData);
    }
}