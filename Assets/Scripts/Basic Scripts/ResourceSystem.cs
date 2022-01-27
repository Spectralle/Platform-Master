using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceSystem : Singleton<ResourceSystem>
{
    [SerializeField] private bool _showDebugLoadInfo = true;

    // Example for all GameObjects (prefabs) in Resources
    [SerializeField] private ResourceType<GameObject> _GOResources = new ResourceType<GameObject>();

    [System.Serializable]
    public class ResourceType<T>
    {
        public string _filePath = "";
        [HideInInspector] public T[] _List { get; private set; }        // List of all type resources
        public Dictionary<string, T> _Dictionary { get; private set; }  // Dictionary using resource name


        public ResourceType() { }
        public ResourceType(string filePath) => _filePath = filePath;

        public void AssembleResource(bool showLoadInfo)
        {
            _List = Resources.LoadAll(_filePath).OfType<T>().ToArray();
            _Dictionary = _List.ToDictionary(n => DictionaryNameFormatter(n.ToString()), n => n);
            
            if (showLoadInfo)
            {
                string typeFormat = _List.GetType().ToString().Replace("[]", "").Replace("UnityEngine.", "");
                Debug.Log($"A resource list of type <b>{typeFormat}</b> was created and loaded with <b>{_List.Count()}</b> found assets.");
            }
        }

        private string DictionaryNameFormatter(string name)
        {
            if (name.Contains(formatChar))
                Debug.LogError($"The Resource System uses the '{formatChar}' (tilde) symbol to format names, but the asset {name} " +
                    $"already contains it. <b>This will probably cause issues with the Resource dictionary key.</b>");

            return NameFormatter(name);
        }

        private string NameFormatter(string name)
        {
            if (name.Contains(" (UnityEngine."))
                name = name.Remove(name.Replace(" (UnityEngine.", formatChar.ToString()).IndexOf(formatChar));

            return name.ToLower().Replace(" ", "_");
        }

        public bool Contains(string name) => _Dictionary.ContainsKey(NameFormatter(name));

        public bool ContainsObject(T value) => _Dictionary.ContainsValue(value);

        public T GetSpecific(string name) => _Dictionary[NameFormatter(name)];

        public T GetRandom() => _List[Random.Range(0, _List.Length)];
    }
    private const char formatChar = '`';


    protected override void Awake()
    {
        base.Awake();
        AssembleResources();
    }

    /// <summary>
    /// IMPORTANT: Add all ResourceType<> variables here so that they are assembled
    /// </summary>
    private void AssembleResources()
    {
        _GOResources.AssembleResource(_showDebugLoadInfo);

    }
}
