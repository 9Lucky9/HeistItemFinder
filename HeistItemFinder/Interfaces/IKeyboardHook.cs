using System;
using System.Windows.Input;

namespace HeistItemFinder.Interfaces
{
    public interface IKeyboardHook
    {
        public void HookKeyboard();
        public void UnHookKeyboard();

        public event EventHandler<Key> OnKeyPressed;

        public event EventHandler<Key> OnKeyUnpressed;

    }
}
