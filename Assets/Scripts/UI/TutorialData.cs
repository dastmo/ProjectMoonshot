using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tutorial")]
public class TutorialData : ScriptableObject
{
    [SerializeField] private string key;
    [SerializeField] private string title;
    [SerializeField] private Sprite image;
    [TextArea] [SerializeField] private string paragraph;

    public string Key { get => key; }
    public string Title { get => title; }
    public Sprite Image { get => image; }
    public string Paragraph { get => paragraph; }
}
