using System;
using System.Windows.Input;

namespace HeistItemFinder.Interfaces
{
    public interface IKeyboardHook
    {
        /// <summary>
        /// Hook keyboard.
        /// </summary>
        public void HookKeyboard();

        /// <summary>
        /// Unhook keyboard.
        /// </summary>
        public void UnHookKeyboard();

        /// <summary>
        /// Event that fires when key is pressed on keyboard.
        /// </summary>

        public event EventHandler<Key> OnKeyPressed;\

        /// <summary>
        /// Event that fires when key is pressed on keyboard.
        /// </summary>

        public event EventHandler<Key> OnKeyUnpressed;

    }
}
