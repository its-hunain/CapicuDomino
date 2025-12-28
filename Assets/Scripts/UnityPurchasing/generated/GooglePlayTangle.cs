// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("b5vMMPO5Yavcellrd1NTsyUK+MHdaahP4hZ1pKX1JXwupISgrS7JHmHthrJEXrh/MEdYGGvfedL8P3Z3ipbyLa9PMOQx+KmDQ2uJiuIFf3WJ+zbLQVNKI2x1B0AaQPhOcGMgxs1//N/N8Pv013u1ewrw/Pz8+P3+KmMHQzT8/hNxNTuIqw1CFfU8D5wwUOuVRzRnOeHTm5vPdmWMITUFcwr+9skobsmdi4a3PV410UB90jSrZr9igh2igdBB1sxTvrNP+yDAKjwPKsfMFt3eyKRcyrbWiMOgje74O3Qa2n2f6sCi/YWRR8aTA9eGfEvzf/zy/c1//Pf/f/z8/UEMZN9IGL1sUKu7xYjzPGTOa/rGE80TYSrkSKWKiBQ0GiJrkv/+/P38");
        private static int[] order = new int[] { 6,7,9,10,10,6,13,13,12,10,12,11,13,13,14 };
        private static int key = 253;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
