using UnityEngine;

public class CoinManager : MonoBehaviour
{
    private int _coinsFound = 0;
    public int CoinsFound => _coinsFound;


    private void Start() => UIManager.Instance?.UpdateCoins(_coinsFound);

    public void FoundCoin(int value)
    {
        _coinsFound += value;
        UIManager.Instance?.UpdateCoins(_coinsFound);
    }

    public bool UsedCoin(int value)
    {
        bool success = _coinsFound >= value;
        if (success)
        {
            _coinsFound -= value;
            UIManager.Instance?.UpdateCoins(_coinsFound);
        }
        return success;
    }
}
