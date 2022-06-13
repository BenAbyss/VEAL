using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Spinner : MonoBehaviour
{
    [SerializeField] private GameObject valueDisplay;
    private TextMeshProUGUI _valueDisplayText;
    private int _value = 1;
    private int _max = -1;
    private int _min = 1;

    private void Start()
    {
        _valueDisplayText = valueDisplay.GetComponent<TextMeshProUGUI>();
    }
    
    /// <summary>
    /// Method <c>IncVal</c> increases the spinner value.
    /// </summary>
    public void IncVal()
    {
        if (_max == -1 || _value < _max)
        {
            _value += 1;
        }

        _valueDisplayText.text = _value.ToString();
    }

    /// <summary>
    /// Method <c>DecVal</c> increases the spinner value.
    /// </summary>
    public void DecVal()
    {
        if (_value > _min)
        {
            _value -= 1;
        }
        
        _valueDisplayText.text = _value.ToString();
    }

    /// <summary>
    /// Method <c>SetBounds</c> sets the possible value bounds.
    /// <param name="new_min">The new minimum value.</param>
    /// <param name="new_max">The new maximum value.</param>
    /// </summary>
    public void SetBounds(int new_min, int new_max)
    {
        _min = new_min;
        _max = new_max;
    }

    /// <summary>
    /// Method <c>GetVal</c> gets the current spinner value.
    /// <returns>The spinner value.</returns>
    /// </summary>
    public int GetVal()
    {
        return _value;
    }
}