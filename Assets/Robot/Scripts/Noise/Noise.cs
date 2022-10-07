/// <summary>
/// Base class for any noise type. Derived class must override the ApplyNoise method.
/// </summary>
public abstract class Noise<T>
{
    /// <summary>
    /// Seed used to generate the noise, if it has a random nature.
    /// </summary>
    public int seed = 0;

    /// <summary>
    /// Return the noisy value of the provided signal value.
    /// </summary>
    public abstract T ApplyNoise(T signalValue);
}
