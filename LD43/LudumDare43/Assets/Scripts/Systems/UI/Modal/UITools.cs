using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITools {

    public static void BringToFront(Transform UIElement)
    {
        UIElement.SetAsLastSibling();
    }


}
