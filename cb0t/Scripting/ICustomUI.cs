using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t.Scripting
{
    interface ICustomUI
    {
        // for Global.getControlById
        String ID { get; set; }

        // for event callbacks
        void KeyPressCallback(int k);
        void ValueChangedCallback();
        void ClickCallback();
        void SelectCallback();
        void SelectedItemChangedCallback();
        void ItemDoubleClickCallback();

        // radio buttons
        String Group { get; set; }
    }
}
