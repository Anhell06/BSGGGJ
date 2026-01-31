using UnityEngine;

public static class Utils
{
     public static bool AlmostEqual(this Color clr1, Color clr2)
     {
         var dr = Mathf.Abs(clr1.r - clr2.r);
         var dg = Mathf.Abs(clr1.g - clr2.g);
         var db = Mathf.Abs(clr1.b - clr2.b);
          return dr < 0.01f &&  dg < 0.01f && db < 0.01f;
     }
}
