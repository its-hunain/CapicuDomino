// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("AnC9QMrYwajn/ozLkctzxfvoq01W4iPEaZ3+Ly5+rvelLw8rJqVClf+RUfYUYUspdg4azE0YiFwN98B4oeiMyL93dZj6vrADIIbJnn63hBcBHXmmJMS7b7pzIgjI4AIBaY70/upmDTnP1TP0u8zTk+BU8ll3tP3859sgME4DeLfvReBxTZhGmOqhb8O722AezL/ssmpYEBBE/e4Hqr6O+IF1fUKj5UIWAA08ttW+Wsv2Wb8g9Hd5dkb0d3x09Hd3dsqH71TDkzaEoUxHnVZVQy/XQT1dA0grBmVzsEb0d1RGe3B/XPA+8IF7d3d3c3Z15BBHu3gy6iBX8dLg/NjYOK6Bc0rtNOkJlikKW8pdR9g1OMRwq0uhty4BA5+/kangGXR1d3Z3");
        private static int[] order = new int[] { 3,7,11,13,10,9,11,9,12,9,12,13,13,13,14 };
        private static int key = 118;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
