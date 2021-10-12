using UnityEngine;

[System.Serializable]
public class PIDController
{
    public float pCoefficient = 0.08f;
    public float iCoefficient = 0.0002f;
    public float dCoefficient = 0.2f;
    public bool clampValue = false;
    public float clampMinimum = -1f;
    public float clampMaximum = 1f;

    private float _p;
    private float _i;
    private float _d;
    private float _previousError;

    /// <summary>
    /// Returns the PID Controller Result for the Given Error Value
    /// </summary>
    /// <param name="currentError">This is the difference between the value which is seeked and the current value (seekValue - currentValue)</param>
    /// <param name="deltaTime">Delta time for calculating the derivative part of the PID</param>
    /// <returns></returns>
    public float GetOutput(float currentError, float deltaTime)
    {
        _p = currentError;
        _i = _p * deltaTime;
        //or
        //_i += _p * deltaTime;
        _d = (_p - _previousError) / deltaTime;
        _previousError = currentError;
        float result = _p * pCoefficient + _i * iCoefficient + _d * dCoefficient;

        if (clampValue)
        {
            result = Mathf.Clamp(result, clampMinimum, clampMaximum);
        }
        
        return result;
    }
}
