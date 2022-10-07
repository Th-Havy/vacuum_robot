using UnityEngine;

/// <summary>
/// Class for applying gaussian noise to signals
/// </summary>
[System.Serializable]
public class GaussianNoise : Noise<float>
{
    /// <summary>
    /// Mean of the gaussian noise.
    /// </summary>
    public float mu;

    /// <summary>
    /// Standard deviation of the gaussian noise.
    /// </summary>
    public float sigma;

    private System.Random _rand;

    public GaussianNoise(float mean = 0f, float std=1f)
    {
        mu = mean;
        sigma = std;
        // TODO: correct error with seed being changed
        // If the seed is changed after playmode is entered, it will not change anything
        _rand = new System.Random(seed);
    }

    /// <summary>
    /// Return the noisy value of the provided signal value.
    /// The <see href="https://en.wikipedia.org/wiki/Box%E2%80%93Muller_transform"> Box-Muller transform </see> is used to compute the gaussian.
    /// </summary>
    public override float ApplyNoise(float signalValue)
    {        
        float u1 = (float)(1.0 - _rand.NextDouble()); 
        float u2 = (float)(1.0 - _rand.NextDouble());
        float normal = mu + sigma * Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);

        return signalValue + normal;
    }
}
