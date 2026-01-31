using UnityEngine;

public static class Utils
{
     public static bool AlmostEqual(this Color clr1, Color clr2)
     {
          return Mathf.Abs(clr1.r - clr2.r) < 0.01f && Mathf.Abs(clr1.g - clr2.g) < 0.01f &&
                 Mathf.Abs(clr1.b - clr2.b) < 0.01f;
     }
}
