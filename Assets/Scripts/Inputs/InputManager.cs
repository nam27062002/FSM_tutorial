using Core;
namespace Inputs
{
    public class InputManager : MonobehaviorSingleton<InputManager>
    {
        public InputPad InputPad { get; set; }
        
        protected override void OnSingletonAwake()
        {
            base.OnSingletonAwake();
            InputPad = new InputPad();
            InputPad.Enable();
        }

        protected override void OnSingletonDestroy()
        {
            base.OnSingletonDestroy();
            InputPad.Disable();
        }
    }
}