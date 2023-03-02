using AlpacaIT.ReactiveLogic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// An <see cref="IReactive"/> that executes a <see cref="UnityEvent"/> when invoked. This makes it
/// easy to communicate with a component that is not reactive, but it encourages visual programming,
/// which is a bad idea. You essentially have code in the scene that tinkers around in C# and can
/// cause serious bugs that are hard to trace and fix. Use this component sparingly!
/// </summary>
public class LogicUnity : MonoBehaviour, IReactive
{
    #region Required IReactive Implementation

    [SerializeField]
    private ReactiveData _reactiveData;
    public ReactiveData reactiveData => _reactiveData;
    public ReactiveMetadata reactiveMetadata => _reactiveMeta;

    private void OnEnable() => this.OnReactiveEnable();

    private void OnDisable() => this.OnReactiveDisable();

    #endregion Required IReactive Implementation

    private static ReactiveMetadata _reactiveMeta = new ReactiveMetadata(
        new MetaInterface(MetaInterfaceType.Input, "Invoke", "Invokes the Unity event.")
    );

    [Space]
    [Header("Use this component sparingly! See source code or wiki!")]
    public UnityEvent unityEvent;

    public void OnReactiveInput(ReactiveInput input)
    {
        if (input.name == "Invoke")
        {
            unityEvent?.Invoke();
        }
    }
}