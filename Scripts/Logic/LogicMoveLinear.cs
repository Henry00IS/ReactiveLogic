using AlpacaIT.ReactiveLogic.Internal;
using UnityEngine;

namespace AlpacaIT.ReactiveLogic
{
    /// <summary>An <see cref="IReactive"/> that moves between two points.</summary>
    [HelpURL("https://github.com/Henry00IS/ReactiveLogic/wiki/Logic-Move-Linear")]
    public class LogicMoveLinear : MonoBehaviour, IReactive
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
            new MetaInterface(MetaInterfaceType.Input, "Open", "Move towards the end of the linear motion with positive speed."),
            new MetaInterface(MetaInterfaceType.Input, "Close", "Move towards the beginning of the linear motion with negative speed."),
            new MetaInterface(MetaInterfaceType.Input, "Reverse", "Reverses the linear motion so that it travels in the other direction."),
            new MetaInterface(MetaInterfaceType.Input, "SetTime", "Sets the linear motion time to the float parameter.", "seconds", MetaParameterType.Float, "The time in seconds a full linear motion takes."),

            new MetaInterface(MetaInterfaceType.Output, "Arrived", "Invoked when the linear movement arrives at a destination."),
            new MetaInterface(MetaInterfaceType.Output, "ArrivedAtTarget1", "Invoked when the linear movement arrives at target 1."),
            new MetaInterface(MetaInterfaceType.Output, "ArrivedAtTarget2", "Invoked when the linear movement arrives at target 2."),
            new MetaInterface(MetaInterfaceType.Output, "Departed", "Invoked when the linear movement departs from a destination."),
            new MetaInterface(MetaInterfaceType.Output, "DepartedFromTarget1", "Invoked when the linear movement departed from target 1."),
            new MetaInterface(MetaInterfaceType.Output, "DepartedFromTarget2", "Invoked when the linear movement departed from target 2.")
        );

        /// <summary>The target point that the movement begins at.</summary>
        [Tooltip("The target point that the movement begins at.")]
        public Transform target1;

        /// <summary>The target point that the movement ends at.</summary>
        [Tooltip("The target point that the movement ends at.")]
        public Transform target2;

        /// <summary>The time in seconds the full linear motion takes.</summary>
        [Tooltip("The time in seconds the full linear motion takes.")]
        public float time = 1f;

        /// <summary>The animation curve adjusts the linear motion.</summary>
        [Tooltip("The animation curve adjusts the linear motion.")]
        public AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        /// <summary>The mathematical construct for linear motion.</summary>
        private Utilities.LinearMotion linear = new Utilities.LinearMotion();

        /// <summary>Gets the rigidbody that was detected on <see cref="Awake"/>.</summary>
        private Rigidbody detectedRigidbody;

        private void Awake()
        {
            detectedRigidbody = GetComponent<Rigidbody>();
        }

        public void OnReactiveInput(ReactiveInput input)
        {
            switch (input.name)
            {
                case "Open":
                    linear.Open(time);
                    break;

                case "Close":
                    linear.Close(time);
                    break;

                case "Reverse":
                    linear.Reverse(time, time);
                    break;

                case "SetTime":
                    time = input.parameter.GetFloat();
                    linear.SetTime(time);
                    break;
            }
        }

        private void Update()
        {
            if (detectedRigidbody) return;

            var t = linear.Update(Time.deltaTime);

            t = curve.Evaluate(t);

            transform.position = Vector3.Lerp(target1.position, target2.position, t);

            AfterLinearUpdate();
        }

        private void FixedUpdate()
        {
            if (!detectedRigidbody) return;

            var t = linear.Update(Time.fixedDeltaTime);

            t = curve.Evaluate(t);

            detectedRigidbody.MovePosition(Vector3.Lerp(target1.position, target2.position, t));

            AfterLinearUpdate();
        }

        private void AfterLinearUpdate()
        {
            if (linear.arrived)
            {
                if (linear.forwards)
                {
                    this.OnReactiveOutput(new ReactiveObject(gameObject), "ArrivedAtTarget2");
                }
                else
                {
                    this.OnReactiveOutput(new ReactiveObject(gameObject), "ArrivedAtTarget1");
                }

                this.OnReactiveOutput(new ReactiveObject(gameObject), "Arrived");
            }

            if (linear.departed)
            {
                if (linear.forwards)
                {
                    this.OnReactiveOutput(new ReactiveObject(gameObject), "DepartedFromTarget1");
                }
                else
                {
                    this.OnReactiveOutput(new ReactiveObject(gameObject), "DepartedFromTarget2");
                }

                this.OnReactiveOutput(new ReactiveObject(gameObject), "Departed");
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!target1 || !target2) return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(target1.position, target2.position);
        }
    }
}