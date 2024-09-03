// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("/xW/PA91r9sRdcBxTFhxOV1lRf9BC5pY+UV/X/gboqqHVi0VeV7EMBVYjGmYTwU1mqCTV6Z6od4fDwSjbiLS0HwNu+adY17z7jyi94YKih119vj3x3X2/fV19vb3VYjKJoYRj8yl3K8lkjnVugqpJjgVzigYY2wP+OvTcBaZtlLWvGyl3ldlCrJoosaXltq/MpN/M9RKYJy+kJ2Q7Q1Q6t7wubGn8EljlaLllIPsfk1kggns7vQOiYX0lbCaFiMYKU963cwDnpLHdfbVx/rx/t1xv3EA+vb29vL39ITnx4jc5jbhzzhxGd0yDimqIhtL+pbNS4srJYgyrkA9f2OmZVkde5OIpmySKUpi53TssZCpsYhNR1bbz8MQjmWIgmrOfvX09vf2");
        private static int[] order = new int[] { 13,7,4,8,7,9,7,12,12,12,13,12,13,13,14 };
        private static int key = 247;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
