using System;
using UnityEngine;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    [Serializable]
    public struct UIElement
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private string _prefix;
        [SerializeField] private string _suffix;

        public void Update(string value) => _text.SetText($"{_prefix}{value}{_suffix}");
    }
    [SerializeField] private UIElement _lives;
    [SerializeField] private UIElement _collectables;


    public void UpdateLives(int value) => _lives.Update(value.ToString());

    public void UpdateCoins(int value) => _collectables.Update(value.ToString());
}
